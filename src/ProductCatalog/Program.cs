using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;
using ProductCatalog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

Dictionary<string, ProductCatalogItem> productCatalog = new()
{
    {
        "12345",
        new ProductCatalogItem
        {
            ProductId = "12345",
            ProductName = "Elite Eco Sleeping Bag",
            ProductDescription = "Weight: 5 lbs, Length: 6 feet, Lowest Temperature Supported: 5 Fahrenheit"
        }
    }
};

app.MapGet("/productCatalog/{id}", Results<Ok<ProductCatalogItem>, NotFound<NotFoundMessage>> (string id) =>
{
    productCatalog.TryGetValue(id, out ProductCatalogItem? productCatalogItem);

    return productCatalogItem != null ? TypedResults.Ok(productCatalogItem) : TypedResults.NotFound(new NotFoundMessage { Message = $"No product catalog item found for id {id}" });
})
.WithName("GetProductCatalogItem")
.WithOpenApi(generatedOperations =>
{
    generatedOperations.Summary = "Get the product specifications. This includes data such as length, highest & lowest supported temperature in Fahrenheit";
    generatedOperations.Parameters[0].Description = "The string product ID of the product. This should be a string, not JSON.";
    return generatedOperations;
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Environment.ContentRootPath),
    RequestPath = "/.well-known"
});

app.Run();
