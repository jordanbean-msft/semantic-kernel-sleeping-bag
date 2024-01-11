using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;
using ProductCatalog;

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
.WithOpenApi();

app.Run();

