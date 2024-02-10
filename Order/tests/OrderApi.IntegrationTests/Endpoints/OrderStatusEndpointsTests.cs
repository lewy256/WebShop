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

public class OrderStatusEndpointsTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public OrderStatusEndpointsTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public SpecOrderStatus Seed() {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var orderStatus = new OrderStatusFaker().Generate();

        context.SpecOrderStatus.Add(orderStatus);

        context.SaveChanges();

        return orderStatus;
    }

    [Fact]
    public async Task CreateOrderStatus_WithValidModel_ReturnsCreated() {
        var orderStatus = Seed();
        var orderStatusRequest = orderStatus.Adapt<OrderStatusRequest>();
        var expectedResponse = orderStatus.Adapt<SpecOrderStatusDto>();

        var postResponse = await _client.PostAsJsonAsync("/api/order-statuses", orderStatusRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<SpecOrderStatusDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.SpecOrderStatusId).Excluding(x => x.StatusDate));
    }

    [Fact]
    public async Task CreateOrderStatus_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/order-statuses", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrderStatus_WithInvalidModel_ReturnsUnprocessableEntity() {
        var orderStatus = Seed();
        orderStatus.OrderId = default;
        var orderStatusRequest = orderStatus.Adapt<OrderStatusRequest>();

        var response = await _client.PostAsJsonAsync("/api/order-statuses", orderStatusRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteOrderStatus_WithValidId_ReturnsNoContent() {
        var orderStatus = Seed();

        var response = await _client.DeleteAsync($"/api/order-statuses/{orderStatus.SpecOrderStatusId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrderStatus_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/order-statuses/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}
