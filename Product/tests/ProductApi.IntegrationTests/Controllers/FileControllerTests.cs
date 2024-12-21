using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ProductApi.Entities;
using ProductApi.Infrastructure;
using ProductApi.Shared.FilesDtos;
using ProductApi.Shared.ProductDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using File = System.IO.File;

namespace ProductApi.IntegrationTests.Controllers;

[Collection("FixtureCollection")]
public class FileControllerTests {
    private readonly HttpClient _client;
    private readonly ProductApiFactory _productApiFactory;

    public FileControllerTests(ProductApiFactory productApiFactory) {
        _client = productApiFactory.CreateClient();
        _productApiFactory = productApiFactory;
    }

    public async Task<Product> SeedAsync() {
        await using var scope = _productApiFactory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

        await context.Database.EnsureCreatedAsync();

        var faker = new ProductFaker();

        await context.Category.AddAsync(faker.Category);

        var product = faker.Generate();

        await context.Product.AddAsync(product);

        await context.SaveChangesAsync();

        return product;
    }

    [Fact]
    public async Task UploadFilesForProduct_WithValidProductId_ReturnsCreated() {
        var product = await SeedAsync();
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

        var response = await _client.PostAsync($"/api/products/{product.Id}/files", formData);
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
    public async Task UploadFilesForProduct_WithInvalidProductId_ReturnsNotFound() {
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" }
        };

        var response = await _client.PostAsync($"/api/products/{Guid.NewGuid()}/files", formData);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UploadFilesForProduct_WithoutFiles_ReturnsUnsupportedMediaType() {
        var response = await _client.PostAsync($"/api/products/{Guid.NewGuid()}/files", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task DeleteFilesForProduct_WithValidFileIds_ReturnsNoContent() {
        var product = await SeedAsync();
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

        var response = await _client.DeleteAsync($"/api/products/{product.Id}/files/collection/({fileDto.FileNames[0]},{fileDto.FileNames[1]})");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteFilesForProduct_WithInvalidProductId_ReturnsNotFound() {
        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}/files/collection/({Guid.NewGuid()})");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteFilesForProduct_WithInvalidFileIds_ReturnsNotFound() {
        var product = await SeedAsync();
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Rocket.png");
        using var content2 = new StreamContent(file2);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Rocket.png" }
        };
        await _client.PostAsync($"/api/products/{product.Id}/files", formData);

        var response = await _client.DeleteAsync($"/api/products/{product.Id}/files/collection/({Guid.NewGuid()},{Guid.NewGuid()})");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_WithFiles_ReturnsOk() {
        var product = await SeedAsync();
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
        productDto.Files.Should().NotBeNull();
        productDto.Files.Should().NotContainNulls();
        productDto.Files.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProducts_WithFiles_ReturnsOk() {
        var products = new List<Product> {
            await SeedAsync(),
            await SeedAsync()
        };
        var product = products.First();
        using var file1 = File.OpenRead(@"./Files/Car.jpg");
        using var content1 = new StreamContent(file1);
        using var file2 = File.OpenRead(@"./Files/Rocket.png");
        using var content2 = new StreamContent(file2);
        using var formData = new MultipartFormDataContent {
            { content1, "files", "Car.jpg" },
            { content2, "files", "Rocket.png" }
        };
        var test = await _client.PostAsync($"/api/products/{product.Id}/files", formData);
        _client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

        var response = await _client.GetAsync($"/api/categories/{product.CategoryId}/products");
        var productDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        var productDto = productDtos.Where(p => p.Id == product.Id).SingleOrDefault();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        productDto.Files.Should().NotBeNull();
        productDto.Files.Should().NotContainNulls();
        productDto.Files.Should().HaveCount(2);
    }
}