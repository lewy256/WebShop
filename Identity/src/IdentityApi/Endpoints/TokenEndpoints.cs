using IdentityApi.Models.Shared;
using IdentityApi.Service;

namespace IdentityApi.Endpoints;

public static class TokenEndpoints {
    public static void MapTokenEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/token");

        group.MapPost("", async (TokenDto tokenDto, IdentityService identityService) => {
            var tokenDtoToReturn = await identityService.RefreshToken(tokenDto);

            return Results.Ok(tokenDtoToReturn);
        }).WithName("refresh");

    }
}
