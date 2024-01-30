using Humanizer;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.Memory;
using RecommendationApi.Plugins;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace RecommendationApi.NativePlugins
{
#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class CustomerServicePlugin
    {
        private readonly ILogger _logger;

        public CustomerServicePlugin(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerServicePlugin>();
        }

        [KernelFunction, Description("Answer the user question. Be sure to use any plugins necessary to get additional information. Use this plugin if you can't completely answer the user question.")]
        [return: Description("The answer to the user question.")]
        public async Task<string> AnswerQuestionAsync(
            Kernel kernel,
            [Description("The user question. Make sure and pass in the whole user question and a summary of any previous conversation.")] string goal,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            var username = "dkschrute";
            var currentDate = DateTime.Now.ToString("MMM-dd-yyyy");

            var localKernel = kernel.Clone();

            //prevent infinite loops
            localKernel.Plugins.Remove(localKernel.Plugins["CustomerServicePlugin"]);
            
            localKernel.ImportPluginFromType<HistoricalWeatherLookupPlugin>();
            localKernel.ImportPluginFromType<LocationLookupPlugin>();
            localKernel.ImportPluginFromType<OrderHistoryPlugin>();
            localKernel.ImportPluginFromType<ProductCatalogPlugin>();
            localKernel.ImportPluginFromType<TextMemoryPlugin>();
            //localKernel.ImportPluginFromType<ConversationSummaryPlugin>();

            var config = new FunctionCallingStepwisePlannerConfig
            {
                ExecutionSettings = new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                }
            };

            var planner = new FunctionCallingStepwisePlanner(config);

            FunctionCallingStepwisePlannerResult? response = null;

            try
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

                //string systemMessage = await promptTemplateFactory.Create(new PromptTemplateConfig("You are a customer support chatbot. You should answer the question posed by the user. Ground your answers based upon the user's purchase history. If you don't know the answer, respond saying you don't know. Make sure and use the CustomerServicePlugin to help you answer the question if the user doesn't provide all the needed information. If you can't answer the question, try and use the CustomerServicePlugin to get a better, more complete answer. Order History: {{ recall $username }} Current Date: {{ $current_date }}")).RenderAsync(_kernel, arguments);
                string prompt = await promptTemplateFactory.Create(new PromptTemplateConfig("You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request. Order History: {{ recall $username }}. The current date is {{ $currentDate }}. If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question. If the TextMemoryPlugin doesn't have answer, use the other plugins to answer the question. The user question is {{ $goal }}")).RenderAsync(localKernel, arguments);

                response = await planner.ExecuteAsync(localKernel, prompt, cancellationToken);
                //response = await planner.ExecuteAsync(localKernel, $"", cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing planner");
                //return ex.Message;
            }

            //if (response == null)
            //{
            //    //return "I'm sorry, I don't know the answer to that question.";
            //}
            //else
            //{
                return SerializeChatHistory(response.ChatHistory);
                //return response.FinalAnswer;
                //return response;
            //}
        }

        private string SerializeChatHistory(ChatHistory chatHistory)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(var chatHistoryItem in chatHistory)
            {
                stringBuilder.AppendLine($"Role: {chatHistoryItem.Role} - Content: {chatHistoryItem.Content}");
            }

            return stringBuilder.ToString();
        }
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
