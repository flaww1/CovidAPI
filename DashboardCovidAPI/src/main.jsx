import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './index.css';
import Dashboard from './components/Dashboard.jsx';
import { UserProvider } from './components/UserContext.jsx';

ReactDOM.createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        <Router>
            <UserProvider>
                <Routes>
                    <Route path="/*" element={<Dashboard />} />
                </Routes>
            </UserProvider>
        </Router>
    </React.StrictMode>,
);
