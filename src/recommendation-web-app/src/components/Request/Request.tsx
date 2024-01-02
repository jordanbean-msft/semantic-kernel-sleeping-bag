import TextField from "@mui/material/TextField";
import "./Request.css";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Divider from "@mui/material/Divider";
import Avatar from "@mui/material/Avatar";
import Grid from "@mui/material/Grid";
import PersonIcon from "@mui/icons-material/Person";
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
        <Grid
            container
            spacing={1} >
            <Grid item xs={12}>
                <Stack
                    direction="row"
                    divider={<Divider orientation="vertical" flexItem />}
                    spacing={2}
                    justifyContent="space-between">
                    <Grid container alignItems="flex-end" spacing={2}>
                        <Grid item sm>
                            <Avatar>
                                <PersonIcon />
                            </Avatar>
                        </Grid>
                        <Grid item xs={11}>
                            <TextField
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
                    </Grid>
                    <Stack spacing={2} >
                        <Button variant="contained" disabled={!success} onClick={handleClickOpen} endIcon={<Psychology />}>
                            Thought Process
                        </Button>
                        <Button variant="contained" disabled={loading} onClick={handleSubmit} endIcon={<Send />}>
                            Submit
                        </Button>
                    </Stack>
                </Stack>
            </Grid>
        </Grid>
    );
}
