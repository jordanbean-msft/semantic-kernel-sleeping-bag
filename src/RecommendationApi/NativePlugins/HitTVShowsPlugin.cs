using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace RecommendationApi.Plugins
{
    public class HitTVShowsPlugin(DaprClient daprClient)
    {
        [KernelFunction, Description("Given a integer show year, returns tv shows (includes their title, actor names, tags & show years).")]
        public async Task<string> HitTVShowsLookupAsync(int showYear, ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogDebug("HitTVShowsLookupAsync: {showYear}", showYear);
            var result = await daprClient.InvokeMethodAsync<string[]>(HttpMethod.Get, "hit-tv-shows-lookup", $"hitTVShows?show={showYear}", cancellationToken);

            return JsonSerializer.Serialize(result);
        }
    }
}
