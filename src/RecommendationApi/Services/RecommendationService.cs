using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.Memory;
using RecommendationApi.Models;
using RecommendationApi.NativePlugins;
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

            _kernel.ImportPluginFromType<CustomerServicePlugin>();
            //_kernel.ImportPluginFromType<HistoricalWeatherLookupPlugin>();
            //_kernel.ImportPluginFromType<LocationLookupPlugin>();
            //_kernel.ImportPluginFromType<OrderHistoryPlugin>();
            //_kernel.ImportPluginFromType<ProductCatalogPlugin>();
            _kernel.ImportPluginFromType<TextMemoryPlugin>();
            //_kernel.ImportPluginFromType<ConversationSummaryPlugin>();
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var username = "dkschrute";

            await PopulateMemoryWithOrderHistoryAsync(request, username);

            var currentDate = DateTime.Now.ToString("MMM-dd-yyyy");

            ChatHistory? chatHistory = null;

            MemoryQueryResult? result = await _memory.GetAsync(username, request.ChatId);

            if (result == null)
            {
                chatHistory = await SetupNewChatHistoryAsync(username, currentDate, chatHistory, request.Message);
            }
            else
            {
                chatHistory = JsonSerializer.Deserialize<ChatHistory>(result.Metadata.Text);
            }

            chatHistory!.AddUserMessage(request.Message);

            _chatHistoryFromEventHandler.Clear();

            _kernel.FunctionInvoked += Kernel_FunctionInvoked;

            await ChatAsync(chatHistory);

            await _memory.SaveInformationAsync(username, JsonSerializer.Serialize(chatHistory), request.ChatId);

            return new Response
            {
                ChatHistory = ParseChatHistory(chatHistory),
                SemanticKernelChatHistory = chatHistory,
                FinalAnswer = chatHistory.Last().Content!
            };
        }

        private async Task ChatAsync(ChatHistory? chatHistory)
        {
            OpenAIPromptExecutionSettings promptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                //ToolCallBehavior = ToolCallBehavior.RequireFunction(_kernel.Plugins["CustomerServicePlugin"][0].)
            };

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            //YOU MUST provide the kernel that the ChatCompletionService is to use, otherwise, it won't have access to all the plugins
            foreach (var chatMessage in await chatCompletionService.GetChatMessageContentsAsync(chatHistory, promptExecutionSettings, _kernel))
            {
                chatHistory.Add(chatMessage);
            }
        }

        private async Task<ChatHistory?> SetupNewChatHistoryAsync(string username, string currentDate, ChatHistory? chatHistory, string goal)
        {
            var arguments = new KernelArguments
            {
                ["username"] = username,
                ["current_date"] = currentDate,
                ["goal"] = goal,
                [TextMemoryPlugin.InputParam] = username,
                [TextMemoryPlugin.CollectionParam] = username,
                [TextMemoryPlugin.LimitParam] = "2",
                [TextMemoryPlugin.RelevanceParam] = "0.79"
            };

            var promptTemplateFactory = new KernelPromptTemplateFactory();

            string systemMessage = await promptTemplateFactory.Create(new PromptTemplateConfig("You are a customer support chatbot. You should answer the question posed by the user. Ground your answers based upon the user's purchase history. If you don't know the answer, respond saying you don't know. Make sure and use the CustomerServicePlugin to help you answer the question if the user doesn't provide all the needed information. If you can't answer the question, try and use the CustomerServicePlugin to get a better, more complete answer. Order History: {{ recall $username }} Current Date: {{ $current_date }} Username: {{ $username }} Goal: {{ $goal }}")).RenderAsync(_kernel, arguments);
            //string systemMessage = await promptTemplateFactory.Create(new PromptTemplateConfig("You are a customer support chatbot. You should answer the question posed by the user. Ground your answers based upon the user's purchase history. If you don't know the answer, respond saying you don't know. Make sure and call the functions you have available to help you answer the question if the user doesn't provide all the needed information. Current Date: {{ $current_date }} Username: {{ $username }} Order History: {{ recall $username }} Goal: {{ $goal }}")).RenderAsync(_kernel, arguments);
            //string systemMessage = await promptTemplateFactory.Create(new PromptTemplateConfig("You are a customer support chatbot. You should answer the question posed by the user. Ground your answers based upon the user's purchase history. If you don't know the answer, respond saying you don't know. Make sure and call the functions you have available to help you answer the question if the user doesn't provide all the needed information. Current Date: {{ $current_date }} Username: {{ $username }} Order History: {{ recall $username }} Goal: {{ $goal }}")).RenderAsync(_kernel, arguments);

            chatHistory = new ChatHistory(systemMessage);
            return chatHistory;
        }

        private async Task PopulateMemoryWithOrderHistoryAsync(Request request, string username)
        {
            Dictionary<string, OrderHistory> orderHistory = new()
            {
                {
                "dkschrute",
                new OrderHistory
                {
                        OrderId = "1",
                        CustomerId = "dkschrute",
                        OrderDate = "2021-01-01",
                        OrderTotal = "100",
                        OrderStatus = "Shipped",
                        OrderItems = [
                            new() {
                                ProductId = "12345",
                                ProductName = "Eco Elite Sleeping Bag",
                                ProductPrice = "100",
                                ProductQuantity = "1"
                            }
                        ]
                }
                }
            };

            await _memory.SaveInformationAsync(
                collection: username,
                text: JsonSerializer.Serialize(orderHistory),
                id: username);
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
