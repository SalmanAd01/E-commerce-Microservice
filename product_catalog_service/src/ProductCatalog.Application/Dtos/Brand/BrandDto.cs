namespace ProductCatalog.Application.Dtos.Brand
{
    public class BrandDto
    {
        public string Id { get; set; } = string.Empty;
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Logo { get; set; }
    }
}
