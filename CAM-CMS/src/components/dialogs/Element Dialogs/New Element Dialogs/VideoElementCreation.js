import React, { useState } from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

import './styles/NewElementCreation.css';

import Requests from '../../../../utils/Requests';

export default function VideoElementCreation(props) {
	const [title, setTitle] = useState();
	const [description, setDescription] = useState('');
	const [citation, setCitation] = useState('');
	const [file, setFile] = useState();
	const [externalSource, setExternalSource] = useState('');
	const [externalSourceThumbnail, setExternalSourceThumbnail] = useState('');
	const navigate = useNavigate();

	function closeDialog() {
		document.getElementById('overlay').style.visibility = 'hidden';
		var dialog = document.getElementById(props.id); 
		dialog.close();
		resetDialog();
	}

	function resetDialog() {
		setTitle('');
		setDescription('');
		setCitation('');
		setFile(null);
		setExternalSource('');
		setExternalSourceThumbnail('');
	}

	function saveFile(e) {
		setFile(e.target.files[0]);
	}
	function clearDialog() {
		var dialogItems = document.getElementsByClassName('dialog-input');

		for (var index = 0; index < dialogItems.length; index++) {
			const element = dialogItems[index];
			element.value = '';
		}
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
		formData.append('Citation', citation);
		formData.append('CreatorId', 0);
		formData.append('LibraryFolderId', props.currentLibraryFolder);
		formData.append('LicenseId', 0);
		formData.append('TypeId', 4);
		
		if (props.uploadType === 0) {
			var fileType = file.type.split('/');
			if (fileType[0] == 'application') {
				fileType = fileType[1];
			}
			else {
				fileType = fileType[0];
			}
			formData.append('formFile', file);
		} else {
			formData.append('Content', JSON.stringify({ url: externalSource,Thumbnail: externalSourceThumbnail}));
		}
	
		postElement(formData);
	}


	return (
		<dialog id={props.id} className='image-dialog new-element-dialog'>
			<div className='dialog-header' id='new-element-header'>
				<h2>New Video Element</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form onSubmit={upload}>
				<div >
					<label htmlFor='text-element-title'>Title:</label>
				</div>
				<div>
					<input type='text' className='text-input' id='video-element-title' onChange={(event) => {setTitle(event.target.value);}} required />
				</div>
				<div>
					<label htmlFor='text-element-description'>Description:</label>
				</div>
				<div>
					<textarea className='description' type='text' id='video-element-description' onChange={(event) => {setDescription(event.target.value);}}/>
				</div>
				<div>
					<label htmlFor='text-element-citation'>Citation:</label>
				</div>
				<div>
					<input type='text' className='text-input' id='video-element-citation' onChange={(event) => {setCitation(event.target.value);}}/>
				</div>
				{props.uploadType === 0 ? (
					<div>
						<div>
							<label htmlFor='file-upload'>Upload File:</label>
						</div>
						<div>
							<input className='file-upload' type='file' onChange={saveFile} required/>
						</div>
						<div>
							<label htmlFor='other-upload'>Thumbnail Link:</label>
						</div>
						<div>
							<input type='text' className='text-input'  onChange={(event) => {setExternalSourceThumbnail(event.target.value);}} placeholder='optional'></input>
						</div>
					</div>
				) : (
				// Rendering elements when props.uploadType is not 'file'
					<div>
						<div>
							<label htmlFor='other-upload'>URL Upload:</label>
						</div>
						<div>
							<input type='text' className='text-input' id='video-url' onChange={(event) => {setExternalSource(event.target.value);}} required></input>
						</div>
						<div>
							<label htmlFor='other-upload'>Thumbnail Link:</label>
						</div>
						<div>
							<input type='text' className='text-input' onChange={(event) => {setExternalSourceThumbnail(event.target.value);}} placeholder='optional'></input>
						</div>
					</div>
				)}
				<div className='submit-row row'>
					<input type='submit' value='Upload' />
					<input type='reset' value='Clear' />
				</div>
			</form>
		</dialog>

	);
}