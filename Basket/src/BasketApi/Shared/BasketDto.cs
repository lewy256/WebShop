using BasketApi.Entities;

namespace BasketApi.Shared;

public record BasketDto {
    public List<BasketItem> Items { get; init; }
    public decimal TotalPrice { get; init; }
}
