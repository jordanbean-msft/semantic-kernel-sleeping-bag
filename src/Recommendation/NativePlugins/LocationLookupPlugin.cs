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

        [SKFunction, Description("Given a string name of a location, returns the latitude & longitude GPS coordinates of that location.")]
        public async Task<string> LocationLookup(string location)
        {
            var result = await _daprClient.InvokeMethodAsync<LatLong>(HttpMethod.Get, "location-lookup", $"location?nameOflocation={location}");

            return JsonSerializer.Serialize(result);
        }
    }
}
