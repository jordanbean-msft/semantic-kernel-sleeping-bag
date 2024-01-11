using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.ApplicationInsights;
using RecommendationApi.Extensions;
using RecommendationApi.Models;
using RecommendationApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var config = builder.Configuration;
string? allowedOrigins = config["Cors:AllowedOrigins"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
               {
                   builder.WithOrigins("http://localhost:3000", allowedOrigins ?? "")
                   .WithHeaders("content-type");
               });
});

builder.Services.AddDaprClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureServices();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapPost("/recommendation", async Task<Results<Ok<Response>, ProblemHttpResult>> (Request request, [FromServices] RecommendationService recommendationService) =>
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
})
.WithName("GetRecommendation")
.WithOpenApi();

app.Run();
