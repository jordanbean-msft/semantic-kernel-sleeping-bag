using LocationLookup;
using Microsoft.AspNetCore.Http.HttpResults;

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

Dictionary<string, LatLong> locations = new()
{
    { "Patagonia", new LatLong
    {
        Latitude = -41.814099,
        Longitude = -68.907384
    } }
};

app.MapGet("/location", Results<Ok<LatLong>, NotFound<string>> (string nameOfLocation) =>
{
    LatLong location = null;
    locations.TryGetValue(nameOfLocation, out location);
    return location != null ? TypedResults.Ok(location) : TypedResults.NotFound($"No location found for {nameOfLocation}.");
})
.WithName("GetLocation")
.WithOpenApi();

app.Run();

