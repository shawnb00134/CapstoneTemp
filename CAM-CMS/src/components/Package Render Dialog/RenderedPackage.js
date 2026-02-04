/* eslint-disable */
import React, { useEffect } from 'react';
import RenderedPackageContent from './RenderedPackageContent';
import './styles/RenderedPackage.css';

export default function RenderedPackage({id, packageData}) {

	const [currentPackageData, setPackageData] = React.useState(packageData);

	useEffect(() => {
		setPackageData(packageData);
	}, [packageData]);
	return (
		<dialog id={id} className='rendered-package-dialog'>
			<div className='rendered-package'>
				<header className='rendered-package-header'>
					<h1>{packageData?.name}</h1>
					<button className='close-button' onClick={() => {document.getElementById(id).close();}}>Close</button>
				</header>
				<div className='rendered-package-content'>
					<RenderedPackageContent packageData={currentPackageData}/>
				</div>
			</div>
			
		</dialog>
	);
}