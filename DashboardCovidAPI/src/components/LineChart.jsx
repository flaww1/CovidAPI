import React, { useEffect, useRef } from 'react';
import Chart from 'chart.js/auto';

const LineChart = ({ data, selectedMetric }) => {
    const chartRef = useRef(null);

    useEffect(() => {
        if (!chartRef.current || !data.length) return;

        const ctx = chartRef.current.getContext('2d');

        if (chartRef.current.chart) {
            chartRef.current.chart.destroy();
        }

        const uniqueWeeks = [...new Set(data.map(entry => entry.week))];
        const datasets = [];

        const metricData = {
            label: selectedMetric,
            data: uniqueWeeks.map((week, index) => {
                const weekEntries = data.filter(entry => entry.week === week);
                const totalMetric = weekEntries.reduce((sum, entry) => sum + entry[selectedMetric], 0);
                return index > 0 ? totalMetric + metricData.data[index - 1] : totalMetric;
            }),
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1,
            fill: false,
        };

        datasets.push(metricData);

        chartRef.current.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: uniqueWeeks,
                datasets,
            },
            options: {
                scales: {
                    x: {
                        type: 'linear',
                        position: 'bottom',
                    },
                    y: {
                        min: 0,
                    },
                },
            },
        });
    }, [data, selectedMetric]);

    return (
        <div>
            <h2>{selectedMetric} History Chart</h2>
            <canvas ref={chartRef} width="400" height="200"></canvas>
        </div>
    );
};

export default LineChart;
