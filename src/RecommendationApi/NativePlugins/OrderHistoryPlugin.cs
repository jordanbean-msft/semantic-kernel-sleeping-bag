﻿using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RecommendationApi.Plugins
{
    public class OrderHistoryPlugin
    {
        private readonly DaprClient _daprClient;
        public OrderHistoryPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Get the order history for a user, including all the products they purchased (which includes the product ID). This is a list of what the user owns.")]
        public async Task<string> OrderHistoryLookupAsync([Description("The string username of the user. There should be no curly braces around the username.")] string username,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogDebug("OrderHistoryLookupAsync called with username {username}", username);

            var httpRequest = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "order-history", $"orderHistory/{username}");
            HttpResponseMessage result = await _daprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

            return await result.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
