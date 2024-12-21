using ProductApi.Entities;

namespace ProductApi.Shared.LinkModels.Categories;

public class CategoryLinkResponse {
    public bool HasLinks { get; set; }
    public IEnumerable<Category> Categories { get; set; }
    public LinkedCategoryEntity LinkedEntity { get; set; }
}
