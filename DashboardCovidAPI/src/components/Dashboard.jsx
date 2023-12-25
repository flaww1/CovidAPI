import React, { useEffect, useState } from 'react';
import Map from './Map';
import { getWeeks, getWeekData } from '../services/ApiService';

const Dashboard = () => {
    const [covidData, setCovidData] = useState([]);
    const [allWeeks, setAllWeeks] = useState([]);
    const [selectedWeek, setSelectedWeek] = useState('W01');

    useEffect(() => {
        const fetchData = async () => {
            try {
                const weeks = await getWeeks();
                const sortedWeeks = weeks.sort();
                setAllWeeks(sortedWeeks);

                const defaultData = await getWeekData('W01');
                setCovidData(defaultData);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchData();
    }, []);

    const handleWeekChange = async (event) => {
        const selected = event.target.value;
        setSelectedWeek(selected);

        try {
            const data = await getWeekData(selected);
            setCovidData(data);
        } catch (error) {
            console.error('Error fetching data for the selected week:', error);
        }
    };

    return (
        <div>
            <h1>COVID-19 Dashboard</h1>
            <select value={selectedWeek} onChange={handleWeekChange}>
                {allWeeks.map((week) => (
                    <option key={week} value={week}>
                        {week}
                    </option>
                ))}
            </select>
            <Map data={covidData} />
        </div>
    );
};

export default Dashboard;
