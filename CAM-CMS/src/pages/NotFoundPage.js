import React from 'react';
import { useNavigate } from 'react-router-dom';
import './styles/NotFoundPage.css';

const useNavigateToDashboard = () => {
	const navigate = useNavigate();

	const handleGoBack = () => {
		navigate('/dashboard');
	};

	return handleGoBack;
};

const NotFoundPage = () => {
	const handleGoBack = useNavigateToDashboard();

	return (
		<div className="not-found-container">
			<h1>404 - Page Not Found</h1>
			<p>	The page you are looking for either does not exist, you lack the permissions to view it, or otherwise cannot be reached. </p>
			<p>	Please check the URL or try again later and contact an administrator if you believe this is an error. </p>
			<button onClick={handleGoBack}>Go to Dashboard</button>
		</div>
	);
};

export default NotFoundPage;
