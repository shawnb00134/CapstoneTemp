import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLink, faLinkSlash } from '@fortawesome/free-solid-svg-icons';
import './styles/StylePropertiesAside.css';

/**
 * Allows the user to change the style of the selected element.
 * 
 * @param {*} props 
 * @returns {ElementStyleAside} The element style aside component.
 */
export default function ElementStyleAside(props) {
	const navigate = useNavigate();
	const maxHeight = 1000;
	const maxWidth = 1500;

	const [width, setWidth] = useState(undefined);
	const [height, setHeight] = useState(undefined);
	const [alignment, setAlignment] = useState(undefined);
	const [displayLink, setDisplayLink] = useState(undefined);
	const [ratioLocked, setRatioLocked] = useState(false);
	const [element, setElement] = useState(undefined);
	const [sectionHeaderLevel, setSectionHeaderLevel] = useState(undefined);
	const [url, setUrl] = useState('');

	useEffect(() => {
		setWidth(undefined);
		setHeight(undefined);
		setAlignment(undefined);
		setRatioLocked(false);
		setDisplayLink(undefined);
		setElement(props.selectedElement);

		if (props.selectedElement !== undefined && props.selectedElement.locationAttributeJson) {
			try {
				const locationAttributes = JSON.parse(props.selectedElement.locationAttributeJson) || {};
				setWidth(locationAttributes.Width !== null ? locationAttributes.Width : 'auto');
				setHeight(locationAttributes.Height !== null ? locationAttributes.Height : 'auto');
				setAlignment(locationAttributes.Alignment !== null ? locationAttributes.Alignment : 'left');
				setSectionHeaderLevel(locationAttributes.HeadingLevel ? locationAttributes.HeadingLevel : 0);
				setDisplayLink(locationAttributes.DisplayLink ? locationAttributes.DisplayLink : 'false');

				// Handle video elements
				if (props.selectedElement.element.typeId === 4) {
					const content = JSON.parse(props.selectedElement.element.content);
					setUrl(content.url);
				} else if (props.selectedElement.element.typeId === 2 || props.selectedElement.element.typeId === 5) {
					setUrl(props.selectedElement.element.content);
				}
			} catch (error) {
				console.error('Failed to parse locationAttributeJson:', error);
			}
		}
	}, [props.selectedElement]);

	/** 
	* Brief description of the function here.
	* @summary If the description is long, write your summary here. Otherwise, feel free to remove this.
	* @param {ParamDataTypeHere} parameterNameHere - Brief description of the parameter here. Note: For other notations of data types, please refer to JSDocs: DataTypes command.
	* @return {ReturnValueDataTypeHere} Brief description of the returning value here.
	*/
	function getElementType() {
		switch (element?.element?.typeId) {
		case 1:
			return 'Text';
		case 2:
			return 'Image';
		case 4:
			return 'Video';
		case 5:
			return 'PDF';
		default:
			return 'Anchor';
		}
	}

	/**
	 * gets the values from the input fields and sets them to the element.
	 * sets the element location attribute json to the new values.
	 * sets the element attributes to the new values.
	 */
	function getNewValues() {
		element.attributes.width = width;
		element.attributes.height = height;
		element.attributes.alignment = alignment;
		element.attributes.headingLevel = parseInt(sectionHeaderLevel);
		element.attributes.displayLink = (String(displayLink).toLowerCase() === 'true');

		// Update content for video elements
		if (element.element.typeId === 4) {
			const content = JSON.parse(element.element.content);
			content.url = url;
			element.element.content = JSON.stringify(content);
		} else if (element.element.typeId === 2 || element.element.typeId === 5) {
			element.element.content = url;
		}

		element.locationAttributeJson = JSON.stringify({
			Width: width,
			Height: height,
			Alignment: alignment,
			HeadingLevel: parseInt(sectionHeaderLevel),
			DisplayLink: (String(displayLink).toLowerCase() === 'true')
		});
	}

	function updateInputValues() {
		console.log(document.getElementById('style-input-width'));
		const newWidth = document.getElementById('style-input-width').value;
		const newHeight = document.getElementById('style-input-height').value;
		const newAlignment = document.getElementById('style-select-alignment').value;
		const newSectionHeaderLevel = document.getElementById('style-input-level')?.value;
		const newDisplayLink = document.getElementById('style-select-display-link')?.value;
		const newUrl = document.getElementById('style-input-url')?.value;

		if(newWidth <= maxWidth){
			setWidth(newWidth);
		}
		setAlignment(newAlignment);
		setSectionHeaderLevel(newSectionHeaderLevel);
		setDisplayLink(newDisplayLink);
		setUrl(newUrl);

		if (ratioLocked) {
			setHeight('auto');
		} else {
			if (newHeight <= maxHeight) {
				setHeight(newHeight);
			}
		}
	}

	/**
	 * locks the ratio of the element sizing.
	 */
	function lockRatio() {
		setRatioLocked(!ratioLocked);
		getNewValues();
	}
	/**
	 * saves the changes to the element.
	 */
	function saveChanges() {
		getNewValues();
		props.onStyleUpdate(element);
		if (element.element.typeId === 1) {
			props.handleSaveEdit();
		}
	}

	function handleCancel() {
		if (props.selectedElement !== undefined && props.selectedElement.locationAttributeJson) {
			try {
				const locationAttributes = JSON.parse(props.selectedElement.locationAttributeJson) || {};
				setWidth(locationAttributes.Width !== null ? locationAttributes.Width : 'auto');
				setHeight(locationAttributes.Height !== null ? locationAttributes.Height : 'auto');
				setAlignment(locationAttributes.Alignment !== null ? locationAttributes.Alignment : 'left');
				setSectionHeaderLevel(locationAttributes.HeadingLevel ? locationAttributes.HeadingLevel : 0);
				setDisplayLink(locationAttributes.DisplayLink ? locationAttributes.DisplayLink : 'false');

				if (props.selectedElement.element.typeId === 4) {
					const content = JSON.parse(props.selectedElement.element.content);
					setUrl(content.url);
				} else if (props.selectedElement.element.typeId === 2 || props.selectedElement.element.typeId === 5) {
					setUrl(props.selectedElement.element.content);
				}
			} catch (error) {
				console.error('Failed to parse locationAttributeJson:', error);
			}
		}
		if (element.element.typeId === 1) {
			const confirmed = window.confirm('Are you sure you want to cancel editing? Any unsaved text changes will be lost.');
			if (!confirmed) {
				return;
			}
		}
		props.onStyleCancel();
	}

	function navigateToElement() {
		const elementHeader = document.getElementById('nav-element-' + element?.elementId);
		if (elementHeader) {
			const selectedItems = document.getElementsByClassName('selected-page');
			Array.from(selectedItems).forEach(item => item.classList.remove('selected-page'));
			elementHeader.classList.add('selected-page');
		}

		navigate('/studio/element/' + element?.elementId);
	}

	return (
		<div className='style-properties collapsed' id='style-properties'>
			<div className='style-properties-header'>
				<h1 className='style-properties-header-text'>Element Style Properties</h1>
			</div>
			{element && (
				<>
					<div className='style-properties-selected-element-info'>
						{element.element.title && (
							<div className='style-properties-selected-element-header'>
								<h2>Element: {element.element.title}</h2>
								<button className='style-properties-view-element-button' onClick={navigateToElement}>
									View Element
								</button>
							</div>
						)}
						{getElementType && <h2>Type: {getElementType()}</h2>}
					</div>
					<div className='style-properties-selected-element-styles'>
						<h3 className='style-h3'>
							Width: <input className='style-input' id='style-input-width' type='number' min='0' max={1500} value={width} onChange={updateInputValues} /> px
						</h3>
						<h3 className='style-h3'>
							Height: <input className='style-input' id='style-input-height' type='number' min='0' max={1000} value={height} disabled={ratioLocked} onChange={updateInputValues} /> px
							<button className='ratio-lock-button' onClick={lockRatio}>
								<FontAwesomeIcon icon={ratioLocked ? faLink : faLinkSlash} />
							</button>
						</h3>
						<h3>
							Alignment: <select className='style-select' id='style-select-alignment' value={alignment} onChange={updateInputValues}>
								<option value='start'>Left</option>
								<option value='center'>Center</option>
								<option value='end'>Right</option>
							</select>
						</h3>
						{element.element.typeId === 7 && (
							<h3 className='style-h3'>
								Section level: <input className='style-input' id='style-input-level' type='number' min='0' max='4' value={sectionHeaderLevel} onChange={updateInputValues} />
							</h3>
						)}
						{(element.element.typeId === 2 || element.element.typeId === 5 || element.element.typeId === 4) && (
							<>
								<h3 className='style-h3'>
									Display Link: <select className='style-select' id='style-select-display-link' value={displayLink} onChange={updateInputValues}>
										<option value='true'>True</option>
										<option value='false'>False</option>
									</select>
								</h3>
								<h3 className='style-h3'>
									URL: <input className='style-input' id='style-input-url' type='text' value={url} onChange={updateInputValues} />
								</h3>
							</>
						)}
					</div>
					<div className='style-properites-footer'>
						<button id='style-close' onClick={saveChanges}>Save and Close</button>
						<button id='style-cancel' onClick={handleCancel}>Close</button>
					</div>
				</>
			)}
		</div>
	);
}
