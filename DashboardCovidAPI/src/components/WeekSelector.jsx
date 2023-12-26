// WeekSelector.jsx
import React from 'react';

const WeekSelector = ({ weeks, selectedWeek, onSelectWeek }) => {
    const handleWeekChange = (event) => {
        const selected = event.target.value;
        onSelectWeek(selected);
    };

    return (
        <div>
            <label htmlFor="weekSelector">Select Week:</label>
            <select id="weekSelector" value={selectedWeek} onChange={handleWeekChange}>
                {weeks.map((week) => (
                    <option key={week} value={week}>
                        {week}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default WeekSelector;
