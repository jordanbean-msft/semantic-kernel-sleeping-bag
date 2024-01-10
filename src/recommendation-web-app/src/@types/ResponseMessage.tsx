import { OpenAIMessage } from "./OpenAIMessage";

export interface ResponseMessage {
    chatHistory: Array<OpenAIMessage>;
    finalAnswer: string;
}