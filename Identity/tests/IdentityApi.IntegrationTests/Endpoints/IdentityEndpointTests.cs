using FluentAssertions;
using IdentityApi.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public async Task RegisterUser_WithInvalidModel_ReturnsUnprocessableEntity() {
        var registerDto = new RegistrationUserDto() {
            FirstName = "",
            LastName = "Kowalski",
            UserName = "%23fjs",
            Password = "Admin01",
            Email = "@test.com",
            PhoneNumber = "44"
        };

        var response = await HttpClient.PostAsJsonAsync("/api/identity", registerDto);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var jsonArray = problemDetails.Extensions["errors"].ToString();
        var errors = JsonConvert.DeserializeObject<ValidationError[]>(jsonArray);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        errors.Should().HaveCount(5);
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
    public async Task Login_WithInvalidModel_ReturnsBadRequest() {
        var response = await HttpClient.PostAsync("/api/identity/login", null);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
