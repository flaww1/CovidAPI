import React from 'react';
import { MapContainer, TileLayer } from 'react-leaflet';
import CountryMarker from './CountryMarker';

function Map({ countries }) {
    const europeCenter = [51.1657, 10.4515]; // Center coordinates for Europe

    // Add a log statement to check the structure of 'countries'
    console.log('Countries:', countries);

    return (
        <MapContainer center={europeCenter} zoom={5} style={{ height: '500px', width: '100%' }}>
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap contributors</a>'
            />
            {countries.map((country) => (
                <CountryMarker key={country.id} country={country} />
            ))}
        </MapContainer>
    );
}

export default Map;
