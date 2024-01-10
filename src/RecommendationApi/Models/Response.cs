using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json.Serialization;

namespace RecommendationApi.Models
{
    public record Response
    {
        public List<ChatHistoryItem> ChatHistory { get; init; } = [];

        public ChatHistory SemanticKernelChatHistory { get; init; } = [];

        public string FinalAnswer { get; set; } = "";
    }

    public record ChatHistoryItem
    {
        public string Content { get; init; } = "";
        public string Role { get; set; } = "";
        public int CompletionTokens { get; init; }
        public int PromptTokens { get; init; }
        public int TotalTokens { get; init; }
        public string FunctionName { get; init; } = "";
        public string FunctionArguments { get; init; } = "";
    }
}