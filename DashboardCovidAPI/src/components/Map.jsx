// Map.jsx
import React from 'react';
import { MapContainer, TileLayer } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import MarkerWithPopup from './MarkerWithPopup';
import LineChart from './LineChart';
import Subtitle from './Subtitle';
import Legend from './Legend';

const Map = ({ data, selectedWeek, selectedMetric }) => {
    console.log('Data in Map component:', data);

    const europeCenter = [51.1657, 10.4515]; // Center coordinates for Europe

    const filteredData = data.filter((entry) => entry[selectedMetric] > 0);

    console.log('Filtered Data in Map component:', filteredData); // Add this line


    // Calculate total cases and total tests for the selected week
    const totalCases = filteredData.reduce((sum, entry) => sum + entry[selectedMetric], 0);
    const totalTests = filteredData.reduce((sum, entry) => sum + entry.testsDone, 0);

    return (
        <div>
            <MapContainer center={europeCenter} zoom={5} style={{ height: '500px', width: '100%' }}>
                <TileLayer
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    attribution='Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap contributors</a>'
                />
                {filteredData.map((entry) => (
                    <MarkerWithPopup
                        key={entry.id}
                        position={[entry.geometry.lat, entry.geometry.lng]}
                        data={entry}
                        selectedMetric={selectedMetric}
                    />
                ))}
            </MapContainer>
            <Legend selectedMetric={selectedMetric} />
            <LineChart data={filteredData} selectedMetric={selectedMetric} />
        </div>
    );
};

export default Map;
