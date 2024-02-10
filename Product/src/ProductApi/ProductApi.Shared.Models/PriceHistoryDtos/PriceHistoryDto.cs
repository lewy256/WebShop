namespace ProductApi.Shared.Model.PriceHistoryDtos;

public record PriceHistoryDto {
    public Guid Id { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal PriceValue { get; init; }
}