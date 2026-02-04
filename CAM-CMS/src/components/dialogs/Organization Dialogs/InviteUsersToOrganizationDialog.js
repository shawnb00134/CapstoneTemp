import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import InviteUsersConfirmationDialog from './InviteUsersConfirmationDialog';
import './styles/InviteUsersToOrganizationDialog.css';

export default function InviteUsersToOrganizationDialog({ isOpen, onClose, contentRoles, modules, organizationId, sendInvites }) {
	console.log(organizationId);
	const [userInput, setUserInput] = useState('');
	const [validUsers, setValidUsers] = useState(new Set());
	const [contentRole, setContentRole] = useState('');
	const [usersToInvite, setUsersToInvite] = useState([]);
	const [emailSubject, setEmailSubject] = useState('');
	const [emailMessage, setEmailMessage] = useState('');
	const [smsMessage, setSmsMessage] = useState('');
	const [initialModule, setInitialModule] = useState('');
	const [isConfirmationDialogOpen, setIsConfirmationDialogOpen] = useState(false);
	const [inviteData, setInviteData] = useState(null);

	const validateInput = (input) => {
		const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
		const phoneRegex = /^\+?([0-9]{1,3})?[-. (]?([0-9]{3})[-. )]?([0-9]{3})[-. ]?([0-9]{4})$/;
		return emailRegex.test(input) || phoneRegex.test(input);
	};

	function handleAddUsers() {
		const newUsers = userInput
			.split(',')
			.map((user) => user.slice(0, 100).trim())
			.filter((user) => user);

		const newValidUsers = new Set([...validUsers]);
		newUsers.forEach((user, idx) => {
			if (validateInput(user)) {
				newValidUsers.add(usersToInvite.length + idx);
			}
		});

		setUsersToInvite([...usersToInvite, ...newUsers]);
		setValidUsers(newValidUsers);
		setUserInput('');
	}

	function handleChange(e, limit) {
		const { name, value } = e.target;
		switch (name) {
		case 'addUsers':
			setUserInput(value.slice(0, limit));
			break;
		case 'emailSubject':
			setEmailSubject(value.slice(0, limit));
			break;
		case 'emailMessage':
			setEmailMessage(value);
			break;
		case 'smsMessage':
			setSmsMessage(value.slice(0, limit));
			break;
		default:
			break;
		}
	}

	function handleRemoveUser(indexToRemove) {
		const updatedUsers = usersToInvite.filter((_, index) => index !== indexToRemove);
		const updatedValidUsers = new Set([...validUsers].filter(index => index !== indexToRemove));
		const reindexedValidUsers = new Set(Array.from(updatedValidUsers).map(index => index > indexToRemove ? index - 1 : index));

		setUsersToInvite(updatedUsers);
		setValidUsers(reindexedValidUsers);
	}

	function generateInviteLink(contentRoleId) {
		var role = contentRoles.find(role => role.contentRoleId.toString() === contentRoleId);

		if (!role) {
			console.error('Selected content role not found');
			role = contentRoles[0];
		}
		const encodedRoleName = encodeURIComponent(role.displayName);
		return `https://.com/join/${encodedRoleName}`;
	}

	function handleSubmit() {
		if (usersToInvite.some((_, index) => !validUsers.has(index))) {
			const userConfirmation = window.confirm('Some user entries are invalid. Do you still want to proceed?');
			if (userConfirmation) {
				proceedWithSubmission();
			}
		} else {
			proceedWithSubmission();
		}
	}

	function proceedWithSubmission() {
		const inviteLink = generateInviteLink(contentRole);
		const updatedEmailMessage = `${emailMessage} \nPlease join here: ${inviteLink}`;
		const updatedSmsMessage = `${smsMessage} \nJoin here: ${inviteLink}`;
		const data = {
			contentRole,
			users: usersToInvite,
			emailSubject,
			emailMessage: updatedEmailMessage,
			smsMessage: updatedSmsMessage,
			initialModule,
		};

		setInviteData(data);
		setIsConfirmationDialogOpen(true);
	}

	function handleBackToEdit() {
		setIsConfirmationDialogOpen(false);
	}

	function handleSendInvites() {
		sendInvites(inviteData);
		handleCloseDialog();
	}
	function userLabel(count) {
		return `${count} User${count === 1 ? '' : 's'} to invite:`;
	}

	if (!isOpen) {
		return null;
	}

	function handleCloseDialog() {
		setContentRole('');
		setUserInput('');
		setUsersToInvite([]);
		setEmailSubject('');
		setEmailMessage('');
		setSmsMessage('');
		setInitialModule('');
		setIsConfirmationDialogOpen(false);
		onClose();
	}

	const disableAll = contentRoles.length === 0;

	return (
		<>
			<dialog open={isOpen && !isConfirmationDialogOpen} className="invite-users-dialog">
				<div className="invite-users-content">
					<div className="invite-users-header">
						<h2>Invite User(s)</h2>
						<button onClick={handleCloseDialog}>
							<FontAwesomeIcon icon={faXmark} />
						</button>
					</div>

					{disableAll && (
						<div className="warning-message">
							<p>No content roles available. Please create a content role before inviting users.</p>
						</div>
					)}

					<div className="invite-users-roles">
						<label>
							Content Role:
							<select name="contentRole" value={contentRole} onChange={(e) => setContentRole(e.target.value)} disabled={disableAll}>
								{contentRoles.map((role) => (
									<option key={role.organizationContentRoleId} value={role.contentRoleId}>
										{role.displayName}
									</option>
								))}
							</select>
						</label>
					</div>
					<div className="invite-users-body">
						<div className="invite-users-left">
							<label>
								Users:
								<br></br>
								<input
									type="text"
									name="addUsers"
									value={userInput}
									onChange={(e) => setUserInput(e.target.value)}
									placeholder="Add users email or phone"
									disabled={disableAll}
								/>
								<button className="add-users-button" onClick={handleAddUsers} disabled={disableAll}>Add users</button>

							</label>
							{userLabel(usersToInvite.length)}
							<div className='invite-users-users'>
								{usersToInvite.map((user, index) => (
									<div key={index} className={`invite-users-user ${validUsers.has(index) ? '' : 'invalid-input'}`}>
										<p>{user}</p>
										<button onClick={() => handleRemoveUser(index)} className="remove-user-button">
											<FontAwesomeIcon icon={faXmark} />
										</button>
									</div>
								))}
							</div>
							<div className='invite-users-sms'>
								<label>
									SMS Message:
									<br></br>
									<input
										type="text"
										name="smsMessage"
										value={smsMessage}
										onChange={(e) => handleChange(e, 80)}
										placeholder="Enter a message for SMS"
										disabled={disableAll}
									/>
								</label>
							</div>
							<button className="submit-button" onClick={handleSubmit} disabled={disableAll}>
								Submit
							</button>
						</div>

						<div className='invite-users-right'>
							<label>
								Email subject:
								<br></br>
								<input
									type="text"
									name="emailSubject"
									value={emailSubject}
									onChange={(e) => handleChange(e, 200)}
									placeholder="Enter a subject for email"
									disabled={disableAll}
								/>
							</label>
							<label>
								Email message:
								<div className="invite-users-email">
									<textarea
										value={emailMessage}
										name="emailMessage"
										onChange={(e) => setEmailMessage(e.target.value)}
										placeholder="Enter a message for email"
										disabled={disableAll}
									/>
								</div>
							</label>

							<label>
								Initial Module: (optional)
								<br></br>
								<select value={initialModule} onChange={(e) => setInitialModule(e.target.value)} disabled={disableAll}>
									<option value="">Link a module</option>
									{modules.map((module) => (
										<option key={module.id} value={module.name}>{module.name}</option>
									))}
								</select>
							</label>
							<button className="cancel-button" onClick={handleCloseDialog}>
								Cancel
							</button>
						</div>

					</div>
					<div className="invite-users-footer">
					</div>
				</div >
			</dialog >
			<InviteUsersConfirmationDialog isOpen={isConfirmationDialogOpen}
				onClose={handleCloseDialog}
				onEdit={handleBackToEdit}
				inviteData={inviteData}
				sendInvites={handleSendInvites}
			/>
		</>

	);
}
