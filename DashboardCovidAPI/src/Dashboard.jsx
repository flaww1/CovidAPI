import React, { useEffect, useState } from 'react';
import Map from './Map';

const Dashboard = () => {
    const [covidData, setCovidData] = useState([]);
    const [filter, setFilter] = useState(0);

    useEffect(() => {
        fetch('YOUR_API_URL/api/coviddata')
            .then((response) => response.json())
            .then((data) => setCovidData(data))
            .catch((error) => console.error('Error fetching data:', error));
    }, []);

    const handleFilterChange = (event) => {
        setFilter(event.target.value);
    };

    return (
        <div>
            <h1>COVID-19 Dashboard</h1>
            <label>
                Show only countries with more than
                <input type="number" value={filter} onChange={handleFilterChange} />
                new cases
            </label>
            <Map data={covidData} filter={filter} />
        </div>
    );
};

export default Dashboard;
