using Microsoft.SemanticKernel.ChatCompletion;
using RecommendationApi.Models;

public record Request
{
    public List<ChatHistoryItem> ChatHistory { get; init; } = new();
    public string Message { get; init; } = "";
    public string ChatId { get; init; } = "";
}