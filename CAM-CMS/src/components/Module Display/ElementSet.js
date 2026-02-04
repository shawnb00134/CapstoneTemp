import React, { useState, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCaretUp, faCaretDown } from '@fortawesome/free-solid-svg-icons';
import OptionsRow from './OptionsRow';
import ExistingOrNewElementDialog from '../dialogs/Element Dialogs/ExistingOrNewElementDialog';
import Element from './Element';
import './styles/ElementSet.css';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';

export default function ElementSet({ elementSet, libraryFolderId, canEdit, onChange, dragItemChanged, isStyleAsideVisible, onEditElement, editingElement, editorContent, setEditorContent, handleSaveEdit, handleCancelEdit, defaultCollapsed }) {
	const [textContent, setTextContent] = useState(elementSet?.elements?.map(() => ''));
	const [isCollapsed, setIsCollapsed] = useState(defaultCollapsed);

	useEffect(() => {
		setIsCollapsed(defaultCollapsed);
	}, [defaultCollapsed]);

	/**
	 * Adds a listener for when the item is dragged.
	 * Adds needed classes to the drag and hover locations and sets the data for the drag event.
	 */
	useEffect(() => {
		const elementSetObject = document.getElementById(`module-element-set-${elementSet.setLocationId}`);

		const handleDragStart = (e) => {
			if (isStyleAsideVisible) return;

			const dropLocations = document.getElementsByClassName('module-element-set-reorder');
			Array.from(dropLocations).forEach((dragLocation) => {
				if (!dragLocation.classList.contains('set-dragging')) dragLocation.classList.toggle('set-dragging');
			});

			dragItemChanged(elementSet);

			e.dataTransfer.effectAllowed = 'move';
			e.dataTransfer.setData(
				'application/json',
				JSON.stringify({ type: 'element-set', from: 'module', item: elementSet })
			);
		};

		elementSetObject.addEventListener('dragstart', handleDragStart);

		return () => {
			elementSetObject.removeEventListener('dragstart', handleDragStart);
		};
	}, [elementSet, isStyleAsideVisible]);

	function handleToggleCollapse() {
		setIsCollapsed(prevState => {
			const newCollapsedState = !prevState;
			if (!elementSet.styling?.is_collapsible && newCollapsedState) {
				document.querySelector(`#module-element-set-${elementSet.setLocationId} .module-element-set-items`).classList.add('hidden');
			} else {
				document.querySelector(`#module-element-set-${elementSet.setLocationId} .module-element-set-items`).classList.remove('hidden');
			}
			return newCollapsedState;
		});
	}

	/**
	 * Adds a new element to the element set
	 * 
	 * @returns {void}
	 * @post The element is added to the element set
	 */
	function addElement(newElement) {
		let newLocation = {
			setLocationId: elementSet.setLocationId,
			elementId: newElement.elementId,
			element: newElement,
			place: elementSet.elements.length,
			isEditable: true,
			locationAttributeJson: JSON.stringify({}),
			attributes: {}
		};

		onChange(8, [newLocation]);
	}

	function openAddDialog() {
		const dialog = document.getElementById(`module-element-set-${elementSet.setLocationId}-dialog`);
		dialog.showModal();
	}

	/**
	 * Toggle the locked property of the element set
	 * 
	 * @returns {void}
	 * @post The element set locked state is toggled
	 */
	function toggleLocked() {
		elementSet.isEditable = !elementSet.isEditable;
		onChange(64, [elementSet]);
	}

	/**
	 * Edit the element set properties
	 * 
	 * @returns {void}
	 */
	function editElementSet() {
		onChange(256, [elementSet]);
	}

	/**
	 * Delete the element set
	 * 
	 * @returns {void}
	 * @post The element set is deleted
	 */
	function deleteElementSet() {
		if (elementSet.isEditable) {
			if (window.confirm('Are you sure you want to remove this element set?')) {
				onChange(2, [elementSet]);
			}
		}
	}

	/**
	 * Delete the element
	 * 
	 * @returns {void}
	 * @post The element is deleted
	 */
	function deleteElement(location) {
		elementSet.elements = elementSet.elements.filter((element) => element !== location);
		onChange(16, [location]);
	}

	/**
	 * Handles the drag end event for the element set reordering.
	 * 
	 * @param {DragEvent} e - The drag event object.
	 * @returns {void}
	 * @post The element set being dragged has its order set to the value of the set.
	 */
	function handleReorderDragEnd(e) {
		if (isStyleAsideVisible) return;
		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		onChange(4, [data.item, elementSet.place]);
	}

	/**
	 * Handles the drag over event for the element set reordering.
	 * 
	 * @param {DragEvent} e - The drag event object.
	 * @returns {void}
	 * @post The drag event's dropEffect is set to 'move'.
	 */
	function handleReorderDragOver(e) {
		if (isStyleAsideVisible) return;
		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		if (data.type !== 'element-set') return;

		e.dataTransfer.dropEffect = 'move';
		e.preventDefault();
	}

	/**
	 * Removes the classes from the drop and hover locations.
	 * 
	 * @returns {void}
	 * @post All elements with the set-dragging class have it removed.
	 */
	function handleReorderDragStop() {
		if (isStyleAsideVisible) return;
		const dropLocations = document.getElementsByClassName('module-element-set-reorder');
		Array.from(dropLocations).forEach((dragLocation) => {
			if (dragLocation.classList.contains('set-dragging')) dragLocation.classList.toggle('set-dragging');
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
		if (isStyleAsideVisible) return;
		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));

		if (data.from === 'nav') {
			addElement(data.item);
		} else {
			onChange(32, [data.item, elementSet]);
		}
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

	function handleEditorChange(content, index) {
		const updatedContent = [...textContent];
		updatedContent[index] = content;
		setTextContent(updatedContent);
		setEditorContent(content);
	}

	const firstAnchorElement = elementSet.elements?.find((element) => element.element.typeId === 7);
	const firstNonAnchorElement = elementSet.elements?.find((element) => element.element.typeId !== 7);

	const isCollapsible = elementSet.styling?.is_collapsible;

	return (
		<>
			<ExistingOrNewElementDialog id={`module-element-set-${elementSet.setLocationId}-dialog`}
				onChange={addElement} libraryFolderId={libraryFolderId} />

			<div id={`module-element-set-reorder-${elementSet.setLocationId}`} className='module-element-set-reorder'
				onDrop={handleReorderDragEnd} onDragEnter={handleReorderDragOver} onDragOver={handleReorderDragOver} />

			<div id={`module-element-set-${elementSet.setLocationId}`} className='module-element-set element-drop-location' draggable={canEdit && !isStyleAsideVisible}
				onDragEnd={handleReorderDragStop} onDrop={handleElementDragEnd} onDragEnter={handleElementDragOver}
				onDragOver={handleElementDragOver}>

				<div className='module-element-set-carrot'>
					<button className={`element-carrot${elementSet.setLocationId}`} onClick={handleToggleCollapse}>
						<FontAwesomeIcon icon={isCollapsed ? faCaretDown : faCaretUp} />
					</button>
				</div>

				<OptionsRow draggable={canEdit && !isStyleAsideVisible} canEdit={canEdit} isLocked={!elementSet.isEditable}
					onNewItem={openAddDialog} onToggleLock={toggleLocked}
					onEdit={editElementSet} onDelete={deleteElementSet} isCollapsed={isCollapsed} />

				<div className={`module-element-set-items ${isCollapsed && isCollapsible === 'false' ? 'hidden' : ''}`}>
					<div className={`module-element-set-items-${elementSet.setLocationId}`}>
						{
							isCollapsed && isCollapsible !== 'false' && (firstAnchorElement || firstNonAnchorElement) ? (
								<div className='module-element-preview horizontal-layout'>
									{firstNonAnchorElement && (
										<Element location={firstNonAnchorElement} canEdit={canEdit} onDelete={deleteElement} onChange={onChange} isStyleAsideVisible={isStyleAsideVisible} onEditElement={onEditElement} isCollapsed={isCollapsed} />
									)}
									{firstAnchorElement && (
										<Element location={firstAnchorElement} canEdit={canEdit} onDelete={deleteElement} onChange={onChange} isStyleAsideVisible={isStyleAsideVisible} onEditElement={onEditElement} isCollapsed={isCollapsed} />
									)}
									<span className='module-element-set-title'>{elementSet.moduleTitle}</span>
								</div>
							) : (
								elementSet?.elements?.map((location, index) => {
									return (
										<div key={index}>
											{(editingElement?.elementId === location.elementId && isStyleAsideVisible) ? (
												<div>
													<ReactQuill
														className='quill-editor'
														value={editorContent}
														onChange={(content) => handleEditorChange(content, index)}
														readOnly={!canEdit}
													/>
													<button onClick={handleSaveEdit}>Save</button>
													<button onClick={handleCancelEdit}>Cancel</button>
												</div>
											) : (
												<Element key={index} location={location} canEdit={canEdit} onDelete={deleteElement} onChange={onChange} isStyleAsideVisible={isStyleAsideVisible} onEditElement={onEditElement} isCollapsed={isCollapsed} />
											)}
										</div>
									);
								})
							)
						}
					</div>
				</div>

			</div>
		</>
	);
}
