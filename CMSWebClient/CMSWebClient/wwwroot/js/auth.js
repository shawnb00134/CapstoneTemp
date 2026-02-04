function getAuthHeaders() {

    const token = sessionStorage.getItem('accessToken');

    if (!token) return {};

    return {
        'Authorization': 'Bearer ' + token
    };
}
