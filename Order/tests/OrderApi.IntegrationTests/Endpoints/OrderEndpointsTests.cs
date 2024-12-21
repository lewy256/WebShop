using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderApi.Entities;
using OrderApi.Features.Orders;
using OrderApi.Infrastructure;
using OrderApi.Shared;
using OrderApi.Shared.OrderDtos;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace OrderApi.IntegrationTests.Endpoints;

public class OrderControllerTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public OrderControllerTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public (Order order, List<OrderItem> items) Seed(int itemCount) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var order = new OrderFaker().Generate();

        context.Order.Add(order);
        context.SaveChanges();

        var items = new OrderItemFaker(order.OrderId).Generate(itemCount);

        context.OrderItem.AddRange(items);
        context.SaveChanges();

        return (order, items);
    }

    [Fact]
    public async Task CreateOrder_WithValidModel_ReturnsCreated() {
        var data = Seed(2);

        var orderRequest = new OrderRequest() {
            PaymentMethodId = data.order.PaymentMethodId,
            AddressId = data.order.AddressId,
            ShipMethodId = data.order.ShipMethodId,
            Notes = data.order.Notes,
            CouponCode = data.order.Coupon.Code,
            Items = data.items.Adapt<List<OrderItemDto>>()
        };

        var response = await _client.PostAsJsonAsync("/api/orders", orderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/orders", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidModel_ReturnsUnprocessableEntity() {
        var data = Seed(1);
        data.order.OrderItem = null;
        var orderRequest = data.order.Adapt<OrderRequest>();

        var response = await _client.PostAsJsonAsync("/api/orders", orderRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }


    [Fact]
    public async Task GetOrder_WithValidId_ReturnsOk() {
        var data = Seed(2);

        var getResponse = await _client.GetAsync($"/api/orders/{data.order.OrderId}");
        var response = await getResponse.Content.ReadFromJsonAsync<OrderSummaryDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.OrderItems.Should().NotBeNullOrEmpty().And.NotContainNulls();
        response.PaymentMethod.Should().NotBeNull();
        response.ShipMethod.Should().NotBeNull();
        response.Address.Should().NotBeNull();
        response.Statuses.Should().NotBeNull().And.NotContainNulls();
        response.OrderName.Should().NotBe(default(Guid));
        response.TotalPrice.Should().BePositive();
    }

    [Fact]
    public async Task GetOrder_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync("/api/orders/44");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrders_ReturnsOk() {
        var orders = new List<OrderDto>() {
            Seed(2).order.Adapt<OrderDto>(),
            Seed(2).order.Adapt<OrderDto>()
        };

        var getResponse = await _client.GetAsync($"/api/orders");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(orders, opt => opt.Excluding(x => x.OrderDate));
    }

    [Fact]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent() {
        var data = Seed(1);

        var response = await _client.DeleteAsync($"/api/orders/{data.order.OrderId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/orders/44");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchOrder_WithValidModel_ReturnsNoContent() {
        var data = Seed(1);
        var content = new StringContent("[{ \"op\": \"replace\", \"path\": \"notes\", \"value\": \"Test\" }]", Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"/api/orders/{data.order.OrderId}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PatchOrder_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PatchAsJsonAsync($"/api/orders/1", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchOrder_WithInvalidModel_ReturnsUnprocessableEntity() {
        var data = Seed(1);
        var content = new StringContent("[{ \"op\": \"replace\", \"path\": \"notes\", \"value\": \"\" }]", Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"/api/orders/{data.order.OrderId}", content);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task PatchOrder_WithInvalidId_ReturnsNotFound() {
        var content = new StringContent("[{ \"op\": \"replace\", \"path\": \"notes\", \"value\": \"Test\" }]", Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync("/api/orders/44", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}