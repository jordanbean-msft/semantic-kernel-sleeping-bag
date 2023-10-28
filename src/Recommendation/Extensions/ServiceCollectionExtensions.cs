using Azure.AI.OpenAI;
using Azure.Identity;

namespace Recommendation.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private static readonly DefaultAzureCredential _defaultAzureCredential = new();

        internal static IServiceCollection AddAzureServices(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var endpoint = config["OpenAI:Endpoint"];

                ArgumentNullException.ThrowIfNull(endpoint, "OpenAI:Endpoint is required");

                var openAIClient = new OpenAIClient(new Uri(endpoint), _defaultAzureCredential);

                return openAIClient;
            });

            return services;
        }
    }
}
