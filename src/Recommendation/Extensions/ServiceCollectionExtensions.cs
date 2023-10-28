using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Recommendation.Services;

namespace Recommendation.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private static DefaultAzureCredential _defaultAzureCredential;

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

                var key = config["OpenAI:Key"];

                ArgumentNullException.ThrowIfNull(key, "OpenAI:Key is required");

                var openAIClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));//_defaultAzureCredential);

                return openAIClient;
            });

            services.AddSingleton<RecommendationService>();

            return services;
        }
    }
}
