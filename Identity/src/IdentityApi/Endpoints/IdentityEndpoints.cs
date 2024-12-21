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
                validationFailed => Results.Problem(validationFailed));
        }).WithName("RegisterUser");

        group.MapPost("login",
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async ([FromBody] AuthenticationUserDto user, IdentityService identityService) => {
            var results = await identityService.ValidateUser(user);

            return results.Match(
               tokenDto => Results.Ok(tokenDto),
               unauthorized => Results.Problem(unauthorized));
        }).WithName("LoginUser");

        group.MapMethods("", ["OPTIONS"],
        [ProducesResponseType(StatusCodes.Status200OK)]
        (HttpContext context, LinkGenerator linkGenerator) => {
            context.Response.Headers.Add("Allow", "OPTIONS, POST");

            var links = new List<Link>{
                new Link(linkGenerator.GetUriByName(context, "RegisterUser", values: new {})!,"register_user","POST"),
                new Link(linkGenerator.GetUriByName(context, "LoginUser", values: new {})!,"login_user","POST")
           };

            return Results.Ok(links);
        });
    }
}
