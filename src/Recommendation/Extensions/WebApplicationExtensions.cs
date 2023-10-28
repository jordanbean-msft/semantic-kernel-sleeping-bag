using Microsoft.AspNetCore.Mvc;
using Recommendation.Services;

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

        private static async Task<IResult> OnPostRecommendationAsync(Request request, [FromServices] RecommendationService recommendationService)
        {
            HistoricalWeather? historicalWeather = new HistoricalWeather { HighestAmbientTemperature = 0, LowestAmbientTemperature = 0 };

            try
            {
                var response = await recommendationService.ResponseAsync(request);
                //historicalWeather = response.Message
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
