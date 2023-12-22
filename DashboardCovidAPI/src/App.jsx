// src/App.jsx
import React, { useEffect, useState } from 'react';
import Map from './Map';

const App = () => {
    const [covidData, setCovidData] = useState([]);
    const [count, setCount] = useState(0);

    useEffect(() => {
        // Fetch COVID-19 data from your API endpoint
        const apiUrl = import.meta.env.REACT_APP_API_URL;
        const apiKey = import.meta.env.REACT_APP_API_KEY;

        if (!apiUrl || !apiKey) {
            console.error('API URL or API key is missing.');
            return;
        }

        fetch(`${apiUrl}/api/coviddata?key=${apiKey}`)
            .then((response) => response.json())
            .then((data) => setCovidData(data))
            .catch((error) => console.error('Error fetching data:', error));
    }, []);

    return (
        <div>
            <h1>COVID-19 Dashboard</h1>
            <Map data={covidData} />

            <div className="counter">
                <button onClick={() => setCount((count) => count + 1)}>
                    Increment Counter
                </button>
                <p>Count is {count}</p>
            </div>
        </div>
    );
};

export default App;
