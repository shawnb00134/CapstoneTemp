import React, { useState, useEffect } from 'react';
import Requests from '../../../utils/Requests';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import './styles/AddPackageToOrganizationDialog.css';
import { useNavigate } from 'react-router-dom';

export default function AddPackageOrganizationDialog({ id, onLoadPackages, organizationId, onUploadSuccess }) {
	const [packageName, setPackage] = useState();
	const [existingPackages, setExistingPackages] = useState();
	const navigate = useNavigate();

	function closeDialog() {
		const dialog = document.getElementById(id);
		dialog.close();
	}

	useEffect(() => {
		if (onLoadPackages) {
			const packages = document.getElementById('package-selector');
			if (packages) {
				const checkbox = packages.querySelector('.published-check');
				checkbox.addEventListener('change', function () {
					if (checkbox.checked) {
						packages.classList.add('selected');
					} else {
						packages.classList.remove('selected');
					}
				});
			}
		}
		selectExistingPackages();
	},);

	useEffect(() => {
		if (onLoadPackages) {
			Requests.getAllPackages().then(data => {
				setPackage(data);
			}).catch(error => {
				console.error(error);
				navigate('/error');
			});
			Requests.getAllPackagesByOrganizationId(organizationId).then(data => {
				const packageIds = data.map(item => item.packageId);
				setExistingPackages(packageIds);
			}).catch(error => {
				console.error(error);
				navigate('/error');
			});
		}
	}, [onLoadPackages]);

	function selectExistingPackages() {
		const packages = document.getElementsByClassName('pack-selection');
		if (packages) {
			for (let i = 0; i < packages.length; i++) {
				const checkbox = packages[i].querySelector('.published-check');
				

				if (existingPackages?.includes(parseInt(checkbox.value))) {
					
					checkbox.checked = true;
					packages[i].classList.add('selected');
				}
			}
		}
	}

	function handlePackageSelect(id) {
		const packages = document.getElementById(id);
		if (packages) {
			const checkbox = packages.querySelector('.published-check');
			checkbox.checked = !checkbox.checked;
			packages.classList.toggle('selected');
		}
	}

	function handleUpload() {
		const selectedPackages = document.querySelectorAll('.pack-selection.selected');
		if (selectedPackages !== undefined && selectedPackages !== null) {
			let packIds = [];
			selectedPackages.forEach(pack => {
				const checkbox = pack.querySelector('.published-check');
				packIds.push(parseInt(checkbox.value));
			});
			let data = {
				organizationId: organizationId,
				packageIds: packIds				
			};
			
			Requests.updateOrganizationAssociatedPackages(data).then(() => {
				closeDialog();
				onUploadSuccess(); 
			}).catch(error => {
				console.error(error);
				navigate('/error');
			});
		}
	}

	return (
		<dialog className='add-package-dialog' id={id}>
			<div className='add-package-content'>
				<div className='add-header'>
					<h2>Add Package</h2>
					<button onClick={closeDialog}><FontAwesomeIcon icon={faXmark} /></button>
				</div>
				<div className='add-body'>
					<div>
						<input className='package-search' placeholder='search packages'></input>
					</div>
					<div className='packages'>
						{packageName?.map((pack, index) => {
							return (
								<div key={index} className='package-container'>
									<div id={`pack-${pack.packageId}`} onClick={() => handlePackageSelect(`pack-${pack.packageId}`)} className='pack-selection'>
										<p>{pack.name}</p>
										<input className='published-check' type='checkbox' value={pack.packageId} name={pack.name} />
									</div>
								</div>
							);
						})}
					</div>
				</div>
				<div className='add-footer'>
					<button className='upload-button' onClick={handleUpload}>Save</button>
					<button className='clear-button' onClick={closeDialog}>Cancel</button>
				</div>
			</div>
		</dialog>
	);
}