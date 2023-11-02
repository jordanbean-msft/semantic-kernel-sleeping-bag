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

        [SKFunction, Description("Given a string product ID, return the product specifications. This includes data such as lowest recommended temperature")]
        public async Task<string> ProductCatalogItemLookup(string productId)
        {
            var result = await _daprClient.InvokeMethodAsync<ProductCatalogItem>(HttpMethod.Get, "product-catalog", $"productCatalog/{productId}");

            return JsonSerializer.Serialize(result);
        }
    }
}
