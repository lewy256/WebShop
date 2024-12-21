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

public class CouponEndpointsTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public CouponEndpointsTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public Coupon Seed() {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var coupon = new CouponFaker().Generate();

        context.Coupon.Add(coupon);

        context.SaveChanges();

        return coupon;
    }

    [Fact]
    public async Task GetCoupon_WithValidCode_ReturnsOk() {
        var coupon = Seed();
        var expectedResponse = coupon.Adapt<CouponDto>();

        var getResponse = await _client.GetAsync($"/api/coupons/{coupon.Code}");
        var response = await getResponse.Content.ReadFromJsonAsync<CouponDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetCoupon_WithInvalidCode_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/coupons/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
