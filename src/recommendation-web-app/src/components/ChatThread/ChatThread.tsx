import Avatar from "@mui/material/Avatar"
import Grid from "@mui/material/Grid"
import { ChatHistoryItem } from "../../@types/ChatHistoryItem"
import Paper from "@mui/material/Paper"
import Typography from "@mui/material/Typography"

interface ChatThreadProps {
    chatHistory: Array<ChatHistoryItem>
}

export default function ChatThread({ chatHistory }: ChatThreadProps) {
    return (
        <Grid container spacing={6}>
            {
                chatHistory.map((chatHistoryItem) => {
                    if (chatHistoryItem.role === "user") {
                        return (
                            <Grid item xs={12}>
                                <Grid container>
                                    <Grid item zeroMinWidth xs={6} >
                                        <Paper>
                                            <Grid container spacing={2}>
                                                <Grid item xs={1}>
                                                    <Avatar>{chatHistoryItem.role[0].toUpperCase()}</Avatar>
                                                </Grid>
                                                <Grid item xs={11}>
                                                    <Typography>{chatHistoryItem.content}</Typography>
                                                </Grid>
                                            </Grid>
                                        </Paper>
                                    </Grid>
                                    <Grid item xs={6} />
                                </Grid>
                            </Grid>
                        )
                    }
                    else {
                        return (
                            <Grid item xs={12}>
                                <Grid container>
                                    <Grid item xs={6} />
                                    <Grid item zeroMinWidth xs={6} >
                                        <Paper>
                                            <Grid container spacing={2}>
                                                <Grid item xs={1}>
                                                    <Avatar>{chatHistoryItem.role[0].toUpperCase()}</Avatar>
                                                </Grid>
                                                <Grid item xs={11}>
                                                    <Typography>{chatHistoryItem.content}</Typography>
                                                </Grid>
                                            </Grid>
                                        </Paper>
                                    </Grid>
                                </Grid>
                            </Grid>
                        )
                    }

                }
                )
            }
        </Grid>
    )
}