// MetricSelector.jsx
import React from 'react';
import * as S from './styles'; // Import your styled components

const MetricSelector = ({ selectedMetric, onMetricChange }) => {
    const metrics = ['New Cases', 'Tests Done', 'Positivity Rate', 'Testing Rate']; // Add more metrics as needed

    return (
        <S.WeekSelectorContainer>

        <div>
            <label>Select Metric:</label>
            <select value={selectedMetric} onChange={(e) => {
                console.log(`Selected metric changed to ${e.target.value}`);
                onMetricChange(e.target.value);
            }}>
                {metrics.map((metric) => (
                    <option key={metric} value={metric}>
                        {metric}
                    </option>
                ))}
            </select>
        </div>
        </S.WeekSelectorContainer>
    );
};


export default MetricSelector;
