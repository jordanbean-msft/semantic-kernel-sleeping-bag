using Azure.AI.OpenAI;
using Dapr.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using Recommendation.Plugins;

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
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var context = _kernel.CreateNewContext();
            var planner = new StepwisePlanner(_kernel);
            var plan = planner.CreatePlan($"Find the historical weather associated with the following GPS coordinates: {request.Message}. Your response should be in JSON format.");
            var response = await plan.InvokeAsync(context);

            return new Response
            {
                Message = response.GetValue<string>()
            };


        }
    }
}
