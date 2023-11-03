import { useState } from "react";
import TextField from "@mui/material/TextField";
import Response from "../Response/Response";
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
import { green } from '@mui/material/colors';
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

    const buttonSx = {
        ...(success && {
            bgcolor: green[500],
            '&:hover': {
                bgcolor: green[700],
            },
        }),
    };

    return (
        <Stack>
            <Paper class="requestPaper">
                <TextField
                    id="request"
                    label="Request"
                    variant="outlined"
                    value={request}
                    onChange={(e) => setRequest(e.target.value)}
                    multiline />
                    <Box sx={{ m: 1, position: 'relative' }}>
                        <Button
                            variant="contained"
                            disabled={loading}
                            onClick={handleSubmit}
                        >
                            Submit
                        </Button>
                        {
                            loading && (
                                <CircularProgress
                                    size={24}
                                    sx={{
                                        color: green[500],
                                        position: 'absolute',
                                        top: '50%',
                                        left: '50%',
                                        marginTop: '-12px',
                                        marginLeft: '-12px',
                                    }}
                                />
                            )
                        }
                    </Box>
            </Paper>
            {
                response && (<Response response={response} />)
            }
        </Stack>
    );

    async function handleSubmit() {
        if (!loading) {
            setSuccess(false);
            setLoading(true);
            setResponse("");
            //e.preventDefault();
            const response = await fetch(`${process.env.REACT_APP_RECOMMENDATION_API_URL}/recommendation`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ message: request }),
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


