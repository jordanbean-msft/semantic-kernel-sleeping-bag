import TextField from "@mui/material/TextField";
import "./Request.css";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Grid from "@mui/material/Grid";
import Send from "@mui/icons-material/Send";
import { Psychology } from "@mui/icons-material";

interface RequestProps {
    request: string,
    success: boolean,
    loading: boolean,
    setRequest: (value: string) => void,
    handleSubmit: () => void,
    handleClickOpen: () => void
}

export default function Request({ request, success, loading, setRequest, handleSubmit, handleClickOpen }: RequestProps) {
    return (
        <Grid container spacing={2}>
            <Grid item xs={10}>
                <Stack spacing={2}>
                    <TextField size="small" sx={{ visibility: "hidden" }} />
                <TextField
                    size="small"
                    id="request"
                    label="Request"
                    variant="outlined"
                    value={request}
                    onChange={(e) => setRequest(e.target.value.replace(/\n/g, ''))}
                    fullWidth
                    onKeyDown={(e) => (
                        e.code === "Enter" ? handleSubmit() : null
                    )}
                />
                </Stack>
            </Grid>
            <Grid item xs={2}>
                <Stack spacing={2} >
                                <Button variant="contained" disabled={!success} onClick={handleClickOpen} endIcon={<Psychology />}>
                                    Thought Process
                                </Button>
                                <Button variant="contained" disabled={loading} onClick={handleSubmit} endIcon={<Send />}>
                                    Submit
                                </Button>
                            </Stack>
                </Grid>
        </Grid>
    );
}
