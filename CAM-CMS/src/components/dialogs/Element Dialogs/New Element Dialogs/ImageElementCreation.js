import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import './styles/NewElementCreation.css';
import Requests from '../../../../utils/Requests';

export default function ImageElementCreation(props) {
	const [title, setTitle] = useState('');
	const [description, setDescription] = useState('');
	const [citation, setCitation] = useState('');
	const [file, setFile] = useState(null);
	const [externalSource, setExternalSource] = useState('');
	const navigate = useNavigate();

	function closeDialog() {
		document.getElementById('overlay').style.visibility = 'hidden';
		var dialog = document.getElementById(props.id);
		dialog.close();
		resetDialog();
	}

	function saveFile(e) {
		setFile(e.target.files[0]);
	}

	function resetDialog() {
		setTitle('');
		setDescription('');
		setCitation('');
		setFile(null);
		setExternalSource('');
	}

	function clearDialog() {
		var dialogItems = document.getElementsByClassName('dialog-input');

		for (var index = 0; index < dialogItems.length; index++) {
			const element = dialogItems[index];
			element.value = '';
		}
		resetDialog();
	}

	function postElement(newElement) {
		Requests.createElement(newElement)
			.then(data => {
				props.onChange(data);
				clearDialog();
				closeDialog();
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	function upload(e) {
		e.preventDefault();
		var formData = new FormData();
		formData.append('ElementId', 0);
		formData.append('Title', title);
		formData.append('Description', description);
		formData.append('ExternalSource', externalSource);
		formData.append('CreatorId', 0);
		formData.append('Citation', citation);
		formData.append('LibraryFolderId', props.currentLibraryFolder);
		formData.append('LicenseId', 0);
		formData.append('TypeId', 2);

		if (props.uploadType === 0) {
			var fileType = file.type.split('/');
			if (fileType[0] === 'application') {
				fileType = fileType[1];
			} else {
				fileType = fileType[0];
			}
			formData.append('formFile', file);
		} else {
			formData.append('Content', JSON.stringify({ url: externalSource, Link: true }));
		}

		postElement(formData);
	}

	return (
		<dialog id={props.id} className='image-dialog new-element-dialog'>
			<div className='dialog-header' id='new-element-header'>
				<h2>New Image Element</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form onSubmit={upload}>
				<div>
					<label htmlFor='text-element-title'>Title:</label>
				</div>
				<div>
					<input type='text' className='text-input dialog-input' id='image-element-title' value={title} onChange={(event) => setTitle(event.target.value)} required />
				</div>
				<div>
					<label htmlFor='text-element-description'>Description:</label>
				</div>
				<div>
					<textarea className='description dialog-input' type='text' id='image-element-description' value={description} onChange={(event) => setDescription(event.target.value)} />
				</div>
				<div>
					<label htmlFor='text-element-citation'>Citation:</label>
				</div>
				<div>
					<input type='text' className='text-input dialog-input' id='image-element-citation' value={citation} onChange={(event) => setCitation(event.target.value)} />
				</div>
				{props.uploadType === 0 ? (
					<div>
						<div>
							<label htmlFor='file-upload'>Upload File:</label>
						</div>
						<div>
							<input className='file-upload dialog-input' type='file' onChange={saveFile} required />
						</div>
					</div>
				) : (
					<div>
						<div>
							<label htmlFor='other-upload'>URL Upload:</label>
						</div>
						<div>
							<input type='text' className='text-input dialog-input' id='image-url' value={externalSource} onChange={(event) => setExternalSource(event.target.value)} required />
						</div>
					</div>
				)}
				<div className='submit-row row'>
					<input type='submit' value='Upload' />
					<input type='reset' value='Clear' onClick={clearDialog} />
				</div>
			</form>
		</dialog>
	);
}
