import { useState } from "react";
import TextField from "@mui/material/TextField";
import { Button } from "@mui/material";
import Response from "./Response";
import * as React from 'react';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';

export default function Request() {
    const [request, setRequest] = useState("Will my sleeping bag work for my trip to Patagonia next month?");
    const [response, setResponse] = useState("");
    const [error, setError] = useState(null);
    const [status, setStatus] = useState("");

    return (
        <>
            <Card xs={{ minWidth: 275 }}>
            <CardContent>
            <form onSubmit={handleSubmit}>
                <TextField
                    id="request"
                    label="Request"
                    variant="outlined"
                    value={request}
                    onChange={(e) => setRequest(e.target.value)}
                    />
                <Button variant="contained" type="submit">
                    Submit
                </Button>
            </form>
                </CardContent>
            </Card>
            <div>
                <TableContainer component={Paper}>
                    <Table sx={{ minWidth: 650 }} size="small" aria-label="a dense table">
                    <TableHead>
                            <TableCell>Action</TableCell>
                            <TableCell>Thought</TableCell>
                        <TableCell>Observation</TableCell>
                        <TableCell>Original Response</TableCell>
                        <TableCell>Final Answer</TableCell>
                    </TableHead>
                    <TableBody>
                        {response?.openAIMessages?.map((row) => (
                            <TableRow sx={{ '& > *': { borderBottom: 'unset' } }}>
                                <TableCell component="th" scope="response">
                                    {row.action}
                                </TableCell>
                                <TableCell>{row.thought}</TableCell>
                                <TableCell>{row.observation}</TableCell>
                                <TableCell>{row.original_response}</TableCell>
                                <TableCell>{row.final_answer}</TableCell>
                            </TableRow>))
                        }
</TableBody>
                </Table>
                </TableContainer>
                </div>
            </>
    );

    async function handleSubmit(e) {
        setResponse("");
        e.preventDefault();
        const response = await fetch(`${process.env.REACT_APP_RECOMMENDATION_API_URL}/recommendation`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ message: e.target.request.value }),
        }).then((response) => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return response.json();
        });
        setResponse(response);
    }
}
