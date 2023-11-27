namespace ProductApi.Shared.Model;

public class ProductParameters {
    private const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;

    public int PageSize {
        get => _pageSize;
        set => _pageSize = value > maxPageSize ? maxPageSize : value;
    }

    public decimal MinPrice { get; set; } = 0;
    public decimal MaxPrice { get; set; } = decimal.MaxValue;
    public bool ValidPriceRange => MaxPrice > MinPrice;

    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }
}