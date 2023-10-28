using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Recommendation.Extensions
{
    internal static class WebApplicationExtensions
    {
        internal static WebApplication MapApi(this WebApplication app)
        {
            app.MapPost("/recommendation", OnPostRecommendationAsync)
                .WithName("GetRecommendation")
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> OnPostRecommendationAsync(Request request, [FromServices] DaprClient _daprClient)
        {
            HistoricalWeather? historicalWeather;

            try
            {
                historicalWeather = await _daprClient.InvokeMethodAsync<HistoricalWeather>(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={1234.5678}&longitude={0987.6543}&dateTime={DateTime.Now}");
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }

            return TypedResults.Ok(new Response
            {
                Message = $@"
                Original message: {request.Message}
                HistoricalWeather.HighestAmbientTemperature: {historicalWeather.HighestAmbientTemperature}
                HistoricalWeather.LowestAmbientTemperature: {historicalWeather.LowestAmbientTemperature}
                "
            });
        }
    }
}
