using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace OrderApi.IntegrationTests.Endpoints;

public class AddressEndpointsTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public AddressEndpointsTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public List<Address> Seed(int count) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var addresses = new AddressFaker().Generate(count);

        context.Address.AddRange(addresses);

        context.SaveChanges();

        return addresses;
    }

    [Fact]
    public async Task CreateAddress_WithValidModel_ReturnsCreated() {
        var address = Seed(1).First();
        var addressRequest = address.Adapt<AddressRequest>();

        var postResponse = await _client.PostAsJsonAsync("/api/addresses", addressRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<AddressDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.AddressId.Should().NotBe(default);
        response.Should().BeEquivalentTo(addressRequest);
    }

    [Fact]
    public async Task CreateAddress_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/addresses", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAddress_WithInvalidModel_ReturnsUnprocessableEntity() {
        var address = Seed(1).First();
        address.FirstName = "";
        var addressRequest = address.Adapt<AddressRequest>();

        var response = await _client.PostAsJsonAsync("/api/addresses", addressRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAddress_WithValidId_ReturnsOk() {
        var address = Seed(1).First();
        var addressDto = address.Adapt<AddressDto>();

        var getResponse = await _client.GetAsync($"/api/addresses/{address.AddressId}");
        var response = await getResponse.Content.ReadFromJsonAsync<AddressDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.AddressId.Should().NotBe(default);
        response.Should().BeEquivalentTo(addressDto);
    }

    [Fact]
    public async Task GetAddress_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/addresses/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAddresses_ReturnsOk() {
        var addresses = Seed(2);
        var addressDtos = addresses.Adapt<IEnumerable<AddressDto>>();

        var getResponse = await _client.GetAsync("/api/addresses");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<AddressDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(addressDtos, opt => opt.Excluding(x => x.AddressId));
    }

    [Fact]
    public async Task UpdateAddress_WithValidModel_ReturnsNoContent() {
        var address = Seed(1).First();
        var addressRequest = address.Adapt<AddressRequest>();

        var response = await _client.PutAsJsonAsync($"/api/addresses/{address.AddressId}", addressRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateAddress_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PutAsync($"/api/addresses/1", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAddress_WithInvalidModel_ReturnsUnprocessableEntity() {
        var address = Seed(1).First();
        address.FirstName = "";
        var addressRequest = address.Adapt<AddressRequest>();

        var response = await _client.PutAsJsonAsync($"/api/addresses/{address.AddressId}", addressRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateAddress_WithInvalidId_ReturnsNotFound() {
        var address = Seed(1).First();
        var addressRequest = address.Adapt<AddressRequest>();

        var response = await _client.PutAsJsonAsync($"/api/addresses/44", addressRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchAddress_WithValidModel_ReturnsNoContent() {
        var address = Seed(1).First();
        var content = new StringContent("[{ \"op\": \"replace\", \"path\": \"firstName\", \"value\": \"Jan\" }]", Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"/api/addresses/{address.AddressId}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PatchAddress_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PatchAsJsonAsync($"/api/addresses/1", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PatchAddress_WithInvalidModel_ReturnsUnprocessableEntity() {
        var address = Seed(1).First();
        var content = new StringContent("[{ \"op\": \"replace\", \"path\": \"firstName\", \"value\": \"\" }]", Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"/api/addresses/{address.AddressId}", content);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task PatchAddress_WithInvalidId_ReturnsNotFound() {
        var content = new StringContent("[{ \"op\": \"replace\", \"path\": \"firstName\", \"value\": \"Jan\" }]", Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync("/api/addresses/44", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteAddress_WithValidId_ReturnsNoContent() {
        var address = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/addresses/{address.AddressId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAddress_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/addresses/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
