import React from 'react';
import Login from './Login';

const LoginPage = ({ onLogin, onClose }) => {
    return (
        <div className="modal">
            <Login onLogin={onLogin} onClose={onClose} />
        </div>
    );
};

export default LoginPage;
