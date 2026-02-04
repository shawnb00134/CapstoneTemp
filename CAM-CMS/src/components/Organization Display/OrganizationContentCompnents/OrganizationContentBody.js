import { React, useEffect, useState } from 'react';
import OrganizationRenderedPackage from './OrganizationRenderedPackage';
import './styles/OrganizationContentBody.css';

export default function OrganizationContentBody({ packageData, organizationRoles }) {
	const [selectedRole, setSelectedRole] = useState('');
	const [selectedArchetypeIds, setSelectedArchetypeIds] = useState([]);
	const [filteredPackages, setFilteredPackages] = useState([]);

	useEffect(() => {
		if (organizationRoles.length > 0) {
			const defaultRole = organizationRoles[0];
			setSelectedRole(defaultRole.organizationContentRoleId);
			setSelectedArchetypeIds(defaultRole.archetypeIds || []);
		}
	}, [organizationRoles]);

	useEffect(() => {
		if (selectedRole) {
			setFilteredPackages(packageData);
		} else {
			setFilteredPackages([]);
		}
	}, [selectedRole, packageData]);

	const handleRoleChange = (e) => {
		const selectedRoleId = parseInt(e.target.value, 10);
		setSelectedRole(selectedRoleId);
		const selectedRole = organizationRoles.find(role => role.organizationContentRoleId === selectedRoleId);
		setSelectedArchetypeIds(selectedRole ? selectedRole.archetypeIds : []);
	};

	return (
		<div>
			<div className='content-role-dropdown'>
				<select name="contentRole" onChange={handleRoleChange} value={selectedRole}>
					{organizationRoles.length === 0 ? (
						<option value="">Create a content role to view</option>
					) : (
						organizationRoles.map((role, index) => (
							<option key={index} value={role.organizationContentRoleId}>{role.displayName}</option>
						))
					)}
				</select>
			</div>
			<div className='org-content-body'>
				{filteredPackages.length > 0 ? (
					<OrganizationRenderedPackage packageData={filteredPackages} archetypeIds={selectedArchetypeIds} />
				) : (
					<p>No packages available for this content role.</p>
				)}
			</div>
		</div>
	);
}
