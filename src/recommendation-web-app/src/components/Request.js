import { useState } from "react";
import TextField from "@mui/material/TextField";
import Response from "./Response";
import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import { styled } from '@mui/material/styles';
import './Request.css'
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import { green } from '@mui/material/colors';
import Button from '@mui/material/Button';
import Fab from '@mui/material/Fab';
import CheckIcon from '@mui/icons-material/Check';
import SaveIcon from '@mui/icons-material/Save';

export default function Request() {
    const [request, setRequest] = useState("Will my sleeping bag work for my trip to Patagonia next month?");
    const [response, setResponse] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState(null);
    const [status, setStatus] = useState("");

    return (
        <Stack>
            <Paper xs={{ minWidth: 275 }}>
            <form onSubmit={handleSubmit} className="table">
                <TextField
                    id="request"
                    label="Request"
                    variant="outlined"
                    value={request}
                    onChange={(e) => setRequest(e.target.value)}
                    multiline />
                <Button variant="contained" type="submit">
                    Submit
                    </Button>
                    {
                        loading && (
                            <CircularProgress />
                        )
                    }
            </form>
            </Paper>
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
            </Stack>
    );

    async function handleSubmit(e) {
        if (!loading) {
            setSuccess(false);
            setLoading(true);
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
            setSuccess(true);
            setLoading(false);
        }
    }
}
