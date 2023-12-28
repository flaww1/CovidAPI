import React, { useState } from 'react';
import { register } from '../services/AuthApiService';
import * as S from './styles'; // Import your styled components
const Register = ({ onRegister, onClose }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState(null);

    const handleRegister = async () => {
        try {
            // Basic validation
            if (!username || !password || password !== confirmPassword) {
                // Handle validation error (e.g., show an error message)
                setError('Passwords do not match. Please check your input.');
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
            setError('An error occurred during registration. Please try again later.');
        }
    };

    return (
        <S.Modal>
            <S.ModalContent>
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

                {error && <p className="error-message">{error}</p>}
            </div>
        </div>
</S.ModalContent>
</S.Modal>
    );
};

export default Register;
