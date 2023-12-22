import TextField from "@mui/material/TextField";
import "./Request.css";
import CircularProgress from "@mui/material/CircularProgress";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import { green } from "@mui/material/colors";
import { useState } from "react";
import Stack from "@mui/material/Stack";

interface RequestProps {
    request: string,
    success: boolean,
    loading: boolean,
    setRequest: (value: string) => void,
    handleSubmit: () => void
}

export default function Request({ request, success, loading, setRequest, handleSubmit }: RequestProps){
    const buttonSx = {
        ...(success && {
            bgcolor: green[500],
            "&:hover": {
                bgcolor: green[700],
            },
        }),
    };

    return (
        <Stack>
                <TextField
                    id="request"
                    label="Request"
                    variant="outlined"
                    value={request}
                    onChange={(e) => setRequest(e.target.value)}
                    multiline
                />
                <Box sx={{ m: 1, position: "relative" }}>
                    <Button variant="contained" disabled={loading} onClick={handleSubmit}>
                        Submit
                    </Button>
                    {loading && (
                        <CircularProgress
                            size={24}
                            sx={{
                                color: green[500],
                                position: "absolute",
                                top: "50%",
                                left: "50%",
                                marginTop: "-12px",
                                marginLeft: "-12px",
                            }}
                        />
                    )}
                </Box>
        </Stack>
    );
}
