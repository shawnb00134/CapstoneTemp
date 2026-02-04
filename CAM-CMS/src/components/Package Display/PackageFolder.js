import { React, useState, useEffect, useCallback } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFolder, faCaretUp, faPlus, faTrash, faPaintBrush, faFolderOpen } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import NewItemDialog from '../dialogs/Package Dialogs/NewItemDialog';
import PackageFolderModule from './PackageFolderModule';
import './styles/PackageFolder.css';
import EditPackageFolderDialog from '../dialogs/Package Dialogs/EditPackageFolderDialog';
import Requests from '../../utils/Requests';

function useForceUpdate() {
	const [, setTick] = useState(0);
	const update = useCallback(() => {
		setTick((tick) => tick + 1);
	}, []);
	return update;
}

export default function PackageFolder({ folder, onAdd, onAddModule, onDelete, onModuleDelete, changeSelected, parentFolder, onFolderChange, onFolderReorder, onModuleChange, moduleFolderChange, canEdit }) {
	const [isCollapsed, setCollapsed] = useState(true);
	const [currentFolder, setCurrentFolder] = useState(folder);
	const [modules, setModules] = useState([]);
	const [subFolders, setSubFolders] = useState([]);
	const [editDialogLoaded, setEditDialogLoaded] = useState(false);
	const [nextParentFolder, setNextParentFolder] = useState(currentFolder);
	const forceUpdate = useForceUpdate();
	const navigate = useNavigate();

	useEffect(() => {
		const folderObject = document.getElementById(`${currentFolder.packageFolderId}-package-folder`);
		folderObject.addEventListener('dragstart', function (e) {
			const dropLocations = document.getElementsByClassName('folder-item-drop');
			const folderDropLocations = document.getElementsByClassName('folder-swap');
			Array.from(dropLocations).forEach((dragLocation) => {
				let folderId = parseInt(dragLocation.id.split('-')[2]);
				if (!dragLocation.classList.contains('folder-dragging')) {
					if (folderId === currentFolder.packageFolderId) return;
					dragLocation.classList.toggle('folder-dragging');
				}
			});
			Array.from(folderDropLocations).forEach((dragLocation) => {
				let folderId = parseInt(dragLocation.id.split('-')[2]);
				if (!dragLocation.classList.contains('folder-dragging')) {
					if (folderId === currentFolder.packageFolderId) return;
					dragLocation.classList.toggle('folder-dragging');
				}
			});
			e.stopPropagation();
			changeSelected(currentFolder);
			e.dataTransfer.effectAllowed = 'move';
			e.dataTransfer.setData(
				'application/json',
				JSON.stringify({ type: 'folder', from: 'package', item: currentFolder })
			);
		});
		setSubFolders(currentFolder?.packageFolders?.sort((a, b) => a.orderInParent - b.orderInParent));
		setModules(currentFolder?.packageFoldersModule?.sort((a, b) => a.orderInFolder - b.orderInFolder) || []);
		setNextParentFolder(currentFolder);
		const elementsList = document.getElementById(`package-folder-items-${currentFolder.packageFolderId}`);
		if (elementsList) {
			if (isCollapsed) {
				elementsList.classList.add('collapsed');
			} else {
				elementsList.classList.remove('collapsed');
			}
		}
	}, [currentFolder]);

	function handleToggleCollapse() {
		const elementsList = document.getElementById(`package-folder-items-${currentFolder.packageFolderId}`);
		const carrot = document.getElementById(`package-folder-carrot-${currentFolder.packageFolderId}`);

		if (elementsList && carrot) {
			if (isCollapsed) {
				elementsList.classList.remove('collapsed');
				setCollapsed(false);
				fectchSubFolders();
				fectchModules();

			} else {
				elementsList.classList.add('collapsed');
				setCollapsed(true);
			}

		}
	}

	function addItem() {
		document.getElementById(`new-item-dialog-${currentFolder.packageFolderId}`)
			.showModal();
	}

	function deleteFolder() {
		var confirmDelete = window.confirm('Are you sure you want to delete this folder?\nAll subfolders and modules will be removed as well.');
		if (confirmDelete) {
			onDelete(currentFolder);
		}
	}

	async function editFolder() {
		const dialog = document.getElementById(`edit-folder-${currentFolder.packageFolderId}`);
		setEditDialogLoaded(true);
		await dialog.showModal();
		setEditDialogLoaded(false);
	}

	function updateFolder(data) {
		setCurrentFolder(data);
		setSubFolders([]);
		setModules([]);
		forceUpdate();
	}

	function buildChildFolders() {
		if (subFolders === undefined) return;
		return subFolders?.map((folder, index) => {
			return (
				<PackageFolder key={index} folder={folder}
					onAdd={onAdd} onDelete={onDelete} parentFolder={nextParentFolder} onAddModule={onAddModule} onModuleDelete={handleDeleteModule} moduleFolderChange={moduleFolderChange} onFolderReorder={onFolderReorder} changeSelected={changeSelected} onModuleChange={onModuleChange} onFolderChange={onFolderChange} />
			);
		});
	}

	function handleDeleteModule(module) {
		setModules(modules?.filter((listModule) => listModule[0]?.packageFolderModuleId !== module.packageFolderModuleId));
		onModuleDelete(module);
	}

	function buildModules() {
		const sortedModules = [...modules].sort((a, b) => a.orderInFolder - b.orderInFolder);
		return sortedModules?.map((module, index) => {
			return (
				<PackageFolderModule key={index} onModuleChange={onModuleChange} module={module} onAddModule={onAddModule} onModuleDelete={handleDeleteModule} moduleFolderChange={moduleFolderChange} />
			);
		});
	}

	function buildSelection(selectedModule) {
		let module = {
			packageFolderModuleId: 0,
			packageFolderId: currentFolder.packageFolderId,
			publishedModuleId: selectedModule.id,
			cache: selectedModule.cache
		};
		onAddModule(module);
	}

	function handleFolderDragOver(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;

		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		if (data.type !== 'folder' && data.type !== 'module') return;
		if (data.item.packageFolderId === currentFolder.packageFolderId) return;
		e.stopPropagation();
		e.dataTransfer.dropEffect = 'move';
		e.preventDefault();
	}

	function handleFolderDragStop() {
		const dropLocations = document.getElementsByClassName('folder-item-drop');
		const folderDropLocations = document.getElementsByClassName('folder-swap');
		Array.from(dropLocations).forEach((dragLocation) => {
			if (dragLocation.classList.contains('folder-dragging')) dragLocation.classList.toggle('folder-dragging');
		});
		Array.from(folderDropLocations).forEach((dragLocation) => {
			if (dragLocation.classList.contains('folder-dragging')) {
				dragLocation.classList.toggle('folder-dragging');
			}
		});
	}

	function handleFolderDragEnd(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;
		const data = JSON.parse(e.dataTransfer.getData('application/json'));

		data.item.parentFolderId = currentFolder.parentFolderId;

		onFolderChange(data.item);
	}

	function checkToChangeContentRole(movingFolderContentId) {
		return (currentFolder?.contentRoleId < 2 || (currentFolder?.contentRoleId === null || currentFolder?.contentRoleId === undefined) || currentFolder?.contentRoleId === movingFolderContentId);
	}

	function fectchModules() {
		if (modules?.length > 0) return;
		Requests.getFolderModules(currentFolder).then((response) => {
			setModules(response || []);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
		buildModules();
		forceUpdate();
	}

	function fectchSubFolders() {
		if (subFolders?.length > 0) return;
		Requests.getSubFolders(currentFolder).then((response) => {
			setSubFolders(response);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
		buildChildFolders();
		forceUpdate();
	}

	function handleNestFolder(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;
		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		e.stopPropagation();
		if (!checkToChangeContentRole(data.item.contentRoleId)) {
			const userConfirm = window.confirm('Are you sure you want to move this folder? The content role will change to the parent folder\'s content role.');
			if (!userConfirm) return;
			data.item.contentRoleId = currentFolder.contentRoleId;
		}
		data.item.parentFolderId = currentFolder.packageFolderId;

		onFolderChange(data.item);
	}

	function handleModuleFolderChange(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;
		const data = JSON.parse(e.dataTransfer.getData('application/json'));

		data.item.packageFolderId = currentFolder.packageFolderId;

		moduleFolderChange(data.item);
	}

	function handleFolderSwap(e) {
		if (e.dataTransfer.types[0] !== 'application/json') return;
		const data = JSON.parse(e.dataTransfer.getData('application/json'));
		e.stopPropagation();

		if (data.item.packageFolderId === currentFolder.packageFolderId) return;
		if (data.item.parentFolderId === currentFolder.packageFolderId) return;
		if (data.item.parentFolderId !== currentFolder.parentFolderId) return;
		if (data.item.parentFolderId === currentFolder.parentFolderId) {
			data.item.orderInParent = currentFolder.orderInParent;
		}

		onFolderReorder(data.item);
	}

	return (
		<>
			<div className='folder-item-drop' id={`folder-drop-${currentFolder.packageFolderId}`} onDragEnter={handleFolderDragOver} onDrop={handleFolderDragEnd} onDragOver={handleFolderDragOver}>
			</div>
			<div id={`folder-drop-${currentFolder.packageFolderId}`} onDragEnter={handleFolderDragOver} onDrop={handleFolderSwap} onDragOver={handleFolderDragOver}>
				<div className='module-drop-location' onDragEnter={handleFolderDragOver} onDrop={handleModuleFolderChange} onDragOver={handleFolderDragOver}>
					<div className='package-folder' id={folder.packageFolderId + '-package-folder'}>
						<NewItemDialog id={`new-item-dialog-${folder.packageFolderId}`}
							onFolder={() => onAdd(currentFolder.packageFolderId, currentFolder.contentRoleId)}
							onModule={(selectedModule) => buildSelection(selectedModule)} />
						<div className='folder-swap' onDragEnter={handleFolderDragOver} onDrop={handleFolderSwap} onDragOver={handleFolderDragOver}>
						</div>
						<header className={`${currentFolder.packageFolderId}-folder-header`} id='folder-drag' onDragEnd={handleFolderDragStop} draggable>
							<h3 className='package-folder-header-left'>
								<button onClick={handleToggleCollapse}
									id={`package-folder-carrot-${currentFolder.packageFolderId}`}>
									<FontAwesomeIcon icon={faCaretUp} />
								</button>
								<FontAwesomeIcon icon={isCollapsed ? faFolder : faFolderOpen} />
								{currentFolder?.displayName}
							</h3>
							<div className='package-folder-header-right'>
								{
									currentFolder.name === null ? <label>All Users</label> : <label>{currentFolder.name}</label>
								}
								<button id='package-folder-add' className={canEdit ? 'package-folder-add' : 'package-folder-add unauthorized'} onClick={addItem} disabled={!canEdit}>
									<FontAwesomeIcon icon={faPlus} />
								</button>
								<button className={canEdit ? 'package-folder-edit' : 'package-folder-edit unauthorized'} disabled={!canEdit} onClick={editFolder}>
									<FontAwesomeIcon icon={faPaintBrush} />
								</button>
								<button id='package-folder-header-remove' className={canEdit ? 'package-folder-header-remove' : 'package-folder-header-remove unauthorized'} onClick={deleteFolder}>
									<FontAwesomeIcon icon={faTrash} />
								</button>
							</div>
						</header>
						<div className='package-folder-items'>
							<div id={`package-folder-items-${currentFolder.packageFolderId}`}>
								{
									subFolders?.length > 0 || modules?.length > 0 ?
										buildChildFolders()
										: <p>No folders or modules inside this folder.</p>
								}
								{
									buildModules()
								}
								<div className='folder-item-drop' id={`folder-drop-${currentFolder.packageFolderId}`} onDragEnter={handleFolderDragOver} onDrop={handleNestFolder} onDragOver={handleFolderDragOver}>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<EditPackageFolderDialog id={`edit-folder-${currentFolder.packageFolderId}`} editDialogLoaded={editDialogLoaded} onUpdate={updateFolder} parentFolder={parentFolder} packageFolder={currentFolder} />
		</>
	);
}
