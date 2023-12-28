import React from 'react';
import Register from './Register';

const RegisterPage = ({ onRegister, onClose }) => {
    return (
        <div className="modal">
            <Register onRegister={onRegister} onClose={onClose} />
        </div>
    );
};

export default RegisterPage;
