import React, { useEffect, useState } from 'react';
import Login from './Login';
import Register from './Register';
import Map from './Map';
import { getWeeks, getWeekData } from '../services/ApiService';
import MetricSelector from './MetricSelector';
import LineChart from './LineChart';
import WeekSelector from './WeekSelector';
import Subtitle from './Subtitle';

const Dashboard = () => {
    const [covidData, setCovidData] = useState([]);
    const [allWeeks, setAllWeeks] = useState([]);
    const [selectedWeek, setSelectedWeek] = useState('W01');
    const [selectedMetric, setSelectedMetric] = useState('newCases');
    const [user, setUser] = useState(null);
    const [showLoginModal, setShowLoginModal] = useState(false);
    const [showRegisterModal, setShowRegisterModal] = useState(false);

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

    const handleLogin = async (credentials) => {
        // Call your backend API for login
        try {
            // Simulate a successful login for now
            setUser({ username: credentials.username });
        } catch (error) {
            console.error('Login failed:', error);
        }
    };

    const handleRegister = async (credentials) => {
        // Call your backend API for registration
        try {
            // Simulate a successful registration and login for now
            setUser({ username: credentials.username });
        } catch (error) {
            console.error('Registration failed:', error);
        }
    };

    const handleLogout = () => {
        // Handle logout logic, clear user data, etc.
        setUser(null);
    };

    const handleLoginClose = () => setShowLoginModal(false);
    const handleRegisterClose = () => setShowRegisterModal(false);

    return (
        <div>
            {user ? (
                <div>
                    <h1>Welcome, {user.username}!</h1>
                    <button onClick={handleLogout}>Logout</button>
                </div>
            ) : (
                <div>
                    <button onClick={() => setShowLoginModal(true)}>Login</button>
                    <button onClick={() => setShowRegisterModal(true)}>Register</button>
                    {showLoginModal && <Login onClose={handleLoginClose} onLogin={handleLogin} />}
                    {showRegisterModal && <Register onClose={handleRegisterClose} onRegister={handleRegister} />}
                </div>
            )}
            <h1>COVID-19 Dashboard</h1>
            <MetricSelector selectedMetric={selectedMetric} onMetricChange={handleMetricChange} />
            <WeekSelector weeks={allWeeks} selectedWeek={selectedWeek} onSelectWeek={handleWeekChange} />
            <Subtitle
                selectedWeek={selectedWeek}
                totalCases={covidData.reduce((sum, entry) => sum + entry[selectedMetric], 0)}
                totalTests={covidData.reduce((sum, entry) => sum + entry.testsDone, 0)}
                selectedMetric={selectedMetric}
            />
            <Map data={covidData} selectedWeek={selectedWeek} selectedMetric={selectedMetric} />
            <LineChart data={covidData} selectedMetric={selectedMetric} />
        </div>
    );
};

export default Dashboard;
