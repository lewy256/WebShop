using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Entities;
using ProductApi.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers;

[Collection("FixtureCollection")]
public class WishlistControllerTests {
    private readonly HttpClient _client;
    private readonly ProductApiFactory _productApiFactory;

    public WishlistControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _client.DefaultRequestHeaders.Add("api-version", "1.0");

        _productApiFactory = productApiFactory;
    }

    public async Task<List<Product>> SeedAsync(int count) {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var wishlistItemToDelete = await context.WishlistItem.ToListAsync();

        context.WishlistItem.RemoveRange(wishlistItemToDelete);

        var faker = new ProductFaker();

        var products = faker.Generate(count);

        await context.Product.AddRangeAsync(products);

        await context.SaveChangesAsync();

        return products;
    }

    [Fact]
    public async Task GetWithlistItems_ReturnsOk() {
        var products = await SeedAsync(2);
        foreach(var product in products) {
            await _client.PostAsync($"/api/wishlists/{product.Id}", null);
        }

        var getResponse = await _client.GetAsync("api/wishlists");
        var response = await getResponse.Content.ReadFromJsonAsync<IEnumerable<WishlistItem>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().HaveCount(2);
        response.Should().NotContainNulls();
        response.Should().BeAssignableTo<List<WishlistItem>>();
    }

    [Fact]
    public async Task AddWishlistItem_WithValidProductId_ReturnsNoContent() {
        var products = await SeedAsync(1);

        var response = await _client.PostAsync($"/api/wishlists/{products.First().Id}", null);
        var getResponse = await _client.GetAsync("api/wishlists");
        var wishlistItems = await getResponse.Content.ReadFromJsonAsync<IEnumerable<WishlistItem>>();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        wishlistItems.Should().HaveCount(1);
        wishlistItems.Should().NotContainNulls();
        wishlistItems.Should().BeAssignableTo<List<WishlistItem>>();
    }

    [Fact]
    public async Task AddWishlistItem_WithInvalidProductId_ReturnsNotFound() {
        var products = await SeedAsync(1);
        var id = Guid.NewGuid();

        var response = await _client.PostAsync($"/api/wishlists/{id}", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWishlistItem_WithValidItemId_ReturnsNoContent() {
        var products = await SeedAsync(1);
        await _client.PostAsync($"/api/wishlists/{products.First().Id}", null);
        var getResponse = await _client.GetAsync("api/wishlists");
        var wishlistItem = await getResponse.Content.ReadFromJsonAsync<List<WishlistItem>>();

        var response = await _client.DeleteAsync($"/api/wishlists/{wishlistItem.First().Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteWishlistItem_WithInvalidItemId_ReturnsNotFound() {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/wishlists/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
