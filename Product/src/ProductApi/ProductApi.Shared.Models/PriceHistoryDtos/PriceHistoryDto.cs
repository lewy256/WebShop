namespace ProductApi.Shared.Model.PriceHistoryDtos;

public record PriceHistoryDto {
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceValue { get; set; }
}