import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { login } from '../services/AuthApiService';
import * as S from './styles'; // Import your styled components


const Login = ({ onLogin, onClose }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleLogin = async () => {
        try {
            const credentials = { username, password };
            const user = await login(credentials);

            // Call the provided callback on successful login
            onLogin(user);
            navigate('/dashboard');
        } catch (error) {
            // Handle login error
            console.error('Login failed:', error);

            if (error.response && error.response.status === 401) {
                setError('Incorrect username or password. Please try again.');
            } else {
                setError('An error occurred during login. Please try again later.');
            }
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

                {error && <p className="error-message">{error}</p>}

                <p>
                    Don't have an account?{' '}
                    <Link to="/register" onClick={onClose}>
                        Register here
                    </Link>
                </p>
            </div>
        </div>
    );
};

export default Login;
