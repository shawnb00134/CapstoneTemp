import React, { useState, useEffect } from 'react';

import { faHouse, faBook, faFile, faBoxArchive } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

import StudioPageButton from './StudioPageButton';
import Request from '../../utils/Requests';

import './styles/StudioNavigation.css';

const pages = {
	'Dashboard': {'icon': faHouse, 'components': []},
	'Packages': {'icon': faBoxArchive, 'components': []},
	'Modules': {'icon': faBook, 'components': []},
	'Elements': {'icon': faFile, 'components': []}
};

export default function StudioNavigation() {
	//eslint-disable-next-line
	const [studioPages, setStudioPages] = useState(pages);
	const navigate = useNavigate();

	useEffect(() => {
		Request.getStudioAside().then((response) => {
			setStudioPages(studioPages => ({
				...studioPages,
				'Modules': {'icon': faBook, 'components': response.libraryFolders},
				'Elements': {'icon': faFile, 'components': response.libraryFolders},
				'Packages': {'icon': faBoxArchive, 'components': response.packages}
			  }));
			  
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}, []);

	useEffect(() => {
		directNavigation();

		for (let i = 0; i < studioPages['Modules'].components.length; i++) {
			let moduleFolder = studioPages['Modules'].components[i];
			let elementFolder = studioPages['Elements'].components[i];

			if (moduleFolder && elementFolder) {

				let containsModules = moduleFolder.modules.length > 0;
				let containsElements = elementFolder.elements.length > 0;

				let containsItems = containsModules || containsElements;
				
				studioPages['Modules'].components[i].deletable = !containsItems;
				studioPages['Elements'].components[i].deletable = !containsItems;
			}
		}

	}, [studioPages]);

	function directNavigation() {
		// Load navigation path
		let navigationPath = document.location.pathname.split('/');
		navigationPath.shift();

		// If navigation path is 3 long and first item is 'studio'
		if (navigationPath.length === 3 && navigationPath[0] === 'studio') {
			let id = parseInt(navigationPath[navigationPath.length - 1]);

			let pageDOMObject = undefined;
			let itemDOMObject = undefined;
			let folderDOMObject = undefined;

			if (navigationPath[1] === 'package') {
				// Open that page dropdown
				pageDOMObject = document.getElementById('studio-nav-page-Packages-items');

				// Get the package header
				itemDOMObject = document.getElementById(`nav-package-${id}`);
			}
			else if (navigationPath[1] === 'element') {
				// Open that page dropdown
				pageDOMObject = document.getElementById('studio-nav-page-Elements-items');

				// Find folder that element is in
				let folder = studioPages.Elements.components.find((folder) => {
					return folder.elements.find((element) => {
						return element.elementId === id;
					});
				});

				if (folder) {
					folderDOMObject = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${false}-items`);
				}

				// Get the element header
				itemDOMObject = document.getElementById(`nav-element-${id}`);
			}
			else if (navigationPath[1] === 'module') {
				// Select the page dropdown
				pageDOMObject = document.getElementById('studio-nav-page-Modules-items');

				// Find folder that module is in
				let folder = studioPages.Modules.components.find((folder) => {
					return folder.modules.find((module) => {
						return module.moduleId === id;
					});
				});
				
				if (folder) {
					folderDOMObject = document.getElementById(`studio-nav-folder-${folder.libraryFolderId}-${true}-items`);
				}

				// Get the module header
				itemDOMObject = document.getElementById(`nav-module-${id}`);
			}

			// Open page dropdown
			if (pageDOMObject && pageDOMObject.classList.contains('collapsed')) {
				pageDOMObject.classList.remove('collapsed');
			}

			// Open folder dropdown
			if (folderDOMObject && folderDOMObject.classList.contains('collapsed')) {
				folderDOMObject.classList.remove('collapsed');
			}

			// Select item
			if (itemDOMObject && !itemDOMObject.classList.contains('selected-page')) {
				itemDOMObject.classList.add('selected-page');
			}
		}
	}

	function buildNavigationPanel() {
		if (studioPages.Modules === undefined || studioPages.Elements === undefined) {
			return (<h2>Loading...</h2>);
		}

		return Object.keys(studioPages).map((pageName, index) => {
			return (
				<StudioPageButton key={index} pageName={pageName} page={studioPages[pageName]}/>
			);
		});
	}

	return (
		<div className='studio-nav'>
			<header className='studio-nav-header'>
				<h1>
					Studio
				</h1>
			</header>
			<div className='studio-nav-pages'>
				{ buildNavigationPanel() }
			</div>
		</div>
	);
}