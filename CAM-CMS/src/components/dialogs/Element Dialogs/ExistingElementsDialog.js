import React, { useState, useEffect } from 'react';

import Requests from '../../../utils/Requests';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

export default function ExistingElementsDialog(props) {
	const [elements, setElements] = useState([]);
	const [selectedElement, setSelectedElement] = useState(undefined);
	const [loadedElements,setLoadedElements] = useState([]);
	const [selectedType, setSelectedType] = useState(2);
	const displaySelectedElement = selectedElement ? 'block' : 'none';
	const navigate = useNavigate();

	useEffect(() => {
		if (props.refresh) {
			Requests.getAllElements().then(data => {
				var elements = [];
				data.forEach(library => {
					library.elements.forEach(element => {
						elements.push(element);
					});
				});
				setLoadedElements(elements);
				setElements(elements);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
		}
	}, [props.refresh]);

	function selectElement(element) {
		setSelectedElement(element);
	}

	function resetElementList() {    
		setElements(loadedElements);  
	}

	function filterList(term, list) {
		var filteredList = list.map(entry => {
			if (entry.title.toUpperCase().includes(term.toUpperCase())) {
				return entry;
			}
		});
		filteredList = filteredList.filter(function (element) {
			return element !== undefined;
		});

		return filteredList;
	}

	function search(term, list) {
		if (term !== undefined && term !== '') {
			var searchedModules = filterList(term, list);
			setElements(searchedModules);
		}
		else {
			resetElementList();
		}
	}

	function searchElements(term) {
		search(term,elements,setElements);
	}

	function closeDialog() {
		var dialog = document.getElementById(props.id);

		setSelectedElement(undefined);
		dialog.close();
	}

	function returnElement(e) {
		e.preventDefault();
		var data = selectedElement;
		if(data !== undefined) {
			props.onChange(data);
			closeDialog();
		}
        
	}

	return (
		<dialog id={props.id} className='existing-elements'>
			<div className='dialog-header'>
				<h1>Select Element</h1>
				<button id='close-button' onClick={closeDialog}><FontAwesomeIcon icon={faXmark} /></button>
			</div>
			<h3 >Selected Element: </h3>
			<div className='display-selection' style={{ display: displaySelectedElement }}>
				<h3 id='selected-element'>{selectedElement?.title}</h3>
				<p id='selected-element'>{selectedElement?.description}</p>
			</div>
			<div className='search-element-bar'>
				<div className='studio-nav-page-items-container-search hidden'>
					<input type='text' placeholder='Search' onChange={(e) => searchElements(e.target.value)} />
					{/* <button onClick={toggleSearchMode}>
						<FontAwesomeIcon icon={faX} />
					</button> */}
				</div>
			</div>
			<form className='dialog-content' onSubmit={returnElement}>
				<div className='row existing-element-tags'>
					<div className={`column existing-element-tag ${selectedType === 2 ? 'existing-element-tag-selected' : ''}`} 
						onClick={() => setSelectedType(2)}>
						<h3>Images</h3>
					</div>
					<div className={`column existing-element-tag ${selectedType === 4 ? 'existing-element-tag-selected' : ''}`} 
						onClick={() => setSelectedType(4)}>
						<h3>Videos</h3>
					</div>
					<div className={`column existing-element-tag ${selectedType === 5 ? 'existing-element-tag-selected' : ''}`} 
						onClick={() => setSelectedType(5)}>
						<h3>PDFs</h3>
					</div>
				</div>
				<div id='element-cards' className='element-cards'>
					{elements.map((element, index) => {

						if (element.typeId !== selectedType) {
							return null;
						}

						return (
							<div id='element-card'className='element-card' key={index} onClick={() => selectElement(element)}>
								<h3>{element.title}</h3>
								<div dangerouslySetInnerHTML={{ __html: element.content ? element.content.content : null }} />
							</div>
						);
					})}
				</div>
				<div id='element-dialog-add-button'>
					<input type='submit' value='Add' />
				</div>
			</form>
		</dialog>
	);
}