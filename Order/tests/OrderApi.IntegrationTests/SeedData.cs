using Bogus;
using OrderApi.Models;

namespace OrderApi.IntegrationTests;

internal static class SeedData {
    public static List<Coupon> CouponFaker { get; private set; }
    public static List<Customer> CustomerFaker { get; private set; }
    public static List<PaymentMethod> PaymentFaker { get; private set; }
    public static List<ShipMethod> ShipFaker { get; private set; }
    public static List<Address> AddressFaker { get; private set; }
    public static List<Order> OrderFaker { get; private set; }

    public static void GeneratingData(int seed) {

        var couponId = 1;
        var couponFaker = new Faker<Coupon>()
           .RuleFor(x => x.CouponId, f => couponId++)
           .RuleFor(x => x.Code, f => f.Commerce.ProductAdjective())
           .RuleFor(x => x.Description, f => f.Lorem.Word())
           .RuleFor(x => x.Amount, f => f.Random.Int(1, 100));

        CouponFaker = couponFaker.Generate(seed);

        var customerId = 1;
        var customerFaker = new Faker<Customer>()
           .RuleFor(x => x.CustomerId, f => customerId++)
           .RuleFor(x => x.Email, f => f.Internet.Email());

        CustomerFaker = customerFaker.Generate(seed);

        var paymentMethodId = 1;
        var paymentMethodFaker = new Faker<PaymentMethod>()
           .RuleFor(x => x.PaymentMethodId, f => paymentMethodId++)
           .RuleFor(x => x.Name, f => f.Company.CompanyName());

        PaymentFaker = paymentMethodFaker.Generate(seed);

        var shipMethodId = 1;
        var shipMethodFaker = new Faker<ShipMethod>()
           .RuleFor(x => x.ShipMethodId, f => shipMethodId++)
           .RuleFor(x => x.Description, f => f.Lorem.Word())
           .RuleFor(x => x.DeliveryTime, f => f.Date.Past())
           .RuleFor(x => x.Price, f => f.Random.Int(1, 100));

        ShipFaker = shipMethodFaker.Generate(seed);

        var addressId = 1;
        var addressFaker = new Faker<Address>()
           .RuleFor(x => x.AddressId, f => addressId++)
           .RuleFor(x => x.FirstName, f => f.Name.FirstName())
           .RuleFor(x => x.AddressLine1, f => f.Name.FirstName())
           .RuleFor(x => x.AddressLine1, f => f.Address.StreetAddress())
           .RuleFor(x => x.AddressLine2, f => f.Address.SecondaryAddress().OrNull(f, .8f))
           .RuleFor(x => x.PostalCode, f => f.Address.ZipCode())
           .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
           .RuleFor(x => x.Country, f => f.Address.Country())
           .RuleFor(x => x.City, f => f.Address.City());

        AddressFaker = addressFaker.Generate(seed);

        var orderId = 1;
        var orderFaker = new Faker<Order>()
           .RuleFor(x => x.OrderId, f => orderId++)
           .RuleFor(x => x.CustomerId, f => f.PickRandom(CustomerFaker).CustomerId)
           .RuleFor(x => x.OrderDate, f => f.Date.Past())
           .RuleFor(x => x.PaymentMethodId, f => f.PickRandom(PaymentFaker).PaymentMethodId)
           .RuleFor(x => x.AddressId, f => f.PickRandom(AddressFaker).AddressId)
           .RuleFor(x => x.ShipMethodId, f => f.PickRandom(ShipFaker).ShipMethodId)
           .RuleFor(x => x.TotalPrice, f => f.Random.Int(1, 100))
           .RuleFor(x => x.Notes, f => f.Lorem.Word())
           .RuleFor(x => x.CouponId, f => f.PickRandom(CouponFaker).CouponId.OrNull(f, .8f));

        OrderFaker = orderFaker.Generate(seed);
    }
}
