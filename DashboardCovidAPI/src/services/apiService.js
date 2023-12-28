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

async function updateMarkerData(markerId, updatedData) {
    try {
        const response = await axios.put(`${BASE_URL}/${markerId}`, updatedData);
        if (response.status === 200) {
            return true; // Update successful
        }
        return false; // Update failed
    } catch (error) {
        console.error(`Error updating marker data for ID ${markerId}:`, error);
        throw error;
    }
}

async function deleteMarkerData(markerId) {
    try {
        const response = await axios.delete(`${BASE_URL}/${markerId}`);
        if (response.status === 200) {
            return true; // Deletion successful
        }
        return false; // Deletion failed
    } catch (error) {
        console.error(`Error deleting marker data for ID ${markerId}:`, error);
        throw error;
    }
}

async function fetchDataForMarkerId(markerId) {
    try {
        const response = await axios.get(`${BASE_URL}/${markerId}`);
        return response.data;
    } catch (error) {
        console.error(`Error fetching data for marker ${markerId}:`, error);
        throw error;
    }
}
async function addMarkerData(newMarkerData) {
    try {
        const response = await axios.post(`${BASE_URL}`, newMarkerData);
        return response;
    } catch (error) {
        console.error('Error adding marker data:', error);
        throw error;
    }
}

export { getWeeks, getWeekData, updateMarkerData, deleteMarkerData, fetchDataForMarkerId, addMarkerData };
