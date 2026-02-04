import React, { useState } from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';

import NewElementDialog from './NewElementDialogs';

import ExistingElements from './ExistingElementsDialog';

export default function ExistingOrNewElementDialog(props) {
	const [type, setType] = useState();
	const [loadElements, setLoadElements] = useState(false);

	function newDialog(e) {

		e.preventDefault();

		closeDialog();
		if (type === 'new') {
			document.getElementById(props.id + '-new-element-dialog').showModal();
		} else {
			setLoadElements(true);
			document.getElementById(props.id + '-existing-elements-dialog').showModal();
		}
	}
	function closeDialog() {
		var dialog = document.getElementById(props.id);
		dialog.close();
	}
	function updateType(evt) {
		setType(evt.target.value);
	}
	return (
		<>
			<NewElementDialog id={props.id + '-new-element-dialog'} onChange={props.onChange} libraryFolder={props.libraryFolderId} />
			<ExistingElements id={props.id + '-existing-elements-dialog'} onChange={props.onChange} refresh={loadElements} />
			<dialog id={props.id} className='choice-dialog'>
				<div className='dialog-header'>
					<h3>Existing or New Element</h3>
					<button onClick={closeDialog}>
						<FontAwesomeIcon icon={faXmark} />
					</button>
				</div>
				<form id='module-element-dialog-form' className='dialog-content' onSubmit={newDialog}>
					<div className='row radio-row'>
						<div className='column'>
							<input type='radio' id='file' name='element_type' value='existing' onChange={updateType} required />
							<label htmlFor='file'>Existing Element</label>
						</div>
						<div className='column'>
							<input type='radio' id='text' name='element_type' value='new' onChange={updateType} />
							<label htmlFor='text'>New Element</label>
						</div>
					</div>
					<div className='row'>
						<input id='input-existing-button' type='submit' value='Next' />
					</div>
				</form>
			</dialog>
		</>
	);
}