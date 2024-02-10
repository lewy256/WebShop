using FluentAssertions;
using IdentityApi.Responses;
using IdentityApi.Shared;
using System.Net.Http.Json;
using Xunit;

namespace IdentityApi.IntegrationTests.Endpoints;

public class IdentityEndpointTests : BaseIntegrationTest {
    public IdentityEndpointTests(IdentityApiFactory factory) : base(factory) {
    }

    [Fact]
    public async Task RegisterUser_WithValidModel_ReturnsCreated() {
        var registerDto = new RegistrationUserFaker().Generate();

        var response = await HttpClient.PostAsJsonAsync("/api/identity", registerDto);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task RegisterUser_WithValidModel_ReturnsUnprocessableEntity() {
        var registerDto = new RegistrationUserDto();

        var response = await HttpClient.PostAsJsonAsync("/api/identity", registerDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidModel_ReturnsBadRequest() {
        var response = await HttpClient.PostAsync("/api/identity", null);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithRegisteredUser_ReturnsOk() {
        var registerDto = new RegistrationUserFaker().Generate();
        await HttpClient.PostAsJsonAsync("/api/identity", registerDto);

        var loginDto = new AuthenticationUserDto() {
            UserName = registerDto.UserName,
            Password = registerDto.Password
        };

        var response = await HttpClient.PostAsJsonAsync("/api/identity/login", loginDto);
        var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        tokenDto.Should().NotBeNull();
        tokenDto.RefreshToken.Should().NotBeNullOrEmpty();
        tokenDto.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithUnregisteredUser_ReturnsUnauthorized() {
        var loginDto = new AuthenticationUserFaker().Generate();

        var response = await HttpClient.PostAsJsonAsync("/api/identity/login", loginDto);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnregisteredUser_ReturnsUnprocessableEntity() {
        var loginDto = new AuthenticationUserDto();

        var response = await HttpClient.PostAsJsonAsync("/api/identity/login", loginDto);
        var errors = await response.Content.ReadFromJsonAsync<ValidationResponse>();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        errors.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Login_WithInvalidModel_ReturnsBadRequest() {
        var response = await HttpClient.PostAsync("/api/identity/login", null);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
