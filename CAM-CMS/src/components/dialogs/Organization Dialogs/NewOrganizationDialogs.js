import { React, useState, useEffect } from 'react';
import './styles/NewOrganizationDialogs.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import Requests from '../../../utils/Requests';

export default function NewOrganizationDialogs({ id, createOrganization}) {
	const [organizationName, setOrganizationName] = useState('');
	const [orgAdmin, setOrgAdmin] = useState(null);
	const [users, setUsers] = useState([]);
	const navigate = useNavigate();

	useEffect(() => {
		// Fetch the list of users when the component mounts
		loadUsers();
	}, []);

	function loadUsers() {
		Requests.getAllUsers().then(response => {
			setUsers(response || []);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function closeDialog() {
		var dialog = document.getElementById(id);
		dialog.close();
	}

	function upload(event) {
		event.preventDefault();
		var newOrganization = {
			name: organizationName,
			organizationId: 0,
			tags: [],
			createdAt: null,
			updatedAt: null,
			isActive: true
		};

		createOrganization(newOrganization, orgAdmin);
		closeDialog();
	}

	return (
		<dialog id={id} className='new-organization-dialog' open>
			<div className='dialog'>
				<div className='dialog-header'>
					<h2>Create New Organization</h2>
					<button className='close-dialog-button' onClick={closeDialog}><FontAwesomeIcon icon={faXmark} /></button>
				</div>
				<form className='organization-form' onSubmit={upload}>
					<div className='dialog-body'>
						<div className='input-label'>
							<p>Organization Name:</p>
						</div>
						<div>
							<input
								type='text'
								name='organizationName'
								placeholder='Organization Name'
								onChange={(event) => setOrganizationName(event.target.value)}
								required
							/>
						</div>
						<div className='input-label'>
							<p>Organization Admin (Optional):</p>
						</div>
						<div>
							<select name='orgAdmin' onChange={(event) => setOrgAdmin(event.target.value)}>
								<option value='' disabled selected>Select an Admin</option>
								{users.length > 0 ? users.map(user => (
									<option key={user.id} value={user.id}>{user.username}</option>
								)) : <option disabled>No users available</option>}
							</select>
						</div>
					</div>
					<div className='dialog-footer'>
						<input type='submit' value='Create'></input>
					</div>
				</form>
			</div>
		</dialog>
	);
}
