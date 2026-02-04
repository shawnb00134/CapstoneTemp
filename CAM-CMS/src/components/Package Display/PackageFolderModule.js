import React, { useEffect, useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faBook } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import './styles/PackageFolderModule.css';

export default function PackageFolderModule({ module, onModuleDelete, onModuleChange, moduleFolderChange }) {
	const [moduleCache] = useState(module?.cache ? JSON?.parse(module?.cache) : null);
	const navigate = useNavigate();

	useEffect(() => {
		const moduleObject = document.getElementById(`folder-module-${module?.packageFolderModuleId}-${module?.packageFolderId}`);
		moduleObject?.addEventListener('dragstart', function (e) {
			const dropLocations = document.getElementsByClassName('module-drop-location');
			Array.from(dropLocations).forEach((dragLocation) => {
				if (!dragLocation.classList.contains('module-dragging')) {
					dragLocation.classList.toggle('module-dragging');
				}
			});

			e.stopPropagation();

			e.dataTransfer.effectAllowed = 'move';
			e.dataTransfer.setData(
				'application/json',
				JSON.stringify({ type: 'module', from: 'package', item: module })
			);
		});
	}, [module]);

	function handleDelete() {
		onModuleDelete(module);
	}

	function handleModuleDragOver(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		e.stopPropagation();

		if (data.type !== 'module') return;

		e.dataTransfer.dropEffect = 'move';
		e.preventDefault();
	}

	function handleModuleDragEnd(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;
		e.stopPropagation();

		const data = JSON.parse(e.dataTransfer.getData('application/json'));

		if (data.item.packageFolderId !== module.packageFolderId) {
			data.item.packageFolderId = module.packageFolderId;
			moduleFolderChange(data.item);
			return;
		} else if (data.item.packageFolderModuleId === module.packageFolderModuleId) return;

		onModuleChange([data.item, module]);
	}

	function handleDragStop() {
		const dropLocations = document.getElementsByClassName('module-drop-location');
		Array.from(dropLocations).forEach((dragLocation) => {
			if (dragLocation.classList.contains('module-dragging')) dragLocation.classList.toggle('module-dragging');
		});
	}

	function handleTitleClick() {
		navigate(`/studio/module/${module.publishedModuleId}`);
	}

	return (
		<>
			<div id={`folder-module-${module?.packageFolderModuleId}-${module?.packageFolderId}`} className='module-drop-location'
				onDragEnter={handleModuleDragOver} onDragEnd={handleDragStop} onDragOver={handleModuleDragOver} onDrop={handleModuleDragEnd}
				draggable>
				<div className='module-item'>
					<div className='module-item-contents'>
						{moduleCache ? (
							<>
								<FontAwesomeIcon icon={faBook} />
								<span className='module-title-link' onClick={handleTitleClick}>{moduleCache?.title}</span>
							</>
						) : (
							'Invalid module cache'
						)}
						<button className='delete-module-button' onClick={handleDelete}>
							<FontAwesomeIcon icon={faTrash} />
						</button>
					</div>
				</div>
			</div>
		</>
	);
}
