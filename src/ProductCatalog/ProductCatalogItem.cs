namespace ProductCatalog
{
    public record ProductCatalogItem
    {
        public string ProductId { get; init; } = "";
        public string ProductName { get; init; } = "";
        public string ProductDescription { get; init; } = "";
    }
}