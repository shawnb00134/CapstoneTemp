
import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeftLong, faTrash, faPlus, faBan, faSave } from '@fortawesome/free-solid-svg-icons';

import Requests from '../utils/Requests';

import './styles/UserPage.css';

function capitalizeFirstLetter(string) {
	return string.charAt(0).toUpperCase() + string.slice(1);
}

export function UserPage() {
	const { id } = useParams();
	const navigate = useNavigate();

	// The user to display
	const [user, setUser] = useState(undefined);

	// If the user is in add mode
	const [isAdding, setIsAdding] = useState(false);

	// Non-parsed contexts
	const [fullContexts, setFullContexts] = useState([]);
	// The possible contexts to choose
	const [contexts, setContexts] = useState({});
	// The possible roles to choose
	const [roles, setRoles] = useState([]);

	// The selected context
	const [selectedContext, setSelectedContext] = useState(undefined);
	// The selected instance of the context
	const [selectedInstance, setSelectedInstance] = useState(undefined);
	// The selected role
	const [selectedRole, setSelectedRole] = useState(undefined);

	useEffect(() => {
		var currentUser = JSON.parse(window.sessionStorage.getItem('userData'));
		if (Number.parseInt(id) === currentUser.id) {
			navigate('/people'); // TODO: Change to user profile page.
			return;
		}

		Requests.getUser(id)
			.then(response => {
				if (!response) {
					navigate('/404');
					return;
				}
				setUser(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});

		getContexts();

		Requests.getAllRoles()
			.then(response => {
				if (response.status === 401) {
					navigate('/404');
				}
				setRoles(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}, [id]);

	useEffect(() => {
		setSelectedInstance(undefined);
		setSelectedContext(undefined);
		setSelectedRole(undefined);
	}, [isAdding]);

	function getContexts() {
		Requests.getAllContexts()
			.then(response => {
				if (response.status === 401) {
					navigate('/404');
				}
				setFullContexts(response);
				let parsedContexts = {};

				if (response && response.length > 0) {
					response.forEach(context => {
						if (Object.keys(parsedContexts).includes(context.type)) {
							parsedContexts[context.type].push(context.instanceName);
						}
						else {
							parsedContexts[context.type] = [context.instanceName];
						}
					});
				}

				setContexts(parsedContexts);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	function deleteRole(role) {
		const deleteData = {
			userId: role.appUserId,
			contextId: role.contextId,
			accessRoleId: role.accessRoleId
		};

		Requests.deleteUserPrivileges(deleteData)
			.then(response => {
				if (response.status === 401) {
					navigate('/404');
				}
				setUser(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});
	}

	function addNewRole() {

		if (!selectedRole || !selectedContext) {
			alert('No data provided');
			return;
		}

		if (selectedContext !== 'system' && !selectedInstance) {
			alert('No instance selected');
			return;
		}

		let roleId = 0;
		roles.forEach(role => {
			if (role.name === selectedRole) {
				roleId = role.id;
			}
		});

		let contextId = 0;

		if (selectedContext == 'system') {
			contextId = 1;
		}
		else {
			fullContexts.forEach(context => {
				if (context.type === selectedContext &&
					context.instanceName === selectedInstance) {
					contextId = context.id;
				}
			});
		}

		const addData = {
			userId: user.id,
			contextId: contextId,
			accessRoleId: roleId
		};

		Requests.createUserPrivileges(addData)
			.then(response => {
				if (response.status === 401) {
					navigate('/404');
				}
				setUser(response);
			}).catch((error) => {
				console.error(error);
				navigate('/error');
			});

		setIsAdding(false);
	}

	if (!user) {
		return (
			<div className='user-page'>
				<h1>Loading...</h1>
			</div>
		);
	}

	const name = `${user.firstname || ''} ${user.lastname || ''}`.trim() || 'No name provided';

	function getInstanceNameFromId(contextType, instanceId) {
		if (contextType === 'system') {
			return '';
		}

		let instanceName = '';
		fullContexts.forEach(context => {
			if (context.type === contextType && context.instance === instanceId) {
				instanceName = context.instanceName;
			}
		});

		return instanceName;
	}

	function checkContexts(role) {	
		let contextTypes = {};

		if (role === 'cam admin') {
			contextTypes['system'] = 'system';
		}

		else if (role === 'org admin' || role === 'brand admin') {
			contextTypes['organization'] = 'organization';
		}

		else {
			contextTypes['library folder'] = 'library folder';
			contextTypes['package'] = 'package';
		}

		return contextTypes;
	}

	function selectedContextChanged(value) {
		setSelectedContext(value);
		setSelectedInstance(undefined);
	}

	function selectedRoleChanged(value) {
		setSelectedRole(value);
		setSelectedContext(undefined);
		setSelectedInstance(undefined);
	}

	return (
		<div className='user-page'>
			<div className='user-page-header'>
				<div className='user-page-heading'>
					<div className='user-page-heading-left'>
						<button onClick={() => navigate('/people')}>
							<FontAwesomeIcon className='icon' icon={faArrowLeftLong} size='xl' />
						</button>
						<h1>{user.username}</h1>
					</div>
					<div className='user-page-heading-right'>
						{/* <button>
							<FontAwesomeIcon className='icon' icon={faTrash} size='xl' />
						</button> */}
					</div>
				</div>
				<div className='user-page-subheading'>
					<h3>Name: {name }</h3>
					<h3>Email: {user.email ? user.email : 'No email provided'}</h3>
					<h3>Phone: {user.phone ? user.phone : 'No phone provided'}</h3>
				</div>
			</div>
			<div className='user-page-content'>
				<div className='user-page-content-privileges'>
					<div className='user-page-content-privileges-header'>
						<h2>Privileges:</h2>
						<button onClick={() => setIsAdding(true)}>
							<FontAwesomeIcon className='icon' icon={faPlus} size='xl' />
						</button>
					</div>
					<div className='user-page-content-privileges-content'>
						<table className='user-page-privileges'>
							<tbody>
								<tr>
									<th>Role</th>
									<th>Context</th>
									<th>Instance</th>
								</tr>
								{
									user.displayRoles.length > 0 ?
										user.displayRoles.map((role, index) => {
											return (
												<tr key={index}>
													<td>{capitalizeFirstLetter(role.name)}</td>
													<td>{capitalizeFirstLetter(role.type)}</td>
													<td>{capitalizeFirstLetter(getInstanceNameFromId(role.type, role.instance))}</td>
													<button onClick={() => deleteRole(role)}>
														<FontAwesomeIcon className='icon' icon={faTrash} size='xl' />
													</button>
												</tr>
											);
										}) :
										!isAdding ?
											<tr>
												<th></th>
												<th>No Roles</th>
											</tr>
											: undefined
								}
								{
									isAdding ?
										<tr>
											<td>
												<select onChange={(evt) => selectedRoleChanged(evt.target.value)}>
													<option value="none" selected disabled>Select a role</option>
													{
														roles.map((role, index) => {
															if (role.name === 'user') {
																return undefined;
															}
															return (
																<option key={index} value={role.name}>
																	{capitalizeFirstLetter(role.name)}
																</option>
															);
														})
													}
												</select>
											</td>
											<td>
												{
													!selectedRole ? undefined :
														<select onChange={(evt) => selectedContextChanged(evt.target.value)}>
															<option value="none" selected={!selectedContext} disabled>Select a context</option>
															{
																Object.keys(checkContexts(selectedRole)).map((context, index) => {
																	return (
																		<option key={index} value={context}>
																			{capitalizeFirstLetter(context)}
																		</option>
																	);
																})
															}
														</select>
												}
											</td>
											<td>
												{
													!selectedContext || selectedContext === 'system' ? undefined :
														instanceSelector()
												}
											</td>
											<button onClick={addNewRole}>
												<FontAwesomeIcon className='icon' icon={faSave} size='xl' />
											</button>
											<button onClick={() => setIsAdding(false)}>
												<FontAwesomeIcon className='icon' icon={faBan} size='xl' />
											</button>
										</tr> : undefined
								}
							</tbody>
						</table>
					</div>
				</div>
				<div>
					<h2>Logs:</h2>
				</div>
			</div>
		</div>
	);

	function instanceSelector() {
		return !contexts[selectedContext] ? <select disabled >
			<option value="none" disabled selected={!selectedInstance}>No instances available.</option>
		</select> :
			<select onChange={(evt) => setSelectedInstance(evt.target.value)}>
				<option value="none" disabled selected={!selectedInstance}>Select an instance</option>
				{contexts[selectedContext].map((instance, index) => {
					if (!instance) {
						return undefined;
					}
					return (
						<option key={index} value={instance}>
							{instance}
						</option>
					);
				})}
			</select>;
	}
}