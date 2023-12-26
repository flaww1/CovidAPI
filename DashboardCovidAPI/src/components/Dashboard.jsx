import React, { useEffect, useState } from 'react';
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

    return (
        <div>
            <h1>COVID-19 Dashboard</h1>
            <MetricSelector selectedMetric={selectedMetric} onMetricChange={handleMetricChange} />
            <WeekSelector weeks={allWeeks} selectedWeek={selectedWeek} onSelectWeek={handleWeekChange} />
            {/* Add Subtitle component */}
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
