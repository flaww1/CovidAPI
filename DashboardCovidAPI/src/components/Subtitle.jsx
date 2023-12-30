import React from 'react';

const Subtitle = ({ selectedWeek, data }) => {
    const getSubtitleText = () => {
        return `Week ${selectedWeek} Summary:`;
    };

    const calculateAverages = () => {
        // Filter data for the selected week
        const weekData = data.filter((entry) => entry.week === selectedWeek);

        if (weekData.length === 0) {
            return {
                totalCases: 0,
                totalTests: 0,
                positivityRate: 'N/A',
                testingRate: 'N/A',
            };
        }

        // Sum up new cases and tests done for the week
        const totalCases = weekData.reduce((sum, entry) => sum + entry.newCases, 0);
        const totalTests = weekData.reduce((sum, entry) => sum + entry.testsDone, 0);

        // Calculate average positivity rate and testing rate
        const positivityRate =
            weekData.reduce((sum, entry) => sum + (entry.newCases / entry.testsDone) * 100, 0) / weekData.length;

        const testingRate = weekData.reduce((sum, entry) => sum + entry.testsDone / entry.population, 0) / weekData.length;

        return {
            totalCases,
            totalTests,
            positivityRate: isNaN(positivityRate) ? 'N/A' : `${positivityRate.toFixed(2)}%`,
            testingRate: isNaN(testingRate) ? 'N/A' : testingRate.toFixed(2),
        };
    };

    const averages = calculateAverages();
    const subtitleText = getSubtitleText();

    return (
        <div>
            <h2>{subtitleText}</h2>
            <p>
                Total new cases: {averages.totalCases} cases, Total tests conducted: {averages.totalTests} tests
            </p>
            <p>
                Average Positivity Rate: {averages.positivityRate}, Average Testing Rate: {averages.testingRate}
            </p>
        </div>
    );
};

export default Subtitle;
