using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RecommendationApi.Plugins
{
    public class ProductCatalogPlugin
    {
        private readonly DaprClient _daprClient;
        public ProductCatalogPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Get the product specifications. This includes data such as length, highest & lowest supported temperature in Fahrenheit")]
        public async Task<string> ProductCatalogItemLookupAsync([Description("The string product ID of the product. This should be a string, not JSON.")] string productId, 
            ILogger logger,
            CancellationToken cancellationToken)
        {
            var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "product-catalog", $"productCatalog/{productId}");
            HttpResponseMessage result = await _daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

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
