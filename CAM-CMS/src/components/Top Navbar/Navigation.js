import React, { useEffect } from 'react';

import { useNavigate } from 'react-router-dom';

import './styles/Navigation.css';
import Requests from '../../utils/Requests';

export default function Navigation() {

	useEffect(() => {
		loadTokenTimer();
	}, []);

	const navigate = useNavigate();

	const userData = JSON.parse(window.sessionStorage.getItem('userData'));

	const brandClick = () => {
		navigate('/dashboard');
	};

	const getActive = () => {
		if (window.location.pathname !== '/dashboard') {

			var splitPath = window.location.pathname.split('/');
			splitPath.shift();
            
			var capitalizedSplit = [];
			splitPath.forEach(element => {
				capitalizedSplit.push(element.charAt(0).toUpperCase() + element.slice(1));
			});
			var pageName = capitalizedSplit.join(' > ');

			return (
				<div className='active'>
					<div className='nav-separator'></div>
					<button className='page-button'>{ pageName }</button>
				</div>
			);
		}
	};

	const logoutClick = () => {
		window.sessionStorage.removeItem('userData');
		window.localStorage.removeItem('previousPath');
		navigate('/');
	};

	const loadTokenTimer = () => {
		setupTokenRefresh(false);
	};

	/**
	 * This function sets up a token refresh interval.
	 * 
	 * @param {boolean} hasRefreshed A boolean that is used to determine if the token has been refreshed.
	 * @returns {setTimeout} Used to ensure that the token is refreshed every 30 minutes.
	 */
	function setupTokenRefresh(hasRefreshed) { 
		let tokenRefresh;
		let lastExecuted = window.sessionStorage.getItem('lastExecuted');
		if (!hasRefreshed) {
			if (!lastExecuted) {
				window.localStorage.setItem('lastExecuted', new Date().getTime());
				tokenRefresh = setTimeout(async () => {
					let tokenExpired = setupTokenRefresh(true);
					await promptUserForRefresh(tokenExpired);
						
				}, 1500000);
			}
			else {
				let currentTime = new Date().getTime();
				let timeDifference = currentTime - lastExecuted;
				if (timeDifference > 1500000) {
					tokenRefresh = setTimeout(async () => {
						let tokenExpired = setupTokenRefresh(true);
						await promptUserForRefresh(tokenExpired);
					}, 1500000);
				}
				else {
					tokenRefresh = setTimeout(async () => {
						let tokenExpired = setupTokenRefresh(true);
						await promptUserForRefresh(tokenExpired);
					}, 1500000 - timeDifference);
				}
				window.sessionStorage.setItem('lastExecuted', new Date().getTime());
			}
		}
		else {
			tokenRefresh = setTimeout(() => {
				window.sessionStorage.removeItem('userData');
				window.sessionStorage.removeItem('userReadPrivileges');
				window.location.href = '/';
			}, 1500000);
		} 
		return tokenRefresh;
	}

	async function promptUserForRefresh(tokenExpired) {
		let response = prompt('You have been logged in for 25 minutes, would you like to remain logged in? (yes/no)');
		try {
			if (response === 'yes') {
				let refreshedToken = await Requests.refreshToken(window.sessionStorage.getItem('userData').refreshToken).catch(error => {
					console.error(error);
					window.sessionStorage.removeItem('userData');
					window.location.href = '/';
				});
				let userData = JSON.parse(window.sessionStorage.getItem('userData'));
				userData.accessToken = refreshedToken.accessToken;
				window.sessionStorage.setItem('userData', JSON.stringify(userData));
				clearTimeout(tokenExpired);
				window.sessionStorage.setItem('lastExecuted', new Date().getTime());
				setupTokenRefresh(false);
			}
			else {
				window.sessionStorage.removeItem('userData');
				window.sessionStorage.removeItem('userReadPrivileges');
				window.location.href = '/';
			}
		}
		catch (error) {
			window.sessionStorage.removeItem('userData');
			window.sessionStorage.removeItem('userReadPrivileges');
			window.location.href = '/';
		}
	}

	return (
		<div id='navigation'>
			<div className='navbar-left'>
				<img id='nav-logo' alt='UWG Logo' onClick={brandClick}/>
				{ getActive() }
			</div>

			<div className='navbar-right'>   
				<button className='logout' onClick={logoutClick}>Logout</button>
				<button className='nav-button'>{ userData.username }</button>
			</div>
		</div>
	);
}