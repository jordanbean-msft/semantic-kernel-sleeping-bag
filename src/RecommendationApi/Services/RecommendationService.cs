﻿using Azure.AI.OpenAI;
using Azure.Identity;
using Dapr.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planners;
using RecommendationApi.Models;
using RecommendationApi.Plugins;
using System.Text.Json;

namespace RecommendationApi.Services
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

            var kernelBuilder = new KernelBuilder().WithAzureOpenAIChatCompletionService(deployedModelName, client);
            var embeddingModelName = _configuration["OpenAI:EmbeddingModelName"];

            if (!string.IsNullOrWhiteSpace(embeddingModelName))
            {
                var endpoint = configuration["OpenAI:Endpoint"];
                ArgumentNullException.ThrowIfNull(endpoint, "OpenAI:Endpoint is required");

                kernelBuilder = kernelBuilder.WithAzureOpenAITextEmbeddingGenerationService(embeddingModelName, endpoint, new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    TenantId = configuration["EntraID:TenantId"]
                }));
            }

            _kernel = kernelBuilder.WithLoggerFactory(_loggerFactory).Build();
            RegisterPlugins();
        }

        private void RegisterPlugins()
        {
            _kernel.ImportFunctions(new HistoricalWeatherLookupPlugin(_daprClient), "HistoricalWeatherLookupPlugin");
            _kernel.ImportFunctions(new OrderHistoryPlugin(_daprClient), "OrderHistoryPlugin");
            _kernel.ImportFunctions(new ProductCatalogPlugin(_daprClient), "ProductCatalogPlugin");
            _kernel.ImportFunctions(new LocationLookupPlugin(_daprClient), "LocationLookupPlugin");
        }

        public async Task<Response> ResponseAsync(Request request)
        {
            var contextVariables = new ContextVariables
            {
                ["username"] = "dkschrute",
                ["current_date"] = DateTime.Now.ToString("MMM-dd-yyyy"),
                ["message"] = request.Message
            };

            var context = _kernel.CreateNewContext(contextVariables);

            var planner = new StepwisePlanner(_kernel, new StepwisePlannerConfig
            {
                MaxIterations = 20
            });

            var plan = planner.CreatePlan("You are a customer support chatbot. You should answer the question posed by the user in the \"{{$message}}\". Make sure and look up any needed context for the specific user that is making the request (the \"{{$username}}\"). The current date is \"{{$current_date}}\". If you don't know the answer, respond saying you don't know. Only use the plugins that are registered to help you answer the question.");

            FunctionResult? response = await plan.InvokeAsync(context);

            return new Response
            {
                FunctionCount = response.Metadata["functionCount"].ToString() ?? "",
                Iterations = int.Parse(response.Metadata["iterations"].ToString() ?? ""),
                StepCount = int.Parse(response.Metadata["stepCount"].ToString() ?? ""),
                OpenAIMessages = JsonSerializer.Deserialize<List<OpenAIMessage>>(response.Metadata["stepsTaken"].ToString() ?? "") ?? []
            };
        }
    }
}
