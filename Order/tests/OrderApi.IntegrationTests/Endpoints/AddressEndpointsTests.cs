using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Shared;
using OrderApi.Shared.AddressDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using static OrderApi.Features.Addresses.CreateAddress;

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

        return addresses;
    }

    [Fact]
    public async Task CreateAddress_WithValidModel_ReturnsCreated() {
        var address = Seed(1).First();
        var addressRequest = address.Adapt<AddressRequest>();

        var postResponse = await _client.PostAsJsonAsync("/api/addresses", addressRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<AddressDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBe(default);
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
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAddress_WithValidId_ReturnsOk() {
        var addressRequest = Seed(1).First().Adapt<AddressRequest>();
        var postResponse = await _client.PostAsJsonAsync("/api/addresses", addressRequest);
        var addressDto = await postResponse.Content.ReadFromJsonAsync<AddressDto>();

        var getResponse = await _client.GetAsync($"/api/addresses/{addressDto.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<AddressDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Id.Should().NotBe(default);
        response.Should().BeEquivalentTo(addressDto);
    }

    [Fact]
    public async Task GetAddress_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/addresses/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAddresses_ReturnsOk() {
        var addresses = Seed(2).Adapt<List<AddressRequest>>();
        _client.PostAsJsonAsync("/api/addresses", addresses[0]);
        _client.PostAsJsonAsync("/api/addresses", addresses[1]);

        var getResponse = await _client.GetAsync("/api/addresses");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<AddressDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotContainNulls();
        response.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAddress_WithValidModel_ReturnsNoContent() {
        var addressRequest = Seed(1).First().Adapt<AddressRequest>();
        var postResponse = await _client.PostAsJsonAsync("/api/addresses", addressRequest);
        var addressDto = await postResponse.Content.ReadFromJsonAsync<AddressDto>();

        var response = await _client.PutAsJsonAsync($"/api/addresses/{addressDto.Id}", addressRequest);

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
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateAddress_WithInvalidId_ReturnsNotFound() {
        var address = Seed(1).First();
        var addressRequest = address.Adapt<AddressRequest>();

        var response = await _client.PutAsJsonAsync($"/api/addresses/44", addressRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAddress_WithValidId_ReturnsNoContent() {
        var addressRequest = Seed(1).First().Adapt<AddressRequest>();
        var postResponse = await _client.PostAsJsonAsync("/api/addresses", addressRequest);
        var addressDto = await postResponse.Content.ReadFromJsonAsync<AddressDto>();

        var response = await _client.DeleteAsync($"/api/addresses/{addressDto.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAddress_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/addresses/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
