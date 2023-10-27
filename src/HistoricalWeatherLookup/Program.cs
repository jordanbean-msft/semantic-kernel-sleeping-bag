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

app.MapGet("/historical-weather-lookup", (double latitude, double longitude, DateTime datetime) =>
{
    return TypedResults.Ok(new HistoricalWeather
    {
        HighestAmbientTemperature = 100,
        LowestAmbientTemperature = 0
    });
})
.WithName("GetHistoricalWeather")
.WithOpenApi();

app.Run();
