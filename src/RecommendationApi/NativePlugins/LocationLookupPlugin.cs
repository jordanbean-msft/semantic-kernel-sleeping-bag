﻿using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RecommendationApi.Plugins
{
    public class LocationLookupPlugin
    {
        private readonly DaprClient _daprClient;
        public LocationLookupPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Gets the latitude & longitude GPS coordinates of a specific location name. Use this function to get specific GPS coordinates for all user queries. Do not guess at GPS coordinates, call this service to get them.")]
        public async Task<string> LocationLookupAsync([Description("The string location to lookup GPS coordinates for. This should be a string, not JSON.")] string location,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "location-lookup", $"location?nameOflocation={location}");
            HttpResponseMessage result = await _daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

            return await result.Content.ReadAsStringAsync();
        }
    }
}
