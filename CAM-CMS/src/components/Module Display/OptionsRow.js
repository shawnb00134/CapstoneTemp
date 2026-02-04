import React from 'react';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowsUpDownLeftRight, faPaintbrush, faShuffle, faPlus, faTrash, faLock, faLockOpen } from '@fortawesome/free-solid-svg-icons';

import './styles/OptionsRow.css';

export default function OptionsRow({ isElement, draggable, isLocked, canEdit, onEdit, onDelete, onToggleLock, onNewItem, isCollapsed }) {

	if (isCollapsed) return null;

	return (
		<div className='module-element-buttons row'>
			{draggable ?
				<button className={canEdit ? 'module-element-button' : 'module-element-button unauthorized'} id='module-element-set-drag-icon' disabled={!canEdit}>
					<FontAwesomeIcon icon={faArrowsUpDownLeftRight} />
				</button> : <FontAwesomeIcon icon={faLock} />
			}
			<div className='row' style={{ alignSelf: 'end' }}>
				{
					isLocked ? undefined :
						<>
							<button className={canEdit ? 'module-element-button' : 'module-element-button unauthorized'} id='module-edit-button'
								disabled={!canEdit} onClick={onEdit}>
								<FontAwesomeIcon icon={faPaintbrush} />
							</button>
							<button className={canEdit ? 'module-element-button' : 'module-element-button unauthorized'}
								disabled={!canEdit} onClick={onNewItem}>
								{isElement ? <FontAwesomeIcon icon={faShuffle} /> : <FontAwesomeIcon icon={faPlus} />}
							</button>
							<button className={canEdit ? 'module-element-button' : 'module-element-button unauthorized'} id='module-delete-button'
								disabled={!canEdit} onClick={onDelete}>
								<FontAwesomeIcon icon={faTrash} />
							</button>
						</>
				}
				<button id='module-element-lock-button' className={canEdit ? 'module-element-button' : 'module-element-button unauthorized'}
					disabled={!canEdit} onClick={onToggleLock}>
					{isLocked ? <FontAwesomeIcon icon={faLock} /> : <FontAwesomeIcon icon={faLockOpen} />}
				</button>
			</div>
		</div>
	);
}
