// Map.jsx
import React, { useState, useEffect } from 'react';
import { MapContainer, TileLayer } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import MarkerWithPopup from './MarkerWithPopup';
import Modal from 'react-modal';
import { addMarkerData } from '../services/ApiService';

import * as S from './styles'; // Import your styled components

Modal.setAppElement('#root');

const Map = ({ data, selectedWeek, selectedMetric, onUpdateData }) => {
    const europeCenter = [51.1657, 10.4515];
    const [markersData, setMarkersData] = useState(data);
    const [showAddModal, setShowAddModal] = useState(false);
    const [newMarkerData, setNewMarkerData] = useState({
        country: '',
        countryCode: '',
        year: new Date().getFullYear(), // Default to the current year
        week: selectedWeek, // Default to the selected week
        region: '',
        regionName: '',
        newCases: 0,
        testsDone: 0,
        population: 0,
        positivityRate: 0,
        testingRate: 0,
        testingDataSource: '',
    });

    useEffect(() => {
        setMarkersData(data);
    }, [data]);

    const filteredData = markersData.filter((entry) => entry[selectedMetric] > 0);

    const handleDelete = (markerId) => {
        // Implement the logic to delete the marker with markerId
        // Update your state or trigger any necessary actions
        setMarkersData((prevMarkersData) => prevMarkersData.filter((marker) => marker.id !== markerId));
    };

    const handleEdit = (markerId) => {
        // Implement the logic to edit the marker with markerId
        // Update your state or trigger any necessary actions
        console.log(`Editing marker with ID: ${markerId}`);
    };

    const handleUpdateData = (updatedData) => {
        // Update the marker data in your state
        // This function will be passed as a prop to MarkerWithPopup
        // and called from there when needed
        // Update your state or trigger any necessary actions
        setMarkersData((prevMarkersData) =>
            prevMarkersData.map((marker) => (marker.id === updatedData.id ? updatedData : marker))
        );
        onUpdateData(updatedData);
    };

    const handleAddMarker = async () => {
        try {
            const response = await addMarkerData(newMarkerData);

            if (response && response.status === 201) {
                const addedMarkerData = response.data;
                setMarkersData([...markersData, addedMarkerData]);
                onUpdateData(addedMarkerData);
                setNewMarkerData({
                    country: '',
                    countryCode: '',
                    year: new Date().getFullYear(),
                    week: selectedWeek,
                    region: '',
                    regionName: '',
                    newCases: 0,
                    testsDone: 0,
                    population: 0,
                    positivityRate: 0,
                    testingRate: 0,
                    testingDataSource: '',
                });
                setShowAddModal(false);
            }
        } catch (error) {
            console.error('Error adding marker data:', error);
        }
    };

    const handleOpenAddModal = () => {
        setShowAddModal(true);
    };

    const handleCloseAddModal = () => {
        setShowAddModal(false);
    };

    return (
        <div>
            <S.AddMarkerButton onClick={handleOpenAddModal}>Add Marker</S.AddMarkerButton>

            <S.MapContainerWrapper>
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
                            onDelete={() => handleDelete(entry.id)}
                            onEdit={() => handleEdit(entry.id)}
                            onUpdateData={handleUpdateData}
                        />
                    ))}
                </MapContainer>
            </S.MapContainerWrapper>

            <Modal isOpen={showAddModal} onRequestClose={handleCloseAddModal}>
                <S.ModalContentWrapper>
                    <S.ModalHeading>Add Marker</S.ModalHeading>
                    <form>
                        <S.ModalLabel>
                            Country:
                            <S.ModalInput
                                type="text"
                                value={newMarkerData.country}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, country: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Country Code:
                            <S.ModalInput
                                type="text"
                                value={newMarkerData.countryCode}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, countryCode: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Year:
                            <S.ModalInput
                                type="number"
                                value={newMarkerData.year}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, year: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Week:
                            <S.ModalInput
                                type="text"
                                value={newMarkerData.week}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, week: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Region:
                            <S.ModalInput
                                type="text"
                                value={newMarkerData.region}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, region: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Region Name:
                            <S.ModalInput
                                type="text"
                                value={newMarkerData.regionName}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, regionName: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            New Cases:
                            <S.ModalInput
                                type="number"
                                value={newMarkerData.newCases}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, newCases: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Tests Done:
                            <S.ModalInput
                                type="number"
                                value={newMarkerData.testsDone}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, testsDone: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Population:
                            <S.ModalInput
                                type="number"
                                value={newMarkerData.population}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, population: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Positivity Rate:
                            <S.ModalInput
                                type="number"
                                value={newMarkerData.positivityRate}
                                onChange={(e) =>
                                    setNewMarkerData({ ...newMarkerData, positivityRate: e.target.value })
                                }
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Testing Rate:
                            <S.ModalInput
                                type="number"
                                value={newMarkerData.testingRate}
                                onChange={(e) => setNewMarkerData({ ...newMarkerData, testingRate: e.target.value })}
                            />
                        </S.ModalLabel>
                        <S.ModalLabel>
                            Testing Data Source:
                            <S.ModalInput
                                type="text"
                                value={newMarkerData.testingDataSource}
                                onChange={(e) =>
                                    setNewMarkerData({ ...newMarkerData, testingDataSource: e.target.value })
                                }
                            />
                        </S.ModalLabel>

                        <S.ModalButtonWrapper>
                            <S.ModalCancelButton type="button" onClick={handleCloseAddModal}>
                                Cancel
                            </S.ModalCancelButton>
                            <S.ModalAddButton type="button" onClick={handleAddMarker}>
                                Add Marker
                            </S.ModalAddButton>
                        </S.ModalButtonWrapper>
                    </form>
                </S.ModalContentWrapper>
            </Modal>
        </div>
    );
};

export default Map;
