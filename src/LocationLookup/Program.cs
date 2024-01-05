using LocationLookup;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

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

app.MapGet("/location", Results<Ok<LatLong>, NotFound<NotFoundMessage>> (string nameOfLocation) =>
{
    locations.TryGetValue(nameOfLocation, out LatLong? location);
    return location != null ? TypedResults.Ok(location) : TypedResults.NotFound(new NotFoundMessage { Message = $"No location found for {nameOfLocation}." });
})
.WithName("GetLocation")
.WithOpenApi();

app.Run();

