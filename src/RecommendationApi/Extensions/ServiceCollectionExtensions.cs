using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using RecommendationApi.Services;

namespace RecommendationApi.Extensions
{
#pragma warning disable SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddAzureServices(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var endpoint = config["OpenAI:Endpoint"];
                ArgumentNullException.ThrowIfNull(endpoint, "OpenAI:Endpoint is required");

                OpenAIClient? client = null;

                if (bool.Parse(config["OpenAI:UseApiKey"]!))
                {
                    var apiKey = config["OpenAI:ApiKey"];
                    ArgumentNullException.ThrowIfNull(apiKey, "OpenAI:ApiKey is required");

                    client = new OpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
                }
                else
                {
                    var tenantId = config["EntraID:TenantId"];
                    ArgumentNullException.ThrowIfNull(tenantId, "EntraID:TenantId is required");

                    var defaultAzureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        TenantId = tenantId
                    });

                    client = new OpenAIClient(new Uri(endpoint), defaultAzureCredential);
                }

                return client;
            });

            services.AddScoped<RecommendationService>();

            services.AddSingleton(memory =>
            {
                var config = memory.GetRequiredService<IConfiguration>();

                var embeddingDeploymentName = config["OpenAI:EmbeddingDeploymentName"];
                ArgumentNullException.ThrowIfNull(embeddingDeploymentName, "OpenAI:EmbeddingDeploymentName is required");

                var embeddingModelId = config["OpenAI:EmbeddingModelId"];
                ArgumentNullException.ThrowIfNull(embeddingModelId, "OpenAI:EmbeddingModelId is required");

                return AddMemory(config, embeddingDeploymentName, embeddingModelId);
            });

            services.AddScoped(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();

                var config = sp.GetRequiredService<IConfiguration>();

                string? connectionString = config["ApplicationInsights:ConnectionString"];

                if (connectionString != null)
                {
                    kernelBuilder.Services.AddLogging(configure =>
                    {
                        configure.AddApplicationInsights(configureTelemetryConfiguration: (telemetryConfiguration) =>
                        {

                            telemetryConfiguration.ConnectionString = connectionString;
                        }, configureApplicationInsightsLoggerOptions: (options) => { });
                    });
                }

                AddChatCompletion(sp, kernelBuilder, config);

                AddTextEmbedding(sp, kernelBuilder, config);                

                kernelBuilder.Services.AddDaprClient();

                kernelBuilder.Services.AddSingleton(sp.GetRequiredService<ISemanticTextMemory>());

                return kernelBuilder.Build();
            });

            return services;
        }

        private static void AddTextEmbedding(IServiceProvider sp, IKernelBuilder kernelBuilder, IConfiguration config)
        {
            var embeddingDeploymentName = config["OpenAI:EmbeddingDeploymentName"];
            ArgumentNullException.ThrowIfNull(embeddingDeploymentName, "OpenAI:EmbeddingDeploymentName is required");

            var embeddingModelId = config["OpenAI:EmbeddingModelId"];
            ArgumentNullException.ThrowIfNull(embeddingModelId, "OpenAI:EmbeddingModelId is required");

            kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(embeddingDeploymentName, sp.GetRequiredService<OpenAIClient>());
        }

        private static ISemanticTextMemory AddMemory(IConfiguration config, string embeddingDeploymentName, string embeddingModelId)
        {
            var endpoint = config["OpenAI:Endpoint"];
            ArgumentNullException.ThrowIfNull(endpoint, "OpenAI:Endpoint is required");

            ISemanticTextMemory? semanticTextMemory;

            if (bool.Parse(config["OpenAI:UseApiKey"]!))
            {
                var apiKey = config["OpenAI:ApiKey"];
                ArgumentNullException.ThrowIfNull(apiKey, "OpenAI:ApiKey is required");

                semanticTextMemory = new MemoryBuilder()
                    .WithAzureOpenAITextEmbeddingGeneration(embeddingDeploymentName, endpoint, apiKey, embeddingModelId)
                    .WithMemoryStore(new VolatileMemoryStore())
                    .Build();
            }
            else
            {
                var tenantId = config["EntraID:TenantId"];
                ArgumentNullException.ThrowIfNull(tenantId, "EntraID:TenantId is required");

                var defaultAzureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    TenantId = tenantId
                });

                semanticTextMemory = new MemoryBuilder()
                    .WithAzureOpenAITextEmbeddingGeneration(embeddingDeploymentName, endpoint, defaultAzureCredential, embeddingModelId)
                    .WithMemoryStore(new VolatileMemoryStore())
                    .Build();
            }

            return semanticTextMemory;
        }

        private static void AddChatCompletion(IServiceProvider sp, IKernelBuilder kernelBuilder, IConfiguration config)
        {
            var chatCompletionDeploymentName = config["OpenAI:ChatCompletionDeploymentName"];
            ArgumentNullException.ThrowIfNull(chatCompletionDeploymentName, "OpenAI:ChatCompletionDeploymentName is required");

            var chatCompletionModelId = config["OpenAI:ChatCompletionModelId"];
            ArgumentNullException.ThrowIfNull(chatCompletionModelId, "OpenAI:ChatCompletionModelId is required");

            kernelBuilder.AddAzureOpenAIChatCompletion(chatCompletionDeploymentName, sp.GetRequiredService<OpenAIClient>(), modelId: chatCompletionModelId);
        }
    }
#pragma warning restore SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0052 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
