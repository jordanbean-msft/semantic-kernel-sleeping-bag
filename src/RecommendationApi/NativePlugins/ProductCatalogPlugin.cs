using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RecommendationApi.Plugins
{
    public class ProductCatalogPlugin
    {
        [KernelFunction, Description("Get the product specifications. This includes data such as length, highest & lowest supported temperature in Fahrenheit")]
        public async Task<string> ProductCatalogItemLookupAsync(
            [Description("The string product ID of the product. This should be a string, not JSON.")] string productId, 
            DaprClient daprClient,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogDebug("ProductCatalogItemLookupAsync called with productId {productId}", productId);

            var httpRequest = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "product-catalog", $"productCatalog/{productId}");
            HttpResponseMessage result = await daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

            return await result.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
