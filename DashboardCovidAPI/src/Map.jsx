import React, { useEffect, useState, useRef } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';

const Map = ({ data, filter }) => {
    const europeCenter = [51.1657, 10.4515]; // Center coordinates for Europe

    const filteredData = data.filter((entry) => entry.NewCases > filter);

    return (
        <MapContainer center={europeCenter} zoom={5} style={{ height: '500px', width: '100%' }}>
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap contributors</a>'
            />
            {filteredData.map((entry) => (
                <Marker position={[entry.Geometry.Lat, entry.Geometry.Lng]} key={entry.Id}>
                    <Popup>
                        <strong>{entry.Country}</strong>
                        <br />
                        New Cases: {entry.NewCases}
                    </Popup>
                </Marker>
            ))}
        </MapContainer>
    );
};

export default Map;
