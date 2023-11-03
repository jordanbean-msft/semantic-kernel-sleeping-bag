import logo from "./logo.svg";
import "./App.css";
import Request from "./components/Request";
import Container from "@mui/material/Container";
import { BrowserRouter, Routes, Route } from "react-router-dom";


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={
                    <div className="App">
                        <Container>
                            <Request />
                        </Container>
                    </div>
                } />
                <Route path="/healthz" element={<h3>healthy</h3> } />
                <Route path="*" />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
