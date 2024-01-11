import { OpenAIMessage } from "./OpenAIMessage";

export interface RequestMessage {
    message: string;
    chatHistory: Array<OpenAIMessage>;
    chatId: string;
}