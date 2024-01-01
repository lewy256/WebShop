namespace ProductApi.Shared.Model.PriceHistoryDtos;

public class PriceHistoryParameters {
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? OrderBy { get; set; }
}
