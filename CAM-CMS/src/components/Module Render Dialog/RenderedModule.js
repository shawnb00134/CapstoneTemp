import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Requests from '../../utils/Requests';
import './styles/RenderedModule.css';
import RenderedModuleContent from './RenderedModuleContent';

/**
 *  Renders the module based on the moduleJson and gives the user a vision of what the module will look like.
 * 
 * @param {JSON} moduleJson the json of the module to be rendered 
 * 
 * @returns {RenderedModule} The rendered module.
 */
export default function RenderedModule({ id, currentModule, moduleJson, isPublished }) {
	const [selectedTab, setSelectedTab] = useState(0);
	const [showPublishedModule, setShowPublishedModule] = useState(false);
	const [publishedModule, setPublishedModule] = useState(null);
	const navigate = useNavigate();

	useEffect(() => {
		const compareButton = document.getElementById('compare-button');
		if (compareButton) {
			compareButton.classList.toggle('unauthorized');
		}
	}, [isPublished]);

	useEffect(() => {
		const tabs = document.getElementsByClassName('render-module-tab-selector');
		for (let i = 0; i < tabs.length; i++) {
			tabs[i].classList.remove('selected-render-tab');
		}

		tabs[selectedTab].classList.add('selected-render-tab');

	}, [selectedTab]);

	async function loadPublishedModuleRequest() {
		await Requests.loadPublishedModuleById(currentModule.moduleId).then(data => {
			if (!data || data.status === 401) {
				alert('No published module found, or you do not have access to it.');
				return null;
			}
			console.log(data.status);
			setPublishedModule(data);
		}).catch(error => {
			console.error(error);
			navigate('/error');
		});

	}

	async function handleShowPublishModule() {
		if (showPublishedModule) {
			setShowPublishedModule(false);
		} else {
			let result = await loadPublishedModuleRequest();

			if (result !== null) {
				setShowPublishedModule(true);
			}
		}
	}

	function closeDialog() {
		var dialog = document.getElementById('rendered-module-dialog');
		setShowPublishedModule(false);
		dialog.close();
	}

	return (
		<dialog id={id} className='rendered-module'>
			<div className='rendered-heading row'>
				<div className='rendered-heading-left'>
					<h1>Rendered {moduleJson.title}</h1>
				</div>
				<div className='rendered-heading-right'>
					<button className='compare-published-button' id='compare-button' onClick={handleShowPublishModule} disabled={!isPublished}>Compare Publish</button>
				</div>
				<div className='rendered-heading-right'>
					<button onClick={closeDialog}>Close</button>
				</div>
			</div>

			<div className='row' id='render-module-tabs'>
				<div className='render-module-tab-selector'
					onClick={() => setSelectedTab(0)}>
					<h3>Content</h3>
				</div>
				<div className='render-module-tab-selector'
					onClick={() => setSelectedTab(1)}>
					<h3>Documents</h3>
				</div>
			</div>
			<div className='row' id='rendered-content-table'>
				<div style={{ display: 'flex' }} className={showPublishedModule ? 'rendered-content-published' : 'rendered-content-nonpublished'}>
					<RenderedModuleContent moduleJson={moduleJson} selectedTab={selectedTab} title={'Current Module'} />
				</div>
				<div style={{ display: showPublishedModule ? 'flex' : 'none' }}>
					{showPublishedModule && (
						<RenderedModuleContent moduleJson={JSON.parse(publishedModule.cache)} style={{ display: showPublishedModule ? 'none' : 'block' }} title={'Published Module'} selectedTab={selectedTab} />
					)
					}
				</div>
			</div>
		</dialog>
	);
}
