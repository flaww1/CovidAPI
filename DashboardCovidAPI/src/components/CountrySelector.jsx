import React, { useState, useEffect } from 'react';
import { getAllCountries } from '../services/ApiService';
import { SelectorContainer } from './styles';

const CountrySelector = ({ onSelectCountry }) => {
    const [countries, setCountries] = useState([]);
    const [selectedCountry, setSelectedCountry] = useState('');

    useEffect(() => {
        const fetchCountries = async () => {
            try {
                const countryList = await getAllCountries();
                setCountries(countryList);
                if (countryList.length > 0) {
                    setSelectedCountry(countryList[0]);
                }
            } catch (error) {
                console.error('Error fetching countries:', error);
            }
        };

        fetchCountries();
    }, []);

    const handleCountryChange = (event) => {
        const country = event.target.value;
        setSelectedCountry(country);
        onSelectCountry(country);
    };

    return (
        <SelectorContainer>
            <div>
                <label htmlFor="countrySelect">Select a country: </label>
                <select id="countrySelect" value={selectedCountry} onChange={handleCountryChange}>
                    {countries.map((country) => (
                        <option key={country} value={country}>
                            {country}
                        </option>
                    ))}
                </select>
            </div>
        </SelectorContainer>
    );
};

export default CountrySelector;
