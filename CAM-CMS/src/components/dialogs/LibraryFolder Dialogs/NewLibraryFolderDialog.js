import React, { useState } from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

import Requests from '../../../utils/Requests';

export default function NewLibraryFolderDialog({id, onChange}) {

	const [title, setTitle] = useState();
	const [description, setDescription] = useState();
	const navigate = useNavigate();


	async function upload(e) {
		
		e.preventDefault();

		var data = {
			name: title,
			description: description,
			libraryFolderId: 0,
		};
		Requests.createLibraryFolder(data)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You are not authorized to create a new folder.');
					clearDialog();
					closeDialog();
					return;
				}
				if (!response.ok) {
					alert('Folder creation failed.');
					clearDialog();
					closeDialog();
					return;
				}
				onChange(response);

				clearDialog();
				closeDialog();
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}


	function clearDialog() {
		var dialogItems = document.getElementsByClassName('dialog-input');

		for (var index = 0; index < dialogItems.length; index++) {
			const element = dialogItems[index];
			element.value = '';
		}
	}

	function closeDialog() {
		var dialog = document.getElementById(id);
		dialog.close();
	}
	return (
		<dialog id={id} >
			<div className='dialog-header'>
				<h2>New Folder</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form className='dialog-content' onSubmit={upload}>
				<div>
					<label htmlFor='title'>Title:</label>
				</div>
				<div className='row'>
					<input id='title-input' className='dialog-input' name='title' placeholder='Title'  onChange={(event) => { setTitle(event.target.value); }} required />
				</div>
				<div>
					<label htmlFor='text-element-description'>Description:</label>
				</div>
				<div>
					<textarea className='description' type='text' id='text-element-description' onChange={(event) => {setDescription(event.target.value);}}/>
				</div>

				<div className='submit-row row'>
					<input type='submit' value='Upload' />
					<input type='reset' value='Clear' />
				</div>
			</form>
		</dialog>
	);
}