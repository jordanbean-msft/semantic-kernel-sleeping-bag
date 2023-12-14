using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;

//using Microsoft.SemanticKernel.Planning.Handlebars;
using RecommendationApi.Models;
using RecommendationApi.Plugins;
using System.Text.Json;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace RecommendationApi.Services
{
    public class RecommendationService
    {
        private readonly Kernel _kernel;
        //private readonly ISemanticTextMemory _memory;

        public RecommendationService(Kernel kernel)
        {
            _kernel = kernel;
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

            //KernelPlugin relevantRecommendationFunctions = new ("Recommendation");
            //var relevantFunctions = _memory.SearchAsync("functions", request.Message, 30, minRelevanceScore: 0.75);

            //var historicalWeatherLookupPlugin = _kernel.CreatePluginFromType<HistoricalWeatherLookupPlugin>();

            //await foreach(MemoryQueryResult relaventFunction in relevantFunctions)
            //{
            //    historicalWeatherLookupPlugin.TryGetFunction(relaventFunction.Metadata.AdditionalMetadata, out var function);
            //    relevantRecommendationFunctions.AddFunction(function!);
            //}

            //var kernelWithRelevantFunctions = _kernel.Clone();
            //kernelWithRelevantFunctions.Plugins.Add(relevantRecommendationFunctions);

            _kernel.PromptRendering += Kernel_PromptRendering;
            _kernel.PromptRendered += Kernel_PromptRendered;
            _kernel.FunctionInvoking += Kernel_FunctionInvoking;
            _kernel.FunctionInvoked += Kernel_FunctionInvoked;

            #region FunctionCallingStepwisePlanner
            var config = new FunctionCallingStepwisePlannerConfig
            {
                //MaxIterations = 5,
                ExecutionSettings = new OpenAIPromptExecutionSettings
                {
                    ChatSystemPrompt = $"You are a customer support chatbot. You should answer the question posed by the user. Make sure and look up any needed context for the specific user that is making the request {username}. The current date is {currentDate}. If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question.",
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                }                
            };

            var planner = new FunctionCallingStepwisePlanner(config);

            FunctionCallingStepwisePlannerResult response = null;
            List<OpenAIMessage> errorContent = null;

            try
            {
                response = await planner.ExecuteAsync(_kernel, request.Message);
            }
            catch(Exception ex)
            {
                 errorContent = [new OpenAIMessage { FinalAnswer = ex.Message }];
            }

            return new Response
            {
                OpenAIMessages = errorContent ?? ([new OpenAIMessage { FinalAnswer = response.FinalAnswer }])
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

        private void Kernel_FunctionInvoked(object? sender, FunctionInvokedEventArgs e)
        {
            Console.WriteLine(e);
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
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
