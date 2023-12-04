using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;
using OrderHistory;

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

Dictionary<string, OrderHistory.OrderHistory> orderHistory = new()
{
    {
    "dkschrute",// "Jordan Bean",
    new OrderHistory.OrderHistory
    {
            OrderId = "1",
            CustomerId = "dkschrute",//"Jordan Bean",
            OrderDate = "2021-01-01",
            OrderTotal = "10",
            OrderStatus = "Shipped",
            OrderItems = [
                new() {
                    ProductId = "12345",
                    ProductName = "Eco Elite Sleeping Bag",
                    ProductPrice = "10",
                    ProductQuantity = "1"
                }
            ]
    }
    }
};

app.MapGet("/orderHistory/{username}", Results<Ok<OrderHistory.OrderHistory>, NotFound<NotFoundMessage>> (string username) =>
{
    OrderHistory.OrderHistory? response = null;
    if (orderHistory.TryGetValue(username, out var orderHistoryItem))
    {
        response = orderHistoryItem;
    }
    return response != null ? TypedResults.Ok(response) : TypedResults.NotFound(new NotFoundMessage { Message = $"No order history found for user {username}" });
})
.WithName("GetOrderHistory")
.WithOpenApi(generatedOperations =>
{
    generatedOperations.Summary = "Get the order history for a user, including all the products they purchased (which includes the product ID). This is a list of what the user owns.";
    generatedOperations.Parameters[0].Description = "The string username of the user. There should be no curly braces around the username.";
    return generatedOperations;
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Environment.ContentRootPath),
    RequestPath = "/.well-known"
});

app.MapHealthChecks("/healthz");

app.Run();

