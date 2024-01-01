namespace ProductApi.Model.LinkModels.Categories;
public record LinkedCategories {
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
    public List<Link> Links { get; set; }
}
