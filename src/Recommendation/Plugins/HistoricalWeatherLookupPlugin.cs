using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Recommendation.Plugins
{
    public class HistoricalWeatherLookupPlugin
    {
        private readonly DaprClient _daprClient;
        public HistoricalWeatherLookupPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Given a latitude, longitude, and date/time, returns the historical weather for that location.")]
        public async Task<string> HistoricalWeatherLookup(double latitude, double longitude, DateTime dateTime)
        {
            var result = await _daprClient.InvokeMethodAsync<HistoricalWeather>(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={1234.5678}&longitude={0987.6543}&dateTime={DateTime.Now}");

            return result.HighestAmbientTemperature.ToString();
        }
    }
}
