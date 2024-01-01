using ProductApi.Model.Entities;

namespace ProductApi.Model.LinkModels.Categories;

public class CategoryLinkResponse {
    public bool HasLinks { get; set; }
    public IEnumerable<Category> Categories { get; set; }
    public LinkedCategoryEntity LinkedEntity { get; set; }
}
