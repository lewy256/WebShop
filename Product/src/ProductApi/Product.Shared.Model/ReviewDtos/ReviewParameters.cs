namespace ProductApi.Shared.Model.ReviewDtos;

public class ReviewParameters {
    private const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;

    public int PageSize {
        get => _pageSize;
        set => _pageSize = value > maxPageSize ? maxPageSize : value;
    }

    public string? SearchTerm { get; set; }
    public string? OrderBy { get; set; }

    public string? Fields { get; set; }
}
