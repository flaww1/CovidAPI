import React, { useState } from 'react';
import { Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import Modal from 'react-modal';
import { updateMarkerData, deleteMarkerData, fetchDataForMarkerId } from '../services/ApiService';
import * as S from './styles';
import './MarkerWithPopup.css';


// Manually include the default icon images
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

let DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow
});

L.Marker.prototype.options.icon = DefaultIcon;

Modal.setAppElement('#root');

const MarkerWithPopup = ({ data, selectedMetric, onEdit, onDelete, onUpdateData }) => {
    const [showEditModal, setShowEditModal] = useState(false);
    const [editedData, setEditedData] = useState({ ...data });

    const getMarkerColor = () => {
        if (!Array.isArray(data)) {
            return 'red';
        }

        const value = data[selectedMetric];
        const severityThresholds = {
            low: 1000,
            medium: 5000,
            high: 10000,
        };

        const getSeverityLevel = (metricValue) => {
            let severityLevel;

            if (metricValue < severityThresholds.low) {
                severityLevel = 'low';
            } else if (metricValue < severityThresholds.medium) {
                severityLevel = 'medium';
            } else {
                severityLevel = 'high';
            }

            return severityLevel;
        };

        const severityLevel = getSeverityLevel(value);
        const severityColors = {
            low: 'green',
            medium: 'yellow',
            high: 'red',
        };

        return severityColors[severityLevel] || 'defaultColor';
    };

    const markerColor = getMarkerColor();

    const handleDelete = async () => {
        try {
            const success = await deleteMarkerData(data.id);

            if (success) {
                onDelete(data.id);
            }
        } catch (error) {
            console.error('Error deleting marker data:', error);
        }
    };

    const handleEdit = () => {
        setShowEditModal(true);
        setEditedData(data);
    };

    const handleEditSubmit = async () => {
        try {
            const success = await updateMarkerData(editedData.id, editedData);

            if (success) {
                setShowEditModal(false);
                onEditSuccess();
            }
        } catch (error) {
            console.error('Error updating marker data:', error);
        }
    };

    const onEditSuccess = async () => {
        try {
            const updatedData = await fetchDataForMarkerId(editedData.id);
            onUpdateData(updatedData);
        } catch (error) {
            console.error('Error fetching updated marker data:', error);
        }
    };

    return (
        <Marker
            position={[data.geometry.lat, data.geometry.lng]}
            icon={L.divIcon({
                className: `custom-marker custom-marker-${markerColor}`,
                html: '<div></div>',
            })}
        >
            <Popup>
                <strong>{data.country}</strong>
                <br />
                Week: {data.week}
                <br />
                New Cases: {data.newCases.toLocaleString()}
                <br />
                Tests Done: {data.testsDone.toLocaleString()}
                <br />
                Positivity Rate: {data.positivityRate.toFixed(2)}%
                <br />
                Testing Rate: {data.testingRate.toFixed(2)}
                <br />
                New Cases Per Capita: {data.perCapitaCases.toLocaleString()}
                <br />
                Tests Done Per Capita: {data.perCapitaTests.toLocaleString()}
                <br />
                Total Cases for the Year: {data.totalCasesYear.toLocaleString()}
                <br />
                Total Tests for the Year: {data.totalTestsYear.toLocaleString()}
                <br />
                <S.Button onClick={handleEdit}>Edit</S.Button>
                <S.Button onClick={handleDelete}>Delete</S.Button>
            </Popup>
            <Modal isOpen={showEditModal} onRequestClose={() => setShowEditModal(false)}>
                <S.ModalContentWrapper>
                    <S.ModalHeading>Edit Marker</S.ModalHeading>
                    <form>
                        <label>
                            New Cases:
                            <S.Input
                                type="number"
                                value={editedData.newCases}
                                onChange={(e) => setEditedData({ ...editedData, newCases: e.target.value })}
                            />
                        </label>

                        <label>
                            Tests Done:
                            <S.Input
                                type="number"
                                value={editedData.testsDone}
                                onChange={(e) => setEditedData({ ...editedData, testsDone: e.target.value })}
                            />
                        </label>

                        <label>
                            Positivity Rate:
                            <S.Input
                                type="number"
                                step="0.01"
                                value={editedData.positivityRate}
                                onChange={(e) => setEditedData({ ...editedData, positivityRate: e.target.value })}
                            />
                        </label>

                        <label>
                            Testing Rate:
                            <S.Input
                                type="number"
                                step="0.01"
                                value={editedData.testingRate}
                                onChange={(e) => setEditedData({ ...editedData, testingRate: e.target.value })}
                            />
                        </label>

                        <S.Button type="button" onClick={handleEditSubmit}>
                            Save
                        </S.Button>
                        <S.Button type="button" onClick={() => setShowEditModal(false)}>
                            Cancel
                        </S.Button>
                    </form>
                </S.ModalContentWrapper>
            </Modal>
        </Marker>
    );
};


export default MarkerWithPopup;
