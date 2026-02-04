import React, { useState, useEffect } from 'react';
import RenderedModuleElements from './RenderedModuleElements';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowRightLong, faArrowLeftLong } from '@fortawesome/free-solid-svg-icons';
import RenderedModuleAppendix from './RenderedModuleAppendix';
import AnchorTable from './AnchorTable';

export default function RenderedModuleContent({ moduleJson, title, selectedTab }) {
	const [selectedPage, setSelectedPage] = useState(0);
	const [hasNextPage, setHasNextPage] = useState(true);
	const [hasPreviousPage, setHasPreviousPage] = useState(false);
	const [selectedAnchor, setSelectedAnchor] = useState();

	/**
	 * Checks for the next page and previous page.
	 */
	useEffect(() => {
		checkForNextPage();
		checkForPreviousPage();
	}, [moduleJson, selectedPage]);

	/**
	 *  Handles the previous page button.
	 */
	function handlePrevious() {
		if (selectedPage > 0) {
			setSelectedPage(selectedPage - 1);
			returnToTop();
		}
	}

	/**
	 *  Handles the next page button.
	 */
	function handleNext() {
		if (selectedPage < moduleJson?.pages.length - 1) {
			setSelectedPage(selectedPage + 1);
			returnToTop();
		}
	}

	/**
	 *  Checks if there is a next page.
	 */
	function checkForNextPage() {
		if (selectedPage < moduleJson?.pages.length - 1) {
			setHasNextPage(true);
		} else {
			setHasNextPage(false);
		}
	}

	/**
	 * Checks if there is a previous page.
	 */
	function checkForPreviousPage() {
		if (selectedPage > 0) {
			setHasPreviousPage(true);
		} else {
			setHasPreviousPage(false);
		}
	}

	/**
	 *  Handles the table of contents click.
	 * 
	 * @param {JSON} anchor JSON of the anchor to be searched for.
	 */
	function handleTableClick(anchor) {
		setSelectedPage(anchor.page);
		setSelectedAnchor(anchor);
	}

	function returnToTop() {
		var dialog = document.getElementById('rendered-module-dialog');
		dialog.scrollTo(0, 0);
	}

	return (
		<>
			<div className='rendered-content'>
				{selectedTab === 0 ? (
					<div>
						<div className='render-module-sets'>
							<div className='rendered-module-display-title'>
								<h1 className='rendered-module-display-title'>{title}</h1>
							</div>
							{moduleJson?.pages[selectedPage].map((element, elementIndex) => (
								!element?.styling?.is_appendix && selectedTab === 0 ? (
									<div className={`rendered-module-elements ${element?.styling?.is_horizontal ? 'horizontal' : 'vertical'}`}
										key={elementIndex}>
										<RenderedModuleElements
											onHandleAnchorSearch={selectedAnchor}
											elementSet={element}
											anchorTable={moduleJson?.anchors}
										/>
									</div>
								) : undefined
							))}
						</div>

						<div className='row' id='page-handling-buttons'>
							<button id='previous-button' onClick={handlePrevious} style={{ visibility: hasPreviousPage ? 'visible' : 'hidden' }}>
								<FontAwesomeIcon icon={faArrowLeftLong} />
							</button>
							<div className='page-counter'>{selectedPage + 1} | {moduleJson?.pages.length}</div>
							<button id='next-button' onClick={handleNext} style={{ visibility: hasNextPage ? 'visible' : 'hidden' }}>
								<FontAwesomeIcon icon={faArrowRightLong} />
							</button>
						</div>
					</div>
				) : selectedTab === 1 ? (
					<div>
						{moduleJson?.elementSets.map((elementSet, index) => {
							return elementSet?.styling?.is_appendix ? (
								<div key={index}>
									<RenderedModuleAppendix elementSet={elementSet} />
								</div>
							) : null;
						})}
					</div>
				) : null}
			</div>
			<div className='table-contents'>
				<h3>Table of Contents:</h3>
				{moduleJson?.anchors ? (
					moduleJson?.anchors?.map((anchor, index) => {
						return (
							<div key={index} className='major-anchor'>
								<div className='anchor-title'>
									<p className='table-item' style={{ paddingLeft: `${anchor.level + 0.5}em` }} onClick={() => handleTableClick(anchor)}>
										{index + 1}. {anchor?.content}
									</p>
									<p>pg.{anchor.page + 1}</p>
								</div>
								{anchor.headerChildren.length > 0 ? (
									<AnchorTable key={index} anchors={anchor.headerChildren} handleTableClick={handleTableClick} runs={index + 1} />
								) : undefined}
							</div>
						);
					})
				) : undefined}
			</div>
		</>
	);
}
