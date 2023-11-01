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

        [SKFunction, Description("Given a C# double latitude, C# double longitude, and C# int month of the year, returns the historical weather for that location.")]
        public async Task<string> HistoricalWeatherLookup(double latitude, double longitude, int monthOfYear)
        {
            var result = await _daprClient.InvokeMethodAsync<HistoricalWeather>(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={latitude}&longitude={longitude}&monthOfYear={monthOfYear}");

            return JsonSerializer.Serialize(result);
        }
    }
}
