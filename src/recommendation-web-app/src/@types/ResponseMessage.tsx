import { OpenAIMessage } from "./OpenAIMessage";

export interface ResponseMessage {
    iterations: number;
    chatHistory: Array<OpenAIMessage>;
    finalAnswer: string;
}