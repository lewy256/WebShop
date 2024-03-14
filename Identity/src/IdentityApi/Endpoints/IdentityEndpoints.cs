using IdentityApi.Service;
using IdentityApi.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Endpoints;

public static class IdentityEndpoints {
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/identity").WithTags("Identity");

        group.MapPost("",
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async ([FromBody] RegistrationUserDto userForRegistration, IdentityService identityService) => {
            var results = await identityService.RegisterUser(userForRegistration);

            return results.Match(
                _ => Results.Created(),
                validationFailed => Results.UnprocessableEntity(validationFailed));
        }).WithName("RegisterUser");

        group.MapPost("login",
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async ([FromBody] AuthenticationUserDto user, IdentityService identityService) => {
            var results = await identityService.ValidateUser(user);

            return results.Match(
               tokenDto => Results.Ok(tokenDto),
               validationFailed => Results.UnprocessableEntity(validationFailed),
               _ => Results.Unauthorized());
        }).WithName("LoginUser");
    }
}
