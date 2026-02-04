import React, { useEffect } from 'react';
import ReactPlayer from 'react-player';
import OptionsRow from './OptionsRow';
import ExistingOrNewElementDialog from '../dialogs/Element Dialogs/ExistingOrNewElementDialog';
import './styles/Element.css';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';

export default function Element({ location, canEdit, onDelete, onChange, isStyleAsideVisible, onEditElement, editingElement, editorContent, setEditorContent, handleSaveEdit, handleCancelEdit, isCollapsed }) {
	const displayLink = JSON?.parse(location?.locationAttributeJson).DisplayLink;

	/**
	 * Adds a listener for when the item is dragged.
	 * Adds needed classes to the drag and hover locations and sets the data for the drag event.
	 */
	useEffect(() => {
		const elementObject = document.getElementById(`module-element-${location.setLocationId}-${location.elementId}`);

		const handleDragStart = (e) => {
			if (isStyleAsideVisible || isCollapsed) return;

			const dropLocations = document.getElementsByClassName('element-drop-location');
			Array.from(dropLocations).forEach((dragLocation) => {
				if (!dragLocation.classList.contains('element-dragging')) dragLocation.classList.toggle('element-dragging');
			});

			e.stopPropagation();

			e.dataTransfer.effectAllowed = 'move';
			e.dataTransfer.setData(
				'application/json',
				JSON.stringify({ type: 'element', from: 'module', item: location })
			);
		};

		elementObject.addEventListener('dragstart', handleDragStart);

		return () => {
			elementObject.removeEventListener('dragstart', handleDragStart);
		};
	}, [location, isStyleAsideVisible, isCollapsed]);

	/**
	 * get video content of the element
	 * 
	 * @returns a string of the video content of the element
	 */
	function getVideoContent(location) {
		if (location.element.content === null || location.element.content === undefined) {
			return 'Processing video...';
		}

		return JSON.parse(location.element.content);
	}

	/**
	 * Swaps the element with another element
	 * 
	 * @returns {void}
	 * @post The element is swapped with another element
	 */
	function swapElement(newElement) {
		location.elementId = newElement.elementId;
		onChange(1024, [location]);
	}

	function openElementDialog() {
		const dialog = document.getElementById(`module-element-${location.setLocationId}-${location.elementId}-dialog`);
		dialog.showModal();
	}

	/**
	 * Toggle the locked property of the element
	 * 
	 * @returns {void}
	 * @post The element locked state is toggled
	 */
	function toggleLocked() {
		location.isEditable = !location.isEditable;
		onChange(128, [location]);
	}

	/**
	 * Edit the element properties
	 * 
	 * @returns {void}
	 */
	function editElement() {
		if (location.element.typeId === 1) {
			onEditElement(location);
		}
		onChange(512, [location]);
	}

	/**
	 * Delete the element
	 * 
	 * @returns {void}
	 * @post The element is deleted
	 */
	function deleteElement() {
		if (location.isEditable) {
			if (window.confirm('Are you sure you want to remove the ' + location.element.title + ' element?')) {
				onDelete(location);
			}
		}
	}

	/**
	 * Removes the classes from the drop and hover locations.
	 * 
	 * @returns {void}
	 * @post All elements with the set-dragging class have it removed.
	 */
	function handleDragStop() {
		if (isStyleAsideVisible || isCollapsed) return;
		const dropLocations = document.getElementsByClassName('module-element-set');
		Array.from(dropLocations).forEach((dragLocation) => {
			if (dragLocation.classList.contains('element-dragging')) dragLocation.classList.toggle('element-dragging');
		});
	}

	/**
	 * Handles the drag end event for the element set reordering.
	 * 
	 * @param {DragEvent} e - The drag event object.
	 * @returns {void}
	 * @post The element set being dragged has its order set to the value of the set.
	 */
	function handleElementDragEnd(e) {
		if (isStyleAsideVisible || isCollapsed) return;
		if (e.dataTransfer.types[0] !== 'application/json') return;
		e.stopPropagation();

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		onChange(32, [data.item, location]);
	}

	/**
	 * Handles the drag over event for the element set reordering.
	 * 
	 * @param {DragEvent} e - The drag event object.
	 * @returns {void}
	 * @post The drag event's dropEffect is set to 'move'.
	 */
	function handleElementDragOver(e) {
		if (isStyleAsideVisible || isCollapsed) return;
		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		if (data.type !== 'element') return;

		e.dataTransfer.dropEffect = 'move';
		e.preventDefault();
	}

	if (location.element === null || location.element === undefined) {
		return (
			<div>
				<p>Element not found</p>
			</div>
		);
	}

	const linkContent = (
		<div className='pdf-link'>
			<a href={location.element.content} rel='noreferrer' target='_blank'>{location.element.content}</a>
		</div>
	);

	return (
		<div id={`module-element-${location.setLocationId}-${location.elementId}`} className='module-element'
			draggable={canEdit && !isStyleAsideVisible && !isCollapsed} onDragEnd={handleDragStop} onDrop={handleElementDragEnd} onDragEnter={handleElementDragOver}
			onDragOver={handleElementDragOver}>

			<ExistingOrNewElementDialog id={`module-element-${location.setLocationId}-${location.elementId}-dialog`}
				onChange={swapElement} libraryFolderId={location.element.libraryFolderId} />

			<OptionsRow isElement draggable={canEdit && !isStyleAsideVisible} canEdit={canEdit} isLocked={!location.isEditable}
				onNewItem={openElementDialog} onToggleLock={toggleLocked}
				onEdit={editElement} onDelete={deleteElement} isCollapsed={isCollapsed} />
			<div className='module-element-content'>
				{
					editingElement?.elementId === location.elementId ? (
						<div>
							<ReactQuill
								value={editorContent}
								onChange={setEditorContent}
							/>
							<button onClick={handleSaveEdit}>Save</button>
							<button onClick={handleCancelEdit}>Cancel</button>
						</div>
					) : location.element.typeId === 1 ? (
						<p id='display-text' dangerouslySetInnerHTML={{ __html: JSON.parse(location.element.content).content }} />
					) : location.element.typeId == 2 ? (
						displayLink ?
							linkContent :
							<img loading='lazy' src={location.element.content} />
					) : location.element.typeId == 4 ? (
						displayLink ?
							linkContent :
							<ReactPlayer url={getVideoContent(location).url} controls={true} className='video-player'
								light={getVideoContent(location).thumbnail ? getVideoContent(location).thumbnail : true}>
								<p>Unable to display video file.</p>
							</ReactPlayer>
					) : location.element.typeId === 5 ? (linkContent
					/*displayLink ?
						linkContent :
						<object data={location.element.content} type='application/pdf' width='75%' height='750px'>
							<p>Unable to display PDF file.</p>
						</object>*/
					) : location.element.typeId === 7 ? (
						<h1 id='anchor-text'>{location.element.title}</h1>
					) : undefined
				}
			</div>
		</div>
	);
}
