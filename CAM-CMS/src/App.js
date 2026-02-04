import React from 'react';

import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

import LoginPage from './pages/LoginPage';
import PostAuthorize from './pages/PostAuthorize';

import './App.css';
import '@fortawesome/fontawesome-svg-core/styles.css';

export default function App() {
	return (
		<div className='App'>
			<BrowserRouter>
				<Routes>
					<Route
						path='/'
						index
						Component={() => {
							return (
								<LoginPage />
							);
						}}
					/>
					<Route
						path='/*'
						
						Component={
							() => {
								var user = JSON.parse(window.sessionStorage.getItem('userData'));
	
								if (!user) {
									window.sessionStorage.setItem('redirect', window.location.pathname);
									return <Navigate to='/' />;
								}
	
								return <PostAuthorize />;
							}	
						}
					/>
				</Routes>
				
			</BrowserRouter>
		</div>
	);
}