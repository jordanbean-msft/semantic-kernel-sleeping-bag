using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using RecommendationApi.Plugins;
using RecommendationApi.Services;

#pragma warning disable SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace RecommendationApi.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private static DefaultAzureCredential? _defaultAzureCredential;

        internal static IServiceCollection AddAzureServices(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                _defaultAzureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    TenantId = config["EntraID:TenantId"]
                });

                var endpoint = config["OpenAI:Endpoint"];

                ArgumentNullException.ThrowIfNull(endpoint, "OpenAI:Endpoint is required");

                var client = new OpenAIClient(new Uri(endpoint), _defaultAzureCredential);

                return client;
            });

            services.AddScoped<RecommendationService>();          

            services.AddScoped(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var chatCompletionDeploymentName = config["OpenAI:ChatCompletionDeploymentName"];
                ArgumentNullException.ThrowIfNull(chatCompletionDeploymentName, "OpenAI:ChatCompletionDeploymentName is required");

                var chatCompletionModelId = config["OpenAI:ChatCompletionModelId"];
                ArgumentNullException.ThrowIfNull(chatCompletionModelId, "OpenAI:ChatCompletionModelId is required");

                var kernelBuilder = new KernelBuilder();

                kernelBuilder.Services.AddLogging(configure =>
                {
                    configure.AddApplicationInsights(configureTelemetryConfiguration: (telemetryConfiguration) =>
                    {
                        var connectionString = config["ApplicationInsights:ConnectionString"];
                        ArgumentNullException.ThrowIfNull(connectionString, "ApplicationInsights:ConnectionString is required");

                        telemetryConfiguration.ConnectionString = connectionString;
                    }, configureApplicationInsightsLoggerOptions: (options) => { });
                });
                
                var embeddingDeploymentName = config["OpenAI:EmbeddingDeploymentName"];
                ArgumentNullException.ThrowIfNull(embeddingDeploymentName, "OpenAI:EmbeddingDeploymentName is required");

                var embeddingModelId = config["OpenAI:EmbeddingModelId"];
                ArgumentNullException.ThrowIfNull(embeddingModelId, "OpenAI:EmbeddingModelId is required");

                kernelBuilder.Services.AddAzureOpenAIChatCompletion(chatCompletionDeploymentName, chatCompletionModelId, sp.GetRequiredService<OpenAIClient>());
                kernelBuilder.Services.AddAzureOpenAITextEmbeddingGeneration(embeddingDeploymentName, embeddingModelId, sp.GetRequiredService<OpenAIClient>());

                return kernelBuilder.Build();
            });

            return services;
        }
    }
}
#pragma warning restore SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
