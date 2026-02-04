import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCaretDown, faPlus, faMagnifyingGlass, faX } from '@fortawesome/free-solid-svg-icons';

import NewElementDialog from '../dialogs/Element Dialogs/NewElementDialogs';
import NewModuleDialog from '../dialogs/Module Dialogs/NewModuleDialog';

import PackageButton from './PackageButton';
import LibraryFolderButton from './LibraryFolderButton';

import './styles/StudioPageButton.css';
import NewPackageDialog from '../dialogs/Package Dialogs/NewPackageDialog';

import useSearch from './UseSearch';

import NewLibraryFolderDialog from '../dialogs/LibraryFolder Dialogs/NewLibraryFolderDialog';

function useForceUpdate() {
	const [, setTick] = useState(0);
	const update = useCallback(() => {
		setTick((tick) => tick + 1);
	}, []);
	return update;
}

export default function StudioPageButton({ pageName, page }) {
	const navigate = useNavigate();
	const forceUpdate = useForceUpdate();

	const [isCollapsed, setCollapsed] = useState(true);

	const { components, searchTerm, searchComponents } = useSearch(page.components);
	const [selectedComponent] = useState(undefined);
	const [selectedLibrary, setSelectedLibrary] = useState(undefined);

	useEffect(() => {
		let pageDOMObject = document.getElementById('studio-nav-page-' + pageName + '-items');

		let classWatcher = new MutationObserver(function (mutations) {
			for (let mutation of mutations) {
				if (mutation.attributeName === 'class') {
					setCollapsed(mutation.target.classList.contains('collapsed'));

					let pageCaret = document.getElementById('studio-nav-page-' + pageName + '-caret');
					let pageButtons = document.getElementById('studio-nav-page-' + pageName + '-buttons');

					pageCaret.classList.toggle('collapsed');
					pageButtons.children[1].classList.toggle('hidden');
				}
			}
		});

		classWatcher.observe(pageDOMObject, { attributes: true });
	}, [pageName]);

	useEffect(() => {
		if (selectedComponent) {
			let selectedItems = document.getElementsByClassName('selected-page');
			for (let i = 0; i < selectedItems.length; i++) {
				selectedItems[i].classList.remove('selected-page');
			}

			let selectedItem = document.getElementById(selectedComponent);
			if (selectedItem) {
				selectedItem.classList.add('selected-page');
			}
		}
	}, [components, selectedComponent]);

	function toggleCurrentPage() {
		if (pageName === 'Dashboard') {
			navigate('/dashboard');
			return;
		}

		if (page.components.length === 0) return;

		let pageItems = document.getElementById('studio-nav-page-' + pageName + '-items');
		if (pageItems) {
			pageItems.classList.toggle('collapsed');
		}
	}

	function toggleSearchMode() {
		if (pageName === 'Dashboard') return;

		let pageItems = document.getElementById('studio-nav-page-' + pageName + '-items');
		let input = pageItems.getElementsByClassName('studio-nav-page-items-container-search')[0];
		input.classList.toggle('hidden');

		if (!input.classList.contains('hidden')) {
			input.children[0].focus();
		} else {
			input.children[0].value = '';
			searchComponents('');
		}
	}

	function addFolder() {
		let newFolderDialog = document.getElementById('studio-nav-folder-' + pageName + '-new-folder');
		newFolderDialog.showModal();
	}

	/**
	 * Adds a new item to the library folder.
	 * 
	 * @param {number} libraryFolderId - The id of the library folder to add the module to.
	 * @returns {void}
	 * @post A new module is in the library folder.
	 */
	function addFolderItem(libraryFolderId) {

		setSelectedLibrary(libraryFolderId);

		let dialog = undefined;
		if (pageName === 'Modules') {
			dialog = document.getElementById('studio-nav-folder-' + pageName + '-new-module');
		}
		else if (pageName === 'Elements') {
			dialog = document.getElementById('studio-nav-folder-' + pageName + '-new-element');
		}

		if (dialog) {
			dialog.showModal();
		}
	}

	/**
	 * Adds a new module to the library.
	 * 
	 * @param {number} newModule - The new module to add to the library.
	 * @returns {void}
	 * @post A new module is in the library folder.
	 */
	function addModule(newModule) {
		if (pageName === 'Modules') {
			page.components.forEach((folder) => {
				if (folder.id === selectedLibrary) {
					folder.components.push(newModule);
				}
			});

			forceUpdate();
			navigate('module/' + newModule.moduleId);
		}
	}

	/**
	 * Adds a new element to the library.
	 * 
	 * @param {number} newElement - The new element to add to the library.
	 * @returns {void}
	 * @post A new element is in the library folder.
	 */
	function addElement(newElement) {
		if (pageName === 'Elements') {
			page.components.forEach((folder) => {
				if (folder.id === selectedLibrary) {
					folder.components.push(newElement);
				}
			});

			forceUpdate();
			navigate('element/' + newElement.elementId);
		}
	}

	function addPackage(packageData) {
		if (packageData) {
			forceUpdate();
			navigate('package/' + packageData.packageId);
		}
		else {
			let dialog = document.getElementById('studio-nav-folder-' + pageName + '-new-package');
			if (dialog) {
				dialog.showModal();
			}
		}
	}

	function addComponent() {
		if (pageName === 'Packages') addPackage();
		else if (pageName === 'Modules' || pageName === 'Elements') addFolder();
	}

	function buildPackages() {
		return components.map((item, index) => {
			return (
				<div key={index}>
					<PackageButton packageItem={item} />
				</div>
			);
		});
	}

	function buildFolders(isModuleFolder) {
		return components.map((folder, index) => {
			return (
				<div key={index}>
					<LibraryFolderButton folder={folder} isModuleFolder={isModuleFolder} closeFolder={isCollapsed} onAdd={addFolderItem} />
				</div>
			);
		});
	}

	function buildPage() {
		if (pageName === 'Packages') {
			return buildPackages();
		}
		else if (pageName === 'Modules' || pageName === 'Elements') {
			return buildFolders(pageName === 'Modules');
		}
	}

	const newElementDialog = (
		<NewElementDialog id={'studio-nav-folder-' + pageName + '-new-element'}
			onChange={addElement} libraryFolder={selectedLibrary} />
	);

	const newModuleDialog = (
		<NewModuleDialog id={'studio-nav-folder-' + pageName + '-new-module'}
			onChange={addModule} libraryFolder={selectedLibrary} isTemplate={false} />
	);

	const newPackageDialog = (
		<NewPackageDialog id={'studio-nav-folder-' + pageName + '-new-package'}
			onChange={addPackage} />
	);

	return (
		<>
			<NewLibraryFolderDialog id={'studio-nav-folder-' + pageName + '-new-folder'} onChange={forceUpdate} />
			<div className='studio-nav-page'>
				{
					pageName === 'Elements' ? newElementDialog
						: pageName === 'Modules' ? newModuleDialog
							: pageName === 'Packages' ? newPackageDialog
								: undefined
				}
				<header>
					<h3 className='studio-nav-page-header' onClick={toggleCurrentPage}>
						<FontAwesomeIcon icon={page.icon} className='studio-nav-page-icon' />
						<FontAwesomeIcon icon={faCaretDown} style={{ display: page.components.length !== 0 ? 'block' : 'none' }}
							className='studio-nav-page-caret' id={'studio-nav-page-' + pageName + '-caret'} />
						{pageName}
					</h3>
					<div id={'studio-nav-page-' + pageName + '-buttons'}>
						<button className={pageName !== 'Dashboard' ? '' : 'hidden'} onClick={addComponent}>
							<FontAwesomeIcon icon={faPlus} />
						</button>
						<button className={'hidden'} onClick={toggleSearchMode}>
							<FontAwesomeIcon icon={faMagnifyingGlass} />
						</button>
					</div>
				</header>
				<div className='studio-nav-page-items-container'>
					<div id={'studio-nav-page-' + pageName + '-items'} className='collapsed'>
						<div className='studio-nav-page-items-container-search hidden'>
							<input type='text' placeholder='Search' value={searchTerm} onChange={(e) => searchComponents(e.target.value)} />
							<button onClick={toggleSearchMode}>
								<FontAwesomeIcon icon={faX} />
							</button>
						</div>
						{buildPage()}
					</div>
				</div>
			</div>
		</>
	);
}