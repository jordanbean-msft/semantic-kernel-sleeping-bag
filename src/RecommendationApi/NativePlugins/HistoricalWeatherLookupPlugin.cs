using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RecommendationApi.Plugins
{
    public class HistoricalWeatherLookupPlugin
    {
        private readonly DaprClient _daprClient;
        public HistoricalWeatherLookupPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Get the historical weather for a location for a month. Make sure and pass in the month of the year the user requested. The weather temperatures will be returned in Fahrenheit. Not Found will be returned if the GPS latitude & longitude or the month of the year are not valid. Make sure and pass in valid GPS latitude & longitude for the location requested by the user. If you don't know the GPS latitude & longitude for a given location, look it up.")]
        public async Task<string> HistoricalWeatherLookupAsync(
            [Description("The latitude of the GPS location to lookup historical weather for. This should be a string, not JSON.")] double latitude,
            [Description("The longitude of the GPS location to lookup historical weather for. This should be a string, not JSON.")] double longitude,
            [Description("The integer month of the year to lookup historical weather for")] int monthOfYear,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={latitude}&longitude={longitude}&monthOfYear={monthOfYear}");
            HttpResponseMessage result = await _daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

            return await result.Content.ReadAsStringAsync();
        }
    }
}
