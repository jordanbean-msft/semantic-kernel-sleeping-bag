import Avatar from "@mui/material/Avatar"
import Grid from "@mui/material/Grid"
import { ChatHistoryItem } from "../../@types/ChatHistoryItem"
import Paper from "@mui/material/Paper"

interface ChatThreadProps {
    chatHistory: Array<ChatHistoryItem>
}

export default function ChatThread({ chatHistory }: ChatThreadProps) {
    return (
        <Grid container spacing={2}>
            {
                chatHistory.map((chatHistoryItem) => {
                    return (
                        <Grid item>
                            <Paper>
                                <Grid container>
                                    <Grid item>
                                        <Avatar>{chatHistoryItem.role[0].toUpperCase()}</Avatar>
                                    </Grid>
                                    <Grid item>
                                        {chatHistoryItem.content}
                                    </Grid>
                                </Grid>
                            </Paper>
                        </Grid>
                    )
                }
                )
            }
        </Grid>
    )
}