// Requests utility object
const Requests = {
    login: async function(credentials) {
        //const response = await fetch('/api/auth/login', {
        const response = await fetch('https://localhost:5432/Authorization/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(credentials)
        });
        
        if (!response.ok) {
            throw new Error('Login failed');
        }
        
        return await response.json();
    }
};

document.addEventListener('DOMContentLoaded', function() {
    // Clear any existing session data
    sessionStorage.removeItem('userData');
    sessionStorage.removeItem('userReadPrivileges');
    
    // Check for OAuth code in URL
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get('code');
    
    if (code) {
        fetchTokens(code);
    }
    
    // Set up login button handler
    document.getElementById('loginBtn').addEventListener('click', handleLogin);
    
    // Allow Enter key to submit
    document.getElementById('loginForm').addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            handleLogin();
        }
    });
});

function handleLogin() {
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorMessage = document.getElementById('errorMessage');
    
    // Clear previous errors
    errorMessage.style.display = 'none';
    document.getElementById('usernameError').textContent = '';
    document.getElementById('passwordError').textContent = '';
    
    // Validate inputs
    if (!username || !password) {
        errorMessage.textContent = 'Please enter both username and password';
        errorMessage.style.display = 'block';
        return;
    }
    
    // Disable button during login
    const loginBtn = document.getElementById('loginBtn');
    loginBtn.disabled = true;
    loginBtn.textContent = 'Logging in...';
    
    const tokens = {
        //username: username,
        //password: password
        Username: username,
        Password: password
    };
    
    getUser(tokens);
}

function fetchTokens(code) {
    const redirect = window.location.href.replace(window.location.search, '');
    
    fetch('https://<your-cognito-domain>.auth.us-east-1.amazoncognito.com/oauth2/token', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({
            grant_type: 'authorization_code',
            client_id: '<your-client-id>',
            redirect_uri: redirect,
            code: code
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        const userTokens = {
            accessToken: data.access_token,
            refreshToken: data.refresh_token
        };
        getUser(userTokens);
    })
    .catch(error => {
        console.error(error);
        showError('Authorization failed. Please try again.');
    });
}

function getUser(tokens) {
    Requests.login(tokens)
        .then(data => {
            // Store user data
            sessionStorage.setItem('userData', JSON.stringify(data));
            
            // Get redirect URL or default to dashboard
            let redirect = sessionStorage.getItem('redirect') || '/Home/Dashboard';
            sessionStorage.removeItem('redirect');
            
            // Redirect to dashboard
            window.location.href = redirect;
        })
        .catch(error => {
            console.error(error);
            showError('Login failed. Please check your credentials.');
            
            // Re-enable button
            const loginBtn = document.getElementById('loginBtn');
            loginBtn.disabled = false;
            loginBtn.textContent = 'Log In';
        });
}

function showError(message) {
    const errorMessage = document.getElementById('errorMessage');
    errorMessage.textContent = message;
    errorMessage.style.display = 'block';
}