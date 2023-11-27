using Bogus;
using FluentAssertions;
using Mapster;
using ProductApi.Shared.Model;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

public class ProductControllerTests : IClassFixture<ProductApiFactory> {
    private readonly HttpClient _client;

    private readonly Faker<CreateProductDto> _productGenerator =
        new Faker<CreateProductDto>()
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.ProductNumber, f => f.Random.Int(0, 10))
            .RuleFor(x => x.Price, f => f.Random.Decimal(0.00M, 100_000M))
            .RuleFor(x => x.Stock, f => f.Random.Int(0, 10))
            .RuleFor(x => x.CategoryID, f => f.Random.Int(0, 10))
            .RuleFor(x => x.Category, f => f.Commerce.Categories(1).FirstOrDefault())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Color, f => f.Commerce.Color())
            .RuleFor(x => x.Weight, f => f.Random.Int(0, 10))
            .UseSeed(1000);


    private ProductApiFactory _productApiFactory;

    public ProductControllerTests(ProductApiFactory productApiFactory) {
        _productApiFactory = productApiFactory;
        _client = _productApiFactory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_WithValidModel_ReturnsCreatedStatus() {
        var createProductDto = _productGenerator.Generate();
        var expectedResponse = createProductDto.Adapt<ProductDto>();

        var postResponse = await _client.PostAsJsonAsync("/api/Product", createProductDto);
        var productResponse = await postResponse.Content.ReadFromJsonAsync<ProductDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        productResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        productResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsOkResult() {
        var createProductDto = _productGenerator.Generate();
        var postResponse = await _client.PostAsJsonAsync("/api/Product", createProductDto);
        var expectedResponse = await postResponse.Content.ReadFromJsonAsync<ProductDto>();

        var getResponse = await _client.GetAsync($"/api/Product/{expectedResponse.Id}");
        var productResponse = await getResponse.Content.ReadFromJsonAsync<ProductDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        productResponse.Should().BeEquivalentTo(expectedResponse);
    }


    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent() {
        var createProductDto = _productGenerator.Generate();
        var postResponse = await _client.PostAsJsonAsync("/api/Product", createProductDto);

        var product = await postResponse.Content.ReadFromJsonAsync<ProductDto>();
        var deleteResponse = await _client.DeleteAsync($"/api/Product/{product.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/Product/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("pageSize=5&pageNumber=1")]
    [InlineData("pageSize=15&pageNumber=2")]
    [InlineData("pageSize=10&pageNumber=3")]
    public async Task GetProducts_WithValidQueryParameters_ReturnsOkResult(string queryParams) {
        var getResponse = await _client.GetAsync($"/api/Product?" + queryParams);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /*    [Theory]
        [InlineData("pageSize=100&pageNumber=3")]
        [InlineData("pageSize=11&pageNumber=1")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetProducts_WithInvalidQueryParameters_ReturnsBadRequest(string queryParams) {
            var getResponse = await _client.GetAsync($"/api/Product?" + queryParams);

            getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }*/
}