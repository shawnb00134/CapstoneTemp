import React, { useState, useEffect } from 'react';
import './styles/StylePropertiesAside.css';

export default function ElementSetStyleAside(props) {
	const [elementSet, setElementSet] = useState(undefined);
	const [isAppendix, setIsAppendix] = useState(undefined);
	const [isHorizontal, setIsHorizontal] = useState(undefined);
	const [hasPageBreak, setHasPageBreak] = useState(undefined);
	const [isCollapsible, setIsCollapsible] = useState(undefined);

	useEffect(() => {
		setIsAppendix(undefined);
		setIsHorizontal(undefined);
		setHasPageBreak(undefined);
		setIsCollapsible(undefined);
		setElementSet(props.selectedElementSet);
		if (props.selectedElementSet !== undefined) {
			setIsAppendix(props.selectedElementSet.styling.is_appendix ? 'true' : 'false');
			setIsHorizontal(props.selectedElementSet.styling.is_horizontal ? 'true' : 'false');
			setHasPageBreak(props.selectedElementSet.styling.has_page_break);
			setIsCollapsible(props.selectedElementSet.styling.is_collapsible ? 'true' : 'false');
		}
	}, [props.selectedElementSet]);

	function getNewValues() {
		elementSet.styling.is_appendix = (isAppendix === 'true');
		elementSet.styling.is_horizontal = (isHorizontal === 'true');
		elementSet.styling.has_page_break = hasPageBreak;
		elementSet.styling.is_collapsible = (isCollapsible === 'true');
		elementSet.stylingJson = JSON.stringify({
			is_appendix: (isAppendix === 'true'),
			is_horizontal: (isHorizontal === 'true'),
			has_page_break: hasPageBreak,
			is_collapsible: (isCollapsible === 'true')
		});
	}

	function updateInputValues() {
		const newIsAppendix = document.getElementById('style-select-appendix').value;
		const newIsHorizontal = document.getElementById('style-select-orientation').value;
		const newHasPageBreak = document.getElementById('style-select-page-break').value;
		const newIsCollapsible = document.getElementById('style-select-collapsible').value;
		setIsAppendix(newIsAppendix);
		setIsHorizontal(newIsHorizontal);
		setHasPageBreak(newHasPageBreak);
		setIsCollapsible(newIsCollapsible);
	}

	function saveChanges() {
		getNewValues();
		props.onStyleUpdate(elementSet);
	}

	function handleCancel() {
		if (props.selectedElementSet !== undefined) {
			setIsAppendix(props.selectedElementSet.styling.is_appendix ? 'true' : 'false');
			setIsHorizontal(props.selectedElementSet.styling.is_horizontal ? 'true' : 'false');
			setHasPageBreak(props.selectedElementSet.styling.has_page_break);
			setIsCollapsible(props.selectedElementSet.styling.is_collapsible ? 'true' : 'false');
		}
		props.onStyleCancel();
	}

	return (
		<div className='style-properties collapsed' id='style-properties'>
			<div className='style-properties-header'>
				<h1 className='style-properties-header-text'>Element Set Properties</h1>
			</div>
			{elementSet && (
				<div>
					<h3>Appendix : <select className='style-select' id='style-select-appendix' value={isAppendix} onChange={updateInputValues}>
						<option value='true'>True</option>
						<option value='false'>False</option>
					</select></h3>
					<h3>Horizontal orientation: <select className='style-select' id='style-select-orientation' value={isHorizontal} onChange={updateInputValues}>
						<option value='true'>True</option>
						<option value='false'>False</option>
					</select></h3>
					<h3>Page Break: <select className='style-select' id='style-select-page-break' value={hasPageBreak} onChange={updateInputValues}>
						<option value='before'>Before</option>
						<option value='after'>After</option>
						<option value='false'>False</option>
					</select></h3>
					<h3>Collapsible: <select className='style-select' id='style-select-collapsible' value={isCollapsible} onChange={updateInputValues}> {/* New select */}
						<option value='true'>True</option>
						<option value='false'>False</option>
					</select></h3>
					<div>
						<button id='style-close' onClick={saveChanges}>Save and Close</button>
						<button id='style-cancel' onClick={handleCancel}>Close</button>
					</div>
				</div>
			)}
		</div>
	);
}