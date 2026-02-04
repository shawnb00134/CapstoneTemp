import React from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

import Requests from '../../../utils/Requests';

export default function NewPackageDialog({ id, onChange }) {
	const navigate = useNavigate();

	const [title, setTitle] = React.useState();

	function upload(e) {
		e.preventDefault();

		let newPackage = {
			name: title,
			packageType: 1,
			isCore: false
		};

		Requests.createPackage(newPackage)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You are not authorized to create a new package.');
					closeDialog();
					return;
				}
				closeDialog();
				onChange(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	function closeDialog() {
		setTitle('');

		let inputs = document.getElementsByClassName('dialog-input');
		for (let i = 0; i < inputs.length; i++) {
			inputs[i].value = '';
		}

		var dialog = document.getElementById(id);
		dialog.close();
	}

	return (
		<dialog id={id} className='module-dialog'>
			<div className='dialog-header'>
				<h2>New Package</h2>
				<button onClick={closeDialog}>
					<FontAwesomeIcon icon={faXmark} />
				</button>
			</div>
			<form className='dialog-content' onSubmit={upload}>
				<div>
					<label htmlFor='name'>Name:</label>
				</div>
				<div className='row'>
					<input id='name-input' className='dialog-input' name='name' placeholder='Title' onChange={(event) => { setTitle(event.target.value); }} required />
				</div>

				<div>
					<label htmlFor='type'>Type:</label>
				</div>
				<div className='row'>
					{/* TODO: Package Type */}
				</div>

				<div className='submit-row row'>
					<input id='add-package-add' type='submit' value='Upload' />
					<input type='reset' value='Clear' />
				</div>
			</form>
		</dialog>
	);
}