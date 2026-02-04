import React, { useState } from 'react';
import './styles/AddUserDialog.css';

export function AddUserDialog({ show, onClose, onAddUser }) {
	const [firstName, setFirstName] = useState('');
	const [lastName, setLastName] = useState('');
	const [email, setEmail] = useState('');
	const [phone, setPhone] = useState('');

	const handleAddUser = () => {
		if (firstName && lastName && email) {
			const newUser = { firstName, lastName, email, phone };
			onAddUser(newUser);
			onClose();
		} else {
			alert('Please fill out all fields.');
		}
	};

	if (!show) return null;

	return (
		<div className="add-user-dialog">
			<div className="add-user-dialog-content">
				<h2>Add User</h2>
				<div className="add-user-dialog-field">
					<label>First Name</label>
					<input type="text" value={firstName} onChange={e => setFirstName(e.target.value)} required />
				</div>
				<div className="add-user-dialog-field">
					<label>Last Name</label>
					<input type="text" value={lastName} onChange={e => setLastName(e.target.value)} required />
				</div>
				<div className="add-user-dialog-field">
					<label>Email</label>
					<input type="email" value={email} onChange={e => setEmail(e.target.value)} required />
				</div>
				<div className="add-user-dialog-field">
					<label>Phone</label>
					<input type="text" value={phone} onChange={e => setPhone(e.target.value)} required />
				</div>
				<div className="add-user-dialog-actions">
					<button onClick={handleAddUser}>Add User</button>
					<button onClick={onClose}>Cancel</button>
				</div>
			</div>
		</div>
	);
}