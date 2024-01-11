export interface OpenAIMessage {
    content: string;
    role: string;
    functionName: string;
    functionArguments: string;
    completionTokens: number;
    promptTokens: number;
    totalTokens: number;
}