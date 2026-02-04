import { React, useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import './styles/OrganizationsPage.css';
import OrganizationUsers from '../components/Organization Display/OrganizationUsers';
import OrganizationContent from '../components/Organization Display/OrganizationContent';
import Requests from '../utils/Requests';

export default function OrganizationsPage() {
	const [selectedTab, setSelectedTab] = useState('Content');
	const [selectedOrganization, setSelectedOrganization] = useState(undefined);
	const [organizations, setOrganizations] = useState([]);
	const [isAdmin, setIsAdmin] = useState(false);
	const { id } = useParams();
	const navigate = useNavigate();

	useEffect(() => {
		loadSelectedOrganization();
		checkIfUserIsAdmin();
	}, [id]);

	useEffect(() => {
		const tabs = document.querySelectorAll('.tab');
		Array.from(tabs).forEach((tab) => {
			if (tab.innerHTML === selectedTab) {
				tab.classList.add('selected-tab');
			} else {
				tab.classList.remove('selected-tab');
			}
		});
	}, [selectedTab]);

	const checkIfUserIsAdmin = async () => {
		try {
			const response = await Requests.userIsAdmin().catch((error) => {
				console.error(error);
				navigate('/error');
			});
			setIsAdmin(response.isAdmin);
		} catch (error) {
			console.error(error);
		}
	};

	function loadSelectedOrganization() {
		Requests.getOrganizationById(id).then((response) => {
			const orgs = [response];
			setOrganizations(orgs);
			setSelectedOrganization(response);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function handleTabClick(event) {
		setSelectedTab(event.target.innerHTML);
	}

	function handleValueClick() {
		navigate('/organizations');
	}

	function handleDeleteOrganization() {
		if (window.confirm('Are you sure you want to delete this organization? This can not be undone.')) {
			Requests.deleteOrganization(id).then((response) => {
				if (!response || response.status === 401) {
					window.alert('No organization found or you do not have access to it.');
					return;
				}
				if (response) {
					navigate('/dashboard');
				} else {
					window.alert('Failed to delete organization.');
				}
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
		}
	}

	return (
		<>	
			<div className='organization-page'>
				<div className='organization-header'>
					<button value={selectedOrganization}>
						{organizations?.map((organization, index) => {
							return (
								<option key={index} onClick={handleValueClick} value={organization.name}>{organization.name}</option>
							);
						})}
					</button>
					<div className='organization-page-tab-selectors'>
						<p className='tab' onClick={(e) => handleTabClick(e)}>
							Content
						</p>
						<p className='tab' onClick={(e) => handleTabClick(e)}>
							Users
						</p>
					</div>
					{isAdmin && (
						<button className='delete-org' onClick={handleDeleteOrganization}>
							Delete Organization
						</button>
					)}
				</div>
				<div className='organization-content'>
					{selectedTab === 'Users' ? (
						<OrganizationUsers organizationId={id} />
					) : selectedTab === 'Content' ? (
						<OrganizationContent organizationId={id} />
					) : undefined}
				</div>
			</div>
		</>
	);
}
