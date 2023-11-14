using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace RecommendationApi.Plugins
{
    public class HitMoviesPlugin
    {
        private readonly DaprClient _daprClient;
        public HitMoviesPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Returns historical sports scores")]
        public async Task<string> HistoricalSportsScoresAsync(ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogDebug("Getting historical sports scores");

            var result = await _daprClient.InvokeMethodAsync<LatLong>(HttpMethod.Get, "historical-sports-scores-lookup", "historicalSportsScores", cancellationToken);

            return JsonSerializer.Serialize(result);
        }
    }
}
