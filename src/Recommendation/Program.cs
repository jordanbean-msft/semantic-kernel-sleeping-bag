using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
               builder =>
               {
                   builder.WithOrigins("http://localhost:3000").WithHeaders("content-type");
               });
});

builder.Services.AddDaprClient();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapPost("/recommendation", async (Request request, [FromServices] DaprClient _daprClient) =>
{
    var historicalWeather = new HistoricalWeather { HighestAmbientTemperature = 0, LowestAmbientTemperature = 0 };

    try
    {
        historicalWeather = await _daprClient.InvokeMethodAsync<HistoricalWeather>(HttpMethod.Get, "historical-weather-lookup", $"historical-weather-lookup?latitude={1234.5678}&longitude={0987.6543}&dateTime={DateTime.Now}");
    }
    catch (Exception ex)
    {
        //return TypedResults.Problem(ex.Message);
    }

    return TypedResults.Ok(new Response
    {
        Message = $"""
    Original message: {request.Message}
    HistoricalWeather.HighestAmbientTemperature: {historicalWeather.HighestAmbientTemperature}
    HistoricalWeather.LowestAmbientTemperature: {historicalWeather.LowestAmbientTemperature}
    """
    });
}).WithName("GetRecommendation")
.WithOpenApi();

app.Run();
