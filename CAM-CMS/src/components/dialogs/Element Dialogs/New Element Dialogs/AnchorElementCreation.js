import {React, useState} from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';

import { useNavigate } from 'react-router-dom';

import './styles/NewElementCreation.css';

import Requests from '../../../../utils/Requests';

export default function AnchorElementCreation(props) {
	const [title, setTitle] = useState();
	const [headingLevel, setHeadingLevel] = useState('0');
	const navigate = useNavigate();

	function closeDialog() {
		document.getElementById('overlay').style.visibility = 'hidden';
		var dialog = document.getElementById(props.id); 
		dialog.close();
		resetDialog();
	}

	function resetDialog() {
		setTitle('');
		setHeadingLevel('0');
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
				navigate('error');
			});
	}

	function upload(e){
		e.preventDefault();

		var formData = new FormData();

		formData.append('ElementId', 0);
		formData.append('Title', title);
		formData.append('Description', '');
		formData.append('Citation', '');
		formData.append('ExternalSource', '');
		formData.append('CreatorId', 0);
		formData.append('LibraryFolderId', props.currentLibraryFolder);
		formData.append('LicenseId', 0);
		formData.append('TypeId', 7);
		formData.append('Content',JSON.stringify({content: '', headingLevel: headingLevel}));

		postElement(formData);
	}
	return (
		<><dialog id={props.id} className='anchor-dialog new-element-dialog'>
			<div className='dialog-header' id='new-element-header'>
				<h2>New Anchor Element</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form onSubmit={upload}>
				<div>
					<label htmlFor='title'>Title:</label>
				</div>
				<div>
					<input className='text-input' id='anchor-element-title' onChange={(event) => {setTitle(event.target.value);}}/>
				</div>
				<div>
					<label htmlFor='Heading-level'>Heading level:</label>
				</div>
				<div>
					<input className='text-input' id='anchor-element-heading' type='number' min={0} max={4} placeholder='0' onChange={(event) =>{setHeadingLevel(event.target.value);}}/>
				</div>
				<div className='submit-row row'>
					<input type='submit' value='Upload' />
					<input type='reset' value='Clear' />
				</div>
			</form>
		</dialog><div id='overlay'></div></>
	);
}