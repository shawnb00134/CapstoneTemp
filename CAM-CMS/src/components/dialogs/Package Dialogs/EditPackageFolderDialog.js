import { React, useEffect, useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import Requests from '../../../utils/Requests';
import './styles/EditPackageFolderDialog.css';

export default function EditPackageFolderDialog({ id, packageFolder, parentFolder, editDialogLoaded, onUpdate }) {
	const navigate = useNavigate();
	const [contentRoles, setContentRoles] = useState([]);
	const [packageFolderName, setPackageFolderName] = useState(undefined);
	const [lockContentRole, setLockContentRole] = useState(false);
	const [selectedContentRole, setSelectedContentRole] = useState(packageFolder.contentRoleId);
	const [newContentRoleName, setNewContentRoleName] = useState('');
	const [selectedArchetypeId, setSelectedArchetypeId] = useState(1);
	const [archetypes, setArchetypes] = useState([]);
	const [showAddContentRole, setShowAddContentRole] = useState(false);
	const [error, setError] = useState('');

	useEffect(() => {
		if (editDialogLoaded) {
			fetchContentRoles();
			setPackageFolderName(packageFolder?.displayName);
			let isLocked = lockContentRoleSelection();
			setLockContentRole(isLocked);
		}
		fetchArchetypes();
	}, [editDialogLoaded]);

	function capitalizeFirstLetter(string) {
		return string.charAt(0).toUpperCase() + string.slice(1).toLowerCase();
	}

	function closeDialog() {
		const dialog = document.getElementById(id);
		dialog.close();
	}

	function saveChanges() {
		let contentRoleName = null;
		if (selectedContentRole !== null) {
			const contentRole = contentRoles?.find(role => role.contentRoleId === parseInt(selectedContentRole));
			contentRoleName = contentRole ? contentRole.name : null;
		}

		packageFolder.displayName = packageFolderName;
		packageFolder.contentRoleId = isNaN(parseInt(selectedContentRole)) ? null : parseInt(selectedContentRole);
		packageFolder.name = contentRoleName;
		updateFolder(packageFolder);
		closeDialog();
	}

	function lockContentRoleSelection() {
		return !(parentFolder?.contentRoleId === null || parentFolder?.contentRoleId === undefined);
	}

	function fetchContentRoles() {
		Requests.getContentRoles().then((data) => {
			setContentRoles(data);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function fetchArchetypes() {
		Requests.getArchetypes().then((data) => {
			const capitalizedArchetypes = data.map(archetype => ({
				...archetype,
				name: capitalizeFirstLetter(archetype.name)
			}));
			setArchetypes(capitalizedArchetypes);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function updateFolder(data) {
		Requests.updatePackageFolder(data).then((response) => {
			if (!response || response.status === 401) {
				alert('You are not authorized to update this package folder.');
				return;
			}
			onUpdate(response);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}

	function addContentRole() {
		if (newContentRoleName.trim() === '') {
			setError('Content role name cannot be empty');
			return;
		}

		const duplicateRole = contentRoles.find(role => role.name.toLowerCase() === newContentRoleName.toLowerCase());
		if (duplicateRole) {
			setError('Content role name already exists');
			return;
		}

		setError('');

		const newRoleData = {
			name: newContentRoleName,
			archetypeId: selectedArchetypeId
		};

		Requests.addContentRole(newRoleData).then((newRole) => {
			if (!newRole || newRole.status === 401) {
				alert('You are not authorized to add a new content role.');
				return;
			}
			setContentRoles([...contentRoles, newRole]);
			setNewContentRoleName('');
			setSelectedArchetypeId(1);
			setShowAddContentRole(false);
			setSelectedContentRole(newRole.contentRoleId);
		}).catch((error) => {
			console.error('Failed to add content role:', error);
			navigate('/error');
		});
	}

	function handleContentRoleChange(event) {
		const value = event.target.value;
		if (value === 'add-new') {
			setShowAddContentRole(true);
			setSelectedContentRole(null);
		} else {
			setShowAddContentRole(false);
			setSelectedContentRole(value);
		}
	}

	function handleArchetypeChange(event) {
		setSelectedArchetypeId(parseInt(event.target.value));
	}

	return (
		<dialog id={id} className='edit-package-folder'>
			<div className='edit-package-content'>
				<div className='edit-package-header'>
					<h2>Edit Package Folder</h2>
					<button onClick={closeDialog}> <FontAwesomeIcon icon={faXmark} onClick={closeDialog} /></button>
				</div>
				<div className='edit-package-body'>
					<div className='package-title'>
						<label htmlFor='package-name'>Folder Name</label>
						<input id='package-name' type='text' placeholder='Enter folder name' defaultValue={packageFolderName} onChange={(event => setPackageFolderName(event.target.value))}></input>
					</div>
					<div className='package-content-role'>
						<label htmlFor='package-content-role'>Content Role</label>
						<select id='package-edit-content-role' value={selectedContentRole} onChange={handleContentRoleChange} disabled={lockContentRole}>
							{contentRoles?.map((role, index) => {
								if (lockContentRole) {
									if (role?.contentRoleId === parentFolder?.contentRoleId) {
										return (<option key={index} value={role.contentRoleId} selected>{role.name}</option>);
									}
								} else {
									return (<option key={index} value={role.contentRoleId}>{role.name}</option>);
								}
							})}
							<option value="null">All Users</option>
							<option value="add-new">Add New Content Role</option>
						</select>
					</div>
					{showAddContentRole && (
						<div className='add-content-role'>
							<label htmlFor='new-content-role'>New Content Role</label>
							<input id='new-content-role' type='text' placeholder='Enter new content role' value={newContentRoleName} onChange={(event => setNewContentRoleName(event.target.value))}></input>
							<label htmlFor='archetype-select'>Select Archetype</label>
							<select id='archetype-select' value={selectedArchetypeId} onChange={handleArchetypeChange}>
								{archetypes.map((archetype) => (
									<option key={archetype.archetypeId} value={archetype.archetypeId}>
										{capitalizeFirstLetter(archetype.name)}
									</option>
								))}
							</select>
							{error && <p className='error-message'>{error}</p>}
							<button onClick={addContentRole}>Add Content Role</button>
						</div>
					)}
				</div>
				<div className='edit-package-folder-footer'>
					<button onClick={saveChanges}>Save</button>
					<button onClick={closeDialog}>Cancel</button>
				</div>
			</div>
		</dialog>
	);
}
