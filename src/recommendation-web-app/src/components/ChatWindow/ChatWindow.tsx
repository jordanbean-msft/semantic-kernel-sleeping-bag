import * as React from 'react';
import { useEffect, useState } from "react";
import ThoughtProcess from '../ThoughtProcess/ThoughtProcess';
import config from '../../config';
import { ResponseMessage } from '../../@types/ResponseMessage';
import ChatThread from '../ChatThread/ChatThread';
import Request from '../Request/Request';
import Grid from '@mui/material/Grid';
import { ChatHistoryItem } from '../../@types/ChatHistoryItem';
import { OpenAIMessage } from '../../@types/OpenAIMessage';
import Dialog from '@mui/material/Dialog';
import { TransitionProps } from '@mui/material/transitions';
import Slide from '@mui/material/Slide';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import CloseIcon from '@mui/icons-material/Close';
import PsychologyIcon from '@mui/icons-material/Psychology';
import Box from '@mui/material/Box';

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});

interface ChatWindowProps {
    reset: boolean;
    setReset: React.Dispatch<React.SetStateAction<boolean>>;
}

export default function ChatWindow({ reset, setReset }: ChatWindowProps) {
    const defaultMessage = "Will my sleeping bag work for my trip to Patagonia next month?";

    const [responseMessage, setResponseMessage] = useState<ResponseMessage | undefined>(undefined);
    const [entireChatHistory, setEntireChatHistory] = useState<Array<OpenAIMessage>>([]);
    const [chatHistory, setChatHistory] = useState<Array<ChatHistoryItem>>([]);

    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [request, setRequest] = useState(defaultMessage);
    const [open, setOpen] = React.useState(false);

    useEffect(() => {
        if (reset) {
            setChatHistory([]);
            setEntireChatHistory([]);
            setRequest(defaultMessage);
            setResponseMessage(undefined);
            setSuccess(false);
            setLoading(false);
            setReset(false);
        }
    });

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    return (
        <React.Fragment>
            <Box sx={{ display: "flex", flexDirection: "column" }}>
                <ChatThread chatHistory={chatHistory} loading={loading} />
                <Box sx={{ p: 2 }} >
                    <Request request={request} success={success} loading={loading} setRequest={setRequest} handleSubmit={handleSubmit} handleClickOpen={handleClickOpen} />
                </Box>
            </Box>
            {/*<Grid container spacing={6}>*/}
            {/*    <Grid item xs={12}>*/}
            {/*        <ChatThread chatHistory={chatHistory} loading={loading} />*/}
            {/*    </Grid>*/}
            {/*    <Grid item xs={12}>*/}
            {/*        <Request request={request} success={success} loading={loading} setRequest={setRequest} handleSubmit={handleSubmit} handleClickOpen={handleClickOpen} />*/}
            {/*    </Grid>*/}
            {/*</Grid>*/}
            <Dialog
                fullScreen
                open={open}
                onClose={handleClose}
                TransitionComponent={Transition}
            >
                <AppBar sx={{ position: 'relative' }}>
                    <Toolbar>
                        <PsychologyIcon />
                        <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
                            Thought Process
                        </Typography>
                        <IconButton
                            edge="start"
                            color="inherit"
                            onClick={handleClose}
                            aria-label="close"
                        >
                            <CloseIcon />
                        </IconButton>
                    </Toolbar>
                </AppBar>
                <ThoughtProcess response={responseMessage} />
            </Dialog>
        </React.Fragment>
    )

    async function handleSubmit() {
        if (!loading && request !== "") {
            setSuccess(false);
            setLoading(true);
            setResponseMessage(undefined);

            let chatHistoryItem: ChatHistoryItem = {
                content: request,
                role: "user"
            };
            setChatHistory(chatHistory => ([...chatHistory, chatHistoryItem]));
            setRequest("");

            let response = await callApi();

            let chatHistoryItemResponse: ChatHistoryItem = {
                content: response.finalAnswer,
                role: "assistant"
            };

            setChatHistory(chatHistory => ([...chatHistory, chatHistoryItemResponse]));
            setEntireChatHistory(entireChatHistory => ([...entireChatHistory, ...response.chatHistory]));
            setResponseMessage(response);
            setSuccess(true);
            setLoading(false);
        }


    }

    async function callApi() {
        const response = await fetch(`${config.api.baseUrl}/recommendation`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ message: request, chatHistory: entireChatHistory }),
        }).then((response) => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return response.json();
        });
        return response;
    }
}