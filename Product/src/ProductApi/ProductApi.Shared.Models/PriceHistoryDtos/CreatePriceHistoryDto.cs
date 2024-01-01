namespace ProductApi.Shared.Model.PriceHistoryDtos;

public record CreatePriceHistoryDto {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceValue { get; set; }
}