using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
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
    public async Task CreateShipMethod_WithValidModel_ReturnsCreated() {
        var shipMethod = Seed(1).First();
        var shipMethodRequest = shipMethod.Adapt<ShipMethodRequest>();

        var postResponse = await _client.PostAsJsonAsync("/api/ship-methods", shipMethodRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<ShipMethod>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.ShipMethodId.Should().NotBe(default);
        response.Should().BeEquivalentTo(shipMethodRequest);
    }

    [Fact]
    public async Task CreateShipMethod_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/ship-methods", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateShipMethod_WithInvalidModel_ReturnsUnprocessableEntity() {
        var shipMethod = Seed(1).First();
        shipMethod.Description = "";
        var shipMethodRequest = shipMethod.Adapt<ShipMethodRequest>();

        var response = await _client.PostAsJsonAsync("/api/ship-methods", shipMethodRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetShipMethod_WithValidId_ReturnsOk() {
        var shipMethod = Seed(1).First();
        var expectedResponse = shipMethod.Adapt<ShipMethodDto>();

        var getResponse = await _client.GetAsync($"/api/ship-methods/{shipMethod.ShipMethodId}");
        var response = await getResponse.Content.ReadFromJsonAsync<ShipMethodDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShipMethodId.Should().NotBe(default);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.ShipMethodId).Excluding(x => x.DeliveryTime));
    }

    [Fact]
    public async Task GetShipMethod_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/ship-methods/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetShipMethods_ReturnsOk() {
        var shipMethods = Seed(2);
        var expectedResponse = shipMethods.Adapt<IEnumerable<ShipMethodDto>>();

        var getResponse = await _client.GetAsync("/api/ship-methods");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ShipMethodDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.ShipMethodId).Excluding(x => x.DeliveryTime));
    }

    [Fact]
    public async Task UpdateShipMethod_WithValidModel_ReturnsNoContent() {
        var shipMethod = Seed(1).First();
        var shipMethodRequest = shipMethod.Adapt<ShipMethodRequest>();

        var response = await _client.PutAsJsonAsync($"/api/ship-methods/{shipMethod.ShipMethodId}", shipMethodRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateShipMethod_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PutAsync($"/api/ship-methods/1", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateShipMethod_WithInvalidModel_ReturnsUnprocessableEntity() {
        var shipMethod = Seed(1).First();
        shipMethod.Description = "";
        var shipMethodRequest = shipMethod.Adapt<ShipMethodRequest>();

        var response = await _client.PutAsJsonAsync($"/api/ship-methods/{shipMethod.ShipMethodId}", shipMethodRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateShipMethod_WithInvalidId_ReturnsNotFound() {
        var shipMethod = Seed(1).First();
        var shipMethodRequest = shipMethod.Adapt<ShipMethodRequest>();

        var response = await _client.PutAsJsonAsync($"/api/ship-methods/44", shipMethodRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteShipMethod_WithValidId_ReturnsNoContent() {
        var shipMethod = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/ship-methods/{shipMethod.ShipMethodId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteShipMethod_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/ship-methods/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
