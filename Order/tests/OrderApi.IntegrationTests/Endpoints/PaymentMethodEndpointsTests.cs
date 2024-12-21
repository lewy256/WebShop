using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Shared;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OrderApi.IntegrationTests.Endpoints;

public class PaymentMethodEndpointsTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public PaymentMethodEndpointsTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public List<PaymentMethod> Seed(int count) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var paymentMethods = new PaymentMethodFaker().Generate(count);

        context.PaymentMethod.AddRange(paymentMethods);

        context.SaveChanges();

        return paymentMethods;
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsOk() {
        var paymentMethods = Seed(2);
        var expectedResponse = paymentMethods.Adapt<IEnumerable<PaymentMethodDto>>();

        var getResponse = await _client.GetAsync("/api/payment-methods");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PaymentMethodDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
    }
}
