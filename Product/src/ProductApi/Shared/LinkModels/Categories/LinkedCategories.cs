using ProductApi.Entities;

namespace ProductApi.Shared.LinkModels.Categories;
public class LinkedCategories {
    public Guid Id { get; init; }
    public string CategoryName { get; init; }
    public List<Link> Links { get; init; }

    public LinkedCategories(Category category, List<Link> links) {
        Id = category.Id;
        CategoryName = category.CategoryName;
        Links = links;
    }

    public LinkedCategories() { }
}
