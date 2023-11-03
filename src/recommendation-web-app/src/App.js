import logo from "./logo.svg";
import "./App.css";
import Request from "./components/Request";
import Container from "@mui/material/Container";
import Navigation from "./components/Navigation";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
import AdbIcon from '@mui/icons-material/Adb';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={
                    <div className="App">
                        <Navigation />
                    </div>
                } />
                <Route path="/healthz" element={<h3>healthy</h3> } />
                <Route path="*" />
            </Routes>
        </BrowserRouter>
    );
}



export default App;
