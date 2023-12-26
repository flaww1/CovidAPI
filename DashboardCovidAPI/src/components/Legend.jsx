// Legend.jsx
import React from 'react';

const Legend = ({ selectedMetric }) => {
    const getColorRanges = (metric) => {
        // Define color ranges based on the selected metric
        const colorRanges = {
            newCases: ['green', 'yellow', 'orange', 'red'],
            testsDone: ['green', 'yellow', 'orange', 'red'],
            positivityRate: ['green', 'yellow', 'orange', 'red'],
            testingRate: ['green', 'yellow', 'orange', 'red'],
        };

        return colorRanges[metric] || [];
    };

    const renderLegendItems = (colorRanges) => {
        return colorRanges.map((color, index) => (
            <div key={index} style={{ display: 'flex', alignItems: 'center', margin: '4px' }}>
                <div
                    style={{
                        width: '20px',
                        height: '20px',
                        backgroundColor: color,
                        marginRight: '8px',
                    }}
                ></div>
                <span>{index === 0 ? 'Low' : index === colorRanges.length - 1 ? 'High' : ''}</span>
            </div>
        ));
    };

    const colorRanges = getColorRanges(selectedMetric);

    return (
        <div>
            <h2>Legend</h2>
            {renderLegendItems(colorRanges)}
        </div>
    );
};

export default Legend;
