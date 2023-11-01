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

        [SKFunction, Description("Given a C# string username, returns the order history for that user, including all the products they purchased (which includes the product ID).")]
        public async Task<string> OrderHistoryLookup(string username)
        {
            var result = await _daprClient.InvokeMethodAsync<OrderHistory>(HttpMethod.Get, "order-history", $"orderHistory/{username}");

            return JsonSerializer.Serialize(result);
        }
    }
}
