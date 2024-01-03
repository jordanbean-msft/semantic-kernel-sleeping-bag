import { ChatHistoryItem } from "../../@types/ChatHistoryItem"
import Skeleton from "@mui/material/Skeleton"
import ChatThreadItem from "../ChatThreadItem/ChatThreadItem"
import Stack from "@mui/material/Stack"
import Box from "@mui/material/Box"
import Paper from "@mui/material/Paper"

interface ChatThreadProps {
    chatHistory: Array<ChatHistoryItem>,
    loading: boolean
}

export default function ChatThread({ chatHistory, loading }: ChatThreadProps) {
    return (
        <Box sx={{ flexGrow: 1, overflow: "auto", p: 2 }}>
            {
                chatHistory.map((chatHistoryItem) => (
                    <Box sx={{
                        display: "flex",
                        justifyContent: chatHistoryItem.role === "user" ? "flex-start" : "flex-end",
                        mb: 2
                    }} key={chatHistoryItem.id}>
                        <ChatThreadItem
                            content={chatHistoryItem.content}
                            isUserRole={chatHistoryItem.role === "user" ? true : false}
                        />
                    </Box>
                ))
            }
            {
                loading && (
                    <Box sx={{
                        display: "flex",
                        justifyContent: "flex-end",
                        mb: 2
                    }}>
                        <Paper elevation={12}>
                            <Box sx={{
                                display: "flex",
                                p: 2,
                                maxWidth: 1000
                            }}>
                                <Skeleton variant="circular" />
                                <Stack spacing={1} sx={{ p: 1 }}>
                                    <Skeleton sx={{ width: 500 }} />
                                    <Skeleton />
                                    <Skeleton />
                                </Stack>
                            </Box>
                        </Paper>
                    </Box>
                )
            }
        </Box>
    )
}