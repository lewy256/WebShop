using IdentityApi.Service;
using IdentityApi.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Endpoints;

public static class TokenEndpoints {
    public static void MapTokenEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/token").WithTags("Token");

        group.MapPost("",
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async ([FromBody] TokenDto? tokenDto, IdentityService identityService) => {
            var results = await identityService.RefreshToken(tokenDto);

            return results.Match(
                refreshedToken => Results.Ok(refreshedToken),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                tokenInvalid => Results.BadRequest(tokenInvalid),
                modelIsNull => Results.BadRequest(modelIsNull));
        }).WithName("RefreshToken");
    }
}
