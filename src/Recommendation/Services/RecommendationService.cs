using Azure.AI.OpenAI;
using Dapr.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using Recommendation.Plugins;
using System.Text.Json;

namespace Recommendation.Services
{
    public class RecommendationService
    {
        private readonly IKernel _kernel;
        private readonly IConfiguration _configuration;
        private readonly DaprClient _daprClient;
        private readonly ILoggerFactory _loggerFactory;

        public RecommendationService(OpenAIClient client, IConfiguration configuration, DaprClient daprClient, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _daprClient = daprClient;
            _loggerFactory = loggerFactory;

            var deployedModelName = _configuration["OpenAI:ChatModelName"];

            ArgumentNullException.ThrowIfNull(deployedModelName, "OpenAI:ChatModelName is required");

            var kernelBuilder = Kernel.Builder.WithAzureChatCompletionService(deployedModelName, client);
            var embeddingModelName = _configuration["OpenAI:EmbeddingModelName"];

            if (!string.IsNullOrWhiteSpace(embeddingModelName))
            {
                var endpoint = configuration["OpenAI:Endpoint"];
                ArgumentNullException.ThrowIfNull(endpoint, "OpenAI:Endpoint is required");

                var key = configuration["OpenAI:Key"];
                ArgumentNullException.ThrowIfNull(key, "OpenAI:Key is required");

                kernelBuilder = kernelBuilder.WithAzureTextEmbeddingGenerationService(embeddingModelName, endpoint, key);
            }

            _kernel = kernelBuilder.WithLoggerFactory(loggerFactory).Build();
            RegisterPlugins();
            _loggerFactory = loggerFactory;
        }

        private void RegisterPlugins()
        {
            _kernel.ImportFunctions(new HistoricalWeatherLookupPlugin(_daprClient), "HistoricalWeatherLookupPlugin");
            _kernel.ImportFunctions(new LocationLookupPlugin(_daprClient), "LocationLookupPlugin");
            _kernel.ImportFunctions(new OrderHistoryPlugin(_daprClient), "OrderHistoryPlugin");
            _kernel.ImportFunctions(new ProductCatalogPlugin(_daprClient), "ProductCatalogPlugin");
            _kernel.ImportSemanticFunctionsFromDirectory("SemanticPlugins/Recommendation");
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var context = _kernel.CreateNewContext();
            var planner = new StepwisePlanner(_kernel);
            var username = "jordanbean";
            var plan = planner.CreatePlan($"You are a customer support chatbot. The username is {username}. The current month is {DateTime.Now.Month} You should answer the question posed by the user here: {request.Message}.");
            var response = await plan.InvokeAsync(context);

            return new Response
            {
                FunctionCount = response.Metadata["functionCount"].ToString(),
                Iterations = int.Parse(response.Metadata["iterations"].ToString()),
                StepCount = int.Parse(response.Metadata["stepCount"].ToString()),
                OpenAIMessages = JsonSerializer.Deserialize<List<OpenAIMessage>>(response.Metadata["stepsTaken"].ToString())
            };
        }
    }
}
