using HistoricalWeatherLookup;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

Dictionary<HistoricalWeatherInput, HistoricalWeather> historicalWeather = new()
{
    {
        new HistoricalWeatherInput {
            Latitude = -41.814099,
            Longitude = -68.907384,
            MonthOfYear = 12
        },
        new HistoricalWeather
        {
            HighestExpectedTemperatureInFahrenheit = 100,
            LowestExpectedTemperatureInFahrenheit = 20
        }
    }
};

app.MapGet("/historical-weather-lookup", Results<Ok<HistoricalWeather>, NotFound<string>> (double latitude, double longitude, int monthOfYear) =>
{
    HistoricalWeather historicalWeatherResponse = null;
    historicalWeather.TryGetValue(new HistoricalWeatherInput { Latitude = latitude, Longitude = longitude, MonthOfYear = monthOfYear }, out historicalWeatherResponse);

    return historicalWeatherResponse != null ? TypedResults.Ok(historicalWeatherResponse) : TypedResults.NotFound($"No historical weather found for latitude ${latitude}, longitude ${longitude} & monthOfYear ${monthOfYear}.");
})
.WithName("GetHistoricalWeather")
.WithOpenApi();

app.MapHealthChecks("/healthz");

app.Run();
