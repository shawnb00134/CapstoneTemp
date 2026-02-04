import React, { useState, useRef } from 'react';
import ReactPlayer from 'react-player';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCaretUp, faCaretDown } from '@fortawesome/free-solid-svg-icons';
import './styles/RenderedModule.css';

/**
 *  Renders the module appendix based on the moduleJson and gives the user a vision of what the module will look like.
 * 
 * @param {props} props the props of the rendered module appendix 
 * 
 * @returns {RenderedModuleAppendix} The rendered module appendix.
 */
export default function RenderedModuleAppendix(props) {
	const [isExpanded, setIsExpanded] = useState(false);
	const elementSetRef = useRef(null);

	const handleExpandClick = () => {
		setIsExpanded(!isExpanded);
	};

	const isCollapsible = props.elementSet?.styling?.is_collapsible;

	if (isCollapsible && !isExpanded) {
		const firstElement = props.elementSet?.locations?.find((loc) => loc.element.typeId !== -1);

		return (
			<div ref={elementSetRef} className={`rendered-module-appendix-preview ${props.elementSet?.styling?.is_horizontal ? 'horizontal' : 'vertical'}`}>
				<button className="caret-button" onClick={handleExpandClick}><FontAwesomeIcon icon={faCaretDown} /></button>
				{firstElement && (
					<div className='rendered-element-preview non-anchor'>
						{firstElement.element.typeId === 1 ? (
							<p className='rendered-content' dangerouslySetInnerHTML={{ __html: JSON.parse(firstElement.element.content).content }} />
						) : firstElement.element.typeId === 2 ? (
							<>
								<img
									className='appendix'
									src={firstElement.element.content}
									style={{ width: firstElement.styling?.width || 'auto', height: firstElement.styling?.height || 'auto' }}
								/>
								<p id='image-desc'>{firstElement.element.description}</p>
							</>
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
						) : undefined}
					</div>
				)}
			</div>
		);
	}

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

	return (
		<div ref={elementSetRef} className={`rendered-module-appendix ${props.elementSet?.styling?.is_horizontal ? 'horizontal' : 'vertical'}`}>
			{isCollapsible && isExpanded && (
				<button className="caret-button" onClick={() => setIsExpanded(false)}><FontAwesomeIcon icon={faCaretUp} /></button>
			)}
			{props.elementSet?.locations.map((location, index) => {
				const { width, height, is_horizontal } = location?.styling || {};

				const linkContent = (
					<div className='pdf-link'>
						<a href={location.element.content} rel='noreferrer' target='_blank'>{location.element.content}</a>
					</div>
				);

				return (
					<div key={index}>
						<div className={`rendered-element ${is_horizontal ? 'horizontal' : 'vertical'}`} key={index}>
							{location.element.typeId === 1 ? (
								<p className='rendered-content' dangerouslySetInnerHTML={{ __html: JSON.parse(location.element.content).content }} />
							) : undefined}
							{location.element.typeId === 2 ? (
								<>
									<img
										className='appendix'
										src={location.element.content}
										style={{ width: width || 'auto', height: height || 'auto' }}
									/>
									<p id='image-desc'>{location.element.description}</p>
								</>
							) : undefined}
							{location.element.typeId === 4 ? (
								<div>
									{location.locationAttribute.displayLink ? (
										<div className='pdf-link'>
											<a href={getVideoContent(location).url} rel='noreferrer' target='_blank'>{getVideoContent(location).url}</a>
										</div>
									) : (
										<ReactPlayer width={width} url={getVideoContent(location).url} className='video-element' controls={true} light={getVideoContent(location).thumbnail ? getVideoContent(location).thumbnail : true}>
											<p className='video-element'>Unable to display video file.</p>
										</ReactPlayer>
									)}
								</div>
							) : undefined}
							{location.element.typeId === 5 ? (
								linkContent
							) : undefined}
							<div className='row'>
								{location.element.citation !== null ? <small className='rendered-citation'>{location.element.citation}</small> : undefined}
							</div>
						</div>
					</div>
				);
			})}
		</div>
	);
}
