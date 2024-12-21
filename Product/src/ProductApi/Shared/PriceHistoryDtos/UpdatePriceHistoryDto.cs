namespace ProductApi.Shared.PriceHistoryDtos;

public record UpdatePriceHistoryDto {
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal PriceValue { get; init; }
}