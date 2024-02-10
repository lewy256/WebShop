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

public class StatusEndpointsTests : BaseIntegrationTest {
    private readonly OrderApiFactory _orderApiFactory;

    public StatusEndpointsTests(OrderApiFactory factory) : base(factory) {
        _orderApiFactory = factory;
    }

    public List<Status> Seed(int count) {
        using var scope = _orderApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        var statuses = new StatusFaker().Generate(count);

        context.Status.AddRange(statuses);

        context.SaveChanges();

        return statuses;
    }

    [Fact]
    public async Task CreateStatus_WithValidModel_ReturnsCreated() {
        var status = Seed(1).First();
        var statusRequest = status.Adapt<StatusRequest>();

        var postResponse = await _client.PostAsJsonAsync("/api/statuses", statusRequest);
        var response = await postResponse.Content.ReadFromJsonAsync<StatusDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.StatusId.Should().NotBe(default);
        response.Should().BeEquivalentTo(statusRequest);
    }

    [Fact]
    public async Task CreateStatus_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync("/api/statuses", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStatus_WithInvalidModel_ReturnsUnprocessableEntity() {
        var status = Seed(1).First();
        status.Description = "";
        var statusRequest = status.Adapt<StatusRequest>();

        var response = await _client.PostAsJsonAsync("/api/statuses", statusRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetStatus_WithValidId_ReturnsOk() {
        var status = Seed(1).First();
        var statusDto = status.Adapt<StatusDto>();

        var getResponse = await _client.GetAsync($"/api/statuses/{status.StatusId}");
        var response = await getResponse.Content.ReadFromJsonAsync<StatusDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.StatusId.Should().NotBe(default);
        response.Should().BeEquivalentTo(statusDto);
    }

    [Fact]
    public async Task GetStatus_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync($"/api/statuses/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStatuses_ReturnsOk() {
        var statuses = Seed(2);
        var statusDtos = statuses.Adapt<IEnumerable<StatusDto>>();

        var getResponse = await _client.GetAsync("/api/statuses");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<StatusDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        statuses.Should().BeEquivalentTo(response, opt => opt.Excluding(x => x.StatusId));
    }

    [Fact]
    public async Task UpdateStatus_WithValidModel_ReturnsNoContent() {
        var status = Seed(1).First();
        var statusRequest = status.Adapt<StatusRequest>();

        var response = await _client.PutAsJsonAsync($"/api/statuses/{status.StatusId}", statusRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PutAsync($"/api/statuses/1", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidModel_ReturnsUnprocessableEntity() {
        var status = Seed(1).First();
        status.Description = "";
        var statusRequest = status.Adapt<StatusRequest>();

        var response = await _client.PutAsJsonAsync($"/api/statuses/{status.StatusId}", statusRequest);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidId_ReturnsNotFound() {
        var status = Seed(1).First();
        var statusRequest = status.Adapt<StatusRequest>();

        var response = await _client.PutAsJsonAsync($"/api/statuses/44", statusRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStatus_WithValidId_ReturnsNoContent() {
        var status = Seed(1).First();

        var response = await _client.DeleteAsync($"/api/statuses/{status.StatusId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteStatus_WithInvalidId_ReturnsNotFound() {
        var response = await _client.DeleteAsync("/api/statuses/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
