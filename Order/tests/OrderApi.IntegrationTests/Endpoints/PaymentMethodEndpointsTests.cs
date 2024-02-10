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
    public async Task CreatePaymentMethod_WithValidModel_ReturnsCreated() {
        var paymentMethod = Seed(1).First();
        var paymentMethodRequest = paymentMethod.Adapt<PaymentMethodRequest>();

        var postResponse = await _client.PostAsJsonAsync("/api/payment-methods", paymentMethodRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<PaymentMethodDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.PaymentMethodId.Should().NotBe(default);
        response.Should().BeEquivalentTo(paymentMethodRequest);
    }

    [Fact]
    public async Task CreatePaymentMethod_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/payment-methods", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePaymentMethod_WithInvalidModel_ReturnsUnprocessableEntity() {
        var paymentMethod = Seed(1).First();
        paymentMethod.Name = "";
        var paymentMethodRequest = paymentMethod.Adapt<PaymentMethodRequest>();

        var response = await _client.PostAsJsonAsync("/api/payment-methods", paymentMethodRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaymentMethod_WithValidId_ReturnsOk() {
        var paymentMethod = Seed(1).First();
        var expectedResponse = paymentMethod.Adapt<PaymentMethodDto>();

        var getResponse = await _client.GetAsync($"/api/payment-methods/{paymentMethod.PaymentMethodId}");
        var response = await getResponse.Content.ReadFromJsonAsync<PaymentMethodDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.PaymentMethodId.Should().NotBe(default);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.PaymentMethodId));
    }

    [Fact]
    public async Task GetPaymentMethod_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/payment-methods/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsOk() {
        var paymentMethods = Seed(2);
        var expectedResponse = paymentMethods.Adapt<IEnumerable<PaymentMethodDto>>();

        var getResponse = await _client.GetAsync("/api/payment-methods");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PaymentMethodDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.PaymentMethodId));
    }

    [Fact]
    public async Task UpdatePaymentMethod_WithValidModel_ReturnsNoContent() {
        var paymentMethod = Seed(1).First();
        var paymentMethodRequest = paymentMethod.Adapt<PaymentMethodRequest>();

        var response = await _client.PutAsJsonAsync($"/api/payment-methods/{paymentMethod.PaymentMethodId}", paymentMethodRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdatePaymentMethod_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PutAsync($"/api/payment-methods/1", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePaymentMethod_WithInvalidModel_ReturnsUnprocessableEntity() {
        var paymentMethod = Seed(1).First();
        paymentMethod.Name = "";
        var paymentMethodRequest = paymentMethod.Adapt<PaymentMethodRequest>();

        var response = await _client.PutAsJsonAsync($"/api/payment-methods/{paymentMethod.PaymentMethodId}", paymentMethodRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdatePaymentMethod_WithInvalidId_ReturnsNotFound() {
        var paymentMethod = Seed(1).First();
        var paymentMethodRequest = paymentMethod.Adapt<PaymentMethodRequest>();

        var response = await _client.PutAsJsonAsync($"/api/payment-methods/44", paymentMethodRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePaymentMethod_WithValidId_ReturnsNoContent() {
        var paymentMethod = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/payment-methods/{paymentMethod.PaymentMethodId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePaymentMethod_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/payment-methods/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
