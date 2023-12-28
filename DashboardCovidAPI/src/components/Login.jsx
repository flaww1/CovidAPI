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
        <S.ModalContentWrapper className="modal">
            <S.ModalHeading>Login</S.ModalHeading>
            <S.ModalLabel>Username:</S.ModalLabel>
            <S.ModalInput type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />

            <S.ModalLabel>Password:</S.ModalLabel>
            <S.ModalInput type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />

            <S.ModalButtonWrapper>
                <S.ModalCancelButton onClick={onClose}>Close</S.ModalCancelButton>
                <S.ModalAddButton onClick={handleLogin}>Login</S.ModalAddButton>
            </S.ModalButtonWrapper>

            {error && <S.ErrorMessage className="error-message">{error}</S.ErrorMessage>}

            <p>
                Don't have an account?{' '}
                <Link to="/register" onClick={onClose}>
                    Register here
                </Link>
            </p>
        </S.ModalContentWrapper>
    );
};
export default Login;
