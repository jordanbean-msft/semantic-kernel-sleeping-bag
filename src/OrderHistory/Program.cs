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

Dictionary<string, OrderHistory> orderHistory = new()
{
    {
    "dkschrute",
    new OrderHistory
    {
            OrderId = "1",
            CustomerId = "dkschrute",
            OrderDate = "2021-01-01",
            OrderTotal = "10",
            OrderStatus = "Shipped",
            OrderItems = new List<OrderItems> {
                new() {
                    ProductId = "12345",
                    ProductName = "Eco Elite Sleeping Bag",
                    ProductPrice = "10",
                    ProductQuantity = "1"
                }
            }
    }
    }
};

app.MapGet("/orderHistory/{username}", Results<Ok<OrderHistory>, NotFound<string>> (string username) =>
{
    OrderHistory response = null;
    if (orderHistory.TryGetValue(username, out var orderHistoryItem))
    {
        response = orderHistoryItem;
    }
    return response != null ? TypedResults.Ok(response) : TypedResults.NotFound($"No order history found for user ${username}");
})
.WithName("GetOrderHistory")
.WithOpenApi();

app.Run();

