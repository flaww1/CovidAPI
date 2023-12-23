import React, { useEffect, useState, useRef } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet-draw/dist/leaflet.draw.css';
import 'leaflet-draw';

const DrawControl = ({ drawnItems }) => {
    const map = useMap();

    useEffect(() => {
        const drawControl = new L.Control.Draw({
            edit: {
                featureGroup: drawnItems,
                poly: {
                    allowIntersection: false,
                },
            },
            draw: {
                marker: false, // Disable marker drawing
            },
        });
        map.addControl(drawControl);
        map.on(L.Draw.Event.CREATED, (event) => {
            const { layer } = event;
            drawnItems.addLayer(layer);
            const coordinates = layer.getLatLng();
            console.log('Drawn Coordinates:', coordinates);
        });
        map.on(L.Draw.Event.EDITED, (event) => {
            const { layers } = event;
            layers.eachLayer((layer) => {
                const editedCoordinates = layer.getLatLng();
                console.log('Edited Coordinates:', editedCoordinates);
            });
        });

        return () => {
            map.removeControl(drawControl);
        };
    }, [map, drawnItems]);

    return null;
};

const Map = ({ data, filter }) => {
    const defaultCenter = [0, 0];
    const drawnItems = useRef(new L.FeatureGroup());

    const filteredData = data.filter(entry => entry.NewCases > filter);

    return (
        <MapContainer center={defaultCenter} zoom={2} style={{ height: '500px', width: '100%' }}>
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap contributors</a>'
            />
            {filteredData.map((entry) => (
                <Marker position={[entry.Geolocation.Latitude, entry.Geolocation.Longitude]} key={entry.Id}>
                    <Popup>
                        <strong>{entry.Country}</strong>
                        <br />
                        New Cases: {entry.NewCases}
                    </Popup>
                </Marker>
            ))}
            <DrawControl drawnItems={drawnItems.current} />
        </MapContainer>
    );
};

export default Map;
