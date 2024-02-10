using BasketApi.Models;
using Bogus;

namespace BasketApi.IntegrationTests;

public class BasketFaker : Faker<Basket> {
    public BasketFaker() {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.Items, f => new BasketItemFaker().Generate(4));
    }
}

public class BasketItemFaker : Faker<BasketItem> {
    public BasketItemFaker() {
        RuleFor(x => x.ProductId, f => Guid.NewGuid());
        RuleFor(x => x.Quantity, f => f.Random.Number(100));
        RuleFor(x => x.Price, f => f.Random.Decimal(0, 1000));
    }
}