using LocationLookup;
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
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Summary = "Gets the latitude & longitude GPS coordinates of a specific location name. Use this function to get specific GPS coordinates for all user queries. Do not guess at GPS coordinates, call this service to get them.";
    generatedOperation.Parameters[0].Description = "The string location to lookup GPS coordinates for. This should be a string, not JSON.";
    return generatedOperation;
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Environment.ContentRootPath),
    RequestPath = "/.well-known"
});


app.MapHealthChecks("/healthz");

app.Run();

