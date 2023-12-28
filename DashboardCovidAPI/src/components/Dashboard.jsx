// Dashboard.jsx
import React, { useEffect, useState } from 'react';
import { Routes, Route, Outlet } from 'react-router-dom';

import Map from './Map';
import { getWeeks, getWeekData } from '../services/ApiService';
import MetricSelector from './MetricSelector';
import LineChart from './LineChart';
import WeekSelector from './WeekSelector';
import Subtitle from './Subtitle';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import LogoutButton from './LogoutButton';
import { useUser, UserProvider } from './UserContext.jsx';

import * as S from './styles'; // Import your styled components


const Dashboard = () => {
    const [covidData, setCovidData] = useState([]);
    const [allWeeks, setAllWeeks] = useState([]);
    const [selectedWeek, setSelectedWeek] = useState('W01');
    const [selectedMetric, setSelectedMetric] = useState('newCases');
    const { user, setUser } = useUser();
    const [showLogin, setShowLogin] = useState(false);
    const [showRegister, setShowRegister] = useState(false);

    const [markersData, setMarkersData] = useState([]);
    const localStorageKey = 'markersData';

    useEffect(() => {
        // Check if there's data in localStorage
        const storedData = localStorage.getItem(localStorageKey);

        if (storedData) {
            setMarkersData(JSON.parse(storedData));
            setCovidData(JSON.parse(storedData));
        } else {
            fetchData(); // Fetch data from the server if not in localStorage
        }

        // Check if there's a user in localStorage
        const storedUser = localStorage.getItem('user');

        if (storedUser) {
            // If there is, set the user state
            setUser(JSON.parse(storedUser));
        }
    }, [setUser]); // Run this effect on mount and when setUser changes

    const fetchData = async () => {
        try {
            const weeks = await getWeeks();
            const sortedWeeks = weeks.sort();
            setAllWeeks(sortedWeeks);

            const data = await getWeekData(selectedWeek);
            setMarkersData(data);
            setCovidData(data);

            // Save the data in localStorage
            localStorage.setItem(localStorageKey, JSON.stringify(data));
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    const handleUpdateData = (updatedData) => {
        // Update the marker data in your state
        setMarkersData((prevMarkersData) =>
            prevMarkersData.map((marker) => (marker.id === updatedData.id ? updatedData : marker))
        );

        // Update covidData with the same data
        setCovidData((prevCovidData) =>
            prevCovidData.map((entry) => (entry.id === updatedData.id ? updatedData : entry))
        );

        // Update localStorage with the updated markersData
        localStorage.setItem(localStorageKey, JSON.stringify(markersData));
    };

    const openLoginModal = () => {
        setShowLogin(true);
    };

    const closeLoginModal = () => {
        setShowLogin(false);
    };

    const openRegisterModal = () => {
        setShowRegister(true);
    };

    const closeRegisterModal = () => {
        setShowRegister(false);
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const weeks = await getWeeks();
                const sortedWeeks = weeks.sort();
                setAllWeeks(sortedWeeks);

                const data = await getWeekData(selectedWeek);
                setCovidData(data);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();
    }, [selectedWeek]);

    const handleWeekChange = async (week) => {
        setSelectedWeek(week);
    };

    const handleMetricChange = (metric) => {
        setSelectedMetric(metric);
    };

    const handleLogin = (userData) => {
        setUser(userData);
        // Save user information in localStorage
        localStorage.setItem('user', JSON.stringify(userData));
    };

    const handleLogout = () => {
        setUser(null);
        // Remove user information from localStorage on logout
        localStorage.removeItem('user');
    };

    return (
        <S.DashboardContainer>
            <div>
                {user ? (
                    <div>
                        <h1>Welcome!</h1>
                        <LogoutButton />
                    </div>
                ) : (
                    <div>
                        <button onClick={openLoginModal}>Login</button>
                        <button onClick={openRegisterModal}>Register</button>
                        {showLogin && <LoginPage onLogin={handleLogin} onClose={closeLoginModal} />}
                        {showRegister && <RegisterPage onClose={closeRegisterModal} />}
                    </div>
                )}
                {user && (
                    <div>
                        <h1>COVID-19 Dashboard</h1>
                        <Outlet />
                        <MetricSelector selectedMetric={selectedMetric} onMetricChange={handleMetricChange} />
                        <WeekSelector weeks={allWeeks} selectedWeek={selectedWeek} onSelectWeek={handleWeekChange} />
                        <Subtitle
                            selectedWeek={selectedWeek}
                            totalCases={covidData.reduce((sum, entry) => sum + entry[selectedMetric], 0)}
                            totalTests={covidData.reduce((sum, entry) => sum + entry.testsDone, 0)}
                            selectedMetric={selectedMetric}
                        />
                        <Map data={covidData} selectedWeek={selectedWeek} selectedMetric={selectedMetric} onUpdateData={handleUpdateData} />
                        <LineChart data={covidData} selectedMetric={selectedMetric} />
                    </div>
                )}
            </div>
        </S.DashboardContainer>
    );
};

export default Dashboard;
