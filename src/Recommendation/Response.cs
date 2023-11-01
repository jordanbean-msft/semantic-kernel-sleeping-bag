using System.Text.Json.Serialization;

public record Response
{
    public string Message { get; init; }
    public int StepCount { get; init; }
    public string FunctionCount { get; init; }
    public int Iterations { get; init; }
    public List<OpenAIMessage> OpenAIMessages { get; init; }
}

public record OpenAIMessage
{
    [JsonPropertyName("thought")]
    public string Thought { get; init; }
    [JsonPropertyName("action")]
    public string Action { get; init; }
    [JsonPropertyName("action_variables")]
    public Dictionary<string, string> ActionVariables;
    [JsonPropertyName("observation")]
    public string Observation { get; init; }
    [JsonPropertyName("final_answer")]
    public string FinalAnswer { get; init; }
    [JsonPropertyName("original_response")]
    public string OriginalResponse { get; init; }
}