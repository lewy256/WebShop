using Bogus;
using ProductApi.Model.Entities;

namespace ProductApi.IntegrationTests;

public class CategoryFaker : Faker<Category> {
    public CategoryFaker() {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.CategoryName, f => f.Commerce.ProductName().Substring(0, 10));
    }
}

public class ProductFaker : Faker<Product> {
    public Category Category { get; init; } = new CategoryFaker().Generate();
    public ProductFaker() {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.ProductName, f => f.Commerce.ProductName().Substring(0, 10));
        RuleFor(x => x.SerialNumber, f => f.Lorem.Word());
        RuleFor(x => x.Price, f => f.Random.Decimal(0.00M, 100_000M));
        RuleFor(x => x.Stock, f => f.Random.Int(0, 10));
        RuleFor(x => x.CategoryId, f => Category.Id);
        RuleFor(x => x.Description, f => f.Commerce.ProductDescription().Substring(0, 10));
        RuleFor(x => x.Color, f => f.Commerce.Color());
        RuleFor(x => x.Weight, f => f.Random.Int(1, 10));
        RuleFor(x => x.Size, f => f.Random.Int(1, 10).ToString());
    }
}

public class ReviewFaker : Faker<Review> {
    public Product Product { get; init; } = new ProductFaker().Generate();
    public ReviewFaker() {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.ProductId, f => Product.Id);
        RuleFor(x => x.UserName, f => f.Person.UserName);
        RuleFor(x => x.Description, f => f.Lorem.Word());
        RuleFor(x => x.Rating, f => f.Random.Int(0, 5));
        RuleFor(x => x.ReviewDate, f => DateTime.UtcNow);
        RuleFor(x => x.Discriminator, f => nameof(Review));
    }
}

public class PriceHistoryFaker : Faker<PriceHistory> {
    public Product Product { get; init; } = new ProductFaker().Generate();
    public PriceHistoryFaker() {
        RuleFor(x => x.Id, f => Guid.NewGuid());
        RuleFor(x => x.ProductId, f => Product.Id);
        RuleFor(x => x.StartDate, f => f.Date.Past());
        RuleFor(x => x.EndDate, f => f.Date.Recent());
        RuleFor(x => x.PriceValue, f => f.Random.Decimal(0.00M, 100_000M));
        RuleFor(x => x.Discriminator, f => nameof(PriceHistory));
    }
}
