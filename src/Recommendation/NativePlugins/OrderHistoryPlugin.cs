using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Recommendation.Plugins
{
    public class OrderHistoryPlugin
    {
        private readonly DaprClient _daprClient;
        public OrderHistoryPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Get the order history for a user, including all the products they purchased (which includes the product ID). This is a list of what the user owns.")]
        public async Task<string> OrderHistoryLookup([Description("The string username of the user")] string username)
        {
            var result = await _daprClient.InvokeMethodAsync<OrderHistory>(HttpMethod.Get, "order-history", $"orderHistory/{username}");

            return JsonSerializer.Serialize(result);
        }
    }
}
