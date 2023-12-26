import React, { useEffect, useRef } from 'react';
import Chart from 'chart.js/auto';

const LineChart = ({ data, selectedMetric }) => {
    const chartRef = useRef(null);

    useEffect(() => {
        if (!chartRef.current) return;

        const ctx = chartRef.current.getContext('2d');

        // Create the line chart
        const lineChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(entry => entry.week),
                datasets: [
                    {
                        label: selectedMetric, // Use selected metric as the label
                        data: data.map(entry => entry[selectedMetric]), // Use selected metric data
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        fill: false,
                    },
                ],
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

        // Cleanup the chart on component unmount
        return () => {
            lineChart.destroy();
        };
    }, [data, selectedMetric]);

    return (
        <div>
            <h2>{selectedMetric} Chart</h2>
            <canvas ref={chartRef} width="400" height="200"></canvas>
        </div>
    );
};

export default LineChart;
