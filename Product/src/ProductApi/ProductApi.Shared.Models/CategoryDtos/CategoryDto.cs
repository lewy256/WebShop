namespace ProductApi.Shared.Model.CategoryDtos;
public record CategoryDto {
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
}
