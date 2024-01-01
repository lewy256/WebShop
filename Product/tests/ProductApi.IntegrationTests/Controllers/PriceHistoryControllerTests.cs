using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.PriceHistory;
using ProductApi.Shared.Model.PriceHistoryDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

public class PriceHistoryControllerTests : IClassFixture<ProductApiFactory> {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public PriceHistoryControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public List<PriceHistory> Seed(int count, bool createPriceHistory) {
        using var scope = _productApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var products = new List<Product>();
        var pricesHistory = new List<PriceHistory>();

        for(int i = 0; i < count; i++) {
            var product = SeedData.ProductGenerator(Guid.NewGuid());
            products.Add(product);
            var priceHistory = SeedData.PriceHistoryGenerator(product.Id);
            pricesHistory.Add(priceHistory);
        }

        if(createPriceHistory) {
            context.Product.AddRange(products);
            context.PriceHistory.AddRange(pricesHistory);
        }
        else {
            context.Product.AddRange(products);
        }

        context.SaveChanges();

        return pricesHistory;
    }

    [Fact]
    public async Task CreatePriceHistory_WithValidModel_ReturnsCreatedStatus() {
        var priceHistory = Seed(1, false).First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();
        var expectedResponse = priceHistory.Adapt<PriceHistoryDto>();

        var postResponse = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory", createPriceHistoryDto);
        var response = await postResponse.Content.ReadFromJsonAsync<PriceHistoryDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var priceHistory = Seed(1, false).First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();
        createPriceHistoryDto.StartDate = new DateTime(12 / 24 / 2023);
        createPriceHistoryDto.EndDate = new DateTime(12 / 24 / 2020);


        var response = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory", createPriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidModel_ReturnsBadRequest() {
        var priceHistory = Seed(1, false).First();
        CreatePriceHistoryDto createPriceHistoryDto = null;

        var response = await _client.PostAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory", createPriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePriceHistory_WithInvalidId_ReturnsNotFound() {
        var priceHistory = Seed(1, false).First();
        var createPriceHistoryDto = priceHistory.Adapt<CreatePriceHistoryDto>();

        var response = await _client.PostAsJsonAsync($"/api/products/{Guid.NewGuid()}/pricesHistory", createPriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistory_WithValidId_ReturnsOkResult() {
        var priceHistory = Seed(1, true).First();
        var expectedResponse = priceHistory.Adapt<PriceHistoryDto>();

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{priceHistory.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<PriceHistoryDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetPriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var priceHistory = Seed(1, true).First();

        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}/pricesHistory/{priceHistory.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var priceHistory = Seed(1, true).First();

        var response = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPricesHistory_ReturnsOkResult() {
        var priceHistory = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/pricesHistory");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PriceHistoryDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetPricesHistory_ReturnsOkLinks() {
        var priceHistory = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/pricesHistory");
        var response = await getResponse.Content.ReadFromJsonAsync<LinkedPriceHistoryEntity>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Should().NotBeNullOrEmpty();
        response.Links.Should().NotBeNullOrEmpty();
        response.Value.First().Links.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("pageSize=10&pageNumber=3&minPrice=4&maxPrice=10&startDate=12/24/2000&endDate=12/24/2023")]
    [InlineData("orderBy=priceValue desc")]
    [InlineData("")]
    public async Task GetPricesHistory_WithValidParameters_ReturnsOkResult(string queryParams) {
        var priceHistory = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageSize=60&pageNumber=30")]
    [InlineData("minPrice=10&maxPrice=5")]
    [InlineData("startDate=12/24/2023&endDate=12/24/2000")]
    [InlineData("orderBy=priceValue,endDate desc")]
    [InlineData("orderBy=id desc")]
    public async Task GetPricesHistory_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams) {
        var priceHistory = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithValidModel_ReturnsNoContent() {
        var priceHistory = Seed(1, true).First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();
        updatePriceHistoryDto.PriceValue = 44.44M;

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdatePriceHistory_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var priceHistory = Seed(1, true).First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();
        updatePriceHistoryDto.StartDate = new DateTime(12 / 24 / 2023);
        updatePriceHistoryDto.EndDate = new DateTime(12 / 24 / 2020);

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidModel_ReturnsBadRequest() {
        var priceHistory = Seed(1, true).First();
        UpdatePriceHistoryDto updatePriceHistoryDto = null;

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var priceHistory = Seed(1, true).First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}/pricesHistory/{priceHistory.Id}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var priceHistory = Seed(1, true).First();
        var updatePriceHistoryDto = priceHistory.Adapt<UpdatePriceHistoryDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{Guid.NewGuid()}", updatePriceHistoryDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePriceHistory_WithValidId_ReturnsNoContent() {
        var priceHistory = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{priceHistory.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePriceHistory_WithInvalidProductId_ReturnsNotFound() {
        var priceHistory = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/pricesHistory/{priceHistory.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeletePriceHistory_WithInvalidPriceHistoryId_ReturnsNotFound() {
        var priceHistory = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/products/{priceHistory.ProductId}/pricesHistory/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}