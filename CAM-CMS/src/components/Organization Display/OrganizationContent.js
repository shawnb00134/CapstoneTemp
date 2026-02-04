import { React, useEffect, useState } from 'react';
import './styles/OrganizationContent.css';
import OrganizationContentBody from './OrganizationContentCompnents/OrganizationContentBody';
import Requests from '../../utils/Requests';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMinus } from '@fortawesome/free-solid-svg-icons';
import NewContentRoleDialog from '../dialogs/Organization Dialogs/NewContentRoleDialog';
import AddPackageOrganizationDialog from '../dialogs/Organization Dialogs/AddPackageOrganizationDialog';
import { useNavigate, useParams } from 'react-router-dom';

export default function OrganizationContent({ organizationId }) {
	const navigate = useNavigate();

	const [packages, setPackages] = useState([]);
	const [showPackages, setShowPackages] = useState(false);
	const [refreshFlag, setRefreshFlag] = useState(false);
	const [loadContentRoles, setLoadContentRoles] = useState(false);
	const [roleSearch, setRoleSearch] = useState('');
	const [packageSearch, setPackageSearch] = useState('');
	const [contentRoles, setContentRoles] = useState([]); 
	const { id } = useParams();
	
	const triggerRefresh = () => {
		setRefreshFlag((prev) => !prev);
	};

	async function openNewContentRoleDialog(){
		const dialog = document.getElementById('new-content-role-dialog');
		setLoadContentRoles(true);
		await dialog.showModal();
		setLoadContentRoles(false);
	}

	useEffect(() => {
		getOrganizationContentRoles();		
	}, [organizationId]);

	useEffect(() => {
		setEventListeners();
	}, [contentRoles]);

	useEffect(() => {
		loadPackages();
	}, [organizationId, refreshFlag]);

	async function loadPackages(){
		await Requests.getAllPackagesByOrganizationId(organizationId).then((response) => {
			if (!response || response.status === 401) {
				alert('No packages found or you do not have access to them.');
				return;
			}
			setPackages(response);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function onAdd() {
		getOrganizationContentRoles();
	}

	async function getOrganizationContentRoles() {
		let id = parseInt(organizationId);
		await Requests.getAllOrganizationContentRoles(id).then((data) => {
			setContentRoles(data);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function setEventListeners() {
		const contentRoles = document.getElementsByClassName('content-role');
		
		Array.from(contentRoles).forEach((role) => {
			role.addEventListener('mouseover', () => {
				role.classList.add('hovering');
			});
			role.addEventListener('mouseout', () => {
				role.classList.remove('hovering');
			});
		});
	}

	function deleteContentRole(id, displayName) {
		var confirmDelete = window.confirm(`Are you sure you want to delete ${displayName}?`);
		if (!confirmDelete) {
			return;
		}
		setContentRoles(contentRoles.filter(role => role.organizationContentRoleId !== id));
		Requests.deleteOrganizationContentRole(id).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function addPackageToOrganization() {
		let dialog = document.getElementById('add-package-dialog');
		setShowPackages(true);
		dialog.showModal();
	}

	const filteredContentRoles = contentRoles.filter(role => role.displayName.toLowerCase().includes(roleSearch));
	const filteredPackages = packages.filter(pack => pack.name.toLowerCase().includes(packageSearch));

	return (
		<>
			<NewContentRoleDialog loadContentRoles={loadContentRoles} organizationId={organizationId} onAdd={onAdd} existingRoles={contentRoles}/>
			<AddPackageOrganizationDialog id='add-package-dialog' onLoadPackages={showPackages} organizationId={id} onUploadSuccess={triggerRefresh}/>
			<div className='organization-tab-content'>
				<div className='packages-aside'>
					
					<div className='aside-section'>
						<div className='packages-header'>
							<div className='title-add-button'>
								<h2>Content Roles </h2>
								<button className='add-button' id='add-content-role' onClick={openNewContentRoleDialog}>Add</button>
							</div>
							<div className='header-search'>
								<input type='text' className='search-packages-input' placeholder='search roles' value={roleSearch} onChange={(e) => setRoleSearch(e.target.value.toLowerCase())} />
							</div>
						</div>
						<div className='content-body'>
							{
								filteredContentRoles?.map((role, index) => (
									<div key={index} className='content-role' id={`content-role-${role.displayName}`}>
										<p>{role.displayName}</p>
										<button id='delete-content-role-button' onClick={() => deleteContentRole(role.organizationContentRoleId, role.displayName)}><FontAwesomeIcon icon={faMinus}/></button>
									</div>
								))
							}
						</div>
					</div>
				
					<div className='aside-section'>
						<div className='packages-header'>
							<div className='title-add-button'>
								<h2>Packages </h2>
								<button className='add-button' id='add-package' onClick={addPackageToOrganization}>Edit</button>
							</div>
							<div className='header-search'>
								<input type='text' className='search-packages-input' placeholder='search packages' value={packageSearch} onChange={(e) => setPackageSearch(e.target.value.toLowerCase())} />
							</div>
						</div>
						<div className='content-body'>
							{
								filteredPackages?.map((pack, index) => (
									<div key={index} className='packages-aside-item' onClick={() => navigate(`/studio/package/${pack.packageId}`)}>
										<p>{pack.name}</p>
									</div>
								))
							}
						</div>
					</div>

					<div className='aside-section' onClick={() => navigate('/studio/')} style={{cursor: 'pointer'}}>
						<h2>Library Folder</h2>
					</div>
				</div>
				<div className='packages-content'>
					<OrganizationContentBody packageData={packages} organizationRoles={contentRoles} />
				</div>
			</div>	
		</>
	);
}
