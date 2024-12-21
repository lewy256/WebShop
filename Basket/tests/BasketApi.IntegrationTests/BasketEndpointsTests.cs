using BasketApi.Shared;
using FluentAssertions;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.Redis;
using Xunit;

namespace BasketApi.IntegrationTests;

public class BasketEndpointsTests : IAsyncLifetime {
    private HttpClient _client = null!;

    private readonly RedisContainer _redisContainer =
        new RedisBuilder()
            .WithImage("redis:7.0")
            .Build();

    [Fact]
    public async Task UpsertBasket_WithValidModel_ReturnsOk() {
        var upsertBasketDto = new BasketFaker().Generate().Adapt<UpsertBasketDto>();

        var response = await _client.PostAsJsonAsync("/api/basket", upsertBasketDto);
        var expectedResponse = await response.Content.ReadFromJsonAsync<BasketDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        upsertBasketDto.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(b => b.TotalPrice));
    }

    [Fact]
    public async Task UpsertBasket_WithInvalidModel_ReturnsBadRequest() {
        var response = await _client.PostAsync($"/api/basket", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpsertBasket_WithInvalidModel_ReturnsUnprocessableEntity() {
        var upsertBasketDto = new BasketFaker().Generate().Adapt<UpsertBasketDto>();
        upsertBasketDto.Items[0].Name = "";

        var response = await _client.PostAsJsonAsync("/api/basket", upsertBasketDto);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetBasket_WithValidId_ReturnsOk() {
        var upsertBasketDto = new BasketFaker().Generate().Adapt<UpsertBasketDto>();
        var postResponse = await _client.PostAsJsonAsync("/api/basket", upsertBasketDto);
        var expectedResponse = await postResponse.Content.ReadFromJsonAsync<BasketDto>();

        var response = await _client.GetAsync("/api/basket");
        var basketDto = await response.Content.ReadFromJsonAsync<BasketDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        basketDto.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(b => b.TotalPrice));
    }


    [Fact]
    public async Task GetBasket_WithInvalidId_ReturnsNotFound() {
        var response = await _client.GetAsync("/api/basket/{Guid.NewGuid()}");
        var basketDto = await response.Content.ReadFromJsonAsync<BasketDto>();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteBasket_WithValidId_ReturnsNoContent() {
        var upsertBasketDto = new BasketFaker().Generate().Adapt<UpsertBasketDto>();
        var postResponse = await _client.PostAsJsonAsync("/api/basket", upsertBasketDto);
        var basketDto = await postResponse.Content.ReadFromJsonAsync<BasketDto>();

        var response = await _client.DeleteAsync("/api/basket");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBasket_WithInvalid_ReturnsNotFound() {
        var response = await _client.DeleteAsync($"/api/basket");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync() {
        await _redisContainer.StartAsync();

        var waf = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    services.AddMassTransitTestHarness();

                    services.AddStackExchangeRedisCache(options => {
                        options.Configuration = _redisContainer.GetConnectionString();
                    });

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            });

        _client = waf.CreateClient();


    }

    public async Task DisposeAsync() {
        await _redisContainer.DisposeAsync();
    }
}


