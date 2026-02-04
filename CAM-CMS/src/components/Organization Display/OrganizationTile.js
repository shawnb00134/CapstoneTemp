import React from 'react';
import './styles/OrganizationTile.css';


export default function OrganizationTile({organization}) {

	return (
		<div className='tile-card'>
			<div className='organization-tile'>
				<div className='organization-thumbnail'>
					<img src='https://via.placeholder.com/150' alt='organization thumbnail' />
					
				</div>
				<div className='organization-title'>
					<h2>{organization.name}</h2>
				</div>
			</div>
		</div>
		
	);
}