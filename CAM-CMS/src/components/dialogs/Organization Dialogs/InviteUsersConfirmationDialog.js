import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import './styles/InviteUsersConfirmationDialog.css';

const InviteUsersConfirmationDialog = ({
	isOpen,
	onClose,
	onEdit,
	inviteData,
}) => {
	if (!isOpen) return null;

	const { users, emailSubject, emailMessage, smsMessage } = inviteData;

	const handleSend = () => {
		//TODO Add dialog when confirmations of invites is actually sent
		console.log(`Invites sent to ${users.length} users`);
		onClose();
	};

	return (
		<dialog open className="invite-confirmation-dialog">
			<div className="invite-confirmation-content">
				<div className="invite-confirmation-header">
					<h2>Invite User(s) Preview
						<button onClick={onClose}>
							<FontAwesomeIcon icon={faXmark} />
						</button>
					</h2>
				</div>
				<div className="invite-confirmation-body">
					<div className='invite-confirmation-left'>

						<label>
							{`${users.length} Users to invite:`}
							<div className='invite-confirmation-invites'>
								<textarea readOnly value={users.join('\n')} name="confirmationUsersToInvite"/>
							</div>
						</label>
						<label>
							SMS Message:
							<div className='invite-confirmation-sms'>
								<textarea readOnly value={smsMessage} name="cofirmationSmsMessage"/>
							</div>
						</label>
						<button className="confirmation-edit-button" onClick={onEdit}>
							Edit
						</button>
					</div>
					<div className='invite-confirmation-right'>
						<label>
							Email subject:
							<br></br>
							<div className='invite-confirmation-email-subject'>
								<textarea readOnly value={emailSubject} name="confirmationEmailSubject" />
							</div>
						</label>
						<label>
							Email message:
							<div className='invite-confirmation-email-message'>
								<textarea readOnly value={emailMessage} name="confirmationEmailMessage"/>
							</div>
						</label>
						<button className="confirmation-send-button" onClick={handleSend}>
							Send
						</button>
					</div>

				</div>
				<div className="invite-confirmation-footer">
				</div>
			</div>
		</dialog>
	);
};

export default InviteUsersConfirmationDialog;