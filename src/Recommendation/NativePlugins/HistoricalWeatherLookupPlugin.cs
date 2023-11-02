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

        [SKFunction, Description("Given a string latitude, string longitude, and integer month of the year, returns the historical weather for that location for that month. The latitude & longitude and the specific GPS coordinates for a given location. Make sure and pass in the user requested month of the year.")]
        public async Task<string> HistoricalWeatherLookup(double latitude, double longitude, int monthOfYear)
        {
            var result = await _daprClient.InvokeMethodAsync<HistoricalWeather>(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={latitude}&longitude={longitude}&monthOfYear={monthOfYear}");

            return JsonSerializer.Serialize(result);
        }
    }
}
