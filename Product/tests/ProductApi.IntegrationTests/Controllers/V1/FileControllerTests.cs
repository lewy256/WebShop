using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ProductDtos;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace ProductApi.IntegrationTests.Controllers.V1;

[Collection("FixtureCollection")]
public class FileControllerTests {
    private readonly HttpClient _client;
    private readonly ProductApiFactory _productApiFactory;

    public FileControllerTests(ProductApiFactory productApiFactory) {
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

        await context.Category.AddAsync(faker.Category);

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
    public async Task UploadImages_WithValidProductId_ReturnsCreated() {
        var products = await SeedAsync(1, true);
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Car.txt");
        using var content2 = new StreamContent(file2);
        using var file3 = File.OpenRead(@"./Files/Rocket.png");
        using var content3 = new StreamContent(file3);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Car.txt" },
            { content3, "files", "Rocker.png" }
        };

        var response = await _client.PostAsync($"/api/products/{products.First().Id}/files", formData);
        var fileDto = await response.Content.ReadFromJsonAsync<FileDto>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        fileDto.NotUploadedFiles.Should().HaveCount(1);
        fileDto.NotUploadedFiles.Should().NotContainNulls();
        fileDto.FileNames.Should().HaveCount(2);
        fileDto.FileNames.Should().NotContainNulls();
        fileDto.TotalFilesUploaded.Should().Be(2);
        fileDto.TotalSizeUploaded.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadImages_WithInvalidProductId_ReturnsNotFound() {
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" }
        };

        var response = await _client.PostAsync($"/api/products/{Guid.NewGuid()}/files", formData);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UploadImages_WithoutFiles_ReturnsUnsupportedMediaType() {
        var requestBody = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/api/products/{Guid.NewGuid()}/files", requestBody);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task DeleteImage_WithValidProductId_ReturnsNoContent() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Rocket.png");
        using var content2 = new StreamContent(file2);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Rocket.png" }
        };
        var postResponse = await _client.PostAsync($"/api/products/{product.Id}/files", formData);
        var fileDto = await postResponse.Content.ReadFromJsonAsync<FileDto>();

        var response = await _client.DeleteAsync($"/api/products/{product.Id}/files/{fileDto.FileNames.First()}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteImage_WithInvalidProductId_ReturnsNotFound() {
        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/files/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteImage_WithInvalidImageId_ReturnsNotFound() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Rocket.png");
        using var content2 = new StreamContent(file2);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Rocket.png" }
        };
        await _client.PostAsync($"/api/products/{product.Id}/files", formData);

        var response = await _client.DeleteAsync($"/api/products/{product.Id}/files/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithImageNames_ReturnsOk() {
        var products = await SeedAsync(1, true);
        var product = products.First();
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Rocket.png");
        using var content2 = new StreamContent(file2);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Rocket.png" }
        };
        await _client.PostAsync($"/api/products/{product.Id}/files", formData);

        var response = await _client.GetAsync($"/api/categories/{product.CategoryId}/products/{product.Id}");
        var productDto = await response.Content.ReadFromJsonAsync<ProductDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        productDto.ImageUris.Should().NotBeNull();
        productDto.ImageUris.Should().NotContainNulls();
        productDto.ImageUris.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProducts_WithImageNames_ReturnsOk() {
        var products = await SeedAsync(2, true);
        var product = products.First();
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Rocket.png");
        using var content2 = new StreamContent(file2);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Rocket.png" }
        };
        await _client.PostAsync($"/api/products/{product.Id}/files", formData);

        var response = await _client.GetAsync($"/api/categories/{product.CategoryId}/products");
        var productDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        var productDto = productDtos.Where(p => p.Id == product.Id).SingleOrDefault();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        productDto.ImageUris.Should().NotBeNull();
        productDto.ImageUris.Should().NotContainNulls();
        productDto.ImageUris.Should().HaveCount(2);
    }
}