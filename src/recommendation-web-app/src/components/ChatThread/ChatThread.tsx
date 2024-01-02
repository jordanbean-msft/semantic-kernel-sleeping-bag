import Avatar from "@mui/material/Avatar"
import Grid from "@mui/material/Grid"
import { ChatHistoryItem } from "../../@types/ChatHistoryItem"
import Paper from "@mui/material/Paper"
import Typography from "@mui/material/Typography"
import SmartToyIcon from "@mui/icons-material/SmartToy"
import PersonIcon from "@mui/icons-material/Person"
import Skeleton from "@mui/material/Skeleton"
import ChatThreadItem from "../ChatThreadItem/ChatThreadItem"
import Stack from "@mui/material/Stack"

interface ChatThreadProps {
    chatHistory: Array<ChatHistoryItem>,
    loading: boolean
}

export default function ChatThread({ chatHistory, loading }: ChatThreadProps) {
    return (
        <Grid container spacing={6}>
            {
                chatHistory.map((chatHistoryItem) => {
                    if (chatHistoryItem.role === "user") {
                        return (
                            <ChatThreadItem
                                avatar={<PersonIcon />}
                                item={
                                    <Typography>{chatHistoryItem.content}</Typography>
                                }
                                isUserRole={true} />
                                                    )
                    }
                    else {
                        return (
                            <ChatThreadItem
                                avatar={
                                    <SmartToyIcon />
                                }
                                item={
                                    <Typography>{chatHistoryItem.content}</Typography>
                                }
                                isUserRole={
                                    false
                                } />
                        )
                    }
                }
                )
            }
            {
                loading && (
                    <ChatThreadItem
                        avatar={<SmartToyIcon />}
                        item={
                            <Stack spacing={1}>
                            <Skeleton />
                            <Skeleton />
                            <Skeleton />
</Stack>
                        }
                        isUserRole={false} />
                )
            }
        </Grid>
    )
}