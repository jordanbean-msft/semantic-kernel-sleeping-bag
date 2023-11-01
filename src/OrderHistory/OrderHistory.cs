public record OrderHistory
{
    public string OrderId { get; init; }
    public string CustomerId { get; init; }
    public string OrderDate { get; init; }
    public string OrderTotal { get; init; }
    public string OrderStatus { get; init; }
    public List<OrderItems> OrderItems { get; init; }
}
