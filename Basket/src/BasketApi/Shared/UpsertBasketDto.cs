using BasketApi.Entities;

namespace BasketApi.Shared;

public record UpsertBasketDto {
    public List<BasketItem> Items { get; init; }
}
