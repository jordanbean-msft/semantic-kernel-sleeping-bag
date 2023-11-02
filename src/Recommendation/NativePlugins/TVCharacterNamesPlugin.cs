using Dapr.Client;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Recommendation.Plugins
{
    public class TVCharacterNamesPlugin
    {
        private readonly DaprClient _daprClient;
        public TVCharacterNamesPlugin(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [SKFunction, Description("Returns the names of common TV characters.")]
        public async Task<string> TVCharacterNamesLookup()
        {
            var result = await _daprClient.InvokeMethodAsync<string[]>(HttpMethod.Get, "tv-character-names-lookup", "tvCharacterNames");

            return JsonSerializer.Serialize(result);
        }
    }
}
