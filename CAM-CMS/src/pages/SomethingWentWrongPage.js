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

const SomethingWentWrongPage = () => {
	const handleGoBack = useNavigateToDashboard();

	return (
		<div className="not-found-container">
			<h1>OOPS! - Something went wrong!</h1>
			<p>	The feature has encountered an unexpected error. Sorry for the inconvenience.</p>
			<p>	Please retry your request or contact an administrator to seek assistance.</p>
			<button onClick={handleGoBack}>Go to Dashboard</button>
		</div>
	);
};

export default SomethingWentWrongPage;
