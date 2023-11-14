using Microsoft.AspNetCore.Mvc;
using RecommendationApi.Models;
using RecommendationApi.Services;

namespace RecommendationApi.Extensions
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
            Response response = new Response();
            try
            {
                response = await recommendationService.ResponseAsync(request);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }

            return TypedResults.Ok(response);
        }
    }
}
