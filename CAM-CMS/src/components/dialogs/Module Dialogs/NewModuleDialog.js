import React, { useState } from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

import Requests from '../../../utils/Requests';

import './styles/NewModuleDialog.css';

export default function NewModuleDialog({ id, onChange, libraryFolder, isTemplate }) {
	const navigate = useNavigate();
	const [title, setTitle] = useState();
	const [description, setDescription] = useState();

	async function upload(e) {
		
		e.preventDefault();

		var data = {
			title: title,
			description: description,
			elementSets: [],
			moduleId: 1,
			surveyStartLink: null,
			surveyEndLink: null,
			contactTitle: null,
			contactNumber: null,
			thumbnail: null,
			tags: [],
			displayTitle: title,
			templateId: -1,
			isTemplate: false,
			publishedTimestamp: null,
			libraryFolderId: libraryFolder,
			filename: null
		};
	
		Requests.createModule(data)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You are not authorized to create a new module.');
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
		<dialog id={id} className='module-dialog'>
			<div className='dialog-header'>
				<h2>New {isTemplate === undefined || !isTemplate ? 'Module' : 'Template'}</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form className='dialog-content' onSubmit={upload}>
				<div>
					<label htmlFor='title'>Title:</label>
				</div>
				<div className='row'>
					<input id='title-input' className='dialog-input' name='title' placeholder='Title' onChange={(event) => { setTitle(event.target.value); }} required />
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