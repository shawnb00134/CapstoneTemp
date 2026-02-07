document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('loginForm');
    const errorBox = document.getElementById('errorMessage');

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        errorBox.style.display = 'none';

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        try {
            const response = await fetch('/Authorization/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    Username: username,
                    Password: password
                })
            });

            if (!response.ok) {
                throw new Error('Invalid login');
            }

            // Token is now in cookie, just redirect
            window.location.href = '/Home/Dashboard';
        }
        catch (err) {
            errorBox.textContent = 'Login failed. Check credentials.';
            errorBox.style.display = 'block';
        }
    });
});