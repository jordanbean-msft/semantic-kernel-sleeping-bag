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
            MonthOfYear = DateTime.Now.AddMonths(1).Month // always next month
        },
        new HistoricalWeather
        {
            HighestExpectedTemperatureInFahrenheit = 100,
            LowestExpectedTemperatureInFahrenheit = 30
        }
    }
};

app.MapGet("/historical-weather-lookup", Results<Ok<HistoricalWeather>, NotFound<NotFoundMessage>> (double latitude, double longitude, int monthOfYear) =>
{
    historicalWeather.TryGetValue(new HistoricalWeatherInput { Latitude = latitude, Longitude = longitude, MonthOfYear = monthOfYear }, out HistoricalWeather? historicalWeatherResponse);

    return historicalWeatherResponse != null ? TypedResults.Ok(historicalWeatherResponse) : TypedResults.NotFound(new NotFoundMessage { 
        Message = $"Not Found: No historical weather found for latitude {latitude}, longitude {longitude} & monthOfYear {monthOfYear}. Make sure this is the correct GPS latitude, longitude & month of the year." 
    });
})
.WithName("GetHistoricalWeather")
.WithOpenApi();

app.MapHealthChecks("/healthz");

app.Run();
