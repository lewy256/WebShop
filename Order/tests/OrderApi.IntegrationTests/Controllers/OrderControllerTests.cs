using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Models;
using OrderApi.Shared.OrderDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OrderApi.IntegrationTests.Controllers;

public class OrderControllerTests : IClassFixture<OrderApiFactory>, IAsyncLifetime {
    private readonly HttpClient _client;
    private readonly Func<Task> _dbReset;
    private OrderApiFactory _orderApiFactory;

    public OrderControllerTests(OrderApiFactory orderApiFactory) {
        _client = orderApiFactory.Client;
        _dbReset = orderApiFactory.ResetDatabaseAsync;
        _orderApiFactory = orderApiFactory;
    }

    public Order Seed() {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        context.Database.EnsureCreated();

        SeedData.GeneratingData(1);

        var address = SeedData.AddressFaker.First();
        context.Address.Add(address);

        var coupon = SeedData.CouponFaker.First();
        context.Coupon.Add(coupon);

        var customer = SeedData.CustomerFaker.First();
        context.Customer.Add(customer);

        var ship = SeedData.ShipFaker.First();
        context.ShipMethod.Add(ship);

        var payment = SeedData.PaymentFaker.First();
        context.PaymentMethod.Add(payment);

        var order = SeedData.OrderFaker.First();
        context.Order.Add(order);

        context.SaveChanges();

        var orderDto = order.Adapt<OrderDto>();

        return order;
    }

    [Fact]
    public async Task GetOrder_WithValidId_ReturnsOkResult() {
        var orderDto = Seed().Adapt<OrderDto>();

        var getResponse = await _client.GetAsync($"/api/Order/{orderDto.OrderId}");
        var orderResponse = await getResponse.Content.ReadFromJsonAsync<OrderDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        orderResponse.Should().BeEquivalentTo(orderDto, opt => opt.Excluding(x => x.OrderDate));

    }

    [Fact]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent() {
        var order = Seed();

        var deleteResponse = await _client.DeleteAsync($"/api/Order/{order.OrderId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_WithInvalidId_ReturnsNotFound() {
        int id = 111111;

        var response = await _client.DeleteAsync($"/api/Order/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() {
        await _dbReset();
    }
}
