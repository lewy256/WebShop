using BasketApi.Entities;
using Bogus;

namespace BasketApi.IntegrationTests;

public class BasketFaker : Faker<Basket> {
    public BasketFaker() {
        RuleFor(x => x.Items, f => new BasketItemFaker().Generate(new Random().Next(1, 10)));
    }
}

public class BasketItemFaker : Faker<BasketItem> {
    public BasketItemFaker() {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.Name, f => f.Commerce.Product());
        RuleFor(x => x.ImageUrl, f => Guid.NewGuid().ToString());
        RuleFor(x => x.Quantity, f => f.Random.Number(1, 100));
        RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000));
    }
}
