using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;

//using Microsoft.SemanticKernel.Planning.Handlebars;
using RecommendationApi.Models;
using RecommendationApi.Plugins;
using System.Text;
using System.Text.Json;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace RecommendationApi.Services
{
    public class RecommendationService
    {
        private readonly Kernel _kernel;
        private readonly List<ChatHistoryItem> _chatHistoryFromEventHandler = new();
        //private readonly ILogger _logger;
        //private readonly ISemanticTextMemory _memory;

        public RecommendationService(Kernel kernel)//, ILogger logger)
        {
            _kernel = kernel;
            //_logger = logger;
            //_memory = kernel.GetRequiredService<ISemanticTextMemory>();

            //var historicalWeatherLookupPlugin = _kernel.CreatePluginFromType<HistoricalWeatherLookupPlugin>();

            //foreach(var function in historicalWeatherLookupPlugin)
            //{
            //    var fullyQualifiedName = historicalWeatherLookupPlugin.Name + "-" + function.Name;
            //    _memory.SaveInformationAsync("functions", 
            //        fullyQualifiedName + ": " + function.Description, 
            //        fullyQualifiedName,
            //        additionalMetadata: function.Name).Wait();
            //}

            _kernel.ImportPluginFromType<HistoricalWeatherLookupPlugin>();
            _kernel.ImportPluginFromType<LocationLookupPlugin>();
            _kernel.ImportPluginFromType<OrderHistoryPlugin>();
            _kernel.ImportPluginFromType<ProductCatalogPlugin>();
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var username = "dkschrute";
            var currentDate = DateTime.Now.ToString("MMM-dd-yyyy");
            _chatHistoryFromEventHandler.Clear();

            _kernel.PromptRendering += Kernel_PromptRendering;
            _kernel.PromptRendered += Kernel_PromptRendered;
            _kernel.FunctionInvoking += Kernel_FunctionInvoking;
            _kernel.FunctionInvoked += Kernel_FunctionInvoked;

            #region FunctionCallingStepwisePlanner
            var config = new FunctionCallingStepwisePlannerConfig
            {
                ExecutionSettings = new OpenAIPromptExecutionSettings
                {
                    ChatSystemPrompt = $"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request (the \"{username}\"). The current date is \"{currentDate}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question.",
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions                    
                }
                
            };

            var planner = new FunctionCallingStepwisePlanner(config);

            FunctionCallingStepwisePlannerResult response = null;
            Response returnValue = new();

            try
            {
                response = await planner.ExecuteAsync(_kernel, $"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request (the username is \"{username}\"). The current date is \"{currentDate}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question. The previous responses were \"{JsonSerializer.Serialize(request.ChatHistory)}\". The user question is \"{request.Message}\"");
            }
            catch(Exception ex)
            {
                 returnValue.FinalAnswer = ex.Message;
            }

            if (returnValue.FinalAnswer != "")
            {
                return returnValue;
            }
            else
            {
                return ParseResponse(response!);
            }

            #endregion

            #region HandlebarsPlanner
            //var config = new HandlebarsPlannerConfig
            //{
            //    AllowLoops = true,
            //    MaxTokens = 4000
            //};

            //var planner = new HandlebarsPlanner(config);
            //HandlebarsPlan plan = null;
            //try
            //{
            //    plan = await planner.CreatePlanAsync(_kernel, $"You are a customer support chatbot. You should answer the question posed by the user \"{request.Message}\". Make sure and look up any needed context for the specific user that is making the request \"{username}\". The current date is \"{currentDate}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question.");
            //}
            //catch (Exception ex)
            //{
            //    return new Response
            //    {
            //        FunctionCount = ex.Message
            //    };
            //}

            //var response = plan.Invoke(_kernel, [], CancellationToken.None);
            //return new Response
            //{
            //    //FunctionCount = response.Metadata["functionCount"].ToString() ?? "",
            //    //Iterations = int.Parse(response.Metadata["iterations"].ToString() ?? ""),
            //    //StepCount = int.Parse(response.Metadata["stepCount"].ToString() ?? ""),
            //    //OpenAIMessages = JsonSerializer.Deserialize<List<OpenAIMessage>>(response.Metadata["stepsTaken"].ToString() ?? "") ?? []
            //    FunctionCount = response ?? ""
            //};
            #endregion

            #region StepwisePlanner
            //var stepwisePlannerConfig = new StepwisePlannerConfig
            //{
            //    MaxIterations = 10,
            //    MaxTokens = 4000
            //};

            //var planner = new StepwisePlanner(_kernel, stepwisePlannerConfig);

            //var plan = planner.CreatePlan($"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request (the \"{username}\"). The current date is \"{currentDate}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question.");

            //FunctionResult? response = await plan.InvokeAsync(request.Message);

            //return new Response
            //{
            //    FunctionCount = response.Metadata["functionCount"].ToString() ?? "",
            //    Iterations = int.Parse(response.Metadata["iterations"].ToString() ?? ""),
            //    StepCount = int.Parse(response.Metadata["stepCount"].ToString() ?? ""),
            //    OpenAIMessages = JsonSerializer.Deserialize<List<OpenAIMessage>>(response.Metadata["stepsTaken"].ToString() ?? "") ?? []
            //}; 
            #endregion

        }

        private void Kernel_FunctionInvoked(object? sender, FunctionInvokedEventArgs e)
        {
            Console.WriteLine(e);
            _chatHistoryFromEventHandler.Add(new ChatHistoryItem
            {
                Content = e.Result.ToString(),
                PromptTokens = (e.Metadata?.GetValueOrDefault("Usage") as Azure.AI.OpenAI.CompletionsUsage)?.PromptTokens ?? 0,
                CompletionTokens = (e.Metadata?.GetValueOrDefault("Usage") as Azure.AI.OpenAI.CompletionsUsage)?.CompletionTokens ?? 0,
                TotalTokens = (e.Metadata?.GetValueOrDefault("Usage") as Azure.AI.OpenAI.CompletionsUsage)?.TotalTokens ?? 0,
                FunctionName = e.Function.Name,
                FunctionArguments = string.Join(", ", e.Arguments)
            });
        }

        private void Kernel_FunctionInvoking(object? sender, FunctionInvokingEventArgs e)
        {
            Console.WriteLine(e);
        }

        private void Kernel_PromptRendered(object? sender, PromptRenderedEventArgs e)
        {
            Console.WriteLine(e);
        }

        private void Kernel_PromptRendering(object? sender, PromptRenderingEventArgs e)
        {
            Console.WriteLine(e);
        }

        private Response ParseResponse(FunctionCallingStepwisePlannerResult functionCallingStepwisePlannerResult)
        {
            var response = new Response
            {
                Iterations = functionCallingStepwisePlannerResult.Iterations,
                FinalAnswer = functionCallingStepwisePlannerResult.FinalAnswer,
                ChatHistory = ParseChatHistory(functionCallingStepwisePlannerResult.ChatHistory!),
            };

            return response;
        }

        private static List<ChatHistoryItem> ParseChatHistory(ChatHistory chatHistory)
        {
            var chatHistoryItems = new List<ChatHistoryItem>();

            //foreach (var item in _chatHistoryFromEventHandler)
            //{
            //    if (item.CompletionTokens > 0) {
            //        var newItem = item;
            //        newItem.Role = "assistant";
            //        chatHistoryItems.Add(item);
            //    }
            //}

            foreach (var item in chatHistory)
            {
                if (item.Role != AuthorRole.User)
                {
                    var chatHistoryItem = new ChatHistoryItem
                    {
                        Content = item.Content ?? "",
                        Role = item.Role.Label,
                        PromptTokens = (item.Metadata?.GetValueOrDefault("Usage") as Azure.AI.OpenAI.CompletionsUsage)?.PromptTokens ?? 0,
                        CompletionTokens = (item.Metadata?.GetValueOrDefault("Usage") as Azure.AI.OpenAI.CompletionsUsage)?.CompletionTokens ?? 0,
                        TotalTokens = (item.Metadata?.GetValueOrDefault("Usage") as Azure.AI.OpenAI.CompletionsUsage)?.TotalTokens ?? 0,
                        FunctionName = (item.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls") as List<Azure.AI.OpenAI.ChatCompletionsFunctionToolCall>)?[0].Name ?? "",
                        FunctionArguments = (item.Metadata?.GetValueOrDefault("ChatResponseMessage.FunctionToolCalls") as List<Azure.AI.OpenAI.ChatCompletionsFunctionToolCall>)?[0].Arguments ?? ""
                    };
                    chatHistoryItems.Add(chatHistoryItem);
                }
            }
            return chatHistoryItems;
        }        
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
