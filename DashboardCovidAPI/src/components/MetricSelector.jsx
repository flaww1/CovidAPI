// MetricSelector.jsx
import React from 'react';

const MetricSelector = ({ selectedMetric, onMetricChange }) => {
    const metrics = ['newCases', 'testsDone', 'positivityRate', 'testingRate']; // Add more metrics as needed

    return (
        <div>
            <label>Select Metric:</label>
            <select value={selectedMetric} onChange={(e) => onMetricChange(e.target.value)}>
                {metrics.map((metric) => (
                    <option key={metric} value={metric}>
                        {metric}
                    </option>
                ))}
            </select>
        </div>
    );
};


export default MetricSelector;
