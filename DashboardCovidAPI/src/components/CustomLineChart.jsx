import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, Label } from 'recharts';

const CustomLineChart = ({ data, selectedMetric }) => {
    return (
        <div>
            <h2>{selectedMetric.toUpperCase()} History Chart</h2>
            <LineChart width={1200} height={600} data={data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="week" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey={selectedMetric} stroke="rgba(75, 192, 192, 1)" />
                <Label
                    valueKey={selectedMetric}
                    position="top"
                    offset={5}
                    content={({ value, x, y }) => (
                        <text x={x} y={y} fill="black" textAnchor="middle">{value}</text>
                    )}
                />
            </LineChart>
        </div>
    );
};

export default CustomLineChart;
