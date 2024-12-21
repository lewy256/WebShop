using Bogus;
using Bogus.Extensions;
using OrderApi.Entities;

namespace OrderApi.IntegrationTests;

public class StatusFaker : Faker<Status> {
    public StatusFaker() {
        RuleFor(s => s.Description, f => f.Random.String2(1, 10));
    }
}

public class CouponFaker : Faker<Coupon> {
    public CouponFaker() {
        RuleFor(c => c.Code, f => f.Commerce.ProductAdjective());
        RuleFor(c => c.DiscountAmount, f => f.Random.Int(1, 100));
        RuleFor(c => c.ExpirationDate, f => f.Date.Future());
        RuleFor(c => c.MaxUsage, f => f.Random.Int(51, 100));
        RuleFor(c => c.UsedCount, f => f.Random.Int(1, 50));
        RuleFor(c => c.MinimumOrderAmount, f => f.Random.Int(1, 1000));
        RuleFor(c => c.IsActive, f => true);
    }
}

public class AddressFaker : Faker<Address> {
    public AddressFaker() {
        RuleFor(a => a.FirstName, f => f.Name.FirstName().ClampLength(1, 50));
        RuleFor(a => a.LastName, f => f.Name.LastName().ClampLength(1, 50));
        RuleFor(a => a.AddressLine1, f => f.Address.StreetAddress().ClampLength(1, 100));
        RuleFor(a => a.AddressLine2, f => f.Address.SecondaryAddress().ClampLength(1, 100).OrNull(f, .8f));
        RuleFor(a => a.PostalCode, f => f.Address.ZipCode().ClampLength(1, 10));
        RuleFor(a => a.PhoneNumber, f => f.Phone.PhoneNumber().ClampLength(1, 20));
        RuleFor(a => a.Country, f => f.Address.Country().ClampLength(1, 50));
        RuleFor(a => a.City, f => f.Address.City().ClampLength(1, 50));
        RuleFor(a => a.CustomerId, f => new Guid("39b74f0a-b286-4d7a-bdfd-56c81da8b895"));
    }
}

public class PaymentMethodFaker : Faker<PaymentMethod> {
    public PaymentMethodFaker() {
        RuleFor(p => p.Name, f => f.Company.CompanyName());
    }
}

public class ShipMethodFaker : Faker<ShipMethod> {
    public ShipMethodFaker() {
        RuleFor(s => s.Name, f => f.Random.String2(1, 10));
        RuleFor(s => s.Price, f => f.Random.Int(1, 100));
    }
}

public class OrderFaker : Faker<Order> {
    public OrderFaker() {
        RuleFor(o => o.CustomerId, f => new Guid("39b74f0a-b286-4d7a-bdfd-56c81da8b895"));
        RuleFor(o => o.OrderDate, f => f.Date.Past());
        RuleFor(o => o.TotalAmount, f => f.Random.Int(1, 100));
        RuleFor(o => o.Notes, f => f.Lorem.Word());
        RuleFor(o => o.OrderName, f => Guid.NewGuid());
        RuleFor(o => o.OrderName, f => Guid.NewGuid());
        RuleFor(o => o.Address, f => new AddressFaker().Generate());
        RuleFor(o => o.PaymentMethod, f => new PaymentMethodFaker().Generate());
        RuleFor(o => o.ShipMethod, f => new ShipMethodFaker().Generate());
        RuleFor(o => o.Coupon, f => new CouponFaker().Generate());
    }
}

public class OrderItemFaker : Faker<OrderItem> {
    public OrderItemFaker(int orderId) {
        RuleFor(o => o.UnitPrice, f => f.Random.Int(1, 1000));
        RuleFor(o => o.Quantity, f => f.Random.Int(1, 100));
        RuleFor(o => o.OrderId, f => orderId);
        RuleFor(o => o.ProductId, f => Guid.NewGuid());
        RuleFor(o => o.ProductName, f => Guid.NewGuid().ToString());
    }
}