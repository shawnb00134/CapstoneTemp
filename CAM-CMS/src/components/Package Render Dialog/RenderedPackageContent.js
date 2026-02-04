import React from 'react';
import RenderedPackageFolder from './RenderedPackageFolder';
import './styles/RenderedPackage.css';

export default function RenedredPackageContent({packageData}) {
	return (
		<div className='render-package-content'>
			{packageData?.packageFolders?.map((folder, index) => {
				return (
					<RenderedPackageFolder key={index} folder={folder} level={0} />
				);
			})}
		</div>
	);
}