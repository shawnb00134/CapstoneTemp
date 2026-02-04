import { React, useState, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import Requests from '../../../utils/Requests';
import './styles/NewContentRoleDialog.css';
export default function NewContentRoleDialog({loadContentRoles,organizationId,onAdd,existingRoles}) {
	const navigate = useNavigate();
	const [contentRoles, setContentRoles] = useState([]);
	const [contentRoleName, setContentRoleName] = useState('');
	const [selectedArchetypes, setSelectedArchetypes] = useState([]);

	useEffect(() => {
		if (loadContentRoles) {
			fetchContentRoles();
		}
	}, [loadContentRoles]);

	function fetchContentRoles() {
		Requests.getContentRoles().then((data) => {
			if (!data || data.status === 401) {
				alert('You are not authorized to view content roles.');
				return;
			}
			setContentRoles(data);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function closeDialog() {
		clearDialog();
		var dialog = document.getElementById('new-content-role-dialog');
		dialog.close();
	}

	function validateContentRoleName() {
		var errorMessage = '';
		var isValid = true;
		existingRoles?.forEach((role) => {
			if (role?.displayName.toLowerCase() === contentRoleName.toLowerCase()) {
				isValid = false;
				errorMessage = 'Role Name Already Exists';
			}
		});
		if (contentRoleName.trim() === '') {
			isValid = false;
			errorMessage = 'Role Name Cannot Be Empty';
		}
		if (contentRoleName.length > 50) {
			isValid = false;
			errorMessage = 'Role Name Cannot Exceed 50 Characters';
		}
		if (!isValid) {
			alert(errorMessage);
		}
		return isValid;
	}

	function createContentRole() {
		if (!validateContentRoleName()) {
			return;
		}

		var organizationContentRole = {
			'organizationId': organizationId,
			'displayName': contentRoleName,
			'archetypeIds': selectedArchetypes
		};

		Requests.createOrganizationContentRole(organizationContentRole).then(() => {
			onAdd();
			clearDialog(); 
			closeDialog();
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function clearDialog() {
		setContentRoleName('');
		setSelectedArchetypes([]);
	}

	function handleArchetypeSelection(event) {
		const options = event.target.options;
		const selectedValues = [];
		for (let i = 0; i < options.length; i++) {
			if (options[i].selected) {
				selectedValues.push(parseInt(options[i].value));
			}
		}
		setSelectedArchetypes(selectedValues);
	}

	return (
		<dialog id='new-content-role-dialog' className='dialog'>
			<div className='new-content-role'>
				<div className='new-content-role-header'>
					<h2>New Content Role</h2>
					<button className='close-dialog-button' onClick={closeDialog}><FontAwesomeIcon icon={faXmark} /></button>
				</div>
				<div className='new-content-role-body'>
					<div className='new-role-name'>
						<p>Role Name</p>
						<input id='content-role-name-input' type='text' name='roleName' placeholder='Role Name' value={contentRoleName} onChange={(event) => setContentRoleName(event.target.value)} required />
					</div>
					<div>
						<p>Content Role Archetype (Hold ctrl to Select Multiple)</p>
						<select name='roleArchetype' multiple={true} value={selectedArchetypes} onChange={handleArchetypeSelection} required>
							{
								contentRoles?.map((role, index) => (
									<option key={index} value={role?.contentRoleId}>{role?.name}</option>
								))
							}
						</select>
					</div>
				</div>
				<div className='new-content-role-footer'>
					<button id='create-content-role-button' onClick={createContentRole}>Create</button>
					<button onClick={closeDialog}>Cancel</button>
				</div>
			</div>
		</dialog>
	);
}
