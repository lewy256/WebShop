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

public class ShipMethodControllerTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public ShipMethodControllerTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public List<ShipMethod> Seed(int count) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var shipMethods = new ShipMethodFaker().Generate(count);

        context.ShipMethod.AddRange(shipMethods);

        context.SaveChanges();

        return shipMethods;
    }

    [Fact]
    public async Task GetShipMethods_ReturnsOk() {
        var shipMethods = Seed(2);
        var expectedResponse = shipMethods.Adapt<IEnumerable<ShipMethodDto>>();

        var getResponse = await _client.GetAsync("/api/ship-methods");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ShipMethodDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
    }
}
