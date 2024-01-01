using Bogus;
using ProductApi.Model.Entities;

namespace ProductApi.IntegrationTests;

internal static class SeedData {
    public static Category CategoryGenerator() {
        var categoryId = Guid.NewGuid();
        var categoryFaker = new Faker<Category>()
            .RuleFor(x => x.Id, f => categoryId)
            .RuleFor(x => x.CategoryName, f => f.Commerce.ProductName().Substring(0, 10));

        var category = categoryFaker.Generate();

        return category;
    }

    public static Product ProductGenerator(Guid categoryId) {
        var productId = Guid.NewGuid();
        var productFaker = new Faker<Product>()
           .RuleFor(x => x.Id, f => productId)
           .RuleFor(x => x.ProductName, f => f.Commerce.ProductName().Substring(0, 10))
           .RuleFor(x => x.SerialNumber, f => f.Lorem.Word())
           .RuleFor(x => x.Price, f => f.Random.Decimal(0.00M, 100_000M))
           .RuleFor(x => x.Stock, f => f.Random.Int(0, 10))
           .RuleFor(x => x.CategoryId, f => categoryId)
           .RuleFor(x => x.Description, f => f.Commerce.ProductDescription().Substring(0, 10))
           .RuleFor(x => x.Color, f => f.Commerce.Color())
           .RuleFor(x => x.Weight, f => f.Random.Int(1, 10))
           .RuleFor(x => x.Size, f => f.Random.Int(1, 10).ToString());

        var product = productFaker.Generate();

        return product;
    }

    public static Review ReviewGenerator(Guid productId) {
        var reviewId = Guid.NewGuid();
        var reviewFaker = new Faker<Review>()
            .RuleFor(x => x.Id, f => reviewId)
            .RuleFor(x => x.ProductId, f => productId)
            .RuleFor(x => x.Description, f => f.Lorem.Word())
            .RuleFor(x => x.Rating, f => f.Random.Int(0, 5))
            .RuleFor(x => x.Discriminator, f => nameof(Review));

        var reviews = reviewFaker.Generate();

        return reviews;
    }

    public static PriceHistory PriceHistoryGenerator(Guid productId) {
        var priceHistoryId = Guid.NewGuid();
        var priceHistoryFaker = new Faker<PriceHistory>()
           .RuleFor(x => x.Id, f => priceHistoryId)
           .RuleFor(x => x.ProductId, f => productId)
           .RuleFor(x => x.StartDate, f => f.Date.Past())
           .RuleFor(x => x.EndDate, f => f.Date.Recent())
           .RuleFor(x => x.PriceValue, f => f.Random.Decimal(0.00M, 100_000M))
           .RuleFor(x => x.Discriminator, f => nameof(PriceHistory));

        var pricesHistory = priceHistoryFaker.Generate();

        return pricesHistory;
    }
}