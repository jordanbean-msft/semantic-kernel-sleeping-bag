using Azure.AI.OpenAI;
using Azure.Identity;
using RecommendationApi.Services;

namespace RecommendationApi.Extensions
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

                var openAIClient = new OpenAIClient(new Uri(endpoint), _defaultAzureCredential);

                return openAIClient;
            });

            services.AddSingleton<RecommendationService>();

            return services;
        }
    }
}
