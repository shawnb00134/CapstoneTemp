import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faPencil, faFloppyDisk, faBan, faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import ReactPlayer from 'react-player';
import Requests from '../utils/Requests';
import './styles/ElementPage.css';

/**
 *	The page that displays the elements
 * @param props the properties passed to the component
 * @returns {ElementPage} The page that displays the elements content
 */
export default function ElementsPage() {
	const navigate = useNavigate();
	const { id } = useParams();
	const [element, setElement] = useState(undefined);
	const [editMode, setEditMode] = useState(false);
	const [fileMode, setFileMode] = useState(false);
	const [file, setFile] = useState('');
	const [description, setDescription] = useState('');
	const [tags, setTags] = useState('');
	const [citation, setCitation] = useState('');
	const [source, setSource] = useState('');
	const [content, setContent] = useState('');
	const [editorDisplayContent, setEditorDisplayContent] = useState('');
	const [canEdit, setCanEdit] = useState(false);
	const [canDelete, setCanDelete] = useState(false);


	useEffect(() => {
		setElement(undefined);
		Requests.getElementById(id).then((response) => {
			if (!response || response.status === 404 || response.status === 401) {
				navigate('/404');
				return;
			}
			response.type = response.typeId === 1 ? 'text' : response.typeId === 2 ? 'image' : response.typeId === 5 ? 'pdf' : response.typeId === 4 ? 'video' : response.typeId === 7 ? 'anchor' : undefined;
			setElement(response);
			Requests.authorizeLibraryFolder('edit', response.libraryFolderId).then((response) => {
				setCanEdit(response);
			});

			Requests.authorizeLibraryFolder('delete', response.libraryFolderId).then((response) => {
				setCanDelete(response);
			});
		}).catch(error => {
			console.error(error);
			navigate('/error');
		});
		
	}, [id]);

	useEffect(() => {
		const details = document.getElementsByClassName('details');
		for (let i = 0; i < details.length; i++) {
			if (editMode) {
				details[i].classList.remove('disabled');
			} else {
				details[i].classList.add('disabled');
			}
		}
		if (element !== undefined) {
			setDescription(element.description);
			setTags(element.tags ? element.tags.join(',') : '');
			setCitation(element.citation);
			setSource(element.type === 'video' ? getVideoContent().url : element.externalSource);
			setContent(element.type === 'text' ? getTextContent() : element.type === 'video' ? getVideoContent() : element);
			setEditorDisplayContent(element.type === 'text' ? getTextContent().content : element.type === 'video' ? getVideoContent().content : element);
		}
	}, [element, editMode]);

	/**
	 *	gets the text content of the element
	 * 
	 * @returns a string of the text content of the element
	 */
	function getTextContent() {
		if (element.content === null || element.content === undefined) {
			return '';
		}
		return JSON.parse(element.content);
	}

	/**
	 * get video content of the element
	 * 
	 * @returns a string of the video content of the element
	 */
	function getVideoContent() {
		if (element.content === null || element.content === undefined) {
			return 'Processing video...';
		}
		return JSON.parse(element.content);
	}

	/**
	 *  trys to parse the content of the element to get the url if not a url element it will return the content
	 * 
	 * @returns the content of the element
	 */
	function getContent() {
		try {
			return JSON.parse(element.content).url;
		} catch (error) {
			return element.content;
		}
	}

	/**
	 *  Handles the change of the editor
	 * @param content the new content of the editor
	*/
	function handleEditorChange(editorContent) {
		let textContent = { content: editorContent };
		setContent(textContent);
		setEditorDisplayContent(editorContent);
	}

	/**
	 * Posts the updated element to the database
	 * 
	 * @param element the element to be updated
	 * 
	 */
	function postElementUpdate(element) {
		Requests.updateElement(element).then((response) => {
			if (!response || response.status === 401) {
				alert('You are not authorized to update this element.');
				return;
			}
			response.type = response.typeId === 1 ? 'text' : response.typeId === 2 ? 'image' : response.typeId === 5 ? 'pdf' : response.typeId === 4 ? 'video' : response.typeId === 7 ? 'anchor' : undefined;
			setEditMode(false);
			setElement(response);
		}).catch(error => {
			console.error(error);
			navigate('/error');
		});
	}

	function postElementUpdateFile(element) {
		Requests.updateElementWithFile(element).then((response) => {
			if (!response || response.status === 401) {
				alert('You are not authorized to update this element.');
				return;
			}
		}).catch(error => {
			console.error(error);
			navigate('/error');
		});
	}

	function toggleEditMode() {
		if (editMode) {
			const confirmed = window.confirm('Are you sure you want to cancel editing? Any unsaved changes will be lost.');
			if (!confirmed) {
				return;
			}
			if (!fileMode) {
				element.description = description;
				element.tags = tags.split(',');
				element.citation = citation;
				element.externalSource = source;
				// TODO: Update external source if it is changed using the edit source dialog
				element.content = JSON.stringify(content);
				postElementUpdate(element);
			} else {
				var formData = new FormData();
				formData.append('ElementId', element.elementId);
				formData.append('Title', element.title);
				formData.append('Description', description);
				formData.append('ExternalSource', source);
				formData.append('CreatorId', element.createdBy);
				formData.append('Citation', citation);
				formData.append('Content', JSON.stringify(content));
				formData.append('LibraryFolderId', element.libraryFolderId);
				formData.append('tags', tags.split(','));
				formData.append('formFile', file);
				postElementUpdateFile(formData);
			}
		}
		setEditMode(!editMode);
	}

	function handleCancel() {
		// TODO: Reset source
		const confirmed = window.confirm('Are you sure you want to cancel editing? Any unsaved changes will be lost.');
		if (!confirmed) {
			return;
		}
		const descriptionElement = document.getElementById('description');
		const citationElement = document.getElementById('citation');
		const tagsElement = document.getElementById('tags');
		descriptionElement.innerHTML = element.description;
		citationElement.value = element.citation;
		tagsElement.value = element.tags;
		setCitation(element.citation);
		setEditMode(false);
	}

	function handleUrlSourceChange(source) {
		setFileMode(false);
		setSource(source);
		try {
			let contentJson = JSON.parse(element.content);
			contentJson.url = source;
			setContent(contentJson);
		} catch (error) {
			let urlContent = {
				url: source,
				Link: 'true'
			};
			setContent(urlContent);
		}
	}

	function handleFileSourceChange(file) {
		setFileMode(true);
		setSource('');
		setFile(file);
	}

	function handleDeleteElement() {
		if (window.confirm('Are you sure you want to delete the element? All content will be lost.')) {
			element.confirmDelete = true;
			Requests.deleteElement(element)
				.then(response => {
					if (response.status === 401) {
						alert('You are not authorized to delete this element');
					}
					if (response.status === 200) {
						navigate('/studio');
					}
				})
				.catch(error => {
					if (error.response && error.response.status === 409) {
						console.error('Failed to delete, element currently in use:', error);
						alert('Failed to delete, element currently in use');
					} else {
						console.error(error);
						navigate('/error');
					}
				});
		}
	}

	if (element === undefined) {
		return <div className='loading'>Loading...</div>;
	}

	return (
		<div className='element-display'>
			<EditElement type={element.type} id='edit-source-dialog' handleUrlSourceChange={handleUrlSourceChange} handleFileSourceChange={handleFileSourceChange} />
			<div className='element-heading'>
				<div className='element-heading-main'>
					<div className='row'>
						<h1>{element.title}</h1>
						<h2 className='element-heading-meta-data'>
							<FontAwesomeIcon icon={faCircleInfo} className='element-heading-meta-data-icon' onClick={() => {
								const metaDialog = document.getElementsByClassName('element-meta-data')[0];
								metaDialog.style.display = metaDialog.style.display === 'none' || metaDialog.style.display === '' ? 'flex' : 'none';
							}} />
							<div className='element-meta-data'>
								<h5>Created At: {element.createdAt}</h5>
								<h5 className='data-link' onClick={() => navigate('/people/' + element.createdBy)}>Created By: User {element.createdBy}</h5>
								{element.updatedAt ? <>
									<h5>Updated At: {element.updatedAt}</h5>
									<h5 className='data-link' onClick={() => navigate('/people/' + element.updatedBy)}>Updated By: User {element.updatedBy}</h5>
								</> : <></>}
							</div>
						</h2>
					</div>
					<small>{element.type}</small>
				</div>
				<>
					<button className={canDelete ? 'element-button-delete' : 'element-button-delete unauthorized'}id='delete-button' onClick={handleDeleteElement}>
						<span>Delete</span> <FontAwesomeIcon icon={faTrash} />
					</button>
					<button style={{ display: editMode ? '' : 'none' }} className='element-button-cancel' id='edit-cancel' onClick={handleCancel}>
						<span>Cancel</span> <FontAwesomeIcon icon={faBan} size='lg' />
					</button>
					<button className='element-button-edit' id='edit-button' onClick={toggleEditMode}>
						{editMode ? <><span>Save</span> <FontAwesomeIcon icon={faFloppyDisk} size='lg' /></> : <><span>Edit</span> <FontAwesomeIcon icon={faPencil} size='lg' /></>}
					</button>
				</>
			</div>
			<div className='element-details'>
				<div className='element-data'>
					<div className='element-data-left'>
						<div className='details disabled'>
							<label htmlFor='description'>Description:</label>
							<textarea id='description' disabled={!editMode} value={description} onChange={(evt) => setDescription(evt.target.value)} />
						</div>
					</div>
					<div className='element-data-right'>
						<div className='details disabled'>
							<label id='tag-row' htmlFor='tags'>Tags:</label>
							<textarea id='tags' disabled={!editMode} value={tags} onChange={(evt) => setTags(evt.target.value)} />
						</div>
						<div className='details disabled'>
							<label htmlFor='citation'>Citation:</label>
							<input id='citation' disabled={!editMode} value={citation} onChange={(evt) => setCitation(evt.target.value)} />
						</div>
						<div className='details disabled'>
							{element.type !== 'text' && element.type !== 'anchor' ? <>
								<label htmlFor='source'>Source:
									<button style={{ display: editMode ? '' : 'none' }} className={canEdit ? 'edit-source-button' : 'edit-source-button unauthorized'} onClick={() => {
										const editDialog = document.getElementById('edit-source-dialog');
										editDialog.showModal();
									}}>Edit</button>
								</label>
								<input id='source' disabled value={source} />
							</> : <></>}
						</div>
					</div>
				</div>
				<div className='element-content'>
					<div id='editor-space'>
						{element.type === 'text' ? <ReactQuill className='quill-editor' value={editorDisplayContent} onChange={handleEditorChange} readOnly={!editMode} />
							: element.type === 'image' ? <img loading='lazy' src={getContent()} />
								: element.type === 'pdf' ? <object data={getContent()} id='cypress-pdf' type='application/pdf' width='100%' height='100%'>
									<p>Unable to display PDF file.</p>
								</object>
									: element.type === 'video' ? <ReactPlayer className='cypress-video' url={getVideoContent().url} controls={true} light={getVideoContent().thumbnail ? getVideoContent().thumbnail : true}>
										<p>Unable to display video file.</p>
									</ReactPlayer>
										: element.type === 'anchor' ? <div className='anchor-element-content'><h1>{element.title}</h1></div> : <div>Invalid element type</div>}
					</div>
				</div>
			</div>
		</div>
	);
}

function EditElement({ type, id, handleUrlSourceChange, handleFileSourceChange }) {
	const typeMap = {
		'image': ['image/*'],
		'pdf': ['application/pdf'],
		'video': ['video/*']
	};

	const [selection, setSelection] = useState('link');
	const [urlString, setUrlString] = useState(undefined);
	const [file, setFile] = useState(undefined);

	const uploadOption = <>
		<label htmlFor='file'>File:</label>
		<input id='element-new-file' type='file' name='file' onChange={(evt) => validateFile(evt.target.files)} required />
	</>;

	const linkOption = <>
		<label htmlFor='source'>URL:</label>
		<input id='element-new-url' type='text' name='source' onChange={(evt) => validateUrl(evt.target.value)} required />
	</>;

	function closeDialog() {
		clearFileInput();
		clearUrlInput();
		const dialog = document.getElementById(id);
		dialog.close();
	}

	function clearFileInput() {
		const fileInput = document.getElementById('element-new-file');
		if (fileInput) {
			fileInput.value = '';
			fileInput.files = null;
		}
	}

	function clearUrlInput() {
		const urlInput = document.getElementById('element-new-url');
		if (urlInput) {
			urlInput.value = '';
		}
	}

	function validateFile(files) {
		if (files.length <= 0) {
			return;
		}
		if (files.length > 1) {
			alert('Please select only one file');
			clearFileInput();
			return;
		}
		const formFile = files[0];
		let typeRegex = new RegExp(typeMap[type].join('|'));
		if (!typeRegex.test(formFile.type)) {
			alert('Invalid file type');
			clearFileInput();
			return;
		}
		setFile(formFile);
	}

	function validateUrl(urlString) {
		if (urlString === undefined || urlString === '') {
			return;
		}
		try {
			var urlObject = new URL(urlString);
			if (!urlObject) {
				throw new Error('Invalid URL');
			}
			setUrlString(urlString);
		} catch (error) {
			alert('Invalid URL');
			return;
		}
	}

	function handleSubmit(evt) {
		evt.preventDefault();
		if (selection === 'link') {
			validateUrl(urlString);
			handleUrlSourceChange(urlString);
			let dialog = document.getElementById(id);
			dialog.close();
			return;
		}
		handleFileSourceChange(file);
		let dialog = document.getElementById(id);
		dialog.close();
	}

	return (
		<dialog id={id} className='edit-element-dialog'>
			<div className='edit-element-dialog-header row'>
				<h1>Edit {type}</h1>
				<button onClick={closeDialog}>Cancel</button>
			</div>
			<div className='source-selection row'>
				<div>
					<label htmlFor='link_option'>Link</label>
					<input type='radio' name='source' value='link' defaultChecked required onChange={(evt) => setSelection(evt.target.value)} />
				</div>
				{type !== 'video' ? <div>
					<label htmlFor='upload_option'>Upload</label>
					<input type='radio' name='source' value='upload' required onChange={(evt) => setSelection(evt.target.value)} />
				</div> : <></>}
			</div>
			<div className='source-input row'>
				{selection === 'link' ? linkOption : uploadOption}
			</div>
			<input type='submit' value='Save' onClick={handleSubmit} />
		</dialog>
	);
}
