import { React, useState, useEffect } from 'react';
import './styles/OrganizationUsers.css';
import InviteUsersToOrganizationDialog from '../dialogs/Organization Dialogs/InviteUsersToOrganizationDialog';
import Requests from '../../utils/Requests';
import { useNavigate } from 'react-router-dom';

export default function OrganizationUsers({ organizationId }) {
	console.log(organizationId);
	const [selectedUser, setSelectedUser] = useState(undefined);
	const [isInviteDialogOpen, setIsInviteDialogOpen] = useState(false);
	const [contentRoles, setContentRoles] = useState([]);
	const [modules, setModules] = useState([]);
	const [userSearch, setUserSearch] = useState('');
	const navigate = useNavigate();

	useEffect(() => {
		console.log(selectedUser);
	}, [selectedUser]);

	useEffect(() => {
		async function fetchData() {
			try {
				const rolesData = await Requests.getAllOrganizationContentRoles(organizationId).catch((error) => {
					console.error(error);
					navigate('/error');
				});
				const modulesData = await Requests.getAllModules().catch((error) => {
					console.error(error);
					navigate('/error');
				});
				setContentRoles(rolesData);
				setModules(modulesData);
			} catch (error) {
				console.error('Error fetching data:', error);
			}
		}
		fetchData();
	}, [organizationId]);

	const users = [
		{
			'username': 'john_doe',
			'email': 'john.doe@example.com',
			'name': 'John Doe',
			'role': 'admin',
			'gender': 'male'
		},
		{
			'username': 'jane_smith',
			'email': 'jane.smith@example.com',
			'name': 'Jane Smith',
			'role': 'admin',
			'gender': 'female'
		},
		{
			'username': 'sam_jackson',
			'email': 'sam.jackson@example.com',
			'name': 'Sam Jackson',
			'role': 'admin',
			'gender': 'male'
		}
	];

	function handleSelectingUser(user) {
		setSelectedUser(user);
	}

	function toggleInviteDialog() {
		setIsInviteDialogOpen(!isInviteDialogOpen);
	}

	const filteredUsers = users.filter(user => user.name.toLowerCase().includes(userSearch) || user.email.toLowerCase().includes(userSearch) );

	return (
		<>
			<InviteUsersToOrganizationDialog
				organizationId={organizationId}
				isOpen={isInviteDialogOpen}
				onClose={toggleInviteDialog}
				contentRoles={contentRoles}
				modules={modules}
			/>
			<div className='organization-users'>
				<div className='users-aside'>
					<div className='users-header'>
						<div className='title-add-button'>
							<h2>Users</h2>
							<button className='invite-user-button' onClick={toggleInviteDialog}>Invite</button>
						</div>
						<div className='header-search'>
							<input type='text' className='search-users-input' placeholder='search users' value={userSearch} onChange={(e) => setUserSearch(e.target.value.toLowerCase())} />
						</div>

					</div>
					<div className='users-aside-content'>
						{
							filteredUsers.map((user, index) => {
								return (
									<div className='users-aside-item' key={index} onClick={() => handleSelectingUser(user)}>
										<p>{user.username}</p>
										<p>{user.email}</p>
									</div>
								);
							})
						}
					</div>
				</div>
				<div className='users-content'>
					{
						selectedUser !== undefined ? (
							<>
								<p>Username: {selectedUser.username}</p>
								<p>Email: {selectedUser.email}</p>
								<p>Name: {selectedUser.name}</p>
								<p>Role: {selectedUser.role}</p>
								<p>Gender: {selectedUser.gender}</p>
							</>
						) : (
							<p>Select a user to view their details</p>
						)
					}
				</div>
			</div>
		</>
	);
}