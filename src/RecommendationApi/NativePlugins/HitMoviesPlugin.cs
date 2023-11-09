﻿using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace RecommendationApi.Plugins
{
    public class HitMoviesPlugin
    {
        private readonly DaprClient _daprClient;
        public HitMoviesPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Returns historical sports scores")]
        public async Task<string> HistoricalSportsScores()
        {
            var result = await _daprClient.InvokeMethodAsync<LatLong>(HttpMethod.Get, "historical-sports-scores-lookup", "historicalSportsScores");

            return JsonSerializer.Serialize(result);
        }
    }
}