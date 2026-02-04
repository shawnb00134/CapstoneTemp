import React, {useEffect, useState} from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBook } from '@fortawesome/free-solid-svg-icons';
import './styles/RenderedPackage.css';

export default function RenderedPackageModule({ module}) {
	const [moduleCache, setModuleCache] = useState(module.cache);
	useEffect(() => {
		setModuleCache(JSON.parse(module.cache));
	}, [module]);
	return (
		<div className='rendered-package-module'>
			<a>{<FontAwesomeIcon icon={faBook}/>}{moduleCache.title}</a>
		</div>
	);
}