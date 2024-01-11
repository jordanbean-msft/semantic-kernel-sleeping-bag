import React from 'react';
import './App.css';
import Navigation from "./components/Navigation/Navigation";
import { BrowserRouter, Routes, Route } from "react-router-dom";

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
