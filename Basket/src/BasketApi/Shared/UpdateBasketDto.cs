using BasketApi.Models;

namespace BasketApi.Shared;

public record UpdateBasketDto {
    public Guid Id { get; init; }
    public List<BasketItem> Items { get; init; }
}
