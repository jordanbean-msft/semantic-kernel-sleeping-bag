using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RecommendationApi.Plugins
{
    public class LocationLookupPlugin(DaprClient daprClient)
    {
        [KernelFunction, Description("Gets the latitude & longitude GPS coordinates of a specific location name. Use this function to get specific GPS coordinates for all user queries. Do not guess at GPS coordinates, call this service to get them.")]
        [return: Description("The GPS latitude & longitude coordinates for the location requested by the user. Not Found will be returned if the location name is not valid.")]
        public async Task<string> LocationLookupAsync(
            [Description("The string location to lookup GPS coordinates for. This should be a string, not JSON.")] string location,
            ILogger logger,
            CancellationToken cancellationToken)
        {
#pragma warning disable CA2254 // Template should be a static expression
            logger.LogDebug($"LocationLookupPlugin.LocationLookupAsync: {location}");
#pragma warning restore CA2254 // Template should be a static expression

            var httpRequest = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "location-lookup", $"location?nameOflocation={location}");
            HttpResponseMessage result = await daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

            return await result.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
