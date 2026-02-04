import React from 'react';
import RenderedPackageModule from './RenderedPackageModule';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFolderOpen } from '@fortawesome/free-solid-svg-icons';
import './styles/RenderedPackage.css';
export default function RenderedPackageFolder({folder,level}) {
	function buildChildFolders() {
		return folder?.packageFolders?.map((folder, index) => {
			return (
				
				<RenderedPackageFolder key={index} folder={folder} level={level + 1} />
				
			);
		});
	}

	function buildModules() {
		return folder?.packageFoldersModule?.map((module, index) => {
			return (
			
				<RenderedPackageModule key={index} module={module} level={level + 1} />
				
			);
		});
	
	}

	return (
		<div id={`rendered-package-folder-items-${folder?.packageFolderId}`} className='rendered-package-folder' style={{ marginLeft: level + 30 }}>
		  <p className='rendered-folder-item'>{<FontAwesomeIcon icon={faFolderOpen}/>} {folder?.displayName}</p>
		  {folder?.packageFolders?.length > 0 ? 
				buildChildFolders() 
				: null }
		  
		  {
		  	buildModules()
		  }
		  <div className='render-seperator' />
		</div>
	  );
	  
}