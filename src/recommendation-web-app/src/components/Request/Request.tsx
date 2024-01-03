import TextField from "@mui/material/TextField";
import "./Request.css";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Grid from "@mui/material/Grid";
import Send from "@mui/icons-material/Send";
import { Psychology } from "@mui/icons-material";
import Typography from "@mui/material/Typography";

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
            <Grid item xs={9}>
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
            </Grid>
            <Grid item xs={3 }>
                <Stack direction="row" spacing={2} >
                    <Button fullWidth variant="contained" disabled={loading} onClick={handleSubmit} endIcon={<Send />}>
                        <Typography>Submit</Typography>
                    </Button>
                    <Button fullWidth variant="contained" disabled={!success} onClick={handleClickOpen} endIcon={<Psychology />}>
                        <Typography>Thought Process</Typography>
                    </Button>
                </Stack>
            </Grid>
        </Grid>
    );
}
