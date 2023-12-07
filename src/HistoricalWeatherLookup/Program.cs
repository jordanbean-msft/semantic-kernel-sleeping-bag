using HistoricalWeatherLookup;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;

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
.WithOpenApi(generatedOperations =>
{
    generatedOperations.Summary = "Get the historical weather for a location for a month. Make sure and pass in the month of the year the user requested. The weather temperatures will be returned in Fahrenheit. Not Found will be returned if the GPS latitude & longitude or the month of the year are not valid. Make sure and pass in valid GPS latitude & longitude for the location requested by the user. If you don't know the GPS latitude & longitude for a given location, look it up.";
    generatedOperations.Parameters[0].Description = "The latitude of the GPS location to lookup historical weather for. This should be a string, not JSON.";
    generatedOperations.Parameters[1].Description = "The longitude of the GPS location to lookup historical weather for. This should be a string, not JSON.";
    generatedOperations.Parameters[2].Description = "The integer month of the year to lookup historical weather for";
    return generatedOperations;
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Environment.ContentRootPath),
    RequestPath = "/.well-known"
});

app.MapHealthChecks("/healthz");

app.Run();
