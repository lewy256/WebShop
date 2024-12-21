using File = ProductApi.Entities.File;

namespace ProductApi.Shared.ProductDtos;

public record ProductDto {
    public Guid Id { get; init; }
    public string ProductName { get; init; }
    public string SerialNumber { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public Guid CategoryId { get; init; }
    public string Description { get; init; }
    public string Colors { get; init; }
    public long Weight { get; init; }
    public string Measurements { get; init; }
    public TimeSpan DispatchTime { get; init; }
    public string Brand { get; init; }
    public List<File>? Files { get; init; }
}

