import React, { useEffect, useState,useCallback } from 'react';

import { useNavigate, useParams } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleInfo } from '@fortawesome/free-solid-svg-icons';

import PackageFolder from '../components/Package Display/PackageFolder';

import Requests from '../utils/Requests';

import './styles/PackagesPage.css';
import RenderedPackage from '../components/Package Render Dialog/RenderedPackage';
import PublishPackageDialog from '../components/dialogs/Package Dialogs/PublishPackageDialog';
import NewPackageFolderDialog from '../components/dialogs/Package Dialogs/NewPackageFolderDialog';

function useForceUpdate() {
	const [, setTick] = useState(0);
	const update = useCallback(() => {
		setTick((tick) => tick + 1);
	}, []);
	return update;
}

export default function PackagesPage() {

	const navigate = useNavigate();
	const { id } = useParams();

	const forceUpdate = useForceUpdate();

	const [packageData, setPackageData] = useState(null);
	const [isPublished, setPublished] = useState(false);

	
	//eslint-disable-next-line
	const [currentDragItem, setDragItem] = useState(null);

	const [folderDialogData, setFolderDialogData] = useState(null);
	const [loadContentRoles, setLoadContentRoles] = useState(false);
	const [showOrganizations, setShowOrganizations] = useState(false);

	const [folders , setFolders] = useState([]);
	const [canEdit, setCanEdit] = useState(true);
	const [canDelete, setCanDelete] = useState(true);

	useEffect(() => {
		return () => {
			setPackageData(null);
			setFolders([]);
		};
	}, [id]);

	useEffect(() => {
		Requests.getPackage(id)
			.then((response) => {
				if (response.status && (response.status === 404 || response.status === 401)) {
					navigate('/404');
				}
				setPackageData(response);
			}).catch((error) => {
				console.error(error); 
				navigate('/error');
			});
		// TODO: Get package published data from server
		setPublished(false);

		Requests.authorizePackage('update', id).then((response) => {
			setCanEdit(response);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});

		Requests.authorizePackage('delete', id).then((response) => {
			setCanDelete(response);
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});

	}, [id]);

	useEffect(() => {
		let folders = packageData?.packageFolders?.sort((a, b) => a.orderInParent - b.orderInParent);
		setFolders(folders);
	}, [packageData]);
	
	function updateDisplay() {
		Requests.getPackage(id)
			.then((response) => {
				if (response.status && response.status === 404 || response.status === 401) {
					navigate('/404');
				}
				setPackageData(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}
	
	async function addPackageFolder(parentId,contentRoleId) {
		let data = {
			parentFolderId: parentId,
			contentRoleId: contentRoleId
		};
		setFolderDialogData(data);
		setLoadContentRoles(true);
		const dialog = document.getElementById('new-package-folder-dialog');
		await dialog.showModal();
		setLoadContentRoles(false);
	}

	function createPackageFolder(data) {
		let newFolder = {
			FolderTypeId: 1,
    		DisplayName: data?.displayName,
			FullDescription: 'New Folder Description for testing purposes.',
			ShortDescription: 'New Folder Description',
			Thumbnail: '',
			ContentRoleId: data?.contentRoleId,
			PackageId: packageData.packageId,
			Editable: true,
			ParentFolderId: data?.parentFolderId,
			OrderInParent: packageData.packageFolders.length
		};
		
		Requests.addPackageFolder(newFolder)
			.then((response) => {
				if (response === 'New folder creation failed.') {
					console.error('Error creating new folder');
					navigate('/error');
					return;
				}

				if (response.status && response.status === 404) {
					navigate('/404');
				}

				if (response.status && response.status === 401) {
					alert('You do not have permission to create a folder');
				}

				setPackageData(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}
	function deletePackage() {
		if (isPublished) {
			alert('Cannot delete a published package');
			return;
		}

		var confirmDelete = window.confirm('Are you sure you want to delete this package?\nAll content will be lost.');
		if (confirmDelete) {
			Requests.deletePackage(packageData).then((response) => {
				if(!response || response.status === 401) {
					alert('You do not have permission to delete this package');
				}
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});

			navigate('/studio');
		}
	}

	function deletePackageFolder(folder) {
		Requests.deletePackageFolder(folder)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You do not have permission to delete this folder');
				}

				if (response.status && response.status === 404) {
					navigate('/404');
				}

				setPackageData(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}
	function addPackageModule(module) {
		Requests.createPackageFolderModule(module)
			.then(() => {
				setPackageData([]);
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
		updateDisplay();
		forceUpdate();
	}
	function deletePackageModule(module) {	
		Requests.deletePackageFolderModule(module)
			.then(() => {
				setPackageData([]);
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
		updateDisplay();
		forceUpdate();
	}
	function renderPackage() {
		let dialog = document.getElementById('rendered-package-dialog');
		dialog.showModal();
	}

	async function publishPackage() {
		let dialog = document.getElementById('publish-package-dialog');
		try
		{
			setShowOrganizations(true);
			await dialog.showModal();
			setShowOrganizations(false);
		}
		catch(e)
		{
			dialog.close();
			setShowOrganizations(false);
			alert('Error publishing package');
		}
	}

	function unpublishPackage() {
		alert('Not Implemented\nUnpublish package'); // TODO: Implement unpublish package
	}

	if (packageData === undefined) {
		return (
			<div>
				<h1>Loading...</h1>
			</div>
		);
	}
	async function folderChanged(data) {
		setPackageData([]);
		setFolders([]);
		await Requests.movePackageFolder(data).then((response) => {
			if (!response || response.status === 401) {
				alert('You do not have permission to move this folder');
			}
			if(response !== undefined) {		
				setPackageData(response);
				setFolders(response.packageFolders);
			}
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
	}
	async function folderReorder(data) {
		setPackageData([]);
		setFolders([]);
		await Requests.reorderPackageFolder(data).then((response) => {
			if (!response || response.status === 401) {
				alert('You do not have permission to reorder this folder');
			}
			if(response !== undefined) {
				setPackageData(response);
				setFolders(response.packageFolders);
			}
		}).catch((error) => {
			console.error(error);
			navigate('/error');
		});
		
	}
	async function changeModuleFolder(data) {
		await Requests.updatePackageFolderModule(data)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You do not have permission to move this module');
				}
				if(response !== undefined) {
					setPackageData(response);
				}
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}
	async function moduleChanged(data) {
		let draggedModule = data[0];
		let droppedOnModule = data[1];
		draggedModule.orderInFolder = droppedOnModule.orderInFolder;	
		await Requests.reorderPackageFolderModule(draggedModule).then((response) => {
			if (!response || response.status === 401) {
				alert('You do not have permission to reorder this module');
			}
			setPackageData(response);
		})
			.catch((error) => {
				console.error(error); 
				navigate('/error');
			});
	}
	

	return (
		<div className='package-display'>
			<div className='package-heading row'>
				<div className='package-heading-left row'>
					<h1>{packageData?.name}</h1>

					<h2 className='package-heading-meta-data'>
						<FontAwesomeIcon icon={faCircleInfo} className='package-heading-meta-data-icon' onClick={
							() => {
								const metaDialog = document.getElementsByClassName('package-meta-data')[0];
								metaDialog.style.display = metaDialog.style.display === 'none' || metaDialog.style.display === '' ? 'flex' : 'none';
							}}/>
						<div className='package-meta-data'>
							<h5>Created At: {packageData?.createdAt}</h5> {/* TODO: Convert date and time to correct time zone */}
							<h5 className='data-link'
								onClick={() => navigate('/people/' + packageData?.createdBy)}>
								Created By: User {packageData?.createdBy}
							</h5>
						</div>
					</h2>
					<button onClick={renderPackage}>Render</button>
				</div>
				<div className='package-heading-right row'>
					<button onClick={publishPackage}>{isPublished ? 'Republish' : 'Publish'}</button>
					<button onClick={unpublishPackage} style={{display: isPublished ? 'block' : 'none'}}>Unpublish</button>
					<button className={canEdit ? 'package-button-edit' : 'package-button-edit unauthorized'} disabled={!canEdit} 
						onClick={() => addPackageFolder(null)}>
						Add Folder
					</button>
					<button className={canDelete ? 'package-button-edit' : 'package-button-edit unauthorized'} disabled={!canDelete}
						onClick={deletePackage}>
							Delete Package
					</button>
				</div>
			</div>
			<div className='package-content'>
				<div>
					{
						folders?.map((folder, index) => {
							return (
								<PackageFolder key={index} folder={folder} 
									onAdd={addPackageFolder} parentFolder = {null} onDelete={deletePackageFolder} onAddModule={addPackageModule} onModuleDelete={deletePackageModule}
									changeSelected={setDragItem} onFolderChange={folderChanged} onFolderReorder={folderReorder} onModuleChange={moduleChanged} moduleFolderChange={changeModuleFolder}
									canEdit={canEdit}/>
							);
						})
					}
				</div>
			</div>
			<RenderedPackage id='rendered-package-dialog' packageData={packageData}/>
			<PublishPackageDialog id='publish-package-dialog'onLoadOrganizations={showOrganizations} packageId={id}/>
			<NewPackageFolderDialog id='new-package-folder-dialog'folderDialogData={folderDialogData} loadContentRoles={loadContentRoles} createPackageFolder={createPackageFolder}/>
		</div>
	);
}
