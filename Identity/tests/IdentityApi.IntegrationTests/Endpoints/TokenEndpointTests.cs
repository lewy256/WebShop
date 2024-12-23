﻿using FluentAssertions;
using IdentityApi.Shared;
using System.Net.Http.Json;
using Xunit;

namespace IdentityApi.IntegrationTests.Endpoints;

public class TokenEndpointTests : BaseIntegrationTest {
    public TokenEndpointTests(IdentityApiFactory factory) : base(factory) {
    }

    [Fact]
    public async Task RefreshToken_WithValidModel_ReturnsOk() {
        var registerDto = new RegistrationUserFaker().Generate();
        var test = await HttpClient.PostAsJsonAsync("/api/identity", registerDto);
        var loginDto = new AuthenticationUserDto() {
            UserName = registerDto.UserName,
            Password = registerDto.Password
        };
        var postResponse = await HttpClient.PostAsJsonAsync("/api/identity/login", loginDto);
        var tokenDto = await postResponse.Content.ReadFromJsonAsync<TokenDto>();

        var response = await HttpClient.PostAsJsonAsync("/api/token", tokenDto);
        var reshfreshedToken = await response.Content.ReadFromJsonAsync<TokenDto>();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        reshfreshedToken.RefreshToken.Should().NotBeNullOrEmpty();
        reshfreshedToken.AccessToken.Should().NotBeNullOrEmpty();

    }

    [Fact]
    public async Task RefreshToken_WithInvalidModel_ReturnsBadRequest() {
        var tokenDto = new TokenDto("test", "test");

        var response = await HttpClient.PostAsJsonAsync("/api/token", tokenDto);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}