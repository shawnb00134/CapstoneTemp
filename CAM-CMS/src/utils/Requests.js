/**
 * @file Requests.js
 * @module Requests
 * @category utilities
 * @description Requests is a utility class that contains static methods for making api requests.
 * @author Steven Kight
 * @version 1.2.0
 */
class Requests {
	static enviornmentVariables = process.env;

	/**
	 * Appends validation data to the end of a given url for api validation.
	 * 
	 * @static
	 * @param {string} baseUrl The url to append validation data to.
	 * @returns url with validation check data appended to it.
	 * @example
	 * const url = Requests.validationUrl('https://example.com');
	 * @memberof Requests
	 */
	static privilegeRequestUrl(baseUrl) {
		var id = JSON.parse(window.sessionStorage.getItem('userData')).id;
		let userAccessToken = JSON.parse(window.sessionStorage.getItem('userData')).accessToken;
		return baseUrl + `-${id}` + `&${userAccessToken}`;
	}

	/**
	 * Makes a get request data to the given url and returns response as json.
	 * 
	 * @async @static
	 * @param {string} url The url to make the get request to.
	 * @returns response data as json
	 * @throws error if request fails
	 * @example
	 * const data = await Requests.get('https://example.com');
	 * console.log(data);
	 * @example
	 * Requests.get('https://example.com')
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async get(url) {
		try {
			const response = await fetch(url);
			const data = await response.json();
			return data;
		}
		catch (error) {
			throw Error(error);
		}
	}

	/**
	 * Makes a get request to the given url and returns the true if accepted and false otherwise.
	 * 
	 * @async @static
	 * @param {string} url The url to make the get request to.
	 * @returns true if accepted and false otherwise
	 * @throws error if request fails
	 * @example
	 * const data = await Requests.get('https://example.com');
	 * console.log(data);
	 * @example
	 * Requests.get('https://example.com')
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAcceptable(url) {
		try {
			const response = await fetch(url);
			return response.status === 202;
		}
		catch (error) {
			return false;
		}
	}

	/**
	 * Makes a post request to the given url with the given body(FormData) and returns response as json.
	 * 
	 * @async @static
	 * @param {string} url The url to make the post request to.
	 * @param {FormData} body The body to send with the post request.
	 * @returns response data as json
	 * @throws error if request fails
	 * @example
	 * const data = await Requests.postFormdata('https://example.com', new FormData());
	 * console.log(data);
	 * @example
	 * Requests.postFormdata('https://example.com', new FormData())
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 * @see https://developer.mozilla.org/en-US/docs/Web/API/FormData
	 */
	static async postFormdata(url, body) {
		try {
			const response = await fetch(url, {
				method: 'POST',
				body: body
			});
			try {
				const data = await response.json();
				return data;
			}
			catch (error) {
				return response;
			}
		}
		catch (error) {
			throw Error(error);
		}
	}

	/**
	 * Makes a post request to the given url with the given body(json) and returns response as json.
	 * 
	 * @async @static
	 * @param {string} url The url to make the post request to.
	 * @param {object} body The body to send with the post request.
	 * @returns response data as json
	 * @throws error if request fails
	 * @example
	 * const data = await Requests.postJson('https://example.com', {});
	 * console.log(data);
	 * @example
	 * Requests.postJson('https://example.com', {})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 * @see https://developer.mozilla.org/en-US/docs/Web/API/Body/json
	 */
	static async postJson(url, body) {
		try {
			const response = await fetch(url, {
				method: 'POST',
				body: JSON.stringify(body),
				headers: {
					'access-control-allow-origin': '*',
					Accept: 'application/json',
					'Content-Type': 'application/json'
				}
			});

			try {
				const data = await response.json();
				return data;
			}
			catch (error) {
				return response;
			}
		}
		catch (error) {
			throw Error(error);
		}
	}

	/**
	 * Makes a api request to check if the given user tokens are valid.
	 * If the tokens are valid the user is authorized and the user data is returned.
	 * 
	 * @async @static
	 * @param {object} userTokens The user tokens to validate.
	 * @returns user data if tokens are valid
	 * @throws error if request fails (invalid request, invalid tokens, or server error)
	 * @example
	 * const data = await Requests.authorize({});
	 * console.log(data);
	 * @example
	 * Requests.authorize({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async authorize(userTokens) {
		return await Requests.postJson(Requests.enviornmentVariables.REACT_APP_AUTHORIZE, userTokens);
	}

	/**
	 * Makes a api request to get all users.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @returns all users
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getAllUsers();
	 * console.log(data);
	 * @example
	 * Requests.getAllUsers()
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAllUsers() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_USERS);
		return await Requests.get(url);
	}

	static async getUser(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_GET_USER);
		return await Requests.postJson(url, id);
	}

	/**
	 * Makes a api request to get all contexts possible.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @returns all contexts
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getAllContexts();
	 * console.log(data);
	 * @example
	 * Requests.getAllContexts()
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAllContexts() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_CONTEXTS);
		return await Requests.get(url);
	}

	/**
	 * Makes a api request to get all user roles possible.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @returns all user roles
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getAllUserRoles();
	 * console.log(data);
	 * @example
	 * Requests.getAllUserRoles()
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAllRoles() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_ROLES);
		return await Requests.get(url);
	}

	/**
	 * Makes a api request to get all role privileges possible.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @returns all role privileges
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getAllRolePrivileges();
	 * console.log(data);
	 * @example
	 * Requests.getAllRolePrivileges()
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAllPrivileges() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_PRIVILEGES);
		return await Requests.get(url);
	}

	/**
	 * Makes a api request to create a add a new role to a user.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} userRoleChange The user role to create.
	 * @returns The user after the roles has been updated.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.createUserRoleChange({});
	 * console.log(data);
	 * @example
	 * Requests.createUserRoleChange({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async createUserPrivileges(userRoleChange) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_USER_PRIVILEGES);
		return await Requests.postJson(url, userRoleChange);
	}

	/**
	 * Makes a api request to update a user's role.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} userRoleChange The user role to update.
	 * @returns The user after the roles has been updated.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.updateUserRoleChange({});
	 * console.log(data);
	 * @example
	 * Requests.updateUserRoleChange({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async updateUserPrivileges(userRoleChange) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_USER_PRIVILEGES);
		return await Requests.postJson(url, userRoleChange);
	}

	/**
	 * Makes a api request to delete a user's role.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} userRoleChange The user role to delete.
	 * @returns The user after the roles has been updated.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.deleteUserRoleChange({});
	 * console.log(data);
	 * @example
	 * Requests.deleteUserRoleChange({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async deleteUserPrivileges(userRoleChange) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_USER_PRIVILEGES);
		return await Requests.postJson(url, userRoleChange);
	}

	static async getAllPackages() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_PACKAGES);
		return await Requests.get(url);
	}

	static async getPackage(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_PACKAGE);
		return await Requests.postJson(url, id);
	}

	static async createPackage(packageData) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_PACKAGE);
		return await Requests.postJson(url, packageData);
	}

	static async addPackageFolder(folderData) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_PACKAGE_FOLDER);
		return await Requests.postJson(url, folderData);
	}

	static async updatePackage(packageData) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_PACKAGE);
		return await Requests.postJson(url, packageData);
	}

	static async deletePackage(packageData) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_PACKAGE);
		return await Requests.postJson(url, packageData);
	}

	static async deletePackageFolder(folderData) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_PACKAGE_FOLDER);
		return await Requests.postJson(url, folderData);
	}

	/**
	 * Makes a api request to get all modules possible.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @returns all modules
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getAllModules();
	 * console.log(data);
	 * @example
	 * Requests.getAllModules()
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAllModules() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_MODULES);
		return await Requests.get(url);
	}

	static async createModule(module) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_MODULE);
		return await Requests.postJson(url, module);
	}

	static async deleteModule(module) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_MODULE);
		return await Requests.postJson(url, module);
	}

	/**
	 * Makes a api request to load a module by id.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} module The module to load.
	 * @returns The module after it has been loaded.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.loadModule(<module_id>);
	 * console.log(data);
	 * @example
	 * Requests.loadModule(<module_id>)
	 *  .then(data => console.log(data))
	 *  .catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async loadModule(module) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_MODULE_BY_MODULE_ID);
		return await Requests.postJson(url, module);
	}

	/**
	 * Makes a api request to create a published module.
	 * This request requires user to have valid privilegs.
	 * 
	 * @param {object} module The module to create.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.createPublishedModule({});
	 * console.log(data);
	 * @example
	 * Requests.createPublishedModule({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async createPublishedModule(module) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_PUBLISHED_MODULE);
		return await Requests.postJson(url, module);
	}

	/**
	 * Makes a api request to load a published module.
	 * This request requires user to have valid privilegs.
	 * 
	 * @param {object} module The module to create.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.loadPublishedModuleById({});
	 * console.log(data);
	 * @example
	 * Requests.loadPublishedModuleById({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async loadPublishedModuleById(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_PUBLISHED_MODULE_BY_MODULE_ID);
		return await Requests.postJson(url, id);
	}

	/**
	 * Makes a api request to check if a moudle has been published.
	 * This request requires user to have valid privilegs.
	 * 
	 * @param {object} module The module to create.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.hasPublishedModule({});
	 * console.log(data);
	 * @example
	 * Requests.hasPublishedModule({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async hasPublishedModule(module) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_HAS_PUBLISHED_MODULE);
		return await Requests.postJson(url, module);
	}
	static async getAllPublishedModules() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ALL_PUBLISHED_MODULE);
		return await Requests.get(url);
	}
	/**
	 * Makes a api request to delete a published module.
	 * This request requires user to have valid privilegs.
	 * 
	 * @param {object} module The module to create.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.deletePublishedModule({});
	 * console.log(data);
	 * @example
	 * Requests.deletePublishedModule({})
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async deletePublishedModule(module) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_PUBLISHED_MODULE);
		await Requests.postJson(url, module);
	}

	static async editSetInModule(set) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_EDIT_SET);
		return await Requests.postJson(url, set);
	}

	static async addSetToModule(newSet) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_ADD_SET);
		return await Requests.postJson(url, newSet);
	}

	static async deleteSetFromModule(set) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_DELETE_SET);
		return await Requests.postJson(url, set);
	}

	static async moveSetInModule(set) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_MOVE_SET);
		return await Requests.postJson(url, set);
	}

	static async editElementInModule(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_EDIT_ELEMENT);
		return await Requests.postJson(url, element);
	}

	static async addElementToModule(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_ADD_ELEMENT);
		return await Requests.postJson(url, element);
	}

	static async swapElementInModule(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_SWAP_ELEMENT);
		return await Requests.postJson(url, element);
	}

	static async deleteElementFromModule(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_DELETE_ELEMENT);
		return await Requests.postJson(url, element);
	}

	static async moveElementInSet(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_MOVE_ELEMENT);
		return await Requests.postJson(url, element);
	}

	static async moveElementToNewSet(updatedSet, currentSet) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_MOVE_ELEMENT_TO_NEW_SET);
		return await Requests.postJson(url, [updatedSet, currentSet]);
	}

	static async moveElementSetAndPlace(updatedElement, currentElement) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_MODULE_MOVE_ELEMENT_SET_AND_PLACE);
		return await Requests.postJson(url, [updatedElement, currentElement]);
	}

	static async createElement(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_ELEMENT);
		return await Requests.postFormdata(url, element);
	}

	/**
	 * Makes a api request to get all elements possible.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @returns all elements
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getAllElements();
	 * console.log(data);
	 * @example
	 * Requests.getAllElements()
	 * 	.then(data => console.log(data))
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getAllElements() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_ELEMENTS);
		return await Requests.get(url);
	}
	/**
	 * Makes a api request to update a element.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} element The element to update.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * await Requests.updateElement({});
	 * @example
	 * Requests.updateElement({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async updateElement(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_ELEMENT);
		return await Requests.postJson(url, element);
	}

	static async updateElementWithFile(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_ELEMENT_WITH_FILE);
		return await Requests.postFormdata(url, element);

	}
	/**
	 * Makes a api request to delete a element.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} element The element to delete.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * await Requests.deleteElement({});
	 * @example
	 * Requests.deleteElement({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async deleteElement(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_ELEMENT);
		return await Requests.postJson(url, element);
	}

	/**
	 * Makes a api request to get element by id.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} elementId The id of the element to get.
	 * @returns The element with the given id.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * const data = await Requests.getElementById({});
	 * console.log(data);
	 * @example
	 * Requests.getElementById({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async getElementById(elementId) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ELEMENT_BY_ELEMENT_ID);
		return await Requests.postJson(url, elementId);
	}

	static async getAllLibraryFolders() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ALL_LIBRARYFOLDERS);
		return await Requests.get(url);
	}

	/**
	 * Makes a api request to create a library folder.
	 * TODO: Add privileges check
	 * 
	 * @async @static
	 * @param {object} libraryFolder The library folder to create.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * await Requests.createLibraryFolder({});
	 * @example
	 * Requests.createLibraryFolder({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async createLibraryFolder(libraryFolder) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_LIBRARYFOLDER);
		return await Requests.postJson(url, libraryFolder);
	}

	/**
	 * Makes a api request to delete a library folder.
	 * TODO: Add privileges check
	 * 
	 * @async @static
	 * @param {object} libraryFolder The library folder to delete.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * await Requests.deleteLibraryFolder({});
	 * @example
	 * Requests.deleteLibraryFolder({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async deleteLibraryFolder(libraryFolder) { // TODO: Check if the return value is needed
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_LIBRARYFOLDER);
		var response = await Requests.postJson(url, libraryFolder);
		if (!response) {
			return [];
		}
		return response;
	}

	/**
	 * Makes a api request to update the folder of the given element.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} element The module to update.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * await Requests.updateElementLibraryFolder({});
	 * @example
	 * Requests.updateElementLibraryFolder({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async updateElementLibraryFolder(element) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_ELEMENT_LIBRARY_FOLDER);
		await Requests.postJson(url, element);
	}

	/**
	 * Makes a api request to update the folder of the given module.
	 * This request requires user to have valid privilegs.
	 * 
	 * @async @static
	 * @param {object} module The module to update.
	 * @throws error if request fails (invalid request or server error)
	 * @example
	 * await Requests.updateModuleLibraryFolder({});
	 * @example
	 * Requests.updateModuleLibraryFolder({})
	 * 	.then(...)
	 * 	.catch(error => console.log(error));
	 * @memberof Requests
	 */
	static async updateModuleLibraryFolder(module) {
		var url = Requests.privilegeRequestUrl(process.env.REACT_APP_UPDATE_MODULE_LIBRARYFOLDER);
		await Requests.postJson(url, module);
	}
	static async createPackageFolderModule(content) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_PACKAGE_FOLDER_MODULE);
		await Requests.postJson(url, content);
	}
	static async deletePackageFolderModule(content) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_PACKAGE_FOLDER_MODULE_BY_ID);
		await Requests.postJson(url, content);
	}
	static async movePackageFolder(content) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_PACKAGE_MOVE_FOLDER);
		return await Requests.postJson(url, content);
	}
	static async reorderPackageFolder(content) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_PACKAGE_FOLDER_REORDER);
		return await Requests.postJson(url, content);
	}
	static async reorderPackageFolderModule(content) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_PACKAGE_FOLDER_MODULE_ORDER);
		await Requests.postJson(url, content);
	}
	static async updatePackageFolderModule(content) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_PACKAGE_FOLDER_MODULE);
		await Requests.postJson(url, content);
	}
	static async getAllOrganizations() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ALL_ORGANIZATIONS);
		return await Requests.get(url);
	}
	static async getOrganizationById(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ORGANIZATION_BY_ORGANIZATION_ID);
		return await Requests.postJson(url, id);
	}
	static async getOrganizationByPackageId(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ORGANIZATION_BY_PACKAGE_ID);
		return await Requests.postJson(url, id);
	}
	static async createOrganization(organization) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_ORGANIZATION);
		return await Requests.postJson(url, organization);
	}
	static async updatePackageAssoicatedOrganizations(data) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_PACKAGE_ASSOICATED_ORGANIZATIONS);
		return await Requests.postJson(url, data);
	}
	static async getAllPackagesByOrganizationId(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_GET_ALL_PACKAGE_BY_ORGANIZATION_ID);
		return await Requests.postJson(url, id);
	}
	static async updateOrganizationAssociatedPackages(data) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_ORGANIZATIONS_ASSOICATED_PACKAGE);
		return await Requests.postJson(url, data);
	}

	static async getStudioAside() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_STUDIO_ASIDE);
		return await Requests.get(url);
	}
	static async getContentRoles() {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_CONTENT_ROLES);
		return await Requests.get(url);
	}
	static async updatePackageFolder(data) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_UPDATE_PACKAGE_FOLDER);
		return await Requests.postJson(url, data);
	}
	static async createOrganizationContentRole(data) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CREATE_ORGANIZATION_CONTENT_ROLE);
		return await Requests.postJson(url, data);
	}
	static async getAllOrganizationContentRoles(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ORGANIZATION_CONTENT_ROLES);
		return await Requests.postJson(url, id);
	}
	static async deleteOrganizationContentRole(data) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_ORGANIZATION_CONTENT_ROLE);
		return await Requests.postJson(url, data);
	}
	static async getModulesByLibraryFolderId(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_MODULE_BY_LIBRARYFOLDER_ID);
		return await Requests.postJson(url, id);
	}
	static async getElementsByLibraryFolderId(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_LOAD_ELEMENT_BY_LIBRARYFOLDER_ID);
		return await Requests.postJson(url, id);
	}
	static async getSubFolders(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_GET_ALL_SUBFOLDERS);
		return await Requests.postJson(url, id);
	}
	static async getFolderModules(id) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_GET_ALL_SUBFOLDERS_MODULES);
		return await Requests.postJson(url, id);
	}

	static async deleteOrganization(organizationId) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_ORGANIZATION);
		return await Requests.postJson(url, organizationId);
	}

	/**
	 * Takes in a userId makes a request to get all the privileges associated with the user.
	 * 
	 * @async @static
	 * @param {int} userId The id of the user to get the privileges of.
	 * @returns A map of the privileges associated with the roles.
	 */
	static async getPrivilegesByRoles(userId) {
		var url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_GET_USER_PRIVILEGES);
		let result = await Requests.postJson(url, userId);
		let privileges = new Map();
		for (const element of result) {
			privileges.set(element.name, element.value);
		}

		return privileges;
	}

	/**
	 * Appends the given request to the end of the given baseUrl.
	 * @param {string} baseUrl 
	 * @param {string} request 
	 * @returns A url with the request appended to the end.
	 */
	static authorizeRequestUrl(baseUrl, request) {
		return baseUrl + `&${request}`;
	}

	/**
	 * Checks what read privileges the logged in user has.
	 * 
	 * @async @static
	 * @returns A tuple of booleans representing the read privileges of the user. True if the user has the privilege, false otherwise.
	 */
	static async userCanRead() {
		var url = Requests.enviornmentVariables.REACT_APP_USER_CAN_READ;
		url = Requests.privilegeRequestUrl(url);
		let result = await Requests.get(url);
		return result;
	}

	/**
	 * Makes a request to refresh the user's access token.
	 * 
	 * @returns The new access token or null if the request fails.
	 */
	static refreshToken() {

		var url = Requests.enviornmentVariables.REACT_APP_REFRESH_TOKEN;

		let refreshToken = JSON.parse(window.sessionStorage.getItem('userData')).refreshToken;
		url = url + `-${refreshToken}`;

		return Requests.get(url);

	}

	static async addUser(request) {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ADD_USER);
		return await this.postJson(url, request);
	}

	static async deleteUser(request) {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_USER);
		return await Requests.postJson(url, request);
	}

	static async deleteTempUser(request) {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_DELETE_TEMP_USER);		
		return await Requests.postJson(url, request);
	}

	static async checkAndUpdateUsers() {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_CHECK_AND_UPDATE_USERS);
		return await Requests.postJson(url, {});
	}

	static async getAllTempUsers() {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_GET_ALL_TEMP_USERS);
		return await Requests.get(url);
	}

	static async login(request) {
		const url = Requests.enviornmentVariables.REACT_APP_LOGIN;
		return await Requests.postJson(url, request);
	}

	/**
	 * Makes a post request to the given url with the given body(json) and returns true if accepted and false otherwise.
	 * 
	 * @param {string} url 
	 * @param {string} body 
	 */
	static async postAcceptable(url, body){
		try {
			const response = await fetch(url, {
				method: 'POST',
				body: JSON.stringify(body),
				headers: {
					'access-control-allow-origin': '*',
					Accept: 'application/json',
					'Content-Type': 'application/json'
				}
				
			});

			return response ? response.status === 202 : false;
		}
		catch (error) {
			throw Error(error);
		}
	}

	static async authorizePackage(request, packageId) {
		let url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_AUTHORIZE_SINGLE_PACKAGE);

		url = Requests.authorizeRequestUrl(url, request);

		return await Requests.postAcceptable(url, packageId);
	}

	static async authorizeLibraryFolder(request, libraryFolderId) {
		let url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_AUTHORIZE_SINGLE_LIBRARY);

		url = Requests.authorizeRequestUrl(url, request);

		return await Requests.postAcceptable(url, libraryFolderId);
	}

	static async addContentRole(request) {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ADD_CONTENT_ROLE);
		return await this.postJson(url, request);
	}

	static async getArchetypes() {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_ALL_ROLE_ARCHETYPES);
		return await this.get(url);
	}

	static async userIsAdmin() {
		const url = Requests.privilegeRequestUrl(Requests.enviornmentVariables.REACT_APP_IS_USER_ADMIN);
		return await Requests.get(url);
	}
}

export default Requests;