// Register.jsx
import React, { useState } from 'react';
import { register } from '../services/AuthApiService';

const Register = ({ onRegister, onClose }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const handleRegister = async () => {
        try {
            // Basic validation
            if (!username || !password || password !== confirmPassword) {
                // Handle validation error (e.g., show an error message)
                console.error('Invalid registration data');
                return;
            }

            const user = { username, password };
            await register(user);
            // Handle successful registration (e.g., show a success message, redirect to login)
            console.log('Registration successful');
            onRegister(); // Call the provided callback on successful registration
        } catch (error) {
            // Handle registration error (e.g., show an error message)
            console.error('Registration failed:', error);
        }
    };

    return (
        <div className="modal">
            <div className="modal-content">
                <h2>Register</h2>
                <input type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
                <input type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />
                <input
                    type="password"
                    placeholder="Confirm Password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                />
                <button onClick={handleRegister}>Register</button>
                <button onClick={onClose}>Close</button>
            </div>
        </div>
    );
};

export default Register;
