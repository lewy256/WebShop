using BasketApi.Models;

namespace BasketApi.Shared;

public record CreateBasketDto {
    public Guid Id { get; init; }
    public List<BasketItem> Items { get; init; }
}
