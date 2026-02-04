import React from 'react';

export default function NewItemDialog({ id }) {

	function closeDialog() {
		document.getElementById(id).close();
	}

	function handleSubmit(event) {
		event.preventDefault();
	}

	return (
		<dialog id={id}>
			<header>
				<h1>New Item</h1>
				{/* TODO: Add X icon */}
				<button onClick={closeDialog}>X</button> 
			</header>
			<form onSubmit={handleSubmit}>
				<div>
					
				</div>
				<div>
					<button type='submit'>Create</button>
					<button onClick={closeDialog}>Cancel</button>
				</div>
			</form>
		</dialog>
	);
}
