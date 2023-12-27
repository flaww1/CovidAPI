// Login.jsx
import React, { useState } from 'react';
import { login } from '../services/AuthApiService';

const Login = ({ onLogin, onClose }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = async () => {
        try {
            const credentials = { username, password };
            const token = await login(credentials);
            console.log('Received token:', token);
            onLogin(); // Call the provided callback on successful login
        } catch (error) {
            // Handle login error (e.g., show an error message)
            console.error('Login failed:', error);
        }
    };

    return (
        <div className="modal">
            <div className="modal-content">
                <h2>Login</h2>
                <input type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
                <input type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />
                <button onClick={handleLogin}>Login</button>
                <button onClick={onClose}>Close</button>
            </div>
        </div>
    );
};

export default Login;
