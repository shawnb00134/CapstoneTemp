import {React, useEffect,useRef,useState} from 'react';
// import { Editor } from '@tinymce/tinymce-react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

import './styles/TextElementCreation.css';
import './styles/NewElementCreation.css';

import Requests from '../../../../utils/Requests';

import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import Quill from 'quill';
export default function TextElementCreation(props) {
	const navigate = useNavigate();
	const [title, setTitle] = useState();
	const [description, setDescription] = useState('');
	const [citation, setCitation] = useState('');
	const [textContent, setTextContent] = useState();

	const editorRef = useRef();

	useEffect(() => {
		// Add listener to package header for when 'selected-page' class is removed
		let classWatcher = new MutationObserver(function(mutations) {
			for (let mutation of mutations) {
				if (mutation.attributeName === 'open' && mutation.target.attributes.open) {
					document.getElementById('overlay').style.visibility = 'visible';
				}
				else if (mutation.attributeName === 'open') {
					document.getElementById('overlay').style.visibility = 'hidden';
				}
			}
		});

		const dialog = document.getElementById(props.id);
		classWatcher.observe(dialog, {attributes: true});
	}, []);

	function handleEditorChange(content) {
		setTextContent( content );
	}

	function closeDialog() {
		var dialog = document.getElementById(props.id); 
		dialog.close();
		resetDialog();
	}
	
	function resetDialog() {
		setTitle('');
		setDescription('');
		setCitation('');
		setTextContent('');
	}

	function clearDialog() {
		var dialogItems = document.getElementsByClassName('dialog-input');
		for (var index = 0; index < dialogItems.length; index++) {
			const element = dialogItems[index];
			element.value = '';
		}
	
		var container = document.getElementById('quill-editor');
		var editor = new Quill(container);
		editor.deleteText(0, editor.getLength());
		setTextContent('');
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
		formData.append('Citation', citation);
		formData.append('ExternalSource', '');
		formData.append('CreatorId', 0);
		formData.append('LibraryFolderId', props.currentLibraryFolder);
		formData.append('LicenseId', 0);
		formData.append('TypeId', 1);
		formData.append('Content', JSON.stringify({ content: textContent }));

		postElement(formData);
	}
	return (
		<><dialog className='text-creation-dialog new-element-dialog' id={props.id}>
			<div className='dialog-header' id='new-element-header'>
				<h2>New Text Element</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form onSubmit={upload}>
				<div >
					<label htmlFor='text-element-title'>Title:</label>
				</div>
				<div>
					<input type='text' className='text-input' id='text-element-title' onChange={(event) => {setTitle(event.target.value);}} required />
				</div>
				<div>
					<label htmlFor='text-element-description'>Description:</label>
				</div>
				<div>
					<textarea className='description' type='text' id='text-element-description' onChange={(event) => {setDescription(event.target.value);}}/>
				</div>
				<div>
					<label htmlFor='text-element-citation'>Citation:</label>
				</div>
				<div>
					<input type='text' className='text-input' id='text-element-citation' onChange={(event) => {setCitation(event.target.value);}}/>
				</div>
				<div className='text-element-content'>
					<label htmlFor='text-element-content'>Content:</label>
					<ReactQuill
						className='quill-editor'
						id='quill-editor'
						value= {textContent}
						onChange = {handleEditorChange}
						ref={editorRef}
						  	/>
				</div>
				<div className='submit-row row'>
					<input type='submit' value='Upload' />
					<input type='reset' value='Clear' />
				</div>
			</form>
		</dialog>
		<div id='overlay'></div></>
	);
}