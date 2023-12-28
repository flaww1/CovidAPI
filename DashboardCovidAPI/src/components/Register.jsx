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
        <S.ModalContentWrapper className="modal">
            <S.ModalHeading>Register</S.ModalHeading>

            <S.ModalLabel>Username:</S.ModalLabel>
            <S.ModalInput type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />

            <S.ModalLabel>Password:</S.ModalLabel>
            <S.ModalInput type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />

            <S.ModalLabel>Confirm Password:</S.ModalLabel>
            <S.ModalInput
                type="password"
                placeholder="Confirm Password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
            />

            <S.ModalButtonWrapper>
                <S.ModalCancelButton onClick={onClose}>Close</S.ModalCancelButton>
                <S.ModalAddButton onClick={handleRegister}>Register</S.ModalAddButton>
            </S.ModalButtonWrapper>

            {error && <S.ErrorMessage className="error-message">{error}</S.ErrorMessage>}
        </S.ModalContentWrapper>
    );
};

export default Register;
