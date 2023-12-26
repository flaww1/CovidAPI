// Subtitle.jsx
import React from 'react';

const Subtitle = ({ selectedWeek, totalCases, totalTests, selectedMetric }) => {
    const getSubtitleText = (metric) => {
        switch (metric) {
            case 'newCases':
                return 'Total new cases for the week:';
            case 'testsDone':
                return 'Total tests conducted for the week:';
            case 'positivityRate':
                return 'Positivity rate for the week:';
            case 'testingRate':
                return 'Testing rate for the week:';
            default:
                return '';
        }
    };

    const subtitleText = getSubtitleText(selectedMetric);

    return (
        <div>
            <h2>Week {selectedWeek} Summary</h2>
            <p>{subtitleText} {selectedMetric === 'positivityRate' ? `${(totalCases / totalTests * 100).toFixed(2)}%` : totalCases}</p>
            {/* You may customize the text based on your needs */}
        </div>
    );
};

export default Subtitle;
