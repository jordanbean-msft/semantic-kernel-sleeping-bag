using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.Memory;
using RecommendationApi.Plugins;
using System.ComponentModel;
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

        [KernelFunction, Description("Answer the user's question. Be sure to use any plugins necessary to get additional information. Use this plugin if other plugins fail to answer the user's question")]
        [return: Description("The answer to the user's question.")]
        public async Task<FunctionCallingStepwisePlannerResult> AnswerQuestionAsync(
            Kernel kernel,
            [Description("The user's question. Make sure and pass in the whole user question and a summary of any previous conversation.")] string goal,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            var username = "dkschrute";
            var currentDate = DateTime.Now.ToString("MMM-dd-yyyy");

            var localKernel = kernel.Clone();

            //prevent infinite loops
            localKernel.Plugins.Remove(localKernel.Plugins["CustomerServicePlugin"]);
            
            //localKernel.ImportPluginFromType<HistoricalWeatherLookupPlugin>();
            //localKernel.ImportPluginFromType<LocationLookupPlugin>();
            //localKernel.ImportPluginFromType<OrderHistoryPlugin>();
            //localKernel.ImportPluginFromType<ProductCatalogPlugin>();
            //localKernel.ImportPluginFromType<TextMemoryPlugin>();
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
                response = await planner.ExecuteAsync(localKernel, $"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request. Order History: {{recall {username} }}. The current date is \"{currentDate}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question. The user question is \"{goal}\"", cancellationToken);
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
                //return response.FinalAnswer;
                return response;
            //}
        }
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
