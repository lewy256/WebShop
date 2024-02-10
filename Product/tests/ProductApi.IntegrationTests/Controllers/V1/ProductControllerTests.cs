using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.Responses;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V1;

[Collection("FixtureCollection")]
public class ProductControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public ProductControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public async Task<List<Product>> SeedAsync(int count, bool saveProduct) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var productsToDelete = await context.Product.ToListAsync();

        context.Product.RemoveRange(productsToDelete);

        var faker = new ProductFaker();

        await context.Category.AddRangeAsync(faker.Category);

        var products = new List<Product>();

        if(saveProduct) {
            products = faker.Generate(count);
            await context.Product.AddRangeAsync(products);
        }
        else {
            products = faker.Generate(count);
        }

        await context.SaveChangesAsync();

        return products;
    }

    [Fact]
    public async Task CreateProduct_WithValidModel_ReturnsCreatedStatus() {
        var products = await SeedAsync(1, false);
        var product = products.First();
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
        var products = await SeedAsync(1, false);

        var response = await _client.PostAsJsonAsync($"/api/categories/{products.First().CategoryId}/products", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidModel_ReturnsUnprocessableEntity() {
        var products = await SeedAsync(1, false);
        var product = products.First();
        product.ProductName = "";
        var createProductDto = product.Adapt<CreateProductDto>();

        var response = await _client.PostAsJsonAsync($"/api/categories/{product.CategoryId}/products", createProductDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidId_ReturnsNotFound() {
        var products = await SeedAsync(1, false);
        var product = products.First();
        var createProductDto = product.Adapt<CreateProductDto>();

        var response = await _client.PostAsJsonAsync($"/api/categories/{Guid.NewGuid()}/products", createProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsOkResult() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        var expectedResponse = product.Adapt<ProductDto>();

        var getResponse = await _client.GetAsync($"/api/categories/{product.CategoryId}/products/{product.Id}");
        var response = await getResponse.Content.ReadFromJsonAsync<ProductDto>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetProduct_WithInvalidCategoryId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/categories/{Guid.NewGuid()}/products/{products.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithInvalidProductId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);

        var response = await _client.GetAsync($"/api/categories/{products.First().CategoryId}/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProducts_ReturnsOk() {
        var products = await SeedAsync(2, true);
        var expectedResponse = products.Adapt<IEnumerable<ProductDto>>();

        var getResponse = await _client.GetAsync($"/api/categories/{products.First().CategoryId}/products");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Theory]
    [InlineData("pageSize=10&pageNumber=3&rating=4&minPrice=4&maxPrice=10")]
    [InlineData("searchTerm=test")]
    [InlineData("orderBy=price desc")]
    public async Task GetProducts_WithValidParameters_ReturnsOkResult(string queryParams) {
        var products = await SeedAsync(2, true);

        var response = await _client.GetAsync($"/api/categories/{products.First().CategoryId}/products/?" + queryParams);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageSize=60&pageNumber=30", 1)]
    [InlineData("minPrice=10&maxPrice=5", 1)]
    [InlineData("orderBy=price,productName desc", 1)]
    [InlineData("orderBy=id desc", 1)]
    public async Task GetProducts_WithInvalidParameters_ReturnsUnprocessableEntity(string queryParams, int count) {
        var products = await SeedAsync(2, true);

        var response = await _client.GetAsync($"/api/categories/{products.First().CategoryId}/products/?" + queryParams);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(count);
    }

    [Fact]
    public async Task UpdateProduct_WithValidModel_ReturnsNoContent() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        product.ProductName = "Test";
        var updateProductDto = product.Adapt<UpdateProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{product.Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task UpdateProduct_WithInvalidModel_ReturnsBadRequest() {
        var products = await SeedAsync(1, true);
        UpdateProductDto updateProductDto = null;

        var response = await _client.PutAsJsonAsync($"/api/categories/{products.First().CategoryId}/products/{products.First().Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidModel_ReturnsUnprocessableEntity() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        product.Price = -44;
        var updateProductDto = product.Adapt<UpdateProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{product.Id}", updateProductDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidCategoryId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        var updateProductDto = product.Adapt<UpdateProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{Guid.NewGuid()}/products/{product.Id}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidProductId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        var updateProductDto = product.Adapt<UpdateProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/categories/{product.CategoryId}/products/{Guid.NewGuid()}", updateProductDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent() {
        var products = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/categories/{products.First().CategoryId}/products/{products.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidProductId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/categories/{Guid.NewGuid()}/products/{products.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteProduct_WithInvalidCategoryId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);

        var response = await _client.DeleteAsync($"/api/categories/{products.First().CategoryId}/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}