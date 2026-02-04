import React from 'react';

import { Outlet } from 'react-router-dom';

import './styles/Studio.css';

import StudioNavigation from '../components/Studio Navigation/StudioNavigation';

/**
 * The page that is displayed after the user has been authorized and has navigated to the /studio route.
 */
export default function Studio() {
	return (
		<div className='studio-content row'>
			<StudioNavigation />

			{/* Outlet is where the actual item on studio is put afer naviagtion */}
			<Outlet />
		</div>
	);
}