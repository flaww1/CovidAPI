import React, { useState, useEffect } from 'react';
import { Routes, Route, Outlet } from 'react-router-dom';

import Map from './Map';
import { getWeeks, getWeekData, getCountryData, getAllCountries } from '../services/ApiService';
import MetricSelector from './MetricSelector';
import CustomLineChart from './CustomLineChart';
import WeekSelector from './WeekSelector';
import Subtitle from './Subtitle';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import LogoutButton from './LogoutButton';
import { useUser, UserProvider } from './UserContext.jsx';
import CountrySelector from './CountrySelector';

import * as S from './styles'; // Import your styled components

const Dashboard = () => {
    const [covidData, setCovidData] = useState([]);
    const [countryCovidData, setCountryCovidData] = useState([]); // New state for country-specific data
    const [allWeeks, setAllWeeks] = useState([]);
    const [selectedWeek, setSelectedWeek] = useState('W01');
    const [selectedMetric, setSelectedMetric] = useState('newCases');
    const { user, setUser } = useUser();
    const [showLogin, setShowLogin] = useState(false);
    const [showRegister, setShowRegister] = useState(false);

    const [countries, setCountries] = useState([]);
    const [selectedCountry, setSelectedCountry] = useState('');

    const [markersData, setMarkersData] = useState([]);
    const localStorageKey = 'markersData';

    useEffect(() => {
        const storedData = localStorage.getItem(localStorageKey);

        if (storedData) {
            setMarkersData(JSON.parse(storedData));
            setCovidData(JSON.parse(storedData));
        } else {
            fetchData();
        }

        const storedUser = localStorage.getItem('user');

        if (storedUser) {
            setUser(JSON.parse(storedUser));
        }
    }, [setUser]);

    const fetchData = async () => {
        try {
            const weeks = await getWeeks();
            const sortedWeeks = weeks.sort();

            const data = await getWeekData(selectedWeek);

            // Filter out duplicate weeks from the data
            const uniqueData = data.filter((entry, index, self) =>
                index === self.findIndex((e) => e.week === entry.week)
            );

            setAllWeeks(sortedWeeks);
            setMarkersData(uniqueData);
            setCovidData(uniqueData);

            localStorage.setItem(localStorageKey, JSON.stringify(uniqueData));
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };


    useEffect(() => {
        const fetchCountries = async () => {
            try {
                const countryList = await getAllCountries();
                setCountries(countryList);
                if (countryList.length > 0) {
                    setSelectedCountry(countryList[0]);
                }
            } catch (error) {
                console.error('Error fetching countries:', error);
            }
        };

        fetchCountries();
    }, []);

    const handleCountryChange = async (country) => {
        if (country) {
            setSelectedCountry(country);

            try {
                const countryMetrics = await getCountryData(country);
                setCountryCovidData(countryMetrics); // Store country-specific data in the new state
            } catch (error) {
                console.error('Error fetching data for the country:', error);
            }
        }
    };

    const handleUpdateData = (updatedData) => {
        setMarkersData((prevMarkersData) =>
            prevMarkersData.map((marker) => (marker.id === updatedData.id ? updatedData : marker))
        );

        setCovidData((prevCovidData) =>
            prevCovidData.map((entry) => (entry.id === updatedData.id ? updatedData : entry))
        );

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
        localStorage.setItem('user', JSON.stringify(userData));
    };

    const handleLogout = () => {
        setUser(null);
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
                        <S.Button onClick={openLoginModal}>Login</S.Button>
                        <S.Button onClick={openRegisterModal}>Register</S.Button>
                        {showLogin && <LoginPage onLogin={handleLogin} onClose={closeLoginModal} />}
                        {showRegister && <RegisterPage onClose={closeRegisterModal} />}
                    </div>
                )}
                {user && (
                    <div>
                        <h1>COVID-19 Dashboard</h1>
                        <Outlet />
                        <WeekSelector weeks={allWeeks} selectedWeek={selectedWeek} onSelectWeek={handleWeekChange} />
                        <Subtitle selectedWeek={selectedWeek} data={covidData} />


                        <Map data={covidData} selectedWeek={selectedWeek} selectedMetric={selectedMetric} onUpdateData={handleUpdateData} />
                        <CountrySelector onSelectCountry={handleCountryChange} />
                        <MetricSelector selectedMetric={selectedMetric} onMetricChange={handleMetricChange} />

                        <CustomLineChart data={countryCovidData} selectedMetric={selectedMetric} />
                    </div>
                )}
            </div>
        </S.DashboardContainer>
    );
};

export default Dashboard;
