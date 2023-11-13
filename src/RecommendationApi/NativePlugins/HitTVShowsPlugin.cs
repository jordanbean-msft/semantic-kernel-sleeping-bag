using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace RecommendationApi.Plugins
{
    public class HitTVShowsPlugin
    {
        private readonly DaprClient _daprClient;
        public HitTVShowsPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Given a integer show year, returns tv shows (includes their title, actor names, tags & show years).")]
        public async Task<string> HitTVShowsLookupAsync(int showYear, ILogger logger,
            CancellationToken cancellationToken)
        {
            var result = await _daprClient.InvokeMethodAsync<string[]>(HttpMethod.Get, "hit-tv-shows-lookup", $"hitTVShows?show={showYear}");

            return JsonSerializer.Serialize(result);
        }
    }
}
