import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAlignLeft, faImage, faFilePdf, faVideo, faAnchor } from '@fortawesome/free-solid-svg-icons';

import './styles/FolderItemButton.css';

/**
 * The element button component for the studio navigation.
 * It defines how to display an element in the studio navigation aside.
 * 
 * 	@version 1.0
 * 	@author Steven Kight
 *  @param {Object} element - The element object to render the button for. (must contain id, name, type, and folderId)
 *  @returns {JSX.Element} - The JSX element for the button.
 */
export default function ElementButton({ element }) {
	const navigate = useNavigate();

	// Defines the icon for each element type
	const elementIcon = {
		'text': faAlignLeft,
		'image': faImage,
		'pdf': faFilePdf,
		'video': faVideo,
		'anchor': faAnchor,
		1: faAlignLeft,
		2: faImage,
		5: faFilePdf,
		4: faVideo,
		7: faAnchor
	};

	/**
	 * Adds a listener for when the item is dragged.
	 * Adds needed classes to the drag and hover locations and sets the data for the drag event.
	 */
	useEffect(() => {
		const elementHeader = document.getElementById('nav-element-' + element.elementId);
		elementHeader.addEventListener('dragstart', function(e) {
			const dragLocations = document.getElementsByClassName('studio-nav-folder');
			Array.from(dragLocations).forEach((dragLocation) => {
				if (!dragLocation.classList.contains('nav-dragging')) dragLocation.classList.toggle('nav-dragging');
			});
			const moduleDragLocations = document.getElementsByClassName('element-drop-location');
			Array.from(moduleDragLocations).forEach((dragLocation) => {
				if (!dragLocation.classList.contains('element-dragging')) dragLocation.classList.toggle('element-dragging');
			});

			e.dataTransfer.effectAllowed = 'move';
			e.dataTransfer.setData(
			  'application/json',
			  JSON.stringify({type: 'element', from: 'nav', item: element})
			);
		});
	}, []);

	/**
	 * Selects the element and navigates to the element page.
	 * Also adds the selected-page class to the element header.
	 * 
	 * @returns {void}
	 * @fires navigate
	 * @post The element page is navigated to and the element header is the only dom object with the selected-page class.
	 */
	function selectElement() {
		navigate('element/' + element.elementId);

		// Remove selected class from all items
		let selectedItems = document.getElementsByClassName('selected-page');
		for (let i = 0; i < selectedItems.length; i++) {
			if (!selectedItems[i].classList.contains('selected-page')) continue;
			selectedItems[i].classList.remove('selected-page');
		}

		// Add selected class to current item
		let elementHeader = document.getElementById('nav-element-' + element.elementId);
		elementHeader.classList.add('selected-page');
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

		const elementDropLocations = document.getElementsByClassName('element-dragging');
		Array.from(elementDropLocations).forEach((dropLocation) => {
			dropLocation.classList.remove('element-dragging');
		});
	}

	return (
		<header id={'nav-element-' + element.elementId} className='studio-nav-folder-item' onClick={selectElement} draggable onDragEnd={handleDragEnd}>
			<FontAwesomeIcon icon={elementIcon[element.typeId]} className='studio-nav-page-icon' />
			<h4>{element.title}</h4>
		</header>
	);
}