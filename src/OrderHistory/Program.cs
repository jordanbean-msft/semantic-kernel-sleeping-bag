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

Dictionary<string, OrderHistory.OrderHistory> orderHistory = new()
{
    {
    "dkschrute",
    new OrderHistory.OrderHistory
    {
            OrderId = "1",
            CustomerId = "dkschrute",
            OrderDate = "2021-01-01",
            OrderTotal = "100",
            OrderStatus = "Shipped",
            OrderItems = [
                new() {
                    ProductId = "12345",
                    ProductName = "Eco Elite Sleeping Bag",
                    ProductPrice = "100",
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
.WithOpenApi();

app.Run();

