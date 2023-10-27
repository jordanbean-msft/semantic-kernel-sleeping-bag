import { useState } from "react";
import TextField from "@mui/material/TextField";
import { Button } from "@mui/material";
import Response from "./Response";

export default function Request() {
    const [request, setRequest] = useState("");
    const [response, setResponse] = useState("");
    const [error, setError] = useState(null);
    const [status, setStatus] = useState("");

    return (
        <>
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
            <p>{response}</p>
            <Response />
        </>
    );

    async function handleSubmit(e) {
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
        setResponse(response.message);
    }
}
