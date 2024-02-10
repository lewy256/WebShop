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

public class CouponEndpointsTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public CouponEndpointsTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public List<Coupon> Seed(int count) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var coupons = new CouponFaker().Generate(count);

        context.Coupon.AddRange(coupons);

        context.SaveChanges();

        return coupons;
    }

    [Fact]
    public async Task CreateCoupon_WithValidModel_ReturnsCreated() {
        var coupon = Seed(1).First();
        var couponRequest = coupon.Adapt<CouponRequest>();

        var postResponse = await _client.PostAsJsonAsync("/api/coupons", couponRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<CouponDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.CouponId.Should().NotBe(default);
        response.Should().BeEquivalentTo(couponRequest);
    }

    [Fact]
    public async Task CreateCoupon_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/coupons", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCoupon_WithInvalidModel_ReturnsUnprocessableEntity() {
        var coupon = Seed(1).First();
        coupon.Description = "";
        var couponRequest = coupon.Adapt<CouponRequest>();

        var response = await _client.PostAsJsonAsync("/api/coupons", couponRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCoupon_WithValidId_ReturnsOk() {
        var coupon = Seed(1).First();
        var expectedResponse = coupon.Adapt<CouponDto>();

        var getResponse = await _client.GetAsync($"/api/coupons/{coupon.CouponId}");
        var response = await getResponse.Content.ReadFromJsonAsync<CouponDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.CouponId.Should().NotBe(default);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.CouponId));
    }

    [Fact]
    public async Task GetCoupon_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/coupons/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCoupons_ReturnsOk() {
        var coupons = Seed(2);
        var expectedResponse = coupons.Adapt<IEnumerable<CouponDto>>();

        var getResponse = await _client.GetAsync("/api/coupons");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<CouponDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.CouponId));
    }

    [Fact]
    public async Task UpdateCoupon_WithValidModel_ReturnsNoContent() {
        var coupon = Seed(1).First();
        var couponRequest = coupon.Adapt<CouponRequest>();

        var response = await _client.PutAsJsonAsync($"/api/coupons/{coupon.CouponId}", couponRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateCoupon_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PutAsync($"/api/coupons/1", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCoupon_WithInvalidModel_ReturnsUnprocessableEntity() {
        var coupon = Seed(1).First();
        coupon.Description = "";
        var couponRequest = coupon.Adapt<CouponRequest>();

        var response = await _client.PutAsJsonAsync($"/api/coupons/{coupon.CouponId}", couponRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateCoupon_WithInvalidId_ReturnsNotFound() {
        var coupon = Seed(1).First();
        var couponRequest = coupon.Adapt<CouponRequest>();

        var response = await _client.PutAsJsonAsync($"/api/coupons/44", couponRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCoupon_WithValidId_ReturnsNoContent() {
        var coupon = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/coupons/{coupon.CouponId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCoupon_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/coupons/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
