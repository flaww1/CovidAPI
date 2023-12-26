// AuthApiService.js

import axios from 'axios';

const BASE_URL = 'https://localhost:7197/api/auth';

async function register(user) {
    try {
        const response = await axios.post(`${BASE_URL}/register`, user);
        return response.data; // Depending on your API response format
    } catch (error) {
        console.error('Error registering user:', error);
        throw error;
    }
}

async function login(credentials) {
    try {
        const response = await axios.post(`${BASE_URL}/login`, credentials);
        return response.data; // Depending on your API response format
    } catch (error) {
        console.error('Error logging in:', error);
        throw error;
    }
}

export { register, login };
