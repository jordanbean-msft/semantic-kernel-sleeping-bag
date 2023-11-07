using Dapr.Client;
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
            var result = await _daprClient.InvokeMethodAsync<ProductCatalogItem>(HttpMethod.Get, "product-catalog", $"productCatalog/{productId}");

            return JsonSerializer.Serialize(result);
        }
    }
}
