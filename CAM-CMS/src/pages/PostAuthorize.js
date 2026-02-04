import React, { useEffect } from 'react';
import { Navigate, Route, Routes, useNavigate, Outlet } from 'react-router-dom';

import './styles/PostAuthorize.css';
import Requests from '../utils/Requests';

import Navigation from '../components/Top Navbar/Navigation';

import Studio from './Studio';
import PackagesPage from './PackagesPage';
import ModulesPage from './ModulesPage';
import ElementsPage from './ElementsPage';
import { PeoplePage } from './PeoplePage';
import { UserPage } from './UserPage';
import NotFoundPage from './NotFoundPage';
import SomethingWentWrongPage from './SomethingWentWrongPage';

// import EcosystemIcon from '../assets/images/static-pages/-Ecosystem.jpeg';
import PeopleIcon from '../assets/images/static-pages/People.jpg';
import OrganizationIcon from '../assets/images/static-pages/Organization.jpg';
import CAMStudioIcon from '../assets/images/static-pages/CAM-Studio.jpg';
import OrganizationsPage from './OrganizationsPage';
import OrganizationTileSelectionPage from './OrganizationTileSelectionPage';

const pages = [
	// { src: EcosystemIcon, alt: ' logo', title: ' Ecosystem', role: '', link: '/statistics' },
	{ src: PeopleIcon, alt: 'People icon', title: 'People', role: '', link: '/people' },
	{ src: OrganizationIcon, alt: 'Shaking hands icon', title: 'Organization', role: '', link: '/organizations' },
	{ src: CAMStudioIcon, alt: 'Painters Palette', title: 'CAM Studio', role: '', link: '/studio' },
];

/**
 * The page that is displayed after the user has been authorized.
 * 
 * @returns {JSX.Element}
 */
export default function PostAuthorize() {
	const navigate = useNavigate();
	const [userReadPrivileges, setUserReadPrivileges] = React.useState({isSystemRead: null, isOrganizationsRead: null, isLibrariesRead: null, isPackagesRead: null});
	
	useEffect(() => {
		async function fetchReadPrivileges() {
			let userReadPrivileges;
			try {
				userReadPrivileges = await Requests.userCanRead();
			}
			catch (error) {
				console.error(error);
				return;
			}
			
			let isSystemRead = userReadPrivileges.systemRead;
			let isOrganizationsRead = userReadPrivileges.organizationRead;
			let isLibrariesRead = userReadPrivileges.libraryRead;
			let isPackagesRead = userReadPrivileges.packageRead;
			setUserReadPrivileges({isSystemRead: isSystemRead, isOrganizationsRead: isOrganizationsRead, isLibrariesRead: isLibrariesRead, isPackagesRead: isPackagesRead});
		}
		fetchReadPrivileges();
	}, []);

	/**
	 * Render the pages that the user can visit.
	 * 
	 * @returns {JSX.Element[]} The pages that the user can visit.
	 */
	function RenderPages() {

		if (userReadPrivileges.isSystemRead === null || userReadPrivileges.isOrganizationsRead === null || userReadPrivileges.isLibrariesRead === null || userReadPrivileges.isPackagesRead === null) {
			return;
		}

		var renderedPages = pages.map(
			(item, index) => {

				var canVisit = false;
				if (item === pages[0]) {
					canVisit = userReadPrivileges.isSystemRead;
				}
				else if (item === pages[1]) {
					canVisit = userReadPrivileges.isOrganizationsRead;
				}
				else if (item === pages[2]) {
					canVisit = (userReadPrivileges.isLibrariesRead || userReadPrivileges.isPackagesRead);
				}

				if (!canVisit) {
					return;
				}
				return (
					<Page title={ item.title } 
						role={ item.role } 
						src={ item.src } 
						alt={ item.alt } 
						url={ item.link } 
						key={ index }
					/>
				);
			}
		);
		
		var filteredPages = renderedPages.filter(page => page !== undefined);
		var pageCount = filteredPages.length;
		useEffect(() => {
			if (pageCount === 1) {
				navigate(filteredPages[0].props.url);
			}
			if (pageCount === 0) {
				navigate('/404');
			}
		}, [pageCount]);

		
		return renderedPages;
	}

	let studioElement;
	if (userReadPrivileges.isLibrariesRead === null && userReadPrivileges.isPackagesRead === null) {
		studioElement = null;
	} else if (userReadPrivileges.isLibrariesRead || userReadPrivileges.isPackagesRead) {
		studioElement = <Studio />;
	} else {
		studioElement = <Navigate to="/404" />;
	}

	let organizationElement;
	if (userReadPrivileges.isOrganizationsRead === null) {
		organizationElement = null;
	}
	else if (userReadPrivileges.isOrganizationsRead) {
		organizationElement = <OrganizationTileSelectionPage isAdmin={true} />;
	}
	else {
		organizationElement = <Navigate to="/404" />;
	}

	let peopleElement;
	if (userReadPrivileges.isSystemRead === null) {
		peopleElement = null;
	}
	else if (userReadPrivileges.isSystemRead) {
		peopleElement = <PeoplePage />;
	}
	else {
		peopleElement = <Navigate to="/404" />;
	}

	return (
		<>
			<Navigation data-testid='navigation-tests'/>
			<div className='page-content'>
				<Routes>
					<Route
						path='/dashboard'
						Component={() => {
							return (
								<div className='dashboard'>
									<div data-testid='dashboard-test' className='page-buttons'>
										{
											RenderPages()
										}
									</div>
								</div>
							);}
						}
					/>

					
					<Route 
						path='studio' 
						element={studioElement}>
						
						<Route path='module' element={ <Outlet /> }>
							<Route path=':id' element={ <ModulesPage /> } />
						</Route>
						<Route path='element' element={ <Outlet /> }>
							<Route path=':id' element={ <ElementsPage /> } />
						</Route>
						<Route path='package' element={ <Outlet /> }>
							<Route path=':id' element={ <PackagesPage /> } />
						</Route>
					</Route>

					<Route path='people' element={peopleElement}/>
					<Route path='people/:id' element={ <UserPage props/> } />
					
					<Route path='/organizations' element={organizationElement} />
					<Route path='/organizations/organization/:id' element={<OrganizationsPage />} />

					<Route path='404' element={ <NotFoundPage />} />
					<Route path='error' element={<SomethingWentWrongPage />} />

					<Route path='*' element={ <Navigate to='/404' /> } />
				</Routes>
			</div>
		</>
	);
}


/**
 * Defines a page button for the dashboard
 * 
 * @param {*} props 
 * @returns {JSX.Element}
 */
function Page(props) {
	const navigate = useNavigate();
    
	/**
	 * Navigate to the page when the button is clicked.
	 */
	const useVertical = () => {
		var navigateUrl = props.url;
		if (props.url === '/statistics') {
			navigateUrl = '/statistics/' + props.title.toLowerCase().replace(/\s/g, '_');
		}

		navigate('../' + navigateUrl);
	};
    
	return (
		<button className='page' onClick={useVertical} key={props.link}>
			<img src={props.src} alt={props.alt} />
			<div className='page-details'>
				<h3>{props.title}</h3>
				<h6>{props.role}</h6>
			</div>
		</button>
	);
}