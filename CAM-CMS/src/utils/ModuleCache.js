/**
 * @file ModuleCache.js
 * @module ModuleCache
 * @category utilities
 * @description A utility class for creating a module cache from a module
 * @author Steven Kight
 * @version 1.0.0
 */
class ModuleCache {
	static buildModuleCache(module) {
		var moduleJson = {
			title: module.title,
			pages: [[]],
			elementSets: [],
			anchors: [],
		};
  
		var currentPage = 0;
		var lastParentNode = undefined;
		module.elementSets.forEach(elementSet => {
			var newElementSet = {
				setLocationId: elementSet.setLocationId,
				place: elementSet.place,
				styling: elementSet.styling,
				locations: []
			};
			
			newElementSet.locations = elementSet.elements.map(element => {
				var newLocation = {
					place: element.place,
					element: element.element,
					locationAttribute: element.attributes
				};
				var anchor = {
					anchorId: '',
					page: '',
					content: '',
					subHeader: false,
					level: 0,
					headerChildren: [],
					headerParent: undefined
				};
				
				if (newLocation.element.typeId === 7) {
					anchor.anchorId = newLocation.element.elementId;
					anchor.level = element.attributes.headingLevel;
					
					if (lastParentNode === undefined || lastParentNode.level == anchor.level) {
						lastParentNode = anchor;
						moduleJson.anchors.push(anchor);
					} else if (lastParentNode.level < anchor.level) {
						if (lastParentNode.headerChildren.length === 0 || lastParentNode.level + 1 === anchor.level) {
							anchor.headerParent = lastParentNode;
							lastParentNode.headerChildren.push(anchor);
						} else {
							ModuleCache.addAnchor(lastParentNode, anchor);
						}
					}
					
					if (newElementSet.styling?.hasPageBreak === 'before') {
						anchor.page = currentPage + 1;
					} else {
						anchor.page = currentPage;
					}
					anchor.content = newLocation.element.title;
					
				}
				return newLocation;
			});

			if (newElementSet.styling?.hasPageBreak === 'before') {
				currentPage++;
				moduleJson.pages[currentPage] = [newElementSet];
			} else if (newElementSet.styling?.hasPageBreak === 'after') {
				moduleJson.pages[currentPage].push(newElementSet);
				currentPage++;
				moduleJson.pages[currentPage] = [];
			} else {
				moduleJson.pages[currentPage].push(newElementSet);
			}

			moduleJson.elementSets.push(newElementSet);
		});
		
		return moduleJson;
	}

	static addAnchor(node, child) {
		if(node.headerChildren.length === 0) {
			child.headerParent = node;
			node.headerChildren.push(child);
		}
		if (node === undefined) {
			return;
		}
		while (node.headerChildren.length !== 0) {
			node = node.headerChildren[node.headerChildren.length - 1];
			if ((node.level + 1) === child.level) {
				child.headerParent = node;
				node.headerChildren.push(child);
				return;
			}
		}
		node.headerChildren.push(child);
	}
}

export default ModuleCache;