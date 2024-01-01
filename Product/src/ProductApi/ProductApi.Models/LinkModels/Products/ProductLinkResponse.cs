using ProductApi.Model.Entities;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Model.LinkModels.Products;

public class ProductLinkResponse {
    public bool HasLinks { get; set; }
    public bool IsShaped { get; set; }
    public List<Entity> ShapedEntities { get; set; }
    public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }
    public IEnumerable<ProductDto> Products { get; set; }
    public ProductLinkResponse() {
        LinkedEntities = new LinkCollectionWrapper<Entity>();
        ShapedEntities = new List<Entity>();
    }
}
