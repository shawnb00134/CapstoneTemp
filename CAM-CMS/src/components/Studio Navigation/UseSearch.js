import { useState, useCallback, useEffect } from 'react';

function useSearch(initialComponents) {
	const [components, setComponents] = useState(initialComponents);
	const [searchTerm, setSearchTerm] = useState('');

	const searchComponents = useCallback((term) => {
		setSearchTerm(term);

		if (term.length === 0) {
			setComponents(initialComponents);
		} else {
			const filteredComponents = initialComponents.map(component => {
				let filteredItems = [];

				const folderMatches = component.name.toLowerCase().includes(term.toLowerCase());

				if (component.elements) {
					filteredItems = component.elements.filter(item =>
						item.title.toLowerCase().includes(term.toLowerCase()));
				} else if (component.modules) {
					filteredItems = component.modules.filter(item =>
						item.title.toLowerCase().includes(term.toLowerCase()));
				}

				if (folderMatches || filteredItems.length > 0) {
					return { ...component, items: filteredItems };
				}
				return null;
			}).filter(component => component !== null);

			setComponents(filteredComponents);
		}
	}, [initialComponents]);

	useEffect(() => {
		searchComponents('');
	}, [searchComponents]);

	return {
		components,
		searchTerm,
		searchComponents,
	};
}

export default useSearch;