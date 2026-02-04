import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import Requests from '../utils/Requests';
import ElementSet from '../components/Module Display/ElementSet';
import ElementSetStyleAside from '../components/Style Aside Panes/ElementSetStyleAside';
import ElementStyleAside from '../components/Style Aside Panes/ElementStyleAside';
import RenderedModule from '../components/Module Render Dialog/RenderedModule';
import ModuleCache from '../utils/ModuleCache';
import './styles/ModulesPage.css';

function useForceUpdate() {
	const [, setTick] = useState(0);
	const update = useCallback(() => {
		setTick((tick) => tick + 1);
	}, []);
	return update;
}

export default function ModulesPage() {
	const forceUpdate = useForceUpdate();
	const navigate = useNavigate();
	const { id } = useParams();

	const [module, setModule] = useState(undefined);
	const [isPublished, setIsPublished] = useState(false);

	const [currentDrag, setCurrentDrag] = useState(undefined);
	const [currentEdit, setCurrentEdit] = useState(undefined);

	const [canEdit, setCanEdit] = useState(true);
	const [canDelete, setCanDelete] = useState(true);

	const [isStyleAsideVisible, setIsStyleAsideVisible] = useState(false);

	const [editingElement, setEditingElement] = useState(null);
	const [editorContent, setEditorContent] = useState('');

	useEffect(() => {
		setModule(undefined);
		loadModule();
		checkPublished();
	}, [id]);

	useEffect(() => {
		const toggleStyleDialogs = () => {
			const styleProperties = document.getElementsByClassName('style-properties');
			if (styleProperties && styleProperties.length > 0) {
				styleProperties[0].classList.toggle('collapsed', !isStyleAsideVisible);
			}
		};
		toggleStyleDialogs();
	}, [isStyleAsideVisible]);

	/**
	 * Loads the module from the server
	 * 
	 * @returns {void}
	 * @post The module is loaded from the server
	 */
	function loadModule() {
		Requests.loadModule(id)
			.then((response) => {
				let currentSet = undefined;
				console.log(response);

				if (!response || response.status === 401 || response.status === 404) {
					navigate('/404');
					return;
				} else {
					response.elementSets = Array.isArray(response.elementSets) ? response.elementSets : [];
					response.elementSets.sort((a, b) => {
						if (a.styling.is_appendix && !b.styling.is_appendix) return 1;
						if (!a.styling.is_appendix && b.styling.is_appendix) return -1;
						return a.place - b.place;
					});
					response.elementSets.forEach((set) => {
						set.elements.sort((a, b) => a.place - b.place);
					});

					currentSet = response.elementSets[0];
				}

				if (!currentSet) {
					currentSet = {
						setLocationId: 0,
						moduleId: response.moduleId,
						place: 0,
						isEditable: true,
						styling: {
							isAppendix: false,
							hasPageBreak: 'false',
							isHorizontal: false
						},
						elements: []
					};
				}

				setCurrentEdit(currentSet);
				setModule(response);

				Requests.authorizeLibraryFolder('update', response.libraryFolderId).then((response) => {
					setCanEdit(response);
				});
				Requests.authorizeLibraryFolder('delete', response.libraryFolderId).then((response) => {
					setCanDelete(response);
				});
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	/**
	 * Checks if the module has been published
	 * 
	 * @returns {void}
	 * @post The module's published status is set
	 */
	function checkPublished() {
		Requests.hasPublishedModule({ moduleId: id })
			.then((response) => {
				setIsPublished(response);
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	/**
	 * Adds an element set to the module
	 * 
	 * @returns {void}
	 * @post An new element set is added to the module
	 */
	function addElementSet() {
		const newElementSet = {
			setLocationId: 0,
			moduleId: module.moduleId,
			place: module.elementSets.length,
			isEditable: true,
			styling: {
				isAppendix: false,
				hasPageBreak: 'false',
				isHorizontal: false
			},
			elements: []
		};

		moduleChanged(1, [newElementSet]);
	}

	/**
	 * Publish the module
	 * 
	 * @returns {void}
	 * @post The module is published
	 */
	function publishModule() {
		const moduleCache = ModuleCache.buildModuleCache(module);
		console.log('module cache');
		console.log(moduleCache);
		const publishedModule = {
			id: module.moduleId,
			cache: JSON.stringify(moduleCache)
		};

		Requests.createPublishedModule(publishedModule)
			.then((response) => {
				if (response === undefined || response.status === 401) {
					alert('You are not authorized to publish this module.');
					return;
				}
				if (!isPublished) {
					alert('Module Published');
					setIsPublished(true);
				} else {
					alert('Module updated');
				}
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	/**
	 * Unpublish the module
	 * 
	 * @returns {void}
	 * @post The module is unpublished
	 */
	function unpublishModule() {
		Requests.deletePublishedModule({ id: module.moduleId })
			.then(response => {
				if (response === undefined) {
					alert('Failed to unpublish the module. The module is currently in use.');
					return;
				}
				if (response.status === 401) {
					alert('You are not authorized to unpublish this module.');
					return;
				}
				if (!response.ok) {
					throw new Error('Failed to unpublish the module. The module is currently in use.');
				}

				alert('Module unpublished');
				setIsPublished(false);

				return response.json();
			})
			.catch((error) => {
				console.error('Failed to unpublish the module:', error);
				alert('Failed to unpublish the module. The module is in use by one or more packages, or an unknown error occurred.');
			});
	}

	/**
	 * Callback for when the module is changed
	 * 
	 * @param {int} flag - The flag for the change
	 * 	- 1 - Add Set
	 * 	- 2 - Delete Set
	 * 	- 4 - Move Set
	 * 	- 8 - Add Element
	 * 	- 16 - Delete Element
	 * 	- 32 - Move Element
	 * 	- 64 - Toggle Set Lock
	 * 	- 128 - Toggle Element Lock
	 * 	- 256 - Edit Set
	 * 	- 512 - Edit Element
	 * 	- 1024 - Swap Element
	 * @param {object} data - The data for the change as a list
	 * 
	 * @returns {void}
	 */
	function moduleChanged(flag, data) {
		const ADD_SET = 0b00000000001;
		const DELETE_SET = 0b00000000010;
		const MOVE_SET = 0b00000000100;
		const ADD_ELEMENT = 0b00000001000;
		const DELETE_ELEMENT = 0b00000010000;
		const MOVE_ELEMENT = 0b00000100000;
		const TOGGLE_SET_LOCK = 0b00001000000;
		const TOGGLE_ELEMENT_LOCK = 0b00010000000;
		const EDIT_SET = 0b00100000000;
		const EDIT_ELEMENT = 0b01000000000;
		const SWAP_ELEMENT = 0b10000000000;

		if (flag & ADD_SET) {
			Requests.addSetToModule(data[0])
				.then((response) => {
					if (!response || response.status === 401) {
						alert('You are not authorized to add a new set.');
						return;
					}
					if (!Array.isArray(response)) {
						alert('Add set failed, contact an administrator');
						return;
					}
					
					module.elementSets = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Add set failed:', error);
					navigate('/error');
				});
		} else if (flag & DELETE_SET) {
			Requests.deleteSetFromModule(data[0])
				.then((response) => {
					if (!response || response.status === 401) {
						alert('You are not authorized to delete this set.');
						return;
					}
					module.elementSets = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Delete set failed:', error);
					navigate('/error');
				});
		} else if (flag & MOVE_SET) {
			data[0].place = data[1];
			Requests.moveSetInModule(data[0])
				.then((response) => {
					if (!response || response.status === 401) {
						alert('You are not authorized to move this set.');
						return;
					}
					module.elementSets = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Move set failed:', error);
					navigate('/error');
				});
		} else if (flag & ADD_ELEMENT) {
			Requests.addElementToModule(data[0])
				.then((response) => {
					if (!response || response.status === 401) {
						alert('You are not authorized to add a new element.');
						return;
					}
					const setIndex = module.elementSets.findIndex(set => set.setLocationId === response.setLocationId);
					module.elementSets[setIndex] = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Add element failed:', error);
					navigate('/error');
				});
		} else if (flag & DELETE_ELEMENT) {
			Requests.deleteElementFromModule(data[0])
				.then((response) => {
					if (!response || response.status === 401) {
						alert('You are not authorized to delete this element.');
						return;
					}
					const setIndex = module.elementSets.findIndex(set => set.setLocationId === response.setLocationId);
					module.elementSets[setIndex] = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Delete element failed:', error);
					navigate('/error');
				});
		} else if (flag & MOVE_ELEMENT) {
			if (data[0].setLocationId === data[1].setLocationId) {
				data[0].place = data[1].place;
				Requests.moveElementInSet(data[0])
					.then((response) => {
						if (!response || response.status === 401) {
							alert('You are not authorized to move this element.');
							return;
						}
						const setIndex = module.elementSets.findIndex(set => set.setLocationId === response.setLocationId);
						module.elementSets[setIndex] = response;
						updateDisplay();
					})
					.catch((error) => {
						console.error('Move element in set failed:', error);
						navigate('/error');
					});
			} else if (!data[1].elementId) {
				const newSet = JSON.parse(JSON.stringify(data[0]));
				newSet.setLocationId = data[1].setLocationId;
				Requests.moveElementToNewSet(data[0], newSet)
					.then((response) => {
						if (!response || response.status === 401) {
							alert('You are not authorized to move this element.');
							return;
						}
						module.elementSets = response;
						updateDisplay();
					})
					.catch((error) => {
						console.error('Move element to new set failed:', error);
						navigate('/error');
					});
			} else {
				Requests.moveElementSetAndPlace(data[0], data[1])
					.then((response) => {
						if (!response || response.status === 401) {
							alert('You are not authorized to move this element.');
							return;
						}
						module.elementSets = response;
						updateDisplay();
					})
					.catch((error) => {
						console.error('Move element set and place failed:', error);
						navigate('/error');
					});
			}
		} else if (flag & EDIT_SET) {
			setCurrentEdit(data[0]);
			if (!data[1]) {
				toggleStyleDialogs();
			}
			Requests.editSetInModule(data[0])
				.then((response) => {
					if (response === undefined || response.status === 401) {
						alert('You are not authorized to edit this set.');
						return;
					}
					if (!Array.isArray(response)) {
						console.error('Edit set response is not an array:', response);
						return;
					}
					module.elementSets = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Edit set failed:', error);
					navigate('/error');
				});
		} else if (flag & TOGGLE_SET_LOCK) {
			Requests.editSetInModule(data[0])
				.then((response) => {
					if (response === undefined || response.status === 401) {
						alert('You are not authorized to toggle the lock on this set.');
						return;
					}
					if (!Array.isArray(response)) {
						console.error('Toggle set lock response is not an array:', response);
						return;
					}
					module.elementSets = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Toggle set lock failed:', error);
					navigate('/error');
				});
		} else if (flag & EDIT_ELEMENT) {
			setCurrentEdit(data[0]);
			if (!data[1]) {
				toggleStyleDialogs();
			}
			Requests.editElementInModule(data[0])
				.then((response) => {
					if (response === undefined || response.status === 401) {
						alert('You are not authorized to edit this element.');
						return;
					}
					const setIndex = module.elementSets.findIndex(set => set.setLocationId === response.setLocationId);
					if (setIndex === -1) {
						console.error('Edit element: set not found');
						return;
					}
					module.elementSets[setIndex] = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Edit element failed:', error);
					navigate('/error');
				});
		} else if (flag & TOGGLE_ELEMENT_LOCK) {
			setCurrentEdit(data[0]);
			Requests.editElementInModule(data[0])
				.then((response) => {
					if (response === undefined || response.status === 401) {
						alert('You are not authorized to toggle the lock on this element.');
						return;
					}
					const setIndex = module.elementSets.findIndex(set => set.setLocationId === response.setLocationId);
					if (setIndex === -1) {
						console.error('Toggle element lock: set not found');
						return;
					}
					module.elementSets[setIndex] = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Toggle element lock failed:', error);
					navigate('/error');
				});
		} else if (flag & SWAP_ELEMENT) {
			Requests.swapElementInModule(data[0])
				.then((response) => {
					if (response === undefined || response.status === 401) {
						alert('You are not authorized to swap this element.');
						return;
					}
					const setIndex = module.elementSets.findIndex(set => set.setLocationId === response.setLocationId);
					if (setIndex === -1) {
						console.error('Swap element: set not found');
						return;
					}
					module.elementSets[setIndex] = response;
					updateDisplay();
				})
				.catch((error) => {
					console.error('Swap element failed:', error);
					navigate('/error');
				});
		} else {
			console.error('Unknown flag', flag);
			navigate('/error');
		}
		updateDisplay();
	}

	function updateDisplay() {
		if (!Array.isArray(module.elementSets)) {
			console.error('module.elementSets is not an array', module.elementSets);
			return;
		}
		module.elementSets.sort((a, b) => {
			if (a.styling.is_appendix && !b.styling.is_appendix) return 1;
			if (!a.styling.is_appendix && b.styling.is_appendix) return -1;
			return a.place - b.place;
		});
		module.elementSets.forEach((set) => {
			set.elements.sort((a, b) => a.place - b.place);
		});
		forceUpdate();
	}

	/**
	 * Renders the module
	 * 
	 * @returns {void}
	 */
	function renderModule() {
		const dialog = document.getElementById('rendered-module-dialog');
		dialog.showModal();
	}

	/**
	 * Opens and closes the style dialogs for the module
	 */
	function toggleStyleDialogs() {
		setIsStyleAsideVisible((prevState) => !prevState);

		const styleProperties = document.getElementsByClassName('style-properties');

		if (styleProperties) {
			if (styleProperties.length === 0) return;
			styleProperties[0].classList.toggle('collapsed');
		}
	}

	function saveStyleDialog(code, data) {
		toggleStyleDialogs();
		moduleChanged(code, [data, true]);
		handleCancelEdit();
	}

	function deleteModule() {
		Requests.deleteModule(module)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You are not authorized to delete this module.');
					return;
				}
				if (!response.ok) {
					alert('Failed to delete the module');
					return;
				}
				alert('Module deleted');
				navigate('/studio');
			})
			.catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	function handleEditElement(location) {
		setEditingElement(location);
		setEditorContent(JSON.parse(location.element.content).content);
	}

	function handleSaveEdit() {
		if (editingElement) {
			const updatedElement = {
				...editingElement,
				element: {
					...editingElement.element,
					content: JSON.stringify({ content: editorContent }),
				},
			};
			console.log('updated element');
			console.log(updatedElement);
			postElementUpdate(updatedElement);
		}
	}

	function postElementUpdate(updatedElement) {
		Requests.updateElement(updatedElement.element)
			.then((response) => {
				if (!response || response.status === 401) {
					alert('You are not authorized to update this element.');
					return;
				}

				if (!response.ok) {
					alert('Failed to update the element');
					return;
				}
				moduleChanged(512, [{ ...editingElement, updatedElement: response }]);
				handleCancelEdit();
			})
			.catch(error => {
				console.error('Error:', error);
				navigate('/error');
			});
	}

	function handleCancelEdit() {
		setEditingElement(null);
		setEditorContent('');
	}

	if (module === undefined) {
		return <div className='loading'>Loading...</div>;
	}

	const elementStyleEditor = (
		<ElementStyleAside className='Module-element-style-aside collapsed'
			selectedElement={currentEdit}
			onStyleUpdate={(data) => saveStyleDialog(512, data)}
			onStyleCancel={() => { toggleStyleDialogs(); handleCancelEdit(); }}
			handleSaveEdit={() => { toggleStyleDialogs(); handleSaveEdit(); }} />
	);
	const setStyleEditor = (
		<ElementSetStyleAside
			className='Module-element-set-style-aside collapsed'
			selectedElementSet={currentEdit}
			onStyleUpdate={(data) => saveStyleDialog(256, data)}
			onStyleCancel={() => { toggleStyleDialogs(); handleCancelEdit(); }}
		/>
	);

	return (
		<div className='module-display'>
			<RenderedModule id='rendered-module-dialog' currentModule={module} moduleJson={ModuleCache.buildModuleCache(module)} isPublished={isPublished} />
			<div className='module-editing-display'>
				<div className='module-heading row'>
					<div className='module-heading-left row'>
						<h1>{module.title}</h1>
						<h2 className='module-heading-meta-data'>
							<FontAwesomeIcon icon={faCircleInfo} className='module-heading-meta-data-icon' onClick={
								() => {
									const metaDialog = document.getElementsByClassName('module-meta-data')[0];
									metaDialog.style.display = metaDialog.style.display === 'none' || metaDialog.style.display === '' ? 'flex' : 'none';
								}} />
							<div className='module-meta-data'>
								<h5>Created At: {module.createdAt}</h5>
								<h5 className='data-link'
									onClick={() => navigate('/people/' + module.createdBy)}>
									Created By: User {module.createdBy}
								</h5>
							</div>
						</h2>
						<button onClick={renderModule}>Render</button>
					</div>
					<div className='module-heading-right row'>
						<button onClick={publishModule}>{isPublished ? 'Republish' : 'Publish'}</button>
						<button onClick={unpublishModule} style={{ display: isPublished ? 'block' : 'none' }}>Unpublish</button>
						<button className={canEdit ? 'module-button-edit' : 'module-button-edit unauthorized'} disabled={!canEdit} onClick={(e) => addElementSet(e)}>Add Set</button>
						<button className={canDelete ? 'module-button-edit' : 'module-button-edit unauthorized'} disabled={!canDelete}
							onClick={() => {
								if (isPublished) {
									alert('Cannot delete a published module');
									return;
								}

								if (window.confirm('Are you sure you want to delete the Module? All content will be lost.')) {
									deleteModule();
								}
							}}>
							Delete Module
						</button>
					</div>
				</div>
				<div className='module-content scrollable'>
					{
						module.elementSets.map((elementSet, index) => {
							return (
								<ElementSet key={index} elementSet={elementSet} libraryFolderId={module.libraryFolderId} canEdit={canEdit} onChange={moduleChanged}
									draggingItem={currentDrag} dragItemChanged={setCurrentDrag} isStyleAsideVisible={isStyleAsideVisible}
									onEditElement={handleEditElement} editingElement={editingElement} editorContent={editorContent} setEditorContent={setEditorContent}
									handleSaveEdit={handleSaveEdit} handleCancelEdit={handleCancelEdit} />
							);
						})
					}
				</div>
			</div>
			{
				currentEdit?.moduleId ? setStyleEditor : elementStyleEditor
			}
		</div>
	);
}
