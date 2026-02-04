import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLayerGroup } from '@fortawesome/free-solid-svg-icons';

import './styles/FolderItemButton.css';

/**
 * The module button component for the studio navigation.
 * It defines how to display an module in the studio navigation aside.
 * 
 * 	@version 1.0
 * 	@author Steven Kight
 *  @param {Object} module - The module object to render the button for. (must contain id, name, and folderId)
 *  @returns {JSX.Element} - The JSX element for the button.
 */
export default function ModuleButton({ module }) {
	const navigate = useNavigate();

	/**
	 * Adds a listener for when the item is dragged.
	 * Adds needed classes to the drag and hover locations and sets the data for the drag event.
	 */
	useEffect(() => {
		const moduleHeader = document.getElementById('nav-module-' + module.moduleId);
		moduleHeader.addEventListener('dragstart', function(e) {
			const dragLocations = document.getElementsByClassName('studio-nav-folder');
			Array.from(dragLocations).forEach((dragLocation) => {
				if (!dragLocation.classList.contains('nav-dragging')) dragLocation.classList.toggle('nav-dragging');
			});

			e.dataTransfer.effectAllowed = 'move';
			e.dataTransfer.setData(
			  'application/json',
			  JSON.stringify({ type: 'module', from: 'nav', item: module })
			);
		});
	}, []);

	/**
	 * Selects the module and navigates to the module page.
	 * Also adds the selected-page class to the module header.
	 * 
	 * @returns {void}
	 * @fires navigate
	 * @post The module page is navigated to and the module header is the only dom object with the selected-page class.
	 */
	function selectModule() {
		navigate('module/' + module.moduleId);

		// Remove selected class from all items
		let selectedItems = document.getElementsByClassName('selected-page');
		for (let i = 0; i < selectedItems.length; i++) {
			if (!selectedItems[i].classList.contains('selected-page')) continue;
			selectedItems[i].classList.remove('selected-page');
		}

		// Add selected class to current item
		let moduleHeader = document.getElementById('nav-module-' + module.moduleId);
		moduleHeader.classList.add('selected-page');
	}

	/**
	 * Removes the nav-dragging and over classes from the drop and hover locations.
	 * 
	 * @returns {void}
	 * @post All elements with the nav-dragging class have it removed.
	 */
	function handleDragEnd() {
		const dropLocations = document.getElementsByClassName('nav-dragging');
		const hoverLocations = document.getElementsByClassName('over');
		Array.from(dropLocations).forEach((dropLocation) => {
			dropLocation.classList.remove('nav-dragging');
		});
		Array.from(hoverLocations).forEach((hoverLocation) => {
			hoverLocation.classList.remove('over');
		});
	}
	return (
		<header id={'nav-module-' + module.moduleId} className='studio-nav-folder-item' onClick={selectModule} draggable onDragEnd={handleDragEnd}>
			<FontAwesomeIcon icon={faLayerGroup} className='studio-nav-page-icon' />
			<h4>{module.title}</h4>
		</header>
	);
}