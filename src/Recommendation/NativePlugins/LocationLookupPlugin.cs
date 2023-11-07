﻿using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Recommendation.Plugins
{
    public class LocationLookupPlugin
    {
        private readonly DaprClient _daprClient;
        public LocationLookupPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Gets the latitude & longitude GPS coordinates of a specific location name. Use this function to get specific GPS coordinates for all user queries. Do not guess at GPS coordinates, call this service to get them.")]
        public async Task<string> LocationLookup([Description("The string location to lookup GPS coordinates for")] string location)
        {
            var result = await _daprClient.InvokeMethodAsync<LatLong>(HttpMethod.Get, "location-lookup", $"location?nameOflocation={location}");

            return JsonSerializer.Serialize(result);
        }
    }
}
