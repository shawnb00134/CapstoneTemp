import React, { useState, useEffect } from 'react';
import Requests from '../../../utils/Requests';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import './styles/PublishPackageDialog.css';

export default function PublishPackageDialog({id,onLoadOrganizations,packageId}) {

	const [organization, setOrganization] = useState();
	const [existingOrgs, setExistingOrgs] = useState();
	const navigate = useNavigate();
	

	function closeDialog() {
		const dialog = document.getElementById(id);
		setOrganization([]);

		dialog.close();
	}
	useEffect(() => {
		if (onLoadOrganizations) {
			const organizations = document.getElementById('organization-selector');
			if (organizations) {
				const checkbox = organizations.querySelector('.published-check');
				checkbox.addEventListener('change', function() {
					if (checkbox.checked) {
						organizations.classList.add('selected');
					} else {
						organizations.classList.remove('selected');
					}
				});
			}
			else {
				return;
			}
		}
		selectExistingOrgs();
	}, );
	
	useEffect(() => {
		if (onLoadOrganizations) {
			Requests.getAllOrganizations().then(data => {
				if (data === null) {
					return;
				}
				setOrganization(data);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});

			Requests.getOrganizationByPackageId(packageId).then(data => {
				if (data === null) {
					return;
				}
				setExistingOrgs(data);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
		}
	}, [onLoadOrganizations]);

	function selectExistingOrgs() {
		const organizations =  document.getElementsByClassName('org-selection');
		if(organizations){
			for (let i = 0; i < organizations.length; i++) {
				const checkbox = organizations[i].querySelector('.published-check');
				
				if (existingOrgs?.includes(parseInt(checkbox.value))) {

					checkbox.checked = true;
					organizations[i].classList.add('selected');
				}
			}
		}

	}
	function handleOrgSelect(id) {
		const organization = document.getElementById(id);
		
		if (organization) {
			const checkbox = organization.querySelector('.published-check');
			checkbox.checked = !checkbox.checked;
			organization.classList.toggle('selected');
		}
	}
	function handleUpload() {
		const selectedOrgs = document.querySelectorAll('.org-selection.selected');
		if(selectedOrgs !== undefined && selectedOrgs !== null){
			let orgIds = [];
			selectedOrgs.forEach(org => {
				const checkbox = org.querySelector('.published-check');
				orgIds.push(parseInt(checkbox.value));
			});
			let data = {
				packageId: packageId,
				organizationIds: orgIds
			};
			
			Requests.updatePackageAssoicatedOrganizations(data).then(() => {
				closeDialog();
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
		
		}
	}

	function organizationMap() {
		try {
			return organization?.map((org, index) => {
				return (
					<div key={index} className='org-container'>
						<div id= {`org-${org.organizationId}`} onClick={() => handleOrgSelect(`org-${org.organizationId}`)} className='org-selection'>
							<p>{org.name}</p>
							<input className='published-check' type='checkbox' value={org.organizationId} name={org.name}/>
						</div>
					</div>
				);
			});
		}
		catch (error) {
			return (
				<div>
					<p>No organizations found</p>
				</div>
			);
		}
	}

	return (
		<dialog className='publish-package-dialog' id={id}>
			<div className='publish-dialog-content'>
				<div className='publish-header'>
					<h2>Publish Package</h2>
					<button onClick={closeDialog}><FontAwesomeIcon icon={faXmark} /></button>
				</div>
				<div className='publish-body'>
					<div>
						<input className='organization-search' placeholder='search organization'></input>
					</div>
					<div className='organizations'> 
						{organizationMap()}
					</div>
				</div>
				<div className='publish-footer'>
					<button className='upload-button' onClick={handleUpload}>Save</button>
					<button className='clear-button' onClick={closeDialog}>Cancel</button>
				</div>
			</div>
			
		</dialog>
	);
}