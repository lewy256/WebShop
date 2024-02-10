namespace ProductApi.Shared.Model.ProductDtos.V1;

public class ProductParameters {
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }
}

