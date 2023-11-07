using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Recommendation.Plugins
{
    public class HistoricalWeatherLookupPlugin
    {
        private readonly DaprClient _daprClient;
        public HistoricalWeatherLookupPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Get the historical weather for a location for a month. Make sure and pass in the month of the year the user requested.")]
        public async Task<string> HistoricalWeatherLookup(
            [Description("The double latitude of the location to lookup historical weather for")] double latitude,
            [Description("The double longitude of the location to lookup historical weather for")] double longitude,
            [Description("The int month of the year to lookup historical weather for")] int monthOfYear)
        {
            var result = await _daprClient.InvokeMethodAsync<HistoricalWeather>(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={latitude}&longitude={longitude}&monthOfYear={monthOfYear}");

            return JsonSerializer.Serialize(result);
        }
    }
}
