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

        [SKFunction, Description("Get the historical weather for a location for a month. Make sure and pass in the month of the year the user requested. The weather temperatures will be returned in Fahrenheit.")]
        public async Task<string> HistoricalWeatherLookup(
            [Description("The double latitude of the GPS location to lookup historical weather for")] double latitude,
            [Description("The double longitude of the GPS location to lookup historical weather for")] double longitude,
            [Description("The int month of the year to lookup historical weather for")] int monthOfYear)
        {
            var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={latitude}&longitude={longitude}&monthOfYear={monthOfYear}");
            HttpResponseMessage result = await _daprClient.InvokeMethodWithResponseAsync(httpRequest);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            else
            {
                return result.ReasonPhrase;
            }
        }
    }
}
