using HistoricalWeatherLookup;

var builder = WebApplication.CreateBuilder(args);

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
            HighestAmbientTemperature = 100,
            LowestAmbientTemperature = 10
        }
    }
};

app.MapGet("/historical-weather-lookup", (double latitude, double longitude, int monthOfYear) =>
{
    HistoricalWeather historicalWeatherResponse = null;
    if (historicalWeather.TryGetValue(new HistoricalWeatherInput { Latitude = latitude, Longitude = longitude, MonthOfYear = monthOfYear }, out var historicalWeatherItem))
    {
        historicalWeatherResponse = historicalWeatherItem;
    }

    return TypedResults.Ok(historicalWeatherResponse);
})
.WithName("GetHistoricalWeather")
.WithOpenApi();

app.Run();
