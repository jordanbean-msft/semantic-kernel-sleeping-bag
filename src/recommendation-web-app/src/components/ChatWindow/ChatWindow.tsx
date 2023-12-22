import * as React from 'react';
import { useState } from "react";
import Tab from '@mui/material/Tab';
import TabList from '@mui/lab/TabList';
import TabPanel from '@mui/lab/TabPanel';
import TabContext from '@mui/lab/TabContext';
import Box from '@mui/material/Box'
import ThoughtProcess from '../ThoughtProcess/ThoughtProcess';
import config from '../../config';
import { ResponseMessage } from '../../@types/ResponseMessage';
import ChatThread from '../ChatThread/ChatThread';
import Request from '../Request/Request';
import Grid from '@mui/material/Grid';
import { ChatHistoryItem } from '../../@types/ChatHistoryItem';

export default function ChatWindow() {
    const [value, setValue] = React.useState("0");
    const [responseMessage, setResponseMessage] = useState<ResponseMessage | undefined>(undefined);

    const [chatHistory, setChatHistory] = useState<Array<ChatHistoryItem>>([]);

    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [isFirstTime, setIsFirstTime] = useState(true);
    const [request, setRequest] = useState("Will my sleeping bag work for my trip to Patagonia next month?");

    const handleChange = (event: React.SyntheticEvent, newValue: string) => {
        setValue(newValue);
    }

    return (
        <Box>
            <TabContext value={value}>
                <Box>
                    <TabList onChange={handleChange} variant="fullWidth">
                        <Tab label="Chatbot" value="0" />
                        <Tab label="Thought Process" value="1" />"
                    </TabList>
                </Box>
                <TabPanel value="0">
                    <Grid container spacing={6}>
                        <Grid item xs={12}>
                            <ChatThread chatHistory={chatHistory} />
                        </Grid>
                        <Grid item xs={12}>
                            <Request request={request} success={success} loading={loading} setRequest={setRequest} handleSubmit={handleSubmit} />
                        </Grid>
                    </Grid>
                </TabPanel>
                <TabPanel value="1">
                    <ThoughtProcess response={responseMessage} />
                </TabPanel>
            </TabContext>
        </Box>
    )
    async function handleSubmit() {
        if (!loading && request !== "") {
            setSuccess(false);
            setLoading(true);
            setResponseMessage(undefined);
            setIsFirstTime(false);

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
            body: JSON.stringify({ message: request, chatHistory: chatHistory }),
        }).then((response) => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return response.json();
        });
        return response;
    }
}