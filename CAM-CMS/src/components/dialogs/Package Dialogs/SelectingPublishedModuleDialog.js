import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Requests from '../../../utils/Requests';
import './styles/SelectingPublishedModuleDialog.css';

export default function SelectingPublishedModuleDialog({id, onRefresh,onSelect}) {
	const navigate = useNavigate();
	const [modules, setModules] = useState([]);
	const [selectedModule, setSelectedModule] = useState(null);
	
	useEffect(() => {
		if (onRefresh) {
			Requests.getAllPublishedModules().then(data => {
				if (!data || data === null) {
					alert('No published modules found');
					return;
				}
				console.log(data);
				setModules(data);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
		}
	}, [onRefresh]);
	

	function closeDialog() {
		document.getElementById(id).close();
	}	
	function selectModule(module) {

		setSelectedModule(module);
		let modules = document.getElementsByClassName('published-module-item');
		for(let classModule of modules) {
			if(module.id === parseInt(classModule.id)) {
				classModule.classList.add('selected');
			} else {
				classModule.classList.remove('selected');
			}
		}
	}
	return (
		<dialog id={id} className='published-module-dialog'>
			<header>
				<h1>Select Published Module</h1>
				{/* TODO: Add X icon */}
				<button onClick={closeDialog}>X</button>
			</header>
			<form>
				<div>
					{modules?.map((module, index) => {
						return (
							<div key={index}  className='published-modules' onClick={() => selectModule(module)}>
								<p className='published-module-item' id={module.id} >Id: {module.id} Title: {JSON?.parse(module.cache)?.title}</p>
							</div>
						);
					})}
				</div>
				<div>
					<button type='button' onClick={() => {
						if (selectedModule) {
							onSelect(selectedModule);
							closeDialog();
						}
					}}>Select</button>
					<button type='button' onClick={closeDialog}>Cancel</button>
				</div>
			</form>
		</dialog>
	);
}