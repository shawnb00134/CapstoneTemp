import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBox, faBoxOpen } from '@fortawesome/free-solid-svg-icons';

import './styles/PackageButton.css';

/**
 * The package button component for the studio navigation.
 * It defines how to display an package in the studio navigation aside.
 * 
 * 	@version 1.0
 * 	@author Steven Kight
 *  @param {Object} packageItem - The package object to render the button for. (must contain id, name)
 *  @returns {JSX.Element} - The JSX element for the button.
 */
export default function PackageButton({ packageItem }) {
	const navigate = useNavigate();

	const [isSelected, setSelected] = React.useState(false);

	/**
	 * Adds a mutation observer to the package header to watch for the selected-page class.
	 * If the class is removed, the package is no longer selected and the selected state is set to false.
	 * If the class is added, the package is selected and the selected state is set to true.
	 * 
	 * This is to show the different icons for the package button when it is selected or not.
	 */
	useEffect(() => {
		// Add listener to package header for when 'selected-page' class is removed
		let classWatcher = new MutationObserver(function(mutations) {
			for (let mutation of mutations) {
				if (mutation.attributeName === 'class') {
					if (!mutation.target.classList.contains('selected-page')) {
						setSelected(false);
					}
					else {
						setSelected(true);
					}
				}
			}
		});

		let packageHeader = document.getElementById('nav-package-' + packageItem.packageId);
		classWatcher.observe(packageHeader, {attributes: true});
	}, []);

	/**
	 * Selects the package and navigates to the package page.
	 * Also adds the selected-page class to the package header.
	 * 
	 * @returns {void}
	 * @fires navigate
	 * @post The package page is navigated to and the package header is the only dom object with the selected-page class.
	 */
	function selectPackage() {
		navigate('package/' + packageItem.packageId);

		// Remove selected class from all items
		let selectedItems = document.getElementsByClassName('selected-page');
		for (let i = 0; i < selectedItems.length; i++) {
			if (!selectedItems[i].classList.contains('selected-page')) continue;
			selectedItems[i].classList.remove('selected-page');
		}

		// Add selected class to current item
		let packageHeader = document.getElementById('nav-package-' + packageItem.packageId);
		packageHeader.classList.add('selected-page');

		setSelected(true);
	}

	return (
		<header id={'nav-package-' + packageItem.packageId} className='studio-nav-package' onClick={selectPackage}>
			<FontAwesomeIcon icon={isSelected ? faBoxOpen : faBox} className='studio-nav-page-icon' />
			<h4>{packageItem.name}</h4>
		</header>
	);
}