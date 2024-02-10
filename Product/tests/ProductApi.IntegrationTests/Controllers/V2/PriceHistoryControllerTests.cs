using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Shared.Model.PriceHistoryDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V2;


[Collection("FixtureCollection")]
public class PriceHistoryControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public PriceHistoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("api-version", "2.0");
        _productApiFactory = productApiFactory;
    }

    public async Task<List<PriceHistory>> SeedAsync(int count, bool savePriceHistory) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var pricesHistoryToDelete = await context.PriceHistory.ToListAsync();

        context.PriceHistory.RemoveRange(pricesHistoryToDelete);

        var faker = new PriceHistoryFaker();

        await context.Product.AddAsync(faker.Product);

        var pricesHistory = new List<PriceHistory>();

        if(savePriceHistory) {
            pricesHistory = faker.Generate(count);
            await context.PriceHistory.AddRangeAsync(pricesHistory);
        }
        else {
            pricesHistory = faker.Generate(count);
        }

        await context.SaveChangesAsync();

        return pricesHistory;
    }

    [Fact]
    public async Task GetPricesHistory_ReturnsOkResult() {
        var pricesHistory = await SeedAsync(2, true);
        var expectedResponse = pricesHistory.Adapt<IEnumerable<PriceHistoryDto>>();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/products/{pricesHistory.First().ProductId}/prices-history");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PriceHistoryDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetPricesHistory_ReturnsOkLinks() {
        var priceHistory = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.First().ProductId}/prices-history");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedPriceHistoryEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNullOrEmpty();
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }
}
