// MarkerWithPopup.jsx
// MarkerWithPopup.jsx

import React, { useState } from 'react';
import { Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import Modal from 'react-modal'; // Import the modal library
import './MarkerWithPopup.css';

const MarkerWithPopup = ({ data, selectedMetric, onAdd, onEdit, onDelete }) => {
    const [showEditModal, setShowEditModal] = useState(false);
    const [showAddModal, setShowAddModal] = useState(false);
    console.log('Data in MarkerWithPopup component:', data);

    const getMarkerColor = () => {
        if (!Array.isArray(data)) {
            // Handle the case where data is not an array
            return 'red'; // Replace with an appropriate default color
        }

        const value = data[selectedMetric];

        // Define severity thresholds based on your requirements
        const severityThresholds = {
            low: 1000,
            medium: 5000,
            high: 10000,
        };

        // Determine the severity level
        const getSeverityLevel = (metricValue) => {
            console.log('Metric Value:', metricValue);

            let severityLevel;

            if (metricValue < severityThresholds.low) {
                severityLevel = 'low';
            } else if (metricValue < severityThresholds.medium) {
                severityLevel = 'medium';
            } else {
                severityLevel = 'high';
            }

            console.log('Severity Level:', severityLevel);

            return severityLevel;
        };

        const severityLevel = getSeverityLevel(value);

        // Map severity level to marker color
        const severityColors = {
            low: 'green',
            medium: 'yellow',
            high: 'red',
        };

        const markerColor = severityColors[severityLevel] || 'defaultColor';

        return markerColor;
    };

    const markerColor = getMarkerColor();

    const handleDelete = () => {
        // Handle delete logic, you might want to call an API to delete the marker
        onDelete(data.id);
    };

    const handleEdit = () => {
        // Handle edit logic, you might want to open a modal or navigate to an edit page
        onEdit(data.id);
    };

    return (
        <Marker
            position={[data.geometry.lat, data.geometry.lng]}
            icon={L.divIcon({
                className: `custom-marker custom-marker-${markerColor}`,
                html: '',
            })}
        >
            <Popup>
                <strong>{data.country}</strong>
                <br />
                Week: {data.week}
                <br />
                New Cases: {data.newCases.toLocaleString()} {/* Format numbers */}
                <br />
                Tests Done: {data.testsDone.toLocaleString()} {/* Format numbers */}
                <br />
                Positivity Rate: {data.positivityRate.toFixed(2)}% {/* Format as percentage with two decimal places */}
                <br />
                Testing Rate: {data.testingRate.toFixed(2)} {/* Format testing rate with two decimal places */}
                <br />
                New Cases Per Capita: {data.perCapitaCases.toLocaleString()}
                <br />
                Tests Done Per Capita: {data.perCapitaTests.toLocaleString()}
                <br />
                Total Cases for the Year: {data.totalCasesYear.toLocaleString()}
                <br />
                Total Tests for the Year: {data.totalTestsYear.toLocaleString()}
                <br />
                <button onClick={handleEdit}>Edit</button>
                <button onClick={handleDelete}>Delete</button>
            </Popup>
        </Marker>
    );
};

export default MarkerWithPopup;
