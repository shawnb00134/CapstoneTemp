import React, { useState, useEffect, useRef } from 'react';
import ReactPlayer from 'react-player';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCaretUp, faCaretDown } from '@fortawesome/free-solid-svg-icons';
import './styles/RenderedModule.css';
//import AnchorTable from './AnchorTable';

/**
 * Renders the content of the module based on the moduleJson and gives the user a vision of what the module will look like.
 *
 * @param {props} props
 *
 * @returns {RenderedModuleElements} The rendered module.
 */
export default function RenderedModuleElements(props) {
	const [isExpanded, setIsExpanded] = useState(false);
	const elementSetRef = useRef(null);

	/**
	 * Converts a string to an int if possible, otherwise returns undefined
	 * @param {string} string The string to convert to int
	 * @returns The converted string to int
	 */
	function handleStringToIntConvert(string) {
		try {
			return parseInt(string);
		} catch {
			return undefined;
		}
	}

	/**
	 * Handles the alignment of the element based on the alignment string.
	 *
	 * @param {string} string
	 * @returns
	 */
	function handleAlignmentChange(string) {
		switch (string) {
		case 'start':
			return 'flex-start';
		case 'center':
			return 'center';
		case 'end':
			return 'flex-end';
		default:
			return 'center';
		}
	}

	/**
	 * Handles the anchor search and scrolls to the anchor.
	 */
	function handleAnchorSearch() {
		if (props?.onHandleAnchorSearch !== undefined || props?.onHandleAnchorSearch !== null) {
			let anchorId = props?.onHandleAnchorSearch?.anchorId;
			let anchor = document.querySelector(`[data-anchor-id="${anchorId}"]`);

			if (!anchor && props.anchorTable) {
				let targetIndex = props.anchorTable.findIndex(anchor => anchor.anchorId === anchorId);
				for (let i = targetIndex - 1; i >= 0; i--) {
					const previousAnchorId = props.anchorTable[i].anchorId;
					anchor = document.querySelector(`[data-anchor-id="${previousAnchorId}"]`);
					if (anchor) {
						anchor.scrollIntoView({ behavior: 'smooth', block: 'start' });
						break;
					}
				}
			}

			if (!anchor) {
				elementSetRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
			} else {
				anchor.scrollIntoView({ behavior: 'smooth', block: 'start' });
			}
		}
	}



	/**
	 * Handles the anchor search on page load.
	 */
	useEffect(() => {
		handleAnchorSearch();
	}, [props.onHandleAnchorSearch]);

	/**
	 * Get video content of the element
	 *
	 * @returns A string of the video content of the element
	 */
	function getVideoContent(location) {
		if (location.element.content === null || location.element.content === undefined) {
			return 'Processing video...';
		}
		return JSON.parse(location.element.content);
	}

	const handleExpandClick = () => {
		setIsExpanded(!isExpanded);
	};

	const isCollapsible = props.elementSet?.styling?.is_collapsible;

	if (isCollapsible && !isExpanded) {
		const firstElement = props.elementSet?.locations?.find((loc) => loc.element.typeId !== -1);

		return (
			<div ref={elementSetRef} className={`rendered-module-elements-preview ${props.elementSet?.styling?.is_horizontal ? 'horizontal' : 'vertical'}`}>
				<button className="caret-button" onClick={handleExpandClick}><FontAwesomeIcon icon={faCaretDown} /></button>
				{firstElement && (
					<div className='rendered-element-preview non-anchor'>
						{firstElement.element.typeId === 1 ? (
							<p className='rendered-content' dangerouslySetInnerHTML={{ __html: JSON.parse(firstElement.element.content).content }} />
						) : firstElement.element.typeId === 2 ? (
							firstElement.locationAttribute.displayLink ? (
								<div className='pdf-link'>
									<a href={firstElement.element.content} rel='noreferrer' target='_blank'>{firstElement.element.content}</a>
								</div>
							) : (
								<img src={firstElement.element.content} />
							)
						) : firstElement.element.typeId === 4 ? (
							<div>
								{firstElement.locationAttribute.displayLink ? (
									<div className='pdf-link'>
										<a href={getVideoContent(firstElement).url} rel='noreferrer' target='_blank'>{getVideoContent(firstElement).url}</a>
									</div>
								) : (
									<ReactPlayer width='100%' url={getVideoContent(firstElement).url} className='video-element' controls={true} light={getVideoContent(firstElement).thumbnail ? getVideoContent(firstElement).thumbnail : true}>
										<p className='video-element'>Unable to display video file.</p>
									</ReactPlayer>
								)}
							</div>
						) : firstElement.element.typeId === 5 ? (
							<div className='pdf-link'>
								<a href={firstElement.element.content} rel='noreferrer' target='_blank'>{firstElement.element.content}</a>
							</div>
						) : firstElement.element.typeId === 7 ? (
							<h1 className='rendered-content' data-set-id={props.elementSet.id} data-anchor-id={firstElement.element.elementId} style={{ fontSize: firstElement?.locationAttribute?.HeadingLevel > 0 ? 30 : 45 }}>
								{firstElement.element.title}
							</h1>
						) : undefined}
					</div>
				)}
			</div>
		);
	}

	return (
		<div ref={elementSetRef} className={`rendered-module-elements ${props.elementSet?.styling?.is_horizontal ? 'horizontal' : 'vertical'}`}>
			{isCollapsible && isExpanded && (
				<button className="caret-button" onClick={() => setIsExpanded(false)}><FontAwesomeIcon icon={faCaretUp}></FontAwesomeIcon></button>
			)}
			{props.elementSet?.locations.map((location, index) => {
				const elementLocationStylingSize = {
					width: location?.locationAttribute?.width ? handleStringToIntConvert(location?.locationAttribute?.width) : 'auto',
					height: location?.locationAttribute?.height ? handleStringToIntConvert(location?.locationAttribute?.height) : 'auto',
				};
				const elementLocationStylingAlignment = {
					justifyContent: handleAlignmentChange(location?.locationAttribute.alignment)
				};

				/**
				 * Get video content of the element
				 *
				 * @returns A string of the video content of the element
				 */
				function getVideoContent(loc) {
					if (loc.element.content === null || loc.element.content === undefined) {
						return 'Processing video...';
					}
					return JSON.parse(loc.element.content);
				}

				const linkContent = (
					<div className='pdf-link'>
						<a href={location.element.content} rel='noreferrer' target='_blank'>{location.element.content}</a>
					</div>
				);

				return (
					<div key={index}>
						<div className='rendered-element' id={location.element.elementId} style={{ justifyContent: elementLocationStylingAlignment.justifyContent }}>
							{location.element.typeId === 1 ? (
								<p className='rendered-content' style={elementLocationStylingSize} dangerouslySetInnerHTML={{ __html: JSON.parse(location.element.content).content }} />
							) : undefined}
							{location.element.typeId === 2 ? (
								location.locationAttribute.displayLink ? (
									linkContent
								) : (
									<img style={elementLocationStylingSize} src={location.element.content} />
								)
							) : undefined}
							{location.element.typeId === 4 ? (
								<div>
									{location.locationAttribute.displayLink ? (
										<div className='pdf-link'>
											<a href={getVideoContent(location).url} rel='noreferrer' target='_blank'>{getVideoContent(location).url}</a>
										</div>
									) : (
										<ReactPlayer width={elementLocationStylingSize.width} url={getVideoContent(location).url} className='video-element' controls={true} light={getVideoContent(location).thumbnail ? getVideoContent(location).thumbnail : true}>
											<p className='video-element'>Unable to display video file.</p>
										</ReactPlayer>
									)}
								</div>
							) : undefined}
							{location.element.typeId === 5 ? (
								linkContent
							) : undefined}
							{location.element.typeId === 7 ? (
								<h1
									className='rendered-content'
									data-set-id={props.elementSet.id}
									data-anchor-id={location.element.elementId}
									style={{ ...elementLocationStylingSize, fontSize: location.locationAttribute.HeadingLevel > 0 ? 30 : 45 }}
								>
									{location.element.title}
								</h1>
							) : undefined}
						</div>
						<div className='row'>
							{location.element.citation !== null ? <small className='rendered-citation'>{location.element.citation}</small> : undefined}
						</div>
					</div>
				);
			})}
		</div>
	);
}
