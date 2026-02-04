import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPencil, faTrash } from '@fortawesome/free-solid-svg-icons';
import Requests from '../utils/Requests';
import './styles/PeoplePage.css';
import { AddUserDialog } from '../components/dialogs/People Dialogs/AddUserDialog';

/**
 * The People page is where you can view all users and their roles.
 * You can also add, remove, and update roles for users.
 * 
 *  @version 1.2
 *  @author Steven Kight
 */
export function PeoplePage() {
	const navigate = useNavigate();

	// All users. (Not filtered by search term)
	const [allUsers, setAllUsers] = useState([]);
	// The users to display. (Filtered by search term)
	const [users, setUsers] = useState([]);
	const [showAddUserDialog, setShowAddUserDialog] = useState(false);

	useEffect(() => {
		checkAndUpdateUsers();
	}, []);

	useEffect(() => {
		loadUsers();
	}, []);

	/**
	 * This function handles the search bar's search.
	 * It filters the users based on the search term by username and email (case insensitive).
	 * If the search term is empty, it will display all users.
	 * 
	 * @param {string} searchTerm The term to search for within each user.
	 */
	function handleSearch(searchTerm) {
		if (!searchTerm) {
			setUsers(allUsers);
			return;
		}
		const filteredUsers = allUsers.filter(user => {
			if (user.username && user.username.toLowerCase().includes(searchTerm.toLowerCase())) {
				return true;
			}
			if (user.email && user.email.toLowerCase().includes(searchTerm.toLowerCase())) {
				return true;
			}
			return false;
		});
		setUsers(filteredUsers);
	}

	/**
	 * This function loads all users and sets the users and allUsers state.
	 * 
	 * @see Requests.getAllUsers()
	 */
	function loadUsers() {
		Requests.getAllUsers().then(response => {
			if (response.status === 401) {
				navigate('/404');
			}
			const activeUsers = response.filter(user => !user.isDeleted);
			Requests.getAllTempUsers().then(tempResponse => {
				if (tempResponse.status === 401) {
					navigate('/404');
				}
				const activeTempUsers = tempResponse.filter(tempUser => !tempUser.isDeleted);
				const tempUsersWithPrefix = activeTempUsers.map((tempUser, index) => ({
					id: `temp_${tempUser.tempId || index}`,
					cognitoId: tempUser.cognitoId,
					username: tempUser.username,
					firstname: tempUser.firstname,
					lastname: tempUser.lastname,
					email: tempUser.email,
					phone: tempUser.phone,
					isDeleted: tempUser.isDeleted,
					displayRoles: []
				}));
				const combinedUsers = [...activeUsers, ...tempUsersWithPrefix];
				setUsers(combinedUsers);
				setAllUsers(combinedUsers);
			});
		}).catch(error => {
			console.error(error);
			navigate('/error');});
	}

	const checkAndUpdateUsers = () => {
		Requests.checkAndUpdateUsers().then(() => {
			loadUsers();
		}).catch(error => {
			console.error(error);
			navigate('/error');
		});
	};

	const handleAddUser = newUser => {
		Requests.addUser(newUser).then(() => {
			loadUsers();
		}).catch(error => {
			console.error(error);
			navigate('/error');
		});
	};

	const handleDeleteUser = user => {
		try {
			const userId = user.id.toString().startsWith('temp_') ? user.id.replace('temp_', '') : user.id;
			const deleteRequest = user.id.toString().startsWith('temp_') ? Requests.deleteTempUser : Requests.deleteUser;
			if (user.displayRoles && user.displayRoles.length > 0) {
				alert(`User ${user.username} has roles assigned. Please delete the roles first.`);
				return;
			}
			if (window.confirm(`Are you sure you want to delete user ${user.username}?`)) {
				console.log(userId);
				deleteRequest(userId).then(() => {
					loadUsers();
				});
			}
		}
		catch (error) {
			console.error(error);
			navigate('/error');
		}
	};

	return (
		<div className='people-page'>
			<div className='people-page-header'>
				<div className='people-page-header-left'>
					<h1>People</h1>
				</div>
				<div className='people-page-header-right'>
					<input type='text' id='search-bar' name='search-bar'
						placeholder='Search Users...' onChange={(evt) => handleSearch(evt.target.value)} />
					<button onClick={() => setShowAddUserDialog(true)}>Add User</button>
				</div>
			</div>
			<div className='people-page-content'>
				<div className='people-page-content-header'>
					<h4>Name</h4>
					<h4>Roles (top 5)</h4>
					<h4>Actions</h4>
				</div>
				<div className='people-page-content-people'>
					{
						users.map(user => {
							const currentUser = JSON.parse(window.sessionStorage.getItem('userData'));
							if (user.id === currentUser.id) {
								return null;
							}
							return <User user={user} key={user.id} onDelete={() => handleDeleteUser(user)} />;
						})
					}
				</div>
			</div>
			<AddUserDialog
				show={showAddUserDialog}
				onClose={() => setShowAddUserDialog(false)}
				onAddUser={handleAddUser}
			/>
		</div>
	);
}

/**
 * This component displays the information for a single user.
 * It displays the user's username, name, and email.
 * 
 *  @param {json} user: The user to display.
 *  @param {function} onDelete: Function to call when deleting the user.
 * 
 *  @version 1.0
 *  @author Steven Kight
 */
function User({ user, onDelete }) {
	const navigate = useNavigate();

	const isTempUser = user.id.toString().startsWith('temp_');
	const handleDelete = () => {
		const userId = user.id.toString().startsWith('temp_') ? user.id.replace('temp_', '') : user.id;
		onDelete({ ...user, id: userId });
	};

	return (
		<div className='user-row' id={`user-row-${user.id}`}>
			<div className='user-row-name'>
				<h4>{user.username}</h4>
				<h5>{user.email ? user.email : 'No email provided'}</h5>
			</div>
			<div className='user-row-roles'>
				{
					user.displayRoles.length > 0 ?
						user.displayRoles.map((role, index) => {
							if (index >= 5) return;

							let roleColor = RoleMap[role.accessRoleId];
							return (
								<h5 className='user-row-role' key={role.id}
									style={{ backgroundColor: roleColor }}>
									{role.name}
								</h5>
							);
						})
						: <h5 className='user-row-no-role'>No roles</h5>
				}
			</div>
			<div className='user-row-actions'>
				<button
					onClick={() => navigate(`/people/${user.id}`)}
					style={isTempUser ? { backgroundColor: 'grey', cursor: 'not-allowed' } : {}}
					disabled={isTempUser}
				>
					<FontAwesomeIcon className='icon' icon={faPencil} size='xl' />
				</button>
				<button onClick={handleDelete}>
					<FontAwesomeIcon className='icon' icon={faTrash} size='xl' />
				</button>
			</div>
		</div>
	);
}

const RoleMap = {
	1: '#227093',
	2: '#218c74',
	3: '#ccae62',
	4: '#cc8e35',
	5: '#cd6133',
	6: '#474787',
	7: '#b33939'
};
