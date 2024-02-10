using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Models;
using OrderApi.Shared;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OrderApi.IntegrationTests.Endpoints;

public class OrderControllerTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public OrderControllerTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public List<Order> Seed(int count) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var orders = new OrderFaker().Generate(count);
        context.Order.AddRange(orders);
        context.SaveChanges();


        return orders;
    }

    [Fact]
    public async Task GetOrder_WithValidId_ReturnsOk() {
        var orderDto = Seed(1).First().Adapt<OrderSummaryDto>();

        var getResponse = await _client.GetAsync($"/api/orders/{orderDto.OrderId}");
        var response = await getResponse.Content.ReadFromJsonAsync<OrderSummaryDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(orderDto, opt => opt.Excluding(x => x.OrderDate));

    }

    [Fact]
    public async Task GetOrder_WithValidId_ReturnsNotFound() {
        var response = await _client.GetAsync("/api/orders/44");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrders_ReturnsOk() {
        var orders = Seed(2).Adapt<IEnumerable<OrderDto>>();

        var getResponse = await _client.GetAsync($"/api/orders");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(orders, opt => opt.Excluding(x => x.OrderDate));
    }

    [Fact]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent() {
        var order = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/orders/{order.OrderId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/orders/44");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
