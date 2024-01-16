using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.Memory;
using RecommendationApi.Models;
using RecommendationApi.Plugins;
using System.Text.Json;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace RecommendationApi.Services
{
    public class RecommendationService
    {
        private readonly Kernel _kernel;
        private readonly List<ChatHistoryItem> _chatHistoryFromEventHandler = [];
        private readonly ISemanticTextMemory _memory;

        public RecommendationService(Kernel kernel, ISemanticTextMemory memory)
        {
            _kernel = kernel;
            _memory = memory;

            _kernel.ImportPluginFromType<HistoricalWeatherLookupPlugin>();
            _kernel.ImportPluginFromType<LocationLookupPlugin>();
            _kernel.ImportPluginFromType<OrderHistoryPlugin>();
            _kernel.ImportPluginFromType<ProductCatalogPlugin>();
            _kernel.ImportPluginFromType<TextMemoryPlugin>();
            _kernel.ImportPluginFromType<ConversationSummaryPlugin>();
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var username = "dkschrute";
            var currentDate = DateTime.Now.ToString("MMM-dd-yyyy");

            ChatHistory? chatHistory = null;

            MemoryQueryResult? result = await _memory.GetAsync(username, request.ChatId);
            if (result == null)
            {
                chatHistory = new ChatHistory($@"System: You are a customer support chatbot. You should answer the question posed by the user. Ground your answers based upon the user's purchase history. Make sure and look up any needed context for the specific user that is making the request. If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question. Username: {username} Current Date: {currentDate}");
            }
            else
            {
                chatHistory = JsonSerializer.Deserialize<ChatHistory>(result.Metadata.Text);
            }

            chatHistory!.AddUserMessage(request.Message);

            _chatHistoryFromEventHandler.Clear();

            _kernel.FunctionInvoked += Kernel_FunctionInvoked;

            #region ChatMessage

            OpenAIPromptExecutionSettings promptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions                    
            };

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            foreach (var chatMessage in await chatCompletionService.GetChatMessageContentsAsync(chatHistory, promptExecutionSettings, _kernel))
            {
                chatHistory.Add(chatMessage);
            }

            await _memory.SaveInformationAsync(username, JsonSerializer.Serialize(chatHistory), request.ChatId);

            return new Response
            {
                ChatHistory = ParseChatHistory(chatHistory),
                SemanticKernelChatHistory = chatHistory,
                FinalAnswer = chatHistory.Last().Content!
            };

            #endregion

            #region FunctionCallingStepwisePlanner
            //var config = new FunctionCallingStepwisePlannerConfig
            //{
            //    ExecutionSettings = new OpenAIPromptExecutionSettings
            //    {
            //        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            //    }
            //};

            //var planner = new FunctionCallingStepwisePlanner(config);

            //FunctionCallingStepwisePlannerResult? response = null;
            //Response returnValue = new();

            //try
            //{
            //    response = await planner.ExecuteAsync(_kernel, $"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request (the username is \"{username}\"). The current date is \"{currentDate}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question. A summary of the current conversation is {{ConversationSummaryPlugin.SummarizeConversation {JsonSerializer.Serialize(request.ChatHistory.Select(x => x.Content))} }} The user question is \"{request.Message}\"");
            //}
            //catch (Exception ex)
            //{
            //    returnValue.FinalAnswer = ex.Message;
            //}

            //if (returnValue.FinalAnswer != "")
            //{
            //    return returnValue;
            //}
            //else
            //{
            //    return ParseResponse(response!);
            //}
            #endregion
        }

        private void Kernel_FunctionInvoked(object? sender, FunctionInvokedEventArgs e)
        {
            _chatHistoryFromEventHandler.Add(new ChatHistoryItem
            {
                Content = e.Result.ToString(),
                PromptTokens = (e.Metadata?.GetValueOrDefault("Usage") as CompletionsUsage)?.PromptTokens ?? 0,
                CompletionTokens = (e.Metadata?.GetValueOrDefault("Usage") as CompletionsUsage)?.CompletionTokens ?? 0,
                TotalTokens = (e.Metadata?.GetValueOrDefault("Usage") as CompletionsUsage)?.TotalTokens ?? 0,
                FunctionName = e.Function.Name,
                FunctionArguments = string.Join(", ", e.Arguments)
            });
        }

        private static Response ParseResponse(FunctionCallingStepwisePlannerResult functionCallingStepwisePlannerResult)
        {
            var response = new Response
            {
                FinalAnswer = functionCallingStepwisePlannerResult.FinalAnswer,
                ChatHistory = ParseChatHistory(functionCallingStepwisePlannerResult.ChatHistory!),
            };

            return response;
        }

        private static List<ChatHistoryItem> ParseChatHistory(ChatHistory chatHistory)
        {
            var chatHistoryItems = new List<ChatHistoryItem>();

            foreach (var item in chatHistory)
            {
                var functionName = "";

                if (item.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls") != null)
                {
                    functionName = (item.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls") as List<ChatCompletionsFunctionToolCall>)?[0].Name ?? "";
                }
                else
                {
                    var functionId = item.Metadata?.GetValueOrDefault("ChatCompletionsToolCall.Id") ?? "";

                    if (functionId != "")
                    {
                        foreach (var chatMessageContent in chatHistory)
                        {
                            List<ChatCompletionsFunctionToolCall>? functionToolCalls = chatMessageContent.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls") as List<ChatCompletionsFunctionToolCall>;

                            if (functionToolCalls != null)
                            {
                                if (functionToolCalls[0].Id == (functionId as string))
                                {
                                    functionName = functionToolCalls[0].Name;
                                    break;
                                }
                            }
                            else //this is needed because "sometimes" the functionToolCall is not a List<ChatCompletionsFunctionToolCall> but a JsonElement (especially when it is the return value of a Tool role call)
                            {
                                try
                                {
                                    JsonElement functionToolCallsJson = (JsonElement)chatMessageContent.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls");

                                    if (functionToolCallsJson[0].GetProperty("Id").ToString() == functionId.ToString())
                                    {
                                        functionName = functionToolCallsJson[0].GetProperty("Name").ToString();
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    //do nothing
                                }
                            }
                        }
                    }
                }

                var chatHistoryItem = new ChatHistoryItem
                {
                    Content = item.Content ?? "",
                    Role = item.Role.Label,
                    PromptTokens = (item.Metadata?.GetValueOrDefault("Usage") as CompletionsUsage)?.PromptTokens ?? 0,
                    CompletionTokens = (item.Metadata?.GetValueOrDefault("Usage") as CompletionsUsage)?.CompletionTokens ?? 0,
                    TotalTokens = (item.Metadata?.GetValueOrDefault("Usage") as CompletionsUsage)?.TotalTokens ?? 0,
                    FunctionName = functionName ?? "",
                    FunctionArguments = (item.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls") as List<ChatCompletionsFunctionToolCall>)?[0].Arguments ?? ""
                };
                chatHistoryItems.Add(chatHistoryItem);
            }
            return chatHistoryItems;
        }
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
