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

Dictionary<string, ProductCatalogItem> productCatalog = new()
{
    {
        "12345",
        new ProductCatalogItem
        {
            ProductId = "12345",
            ProductName = "Elite Eco Sleeping Bag",
            ProductDescription = "Weight: 5 lbs, Length: 6 feet, Lowest Ambient Temperature Supported: -20 Fahrenheit"
        }
    }
};

app.MapGet("/productCatalog/{id}", (string id) =>
{
    var response = productCatalog.TryGetValue(id, out var productCatalogItem) ? productCatalogItem : null;

    return productCatalogItem;
})
.WithName("GetProductCatalogItem")
.WithOpenApi();

app.Run();
