using ProductApi.Shared.ProductDtos;

namespace ProductApi.Shared.LinkModels.Products;

public class ProductLinkResponse {
    public bool HasLinks { get; set; }
    public IEnumerable<ProductDto> Products { get; set; }    public LinkedProductEntity LinkedEntities { get; set; }
}