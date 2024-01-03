import { ChatHistoryItem } from "../../@types/ChatHistoryItem"
import Typography from "@mui/material/Typography"
import SmartToyIcon from "@mui/icons-material/SmartToy"
import PersonIcon from "@mui/icons-material/Person"
import Skeleton from "@mui/material/Skeleton"
import ChatThreadItem from "../ChatThreadItem/ChatThreadItem"
import Stack from "@mui/material/Stack"
import Box from "@mui/material/Box"

interface ChatThreadProps {
    chatHistory: Array<ChatHistoryItem>,
    loading: boolean
}

export default function ChatThread({ chatHistory, loading }: ChatThreadProps) {
    return (
        <Box sx={{ flexGrod: 1, overflow: "auto", p: 2 }}>
            {
                chatHistory.map((chatHistoryItem) => (
                    <Box sx={{
                        display: "flex",
                        justifyContent: chatHistoryItem.role === "user" ? "flex-start" : "flex-end",
                        mb: 2                   
                    }}>
                        <ChatThreadItem
                            avatar={
                                chatHistoryItem.role === "user" ? (
                                    <PersonIcon />
                                ) : (
                                    <SmartToyIcon />
                                )
                            }
                            item={
                                <Typography align="left" sx={{ p:1 }}>{chatHistoryItem.content}</Typography>
                            }
                            isUserRole={chatHistoryItem.role === "user" ? true : false} />
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
                        <ChatThreadItem
                            avatar={<Skeleton variant="circular" />}
                            item={
                                <Stack spacing={1} sx={{ p:1 }}>
                                    <Skeleton sx={{ width: 500 }} />
                                    <Skeleton />
                                    <Skeleton />
                                </Stack>
                            }
                            isUserRole={false} />
                    </Box>
                )
            }
        </Box>
    )
}