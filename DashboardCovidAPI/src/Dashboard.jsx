import React, { useEffect, useState } from 'react';
import Map from './Map';

const Dashboard = () => {
    const [covidData, setCovidData] = useState([]);
    const [filter, setFilter] = useState(0);
    const [allWeeks, setAllWeeks] = useState([]);
    const [selectedWeek, setSelectedWeek] = useState('');

    // Dummy data for testing
    const dummyData = [
        { Country: 'A', NewCases: 100, Geometry: { Lat: 50, Lng: 10 } },
        { Country: 'B', NewCases: 50, Geometry: { Lat: 51, Lng: 9 } },
        // Add more dummy entries as needed
    ];

    useEffect(() => {
        // Fetch all weeks when the component mounts
        // For now, let's use a hardcoded list of weeks
        const dummyWeeks = ['W41', 'W42', 'W43'];
        setAllWeeks(dummyWeeks);

        // Fetch data for the selected week (initially, fetch data for the first week)
        if (dummyWeeks.length > 0) {
            setSelectedWeek(dummyWeeks[0]);
            setCovidData(dummyData);
        }
    }, []); // Empty dependency array to run the effect once on mount

    const handleFilterChange = (event) => {
        setFilter(event.target.value);
    };

    const handleWeekChange = (event) => {
        const selected = event.target.value;
        setSelectedWeek(selected);
        // Fetch data for the selected week (replace with actual API call)
        setCovidData(dummyData);
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

            <Map data={covidData} filter={filter} />
        </div>
    );
};

export default Dashboard;
