﻿using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Recommendation.Plugins
{
    public class ProductCatalogPlugin
    {
        private readonly DaprClient _daprClient;
        public ProductCatalogPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Get the product specifications. This includes data such as length, highest & lowest supported temperature in Fahrenheit")]
        public async Task<string> ProductCatalogItemLookup([Description("The string product ID of the product")] string productId)
        {
            var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "product-catalog", $"productCatalog/{productId}");
            HttpResponseMessage result = await _daprClient.InvokeMethodWithResponseAsync(httpRequest);

            return JsonSerializer.Serialize(result.Content.ReadAsStringAsync());
        }
    }
}
