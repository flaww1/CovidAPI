// LogoutButton.jsx
import React from 'react';
import { useUser } from './UserContext.jsx';
import { logout } from '../services/AuthApiService';
import * as S from './styles'; // Import your styled components

const LogoutButton = () => {
    const { setUser } = useUser();

    const handleLogout = async () => {
        try {
            await logout();
            // Clear the user information in the context
            setUser(null);
            // Handle any additional client-side logout logic (e.g., clearing local storage)
            console.log('Logout successful');
        } catch (error) {
            console.error('Logout failed:', error);
        }
    };

    return (

    <S.Button onClick={handleLogout}>
        Logout
    </S.Button>

    );
};

export default LogoutButton;
