import { useState } from "react";
import TextField from "@mui/material/TextField";
import Response from "./Response";
import * as React from 'react';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import './Request.css'
import CircularProgress from '@mui/material/CircularProgress';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import FormControl from '@mui/material/FormControl';
import Box from '@mui/material/Box';
import { styled } from '@mui/material/styles';

export default function Request() {
    const [request, setRequest] = useState("Will my sleeping bag work for my trip to Patagonia next month?");
    const [response, setResponse] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState(null);
    const [status, setStatus] = useState("");

    return (
        <Stack>
            <Paper>
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
            {
                response && (<Response response={response} />)
            }
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


