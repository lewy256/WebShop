using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model.ProductDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

public class ProductControllerTests : IClassFixture<ProductApiFactory> {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public ProductControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public List<Product> Seed(int count, bool createProduct) {
        using var scope = _productApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var categoires = new List<Category>();
        var products = new List<Product>();

        for(int i = 0; i < count; i++) {
            var category = SeedData.CategoryGenerator();
            categoires.Add(category);
            var product = SeedData.ProductGenerator(category.Id);
            products.Add(product);
        }

        if(createProduct) {
            context.Category.AddRange(categoires);
            context.Product.AddRange(products);
        }
        else {
            context.Category.AddRange(categoires);
        }

        context.SaveChanges();

        return products;
    }

    [Fact]
    public async Task CreateProduct_WithValidModel_ReturnsCreatedStatus() {
        var product = Seed(1, false).First();
        var createProductDto = product.Adapt<CreateProductDto>();
        var expectedResponse = product.Adapt<ProductDto>();

        var postResponse = await _client.PostAsJsonAsync($"/api/categories/{product.CategoryId}/products", createProductDto);
        var response = await postResponse.Content.ReadFromJsonAsync<ProductDto>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Id.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task CreateProduct_WithInvalidModel_ReturnsBadRequest() {
        var product = Seed(1, false).First();
        CreateProductDto createProductDto = null;

        var response = await _client.PostAsJsonAsync($"/api/categories/{product.CategoryId}/products", createProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var product = Seed(1, false).First();
        var createProductDto = product.Adapt<CreateProductDto>();
        createProductDto.ProductName = "";

        var response = await _client.PostAsJsonAsync($"/api/categories/{product.CategoryId}/products", createProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidId_ReturnsNotFound() {
        var product = Seed(1, false).First();
        var createProductDto = product.Adapt<CreateProductDto>();

        var response = await _client.PostAsJsonAsync($"/api/categories/{Guid.NewGuid()}/products", createProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsOkResult() {
        var product = Seed(1, true).First();
        var expectedResponse = product.Adapt<ProductDto>();

        var getResponse = await _client.GetAsync($"/api/categories/{product.CategoryId}/products/{product.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<ProductDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetProduct_WithInvalidCategoryId_ReturnsNotFound() {
        var product = Seed(1, true).First();

        var response = await _client.GetAsync($"/api/categories/{Guid.NewGuid()}/products/{product.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithInvalidProductId_ReturnsNotFound() {
        var product = Seed(1, true).First();

        var response = await _client.GetAsync($"/api/categories/{product.CategoryId}/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProducts_WithValidFields_ReturnsOkResult() {
        var product = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        var fields = "productName, price";

        var getResponse = await _client.GetAsync($"/api/categories/{product.CategoryId}/products?fields=" + fields);
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<object>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult() {
        var product = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/categories/{product.CategoryId}/products");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProducts_WithValidFields_ReturnsOkLinks() {
        var product = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");
        var fields = "productName, price";

        var getResponse = await _client.GetAsync($"/api/categories/{product.CategoryId}/products?fields=" + fields);
        var response = await getResponse.Content.ReadFromJsonAsync<object>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkLinks() {
        var product = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/categories/{product.CategoryId}/products");
        var response = await getResponse.Content.ReadFromJsonAsync<object>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
    }

    [Theory]
    [InlineData("pageSize=10&pageNumber=3&rating=4&minPrice=4&maxPrice=10")]
    [InlineData("searchTerm=test")]
    [InlineData("orderBy=price desc")]
    public async Task GetProducts_WithValidParameters_ReturnsOkResult(string queryParams) {
        var product = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/categories/{product.CategoryId}/products/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageSize=60&pageNumber=30")]
    [InlineData("minPrice=10&maxPrice=5")]
    [InlineData("orderBy=price,productName desc")]
    [InlineData("orderBy=id desc")]
    public async Task GetProducts_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams) {
        var product = Seed(2, true).First();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/categories/{product.CategoryId}/products/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateProduct_WithValidModel_ReturnsNoContent() {
        var product = Seed(1, true).First();
        var updateProductDto = product.Adapt<UpdateProductDto>();
        updateProductDto.ProductName = "Test";

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{product.Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdateProduct_WithInvalidModel_ReturnsBadRequest() {
        var product = Seed(1, true).First();
        UpdateProductDto updateProductDto = null;

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{product.Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidModel_ReturnsUnprocessableEntityResult() {
        var product = Seed(1, true).First();
        var updateProductDto = product.Adapt<UpdateProductDto>();
        updateProductDto.Price = -44;

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{product.Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidCategoryId_ReturnsNotFound() {
        var product = Seed(1, true).First();
        var updateProductDto = product.Adapt<UpdateProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{Guid.NewGuid()}/products/{product.Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidProductId_ReturnsNotFound() {
        var product = Seed(1, true).First();
        var updateProductDto = product.Adapt<UpdateProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{Guid.NewGuid()}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent() {
        var product = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/categories/{product.CategoryId}/products/{product.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidProductId_ReturnsNotFound() {
        var product = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/categories/{Guid.NewGuid()}/products/{product.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteProduct_WithInvalidCategoryId_ReturnsNotFound() {
        var product = Seed(1, true).First();

        var response = await _client.DeleteAsync($"/api/categories/{product.CategoryId}/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}