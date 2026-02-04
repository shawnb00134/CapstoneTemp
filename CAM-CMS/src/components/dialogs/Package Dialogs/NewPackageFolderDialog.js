import {React, useState,useEffect} from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import './styles/NewPackageFolderDialog.css';
import Requests from '../../../utils/Requests';
export default function NewPackageFolderDialog({folderDialogData, loadContentRoles,createPackageFolder}) {
	const [contentRoles, setContentRoles] = useState([]);
	const [contentSelectorLocked, setContentSelectorLocked] = useState(false);
	const [folderName, setFolderName] = useState('');
	const navigate = useNavigate();

	useEffect(() => {
		if(loadContentRoles) {
			fetchContentRoles();
		}
		let isLocked = lockContentRoleSelection();
		setContentSelectorLocked(isLocked);
	}, [loadContentRoles]);
	
	function fetchContentRoles() {
		Requests.getContentRoles().then((data) => {
			setContentRoles(data);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});

	}
	function closeDialog() {
		setContentRoles([]);
		const folderName = document.getElementById('package-name');
		folderName.value = '';
		document.getElementById('new-package-folder-dialog').close();
	}
	function createFolder() {
		const contentRole = document.getElementById('package-content-role').value;
		var folderdata = {
			displayName : folderName,
			contentRoleId : isNaN(parseInt(contentRole)) ? null : parseInt(contentRole),
			parentFolderId : folderDialogData.parentFolderId ? parseInt(folderDialogData.parentFolderId) : null
		};
		createPackageFolder(folderdata);
		setFolderName('');
		closeDialog();
	} 
	function lockContentRoleSelection() {
		const selector = document.getElementById('package-content-role');
		if((folderDialogData?.contentRoleId === null || folderDialogData?.contentRoleId === undefined)) {
			selector.disabled = false;
			return false;
		} else {
			selector.disabled = true;
			return true;
		}
	}
	return (
		<dialog id='new-package-folder-dialog'>
			<div className='new-package-folder-content'>
				<div className='new-package-folder-header'>
					<h2>New Package Folder</h2>
					<button onClick={closeDialog}> <FontAwesomeIcon icon={faXmark} /></button>
				</div>
				<div className='new-package-folder-body'>
					<div className='package-title'>
						<label htmlFor='package-name'>Folder Name</label>
						<input id='package-name' type='text' placeholder='Enter folder name' onChange={(event) => setFolderName(event.target.value)}></input>
					</div>
					<div className='package-content-role'>
						<label htmlFor='package-content-role'>Content Role</label>
						<select id='package-content-role'>
							<option value={null}>All Users</option>
							{contentRoles?.map((role, index) => {
								if(contentSelectorLocked) {
									if(role?.contentRoleId === folderDialogData?.contentRoleId) {
										return (<option key={index} value={role?.contentRoleId} selected>{role?.name}</option>);
									}
								} else {
									return (<option key={index} value={role?.contentRoleId}>{role?.name}</option>);
								}
							})}
						</select>
					</div>
				</div>
				<div className='new-package-folder-footer'>
					<button onClick={createFolder}>Save</button>
					<button onClick={closeDialog}>Cancel</button>

				</div>
			</div>
		</dialog>
	);
}