using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Handlebars;
using RecommendationApi.Models;
using RecommendationApi.Plugins;
using System.Text.Json;

#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace RecommendationApi.Services
{
    public class RecommendationService
    {
        private readonly Kernel _kernel;

        public RecommendationService(Kernel kernel)
        {
            _kernel = kernel;

            _kernel.ImportPluginFromType<HistoricalWeatherLookupPlugin>();
            _kernel.ImportPluginFromType<LocationLookupPlugin>();
            _kernel.ImportPluginFromType<OrderHistoryPlugin>();
            _kernel.ImportPluginFromType<ProductCatalogPlugin>();
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var username = "dkschrute";
            var currentDate = DateTime.Now.ToString("MMM-dd-yyyy");

            #region FunctionCallingStepwisePlanner
            var config = new FunctionCallingStepwisePlannerConfig
            {
                MaxIterations = 10,
                MaxTokens = 4000,
                ExecutionSettings = new OpenAIPromptExecutionSettings
                {
                    ChatSystemPrompt = $"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request {username}. The current date is {currentDate}. If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question.",
                    FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions,
                    MaxTokens = 4000
                }
            };

            var planner = new FunctionCallingStepwisePlanner(config);

            var response = await planner.ExecuteAsync(_kernel, request.Message);
            return new Response
            {
                OpenAIMessages = [new OpenAIMessage { FinalAnswer = response.FinalAnswer }]
            };
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
    }
}
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
