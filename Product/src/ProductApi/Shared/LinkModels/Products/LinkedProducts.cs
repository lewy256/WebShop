using ProductApi.Shared.ProductDtos;
using File = ProductApi.Entities.File;

namespace ProductApi.Shared.LinkModels.Products;

public class LinkedProducts {
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
    public List<Link> Links { get; init; }

    public LinkedProducts(ProductDto productDto, List<Link> links) {
        Id = productDto.Id;
        ProductName = productDto.ProductName;
        SerialNumber = productDto.SerialNumber;
        Price = productDto.Price;
        Stock = productDto.Stock;
        CategoryId = productDto.CategoryId;
        Description = productDto.Description;
        Colors = productDto.Colors;
        Weight = productDto.Weight;
        Measurements = productDto.Measurements;
        DispatchTime = productDto.DispatchTime;
        Brand = productDto.Brand;
        Files = productDto.Files;
        Links = links;
    }

}
