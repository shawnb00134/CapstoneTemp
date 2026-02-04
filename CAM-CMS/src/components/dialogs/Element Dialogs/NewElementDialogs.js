import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import './styles/ElementDialog.css';
import './styles/ExistingOrNewElementDialog.css';
import TextElementCreation from './New Element Dialogs/TextElementCreation';
import ImageElementCreation from './New Element Dialogs/ImageElementCreation';
import AnchorElementCreation from './New Element Dialogs/AnchorElementCreation';
import PDFElementCreation from './New Element Dialogs/PDFElementCreation';
import VideoElementCreation from './New Element Dialogs/VideoElementCreation';

/**
 * A NewElementDialog is a dialog that is used to create a new element
 * 
 * @param props the properties passed to the component 
 * @returns {NewElementDialog} a NewElementDialog component
 */
export default function NewElementDialog({ id, onChange, libraryFolder }) {
	const [type, setType] = useState(-1);
	const [uploadType, setUploadType] = useState(undefined);
	const [showUploadType, setShowUploadType] = useState(false);

	/**
	 * next dialog is used to open the next dialog in the process of creating a new element
	 * 
	 * @param {event} e 
	 */
	function nextDialog(e) {
		e.preventDefault();

		if (type === 0) {
			var textDialog = document.getElementById(id + '-new-text-element-creation-dialog');
			textDialog.show();
			closeDialog();
		}
		if (type === 1) {
			var imageDialog = document.getElementById(id + '-new-image-element-creation-dialog');
			imageDialog.showModal();
			closeDialog();
		}
		if (type === 2) {
			var pdfDialog = document.getElementById(id + '-new-pdf-element-creation-dialog');
			pdfDialog.showModal();
			closeDialog();
		}
		if (type === 3) {
			var videoDialog = document.getElementById(id + '-new-video-element-creation-dialog');
			videoDialog.showModal();
			closeDialog();
		}
		if (type === 4) {
			var anchorDialog = document.getElementById(id + '-new-anchor-element-creation-dialog');
			anchorDialog.showModal();
			closeDialog();
		}
	}

	/**
	 *  Closes the dialog
	 */
	function closeDialog() {
		var dialog = document.getElementById(id);
		dialog.close();
		resetDialog();
	}

	/**
	 * Resets the dialog state
	 */
	function resetDialog() {
		setType(-1);
		setUploadType(undefined);
		setShowUploadType(false);
	}

	/**
	 *  updates the type state
	 * @param {event} evt 
	 */
	function updateType(evt) {
		const selectedType = parseInt(evt.target.value);
		setType(selectedType);
		handleShowUploadType(selectedType);
	}

	function handleShowUploadType(selectedType) {
		if (selectedType === 1 || selectedType === 2) {
			setShowUploadType(true);
		} else {
			setShowUploadType(false);
		}
	}

	return (
		<>
			<TextElementCreation id={id + '-new-text-element-creation-dialog'} type={type} onChange={onChange} currentLibraryFolder={libraryFolder} />
			<ImageElementCreation id={id + '-new-image-element-creation-dialog'} type={type} onChange={onChange} uploadType={uploadType} currentLibraryFolder={libraryFolder} />
			<AnchorElementCreation id={id + '-new-anchor-element-creation-dialog'} type={type} onChange={onChange} currentLibraryFolder={libraryFolder} />
			<PDFElementCreation id={id + '-new-pdf-element-creation-dialog'} type={type} onChange={onChange} uploadType={uploadType} currentLibraryFolder={libraryFolder} />
			<VideoElementCreation id={id + '-new-video-element-creation-dialog'} type={type} onChange={onChange} uploadType={1} currentLibraryFolder={libraryFolder} />
			<dialog id={id} className='choice-dialog'>
				<div className='dialog-header'>
					<h2>New Element</h2>
					<button onClick={closeDialog}>
						<FontAwesomeIcon icon={faXmark} />
					</button>
				</div>
				<form className='dialog-content' onSubmit={nextDialog}>
					<div className='row select-element-type'>
						<select className='dialog-input' onChange={updateType} value={type} required>
							<option value={-1} disabled>Select Element Type</option>
							<option value={0}>Text Element</option>
							<option value={1}>Image Element</option>
							<option value={2}>PDF Element</option>
							<option value={3}>Video Element</option>
							<option value={4}>Anchor Element</option>
						</select>
					</div>
					<div className='upload-type' style={{ display: showUploadType ? 'block' : 'none' }}>
						<p>Select upload type</p>
						<div className='row'>
							<div className='column'>
								<input type='radio' id='file' name='element_type' value={0} onChange={() => { setUploadType(0); }} />
								<label htmlFor='text'>File</label>
							</div>
							<div className='column'>
								<input type='radio' id='url' name='element_type' value={1} onChange={() => { setUploadType(1); }} />
								<label htmlFor='text'>URL</label>
							</div>
						</div>
					</div>
					<div className='row'>
						<input type='submit' value='Next' />
					</div>
				</form>
			</dialog>
		</>
	);
}
