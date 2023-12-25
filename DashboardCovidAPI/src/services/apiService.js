// ApiService.js
import axios from 'axios';

const BASE_URL = 'https://localhost:7197/api/coviddata';

async function getWeeks() {
    try {
        const response = await axios.get(`${BASE_URL}/weeks`);
        return response.data;
    } catch (error) {
        console.error('Error fetching weeks:', error);
        throw error;
    }
}

async function getWeekData(week) {
    try {
        const response = await axios.get(`${BASE_URL}/week/${week}`);
        return response.data;
    } catch (error) {
        console.error(`Error fetching data for week ${week}:`, error);
        throw error;
    }
}

export { getWeeks, getWeekData };
