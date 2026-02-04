import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFolderOpen, faFolderClosed, faCaretDown, faPlus, faMinus } from '@fortawesome/free-solid-svg-icons';

import ModuleButton from './ModuleButton';
import ElementButton from './ElementButton';

import './styles/FolderButton.css';
import Requests from '../../utils/Requests';

function useForceUpdate() {
	const [, setTick] = useState(0);
	const update = useCallback(() => {
		setTick((tick) => tick + 1);
	}, []);
	return update;
}

/**
 * The library folder button component for the studio navigation.
 * It defines how to display an library folder in the studio navigation aside.
 * 
 * 	@version 1.0
 * 	@author Steven Kight
 *  @param {Object} folder - The library folder object to render the button for. (must contain id, name)
 *  @param {boolean} isModuleFolder - Whether or not the folder contains modules or not.
 *  @param {boolean} closeFolder - Whether or not to close the folder.
 *  @returns {JSX.Element} - The JSX element for the button.
 */
export default function LibraryFolderButton({ folder, isModuleFolder, closeFolder, onAdd }) {

	const [isFolderCollapsed, setFolderCollapsed] = React.useState(true);
	const [folderItems, setFolderItems] = React.useState([]);
	const [currentFolder, setCurrentFolder] = React.useState(folder);
	const forceUpdate = useForceUpdate();
	const navigate = useNavigate();

	/**
	 * Adds a listener for when the 'dragging' event is fired and when toggling.
	 * Adds needed classes to the drag and hover locations when the event is fired.
	 */
	useEffect(() => {
		window.addEventListener('dragging', (e) => {
			e.preventDefault();

			const folderDiv = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${isModuleFolder}`);
			if (folderDiv.classList.contains('nav-dragging')) return;
			folderDiv.classList.add('nav-dragging');
		});

		// Add listener to package header for when 'selected-page' class is removed
		let classWatcher = new MutationObserver(function (mutations) {
			for (let mutation of mutations) {
				if (mutation.attributeName === 'class') {
					if (!mutation.target.classList.contains('collapsed')) {
						mutation.target.style.marginTop = '0';
					}
					else {
						mutation.target.style.marginTop = `-${mutation.target.offsetHeight}px`;
					}

					let pageCaret = document.getElementById('studio-nav-page-' + folder.libraryFolderId + '-' + isModuleFolder + '-caret');
					pageCaret.classList.toggle('collapsed');

					setFolderCollapsed(mutation.target.classList.contains('collapsed'));
				}
			}
		});

		const folderItems = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${isModuleFolder}-items`);
		classWatcher.observe(folderItems, { attributes: true });

		if (folderItems.classList.contains('collapsed')) {
			folderItems.style.marginTop = `-${folderItems.offsetHeight}px`;
		}

	},);

	useEffect(() => {
		if (isModuleFolder) {
			setFolderItems(currentFolder.modules);
		}
		else {
			setFolderItems(currentFolder.elements);
		}
		forceUpdate();
	}, [currentFolder]);

	/**
	 * Detects when the closeFolder prop is changed and toggles the folder if it is open.
	 */
	useEffect(() => {
		if (closeFolder && !isFolderCollapsed) {
			toggleFolder();
		}
	}, [closeFolder]);
	/**
	 * Toggles the folder open or closed.
	 * 
	 * @returns {void}
	 * @post If the folder was open, it is now closed. If the folder was closed, it is now open.
	 */
	function toggleFolder() {
		const folderItemsDiv = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${isModuleFolder}-items`);
		console.log(folderItemsDiv);
		if (folderItemsDiv) {
			folderItemsDiv.classList.toggle('collapsed');
		}
		console.log(currentFolder);
		let fetchFolder = {
			libraryFolderId: folder.libraryFolderId,
			name: folder.name,
			description: folder.description,
			elements: [],
			modules: [],
			createdBy: folder.createdBy,
		};
		if (isModuleFolder) {
			if (currentFolder.modules.length === 0) {
				fetchModules(fetchFolder);
			} else {
				setFolderItems(currentFolder.modules); 
			}
		} else {
			if (currentFolder.elements.length === 0) {
				fetchElements(fetchFolder);
			} else {
				setFolderItems(currentFolder.elements); 
			}
		}
	}
	/**
	 * Removes the folder from the library.
	 * 
	 * @returns {void}
	 * @post The folder is removed from the library.
	 */
	function removeFolder() {
		Requests.deleteLibraryFolder(folder)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You are not authorized to delete this folder.');
					return;
				}
				if (response.status === 400) {
					alert('Folder contains items or is owned by an organization');
					return;
				}
				alert('Folder removed'); 
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	/**
	 * Adds a new component to the library.
	 * 
	 * @returns {void}
	 * @post If isModuleFolder is true, a new module is in the library folder. 
	 *		  If isModuleFolder is false, a new element is in the library folder.
	 *		  If the folder is collapsed, it is now open.
	 */
	function addComponent() {
		if (isFolderCollapsed) toggleFolder();

		onAdd(currentFolder.libraryFolderId);
	}

	/**
	 * Moves a module to the library.
	 * 
	 * @returns {void}
	 * @post Module is in the library folder.
	 */
	function moveModule(module, folder) {
		module.libraryFolderId = folder.libraryFolderId;

		Requests.updateModuleLibraryFolder(module)
			.then(() => {
				alert('Module moved'); // TODO: Give user feedback and refresh navigation
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	/**
	 * Moves an element to the library.
	 * 
	 * @returns {void}
	 * @post Element is in the library folder.
	 */
	function moveElement(element, folder) {
		element.libraryFolderId = folder.libraryFolderId;

		Requests.updateElementLibraryFolder(element)
			.then(() => {
				alert('Element moved'); // TODO: Give user feedback and refresh navigation
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	/**
	 * Moves a component to the library.
	 * 
	 * @returns {void}
	 * @post If isModuleFolder is true, a moves module to the library folder. 
	 *		  If isModuleFolder is false, a moves element to the library folder.
	 *		  If the folder is collapsed, it is now open.
	 */
	function moveComponent(dropComponent) {

		let isModule = dropComponent.type === 'module';
		const folderDiv = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${isModule}-items`);
		if (folderDiv.classList.contains('collapsed')) folderDiv.classList.toggle('collapsed');

		if (isModule) {
			moveModule(dropComponent.item, currentFolder);
		}
		else {
			moveElement(dropComponent.item, currentFolder);
		}
	}

	/**
	 * Handles the drag end event for the LibraryFolderButton component.
	 * 
	 * @param {DragEvent} e - The drag event object.
	 * @returns {void}
	 * @post The drag event's dataTransfer data is parsed and the component is moved to the folder.
	 */
	function handleDragEnd(e) {

		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		if (data.from !== 'nav') return;
		e.preventDefault();

		moveComponent(data);
	}

	/**
	 * Handles the drag over event for the library folder button.
	 * 
	 * @param {DragEvent} e - The drag event object.
	 * @returns {void}
	 * @post The drag event's dropEffect is set to 'move' and the folder button is the only item highlighted when the event is fired.
	 */
	function handleDragOver(e) {

		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		if (data.from !== 'nav') return;

		e.dataTransfer.dropEffect = 'move';

		const dropLocations = document.getElementsByClassName('nav-dragging over');
		Array.from(dropLocations).forEach((dropLocation) => {
			dropLocation.classList.remove('over');
		});

		const folderDiv = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${isModuleFolder}`);
		folderDiv.classList.add('over');
		e.preventDefault();
	}
	function folderCheck() {
		return !folder.deletable;
	}
	function buildFolderItems() {
		if (Array.isArray(folderItems)) {
			if (isModuleFolder) {
				return folderItems.map((module) => {
					return <ModuleButton key={module.moduleId} module={module} />;
				});
			} else {
				return folderItems.map((element) => {
					return <ElementButton key={element.elementId} element={element} />;
				});
			}
		}
		return null;
	}

	function fetchModules(fetchFolder) {
		Requests.getModulesByLibraryFolderId(fetchFolder).then((modules) => {
			console.log(modules.length);
			fetchFolder.modules = modules;
			setFolderItems(modules); 
			setCurrentFolder(fetchFolder);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}
	function fetchElements(fetchFolder) {
		Requests.getElementsByLibraryFolderId(fetchFolder).then((elements) => {
			console.log(elements.length);
			fetchFolder.elements = elements;
			setFolderItems(elements);
			setCurrentFolder(fetchFolder);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}
	return (
		<div id={'studio-nav-folder-' + folder.libraryFolderId + '-' + isModuleFolder} className='studio-nav-folder'
			onDrop={handleDragEnd} onDragEnter={handleDragOver} onDragOver={handleDragOver}>

			{/* Main folder button display with icon, dropdown caret, name, and add/delete buttons */}
			<header className='studio-nav-folder-header'>
				<div id='studio-nav-item' style={{ display: 'flex' }} onClick={folderItems !== 0 ? toggleFolder : undefined}>
					<FontAwesomeIcon icon={isFolderCollapsed ? faFolderClosed : faFolderOpen} className='studio-nav-page-icon' />
					<FontAwesomeIcon icon={faCaretDown} className='studio-nav-page-caret' id={'studio-nav-page-' + folder.libraryFolderId + '-' + isModuleFolder + '-caret'}
						style={{ display: folderItems.length !== 0 ? 'block' : 'none' }} />
					<h4>{folder.name}</h4>
				</div>
				<div id={'studio-nav-page-' + folder.libraryFolderId + '-' + isModuleFolder + '-buttons'}>
					<button className='hidden' onClick={addComponent}>
						<FontAwesomeIcon icon={faPlus} style={{ margin: 0 }} />
					</button>
					<button className={'hidden ' + (folderCheck() ? 'unauthorized' : '')}
						onClick={removeFolder} disabled={folderCheck()} data-error='Folder contains items'>
						<FontAwesomeIcon icon={faMinus} style={{ margin: 0 }} />
					</button>
				</div>
			</header>
			{/* Folder items display */}
			<div className='studio-nav-folder-items-container' >
				<div id={'studio-nav-folder-' + folder.libraryFolderId + '-' + isModuleFolder + '-items'} className='collapsed'>
					{
						buildFolderItems()
					}
				</div>
			</div>
		</div>
	);
}




