/* eslint-disable no-unused-vars */
import React, { useState, useEffect } from 'react';

import './styles/LoginPage.css';

import { useNavigate } from 'react-router-dom';
import Requests from '../utils/Requests';

/**
 * The page that is displayed after the user has been authorized.
 * 
 * @returns {JSX.Element}
 */
export default function Login() {
	const navigate = useNavigate();

	const [loginData, setLoginData] = useState({ userData: '', isAuth: false });
	const [redirect, setRedirect] = useState(window.location.href.replace(window.location.search, ''));
	const [username, setUsername] = useState('');
	const [password, setPassword] = useState('');

	/**
	 * This effect is run when the page is loaded.
	 * 
	 * It removes user data from the session storage, and then checks if the user has been redirected from Cognito Hosted UI.
	 * If the user has been redirected from Cognito, it fetches the tokens from Cognito and then fetches the user data from the API.
	 * If the user has not been redirected from Cognito, it formats the redirect URL for Cognito Hosted UI.
	 */
	useEffect(() => {
		window.sessionStorage.removeItem('userData');
		window.sessionStorage.removeItem('userReadPrivileges');

		const code = new URLSearchParams(window.location.search).get('code');

		if (code) {
			fetchTokens(code);
		}
		else {
			var formattedRedirect = window.location.href;

			formattedRedirect = formattedRedirect.replaceAll('/', '%2F');
			formattedRedirect = formattedRedirect.replaceAll(':', '%3A');
			const url = '';

		}
	}, []);

	/**
	 * This effect is run when the user data is fetched from the API.
	 * 
	 * It stores the user data in the session storage and then redirects the user to the dashboard if they are authorized.
	 */
	useEffect(() => {
		if (loginData.isAuth) {
			window.sessionStorage.setItem('userData', JSON.stringify(loginData.userData));

			var redirect = window.sessionStorage.getItem('redirect');
			if (!redirect) {
				redirect = '/dashboard';
			}
			else {
				window.sessionStorage.removeItem('redirect');
			}

			navigate(redirect);

		}
	}, [loginData]);



	/**
	 * This function fetches the tokens from Cognito.
	 * 
	 * @param {string} code The authorization code that is used to fetch the tokens.
	 */
	function fetchTokens(code) {

		// fetch('https://.auth.us-east-1.amazoncognito.com/oauth2/token',{
		// 	method:'POST',
		// 	headers:{
		// 		'Content-Type': 'application/x-www-form-urlencoded'
		// 	},
		// 	body: new URLSearchParams({
		// 		grant_type: 'authorization_code',
		// 		client_id: '1urq466qsrq9clf5am3t24n9v6',
		// 		redirect_uri: redirect,
		// 		code
		// 	})
		// })
		// 	.then((response) => {
		// 		if (!response.ok) {
		// 			throw new Error('Network response was not ok');
		// 		}

		// 		return response.json();
		// 	})
		// 	.then((data) => {
		// 		var userTokens = {
		// 			accessToken: data.access_token,
		// 			refreshToken: data.refresh_token
		// 		};
		// 		getUser(userTokens);
		// 	})
		// 	.catch((error) => {
		// 		console.error(error); // TODO: Give better error message to user
		// 	});
	}

	/**
	 * This function fetches the user data from the API.
	 * 
	 * @param {object} tokens The user tokens that are used to fetch the user data.
	 */
	function getUser(tokens) {
		Requests.login(tokens)
			.then(data => {
				setLoginData({ userData: data, isAuth: true });
			}).catch((error) => {
				console.error(error);
				window.sessionStorage.removeItem('userData');
				navigate('/');
			});
	}

	/**
	 * 
	 */
	function handleLogin() {
		let tokens = {
			username: username,
			password: password,
		};

		getUser(tokens);
	}

	return (
		<div className='login-container'>
			<h1>Log In</h1>
			<p>Enter your username and password to log in.</p>
			<input
				type="text"
				placeholder="Username"
				value={username}
				onChange={(e) => setUsername(e.target.value)}
			/>
			<input
				type="password"
				placeholder="Password"
				value={password}
				onChange={(e) => setPassword(e.target.value)}
			/>
			<button onClick={handleLogin}>Log In</button>
		</div>

	);
}