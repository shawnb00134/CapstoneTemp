import React, { useState} from 'react';
import SelectingPublishedModuleDialog from './SelectingPublishedModuleDialog';

export default function NewItemDialog({ id, onFolder, onModule }) {

	const [loadModules, setLoadModules] = useState(false);

	function closeDialog() {
		document.getElementById(id).close();
	}
	
	function handleSubmit(event) {
		event.preventDefault();

		let choice = event.target.new_item.value;
		if (choice === 'folder') {
			onFolder();
		}
		else {
			setLoadModules(true);
			document.getElementById(id +'-selectModuleDialog').showModal();
			
		}

		closeDialog();
	}
	function handleSelection(module) {
		onModule(module);
	}

	return (
		<>
			<SelectingPublishedModuleDialog id={id +'-selectModuleDialog'} onRefresh={loadModules} onSelect={handleSelection} />
			<dialog id={id}>
				<header>
					<h1>New Item</h1>
					{/* TODO: Add X icon */}
					<button onClick={closeDialog}>X</button>
				</header>
				<form onSubmit={handleSubmit}>
					<div>
						<label htmlFor='new_item'>Folder</label>
						<input type='radio' id='folder' name='new_item' value='folder' required defaultChecked />
						<label htmlFor='module'>Module</label>
						<input type='radio' id='module' name='new_item' value='module' />
					</div>
					<div>
						<button id='new-dialog-create-button' type='submit'>Create</button>
						<button onClick={closeDialog}>Cancel</button>
					</div>
				</form>
			</dialog></>
	);
}
