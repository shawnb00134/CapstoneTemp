import { React, useState, useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './styles/OrganizationTileSelectionPage.css';
import OrganizationTile from '../components/Organization Display/OrganizationTile';
import NewOrganizationDialogs from '../components/dialogs/Organization Dialogs/NewOrganizationDialogs';
import Requests from '../utils/Requests';

function useForceUpdate() {
	const [, setTick] = useState(0);
	const update = useCallback(() => {
		setTick((tick) => tick + 1);
	}, []);
	return update;
}

export default function OrganizationTileSelectionPage() {
	const [organizations, setOrganizations] = useState([]);
	const [searchOrganizations, setSearchOrganizations] = useState('');
	const [isAdmin, setIsAdmin] = useState(false);
	const [showNewOrganizationDialog, setShowNewOrganizationDialog] = useState(false);
	const forceUpdate = useForceUpdate();
	const navigate = useNavigate();

	useEffect(() => {
		getAllOrganizations();
		checkIfUserIsAdmin();
	}, []);

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

	function createOrganization(organization, orgAdminId) {
		Requests.createOrganization(organization).then((response) => {
			if (!response || response.status === 401) {
				alert('You do not have permission to create organizations.');
				return;
			}
			if (response) {
				if (orgAdminId) {
					const orgAdminRole = {
						userId: orgAdminId,
						contextId: response.organizationId,
						accessRoleId: 5
					};
					Requests.createUserPrivileges(orgAdminRole).then(() => {
						getAllOrganizations();
					});
				} else {
					getAllOrganizations();
				}
			} else {
				window.alert('Failed to create organization.');
			}
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function handleAddOrganization() {
		setShowNewOrganizationDialog(true);
	}

	function handleDeleteOrganization(organizationId) {
		if (window.confirm('Are you sure you want to delete this organization? This can not be undone.')) {
			Requests.deleteOrganization(organizationId).then((response) => {
				if (!response || response.status === 401) {
					window.alert('No organization found or you do not have access to delete it.');
					return;
				}
				if (response) {
					getAllOrganizations();
				} else {
					window.alert('Failed to delete organization.');
				}
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
		}
	}

	function getAllOrganizations() {
		Requests.getAllOrganizations().then((response) => {
			if (!response || response.status === 401) {
				alert('No organizations found or you do not have access to them.');
				return;
			}
			setOrganizations(response);
			forceUpdate();
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function handleSelectOrganization(organization) {
		navigate('/organizations/organization/' + organization.organizationId);
	}

	const filteredOrganizations = organizations.filter(org => org.name.toLowerCase().includes(searchOrganizations));

	return (
		<>
			{showNewOrganizationDialog && (
				<NewOrganizationDialogs
					id={'new-organization'}
					createOrganization={createOrganization}
				/>
			)}
			<div className='tile-page'>
				<div className='tile-header'>
					<div>
						<h1>Organizations</h1>
					</div>
					<div className='tile-search'>
						<input
							type='text'
							className='Search Organization'
							placeholder='Search Organization'
							value={searchOrganizations}
							onChange={(e) => setSearchOrganizations(e.target.value.toLowerCase())}
						/>
						<div className='tile-buttons'>
							{isAdmin ? <button className='add-org' onClick={handleAddOrganization}>Add</button> : null}
						</div>
					</div>
				</div>
				<div className='tile-content'>
					{filteredOrganizations?.map((organization, index) => (
						<div key={index} className='tile' onClick={() => handleSelectOrganization(organization)}>
							<OrganizationTile organization={organization} />
							{isAdmin && (
								<button
									className='delete-org'
									onClick={(e) => { e.stopPropagation(); handleDeleteOrganization(organization.organizationId); }}
								>
									Delete
								</button>
							)}
						</div>
					))}
				</div>
			</div>
		</>
	);
}
