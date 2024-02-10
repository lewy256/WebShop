using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model.ProductDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V2;


[Collection("FixtureCollection")]
public class ProductControllerTests {
    private readonly HttpClient _client;

    private readonly ProductApiFactory _productApiFactory;

    public ProductControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("api-version", "2.0");
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
    public async Task GetProducts_WithValidFields_ReturnsOkResult() {
        var product = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        var fields = "productName, price";

        var getResponse = await _client.GetAsync($"/api/categories/{product.First().CategoryId}/products?fields=" + fields);
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<object>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult() {
        var products = await SeedAsync(2, true);
        var expectedResponse = products.Adapt<IEnumerable<ProductDto>>();
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var getResponse = await _client.GetAsync($"/api/categories/{products.First().CategoryId}/products");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        expectedResponse.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetProducts_WithValidFields_ReturnsOkLinks() {
        var product = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");
        var fields = "productName, price";

        var getResponse = await _client.GetAsync($"/api/categories/{product.First().CategoryId}/products?fields=" + fields);
        var response = await getResponse.Content.ReadFromJsonAsync<object>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkLinks() {
        var product = await SeedAsync(2, true);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.lewy256.hateoas+json");

        var getResponse = await _client.GetAsync($"/api/categories/{product.First().CategoryId}/products");
        var response = await getResponse.Content.ReadFromJsonAsync<object>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
    }
}
