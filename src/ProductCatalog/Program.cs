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
    ProductCatalogItem productCatalogItem = null;
    productCatalog.TryGetValue(id, out productCatalogItem);

    return productCatalogItem != null ? TypedResults.Ok(productCatalogItem) : TypedResults.NotFound(new NotFoundMessage { Message = $"No product catalog item found for id {id}" });
})
.WithName("GetProductCatalogItem")
.WithOpenApi();

app.MapHealthChecks("/healthz");

app.Run();
