using IdentityApi.Service;
using IdentityApi.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Endpoints;

public static class TokenEndpoints {
    public static void MapTokenEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/token").WithTags("Token");

        group.MapPost("",
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async ([FromBody] TokenDto tokenDto, IdentityService identityService) => {
            var results = await identityService.RefreshToken(tokenDto);

            return results.Match(
                refreshedToken => Results.Ok(refreshedToken),
                validationFailed => Results.Problem(validationFailed));
        }).WithName("RefreshToken");

        group.MapMethods("", ["OPTIONS"],
        [ProducesResponseType(StatusCodes.Status200OK)]
        (HttpContext context, LinkGenerator linkGenerator) => {
            context.Response.Headers.Add("Allow", "OPTIONS, POST"); ;

            var links = new List<Link>{
                new Link(linkGenerator.GetUriByName(context, "RefreshToken", values: new {})!,"refresh_token","POST"),
            };

            return Results.Ok(links);
        });
    }
}
