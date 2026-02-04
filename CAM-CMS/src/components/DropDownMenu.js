import React, { useState, useEffect } from 'react';
import './styles/DropDownMenu.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleDown } from '@fortawesome/free-solid-svg-icons';

export default function DropDownMenu({ folders, onChangeLibrary, show }) {	
	const [open, setOpen] = useState(false);
	const [libraryFolders, setLibraryFolders] = useState([]);
	const [selectedLibraryFolder, setSelectedLibraryFolder] = useState(undefined);

	useEffect(() => {
		setLibraryFolders(folders);
	}, [folders, selectedLibraryFolder]);

	const handleOpen = () => {
		setOpen(!open);
	};

	function changeSelectedLibrary(libraryFolder) {
		setSelectedLibraryFolder(libraryFolder.name);
		onChangeLibrary(libraryFolder);
		handleOpen();
	}

	return show ? (
		<div>
			<button id='drop-menu-main-button' onClick={handleOpen}><FontAwesomeIcon icon={faAngleDown} />{selectedLibraryFolder ? selectedLibraryFolder : 'Select a library folder'}</button>
			{open ? (
				<div className='menu'>
					{
						libraryFolders.map((libraryFolder, key) => {
							return (
								<div className='menu-item' key={key}>
									<button className='menu-clickable'onClick={() => changeSelectedLibrary(libraryFolder)}>{libraryFolder.name}</button>
								</div>
							);
						})
					}
				</div>
			) : null}
		</div>
	) : null;
}