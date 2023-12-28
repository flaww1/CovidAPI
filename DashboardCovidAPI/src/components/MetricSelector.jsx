import React from 'react';
import * as S from './styles'; // Import your styled components

const MetricSelector = ({ selectedMetric, onMetricChange }) => {
    const metrics = {
        newCases: 'New Cases',
        testsDone: 'Tests Done',
        positivityRate: 'Positivity Rate',
        testingRate: 'Testing Rate'
    };

    return (
        <S.SelectorContainer>
            <div>
                <label>Select Metric:</label>
                <select value={selectedMetric} onChange={(e) => onMetricChange(e.target.value)}>
                    {Object.entries(metrics).map(([metricKey, metricLabel]) => (
                        <option key={metricKey} value={metricKey}>
                            {metricLabel}
                        </option>
                    ))}
                </select>
            </div>
        </S.SelectorContainer>
    );
};

export default MetricSelector;
