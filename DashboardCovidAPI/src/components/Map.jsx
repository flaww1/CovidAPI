import React from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';

const MarkerWithPopup = ({ position, data }) => {
    return (
        <Marker position={position}>
            <Popup>
                <strong>{data.country}</strong>
                <br />
                New Cases: {data.newCases}
                <br />
                Week: {data.week}
            </Popup>
        </Marker>
    );
};

const Map = ({ data }) => {
    const europeCenter = [51.1657, 10.4515]; // Center coordinates for Europe

    const filteredData = data.filter((entry) => entry.newCases > 0); // Adjust filter criteria if needed

    return (
        <MapContainer center={europeCenter} zoom={5} style={{ height: '500px', width: '100%' }}>
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap contributors</a>'
            />
            {filteredData.map((entry) => (
                <MarkerWithPopup position={[entry.geometry.lat, entry.geometry.lng]} data={entry} key={entry.id} />
            ))}
        </MapContainer>
    );
};

export default Map;
