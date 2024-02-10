using Bogus;
using OrderApi.Models;

namespace OrderApi.IntegrationTests;

public class StatusFaker : Faker<Status> {
    public StatusFaker() {
        RuleFor(s => s.Description, f => f.Random.String2(1, 10));
    }
}

public class CouponFaker : Faker<Coupon> {
    public CouponFaker() {
        RuleFor(c => c.Code, f => f.Commerce.ProductAdjective());
        RuleFor(c => c.Description, f => f.Lorem.Word());
        RuleFor(c => c.Amount, f => f.Random.Int(1, 100));
    }
}

public class AddressFaker : Faker<Address> {
    public AddressFaker() {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.AddressLine1, f => f.Address.StreetAddress());
        RuleFor(a => a.AddressLine2, f => f.Address.SecondaryAddress().OrNull(f, .8f));
        RuleFor(a => a.PostalCode, f => f.Address.ZipCode());
        RuleFor(a => a.Phone, f => f.Phone.PhoneNumber());
        RuleFor(a => a.Country, f => f.Address.Country());
        RuleFor(a => a.City, f => f.Address.City());
    }
}

public class PaymentMethodFaker : Faker<PaymentMethod> {
    public PaymentMethodFaker() {
        RuleFor(p => p.Name, f => f.Company.CompanyName());
    }
}

public class ShipMethodFaker : Faker<ShipMethod> {
    public ShipMethodFaker() {
        RuleFor(s => s.Description, f => f.Random.String2(1, 10));
        RuleFor(s => s.DeliveryTime, f => f.Date.Future().ToUniversalTime());
        RuleFor(s => s.Price, f => f.Random.Int(1, 100));
    }
}

public class OrderFaker : Faker<Order> {
    public OrderFaker() {
        RuleFor(o => o.CustomerId, f => new Guid("39b74f0a-b286-4d7a-bdfd-56c81da8b895"));
        RuleFor(o => o.OrderDate, f => f.Date.Past());
        RuleFor(o => o.TotalPrice, f => f.Random.Int(1, 100));
        RuleFor(o => o.Notes, f => f.Lorem.Word());
        RuleFor(o => o.OrderName, f => Guid.NewGuid());
        RuleFor(o => o.OrderName, f => Guid.NewGuid());
        RuleFor(o => o.Address, f => new AddressFaker().Generate());
        RuleFor(o => o.PaymentMethod, f => new PaymentMethodFaker().Generate());
        RuleFor(o => o.ShipMethod, f => new ShipMethodFaker().Generate());
        RuleFor(o => o.Coupon, f => new CouponFaker().Generate());
    }
}

public class OrderStatusFaker : Faker<SpecOrderStatus> {
    public OrderStatusFaker() {
        RuleFor(o => o.Order, f => new OrderFaker().Generate());
        RuleFor(o => o.Status, f => new StatusFaker().Generate());
        RuleFor(o => o.StatusDate, f => f.Date.Past());
    }
}