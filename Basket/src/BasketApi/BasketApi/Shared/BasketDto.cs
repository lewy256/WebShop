using BasketApi.Models;

namespace BasketApi.Shared;

public record BasketDto {
    public Guid Id { get; init; }
    public List<BasketItem> Items { get; init; }
    public decimal TotalPrice { get; init; }
}
