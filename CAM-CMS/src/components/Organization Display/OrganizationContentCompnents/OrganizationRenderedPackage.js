import React, { useEffect, useState } from 'react';
import './styles/OrganizationRenderedPackage.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCaretRight, faCaretDown, faBook } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import Requests from '../../../utils/Requests';

export default function OrganizationRenderedPackage({ packageData, archetypeIds }) {
	const [filteredPackages, setFilteredPackages] = useState([]);
	const [openFolders, setOpenFolders] = useState({});
	const [folderContents, setFolderContents] = useState({});
	const navigate = useNavigate();

	useEffect(() => {
		if (archetypeIds.length > 0) {
			const filtered = packageData.map(pkg => ({
				...pkg,
				packageFolders: (pkg.packageFolders || []).filter(folder =>
					archetypeIds.includes(parseInt(folder.contentRoleId, 10)) || folder.contentRoleId === null
				)
			})).filter(pkg => pkg.packageFolders.length > 0);

			setFilteredPackages(filtered);
		} else {
			setFilteredPackages(packageData);
		}
		setOpenFolders({});
		setFolderContents({});
	}, [packageData, archetypeIds]);

	const toggleFolder = async (id) => {
		setOpenFolders(prev => {
			const isCurrentlyOpen = prev[id];
			let newOpenFolders = { ...prev, [id]: !isCurrentlyOpen };

			if (!isCurrentlyOpen) {
				loadFolderContents(id);
			} else {
				const recursivelyCloseFolders = (folderId) => {
					const folder = findFolderById(folderId, filteredPackages);
					if (folder && folder.packageFolders) {
						folder.packageFolders.forEach(subFolder => {
							newOpenFolders[subFolder.packageFolderId] = false;
							recursivelyCloseFolders(subFolder.packageFolderId);
						});
					}
				};
				recursivelyCloseFolders(id);
			}

			return newOpenFolders;
		});
	};

	const loadFolderContents = async (id) => {
		if (!folderContents[id]) {
			try {
				const subFolders = await Requests.getSubFolders({ packageFolderId: id }).catch(error => {
					console.error(error);
					navigate('/error');
				});
				const modules = await Requests.getFolderModules({ packageFolderId: id }).catch(error => {
					console.error(error);
					navigate('/error');
				});
				console.log('modules', modules);
				setFolderContents(prev => ({
					...prev,
					[id]: { subFolders, modules: modules.sort((a, b) => a.orderInFolder - b.orderInFolder) }
				}));
			} catch (error) {
				console.error('Error loading folder contents:', error);
			}
		}
	};

	const findFolderById = (id, packages) => {
		for (let pkg of packages) {
			for (let folder of (pkg.packageFolders || [])) {
				if (folder.packageFolderId === id) {
					return folder;
				}
				let found = findFolderById(id, [folder]);
				if (found) return found;
			}
		}
		return null;
	};

	const renderFolders = (folders) => {
		return folders.map((folder, idx) => (
			<div key={idx}>
				<div className="content-folder-header" onClick={() => toggleFolder(folder.packageFolderId)}>
					<FontAwesomeIcon icon={openFolders[folder.packageFolderId] ? faCaretDown : faCaretRight} />
					<p>{folder.displayName}</p>
				</div>
				{openFolders[folder.packageFolderId] && (
					<div className="content-folder-contents">
						<div className="content-folder-subfolders">
							{folderContents[folder.packageFolderId]?.subFolders && renderFolders(folderContents[folder.packageFolderId].subFolders)}
						</div>
						<div className="content-folder-modules">
							{folderContents[folder.packageFolderId]?.modules?.map((module, index) => {
								let moduleTitle = '';
								try {
									const cache = JSON.parse(module.cache);
									moduleTitle = cache.title || 'Untitled Module';
								} catch (error) {
									moduleTitle = 'Invalid Module';
								}
								return (
									<div key={index} className='content-module-item'>
										<FontAwesomeIcon icon={faBook} className='faBook' />
										<span>{moduleTitle}</span>
									</div>
								);
							})}
						</div>
					</div>
				)}
			</div>
		));
	};

	return (
		<div className='content-packages-body'>
			<div className="content-packages-list">
				{filteredPackages.map((pkg, index) => (
					<div key={index} className="content-package-item">
						<h4>{pkg.name}</h4>
						<ul>{renderFolders(pkg.packageFolders || [])}</ul>
					</div>
				))}
			</div>
		</div>
	);
}
