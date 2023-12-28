// styles.js

import styled from 'styled-components';

export const DashboardContainer = styled.div`
  max-width: 1200px; /* Adjust the maximum width as needed */
  margin: 0 auto;    /* Center the container */
  padding: 20px;
  background-color: #f0f0f0;
`;

export const ModalContentWrapper = styled.div`
  max-width: 300px; /* Adjust the value as needed */
  margin: auto; /* Center the modal horizontally */
  padding: 20px; /* Add padding for better aesthetics */
  background: transparent; /* Set the background to transparent */
  border-radius: 8px; /* Add rounded corners for a modern look */
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); /* Add a subtle box shadow for depth */
`;


export const ModalHeading = styled.h2`
    color: #007bff;
    margin-bottom: 20px;
`;

export const ModalLabel = styled.label`
    display: block;
    margin-bottom: 10px;
    font-weight: bold;
`;

export const ModalInput = styled.input`
    width: 100%;
    padding: 8px;
    margin-bottom: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
`;

export const ModalButtonWrapper = styled.div`
    display: flex;
    justify-content: space-between;
`;

export const ModalCancelButton = styled.button`
  background-color: #ccc;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;

  &:hover {
    background-color: #999;
  }
`;

export const ModalAddButton = styled.button`
  background-color: #007bff;
  color: white;
  padding: 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;

  &:hover {
    background-color: #0056b3;
  }
`;


export const Button = styled.button`
    padding: 10px 15px;
    margin: 5px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;

    &:hover {
        background-color: #0056b3;
    }
`;

export const Input = styled.input`
    padding: 10px;
    margin: 5px;
    border: 1px solid #ccc;
    border-radius: 4px;
`;

export const ErrorMessage = styled.p`
    color: red;
    font-size: 14px;
    margin-top: 5px;
`;

export const WeekSelectorContainer = styled.div`
    display: flex;

    align-items: center;
    margin-top: 10px;

    label {
        margin-right: 10px;
    }

    select {
        padding: 8px;
        border: 1px solid #ccc;
        border-radius: 4px;
    }
`;

export const MapContainerWrapper = styled.div`
  position: relative;
  z-index: 0; /* Ensure the map is behind other elements */
`;


export const AddMarkerButton = styled.button`
    background-color: #007bff;
    color: white;
    padding: 10px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;

    &:hover {
        background-color: #0056b3;
    }
`;

export const LogoutButton = styled.button`
    background-color: #007bff;
    color: white;
    padding: 10px;
    border: none; /* Add this line to remove any default button border */
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;

    &:hover {
        background-color: #0056b3;
    }
`;

export const CustomMarker = styled.div`
 &.custom-marker {
   width: 20px;
   height: 20px;
   border-radius: 50%;
   background-color: ${props => props.color || 'blue'};
   display: flex;
   justify-content: center;
   align-items: center;
   color: white;
   font-size: 14px;
 }

 &.custom-marker-low {
   background-color: green;
 }

 &.custom-marker-medium {
   background-color: yellow;
 }

 &.custom-marker-high {
   background-color: red;
 }
`;


// Add more styled components as needed
