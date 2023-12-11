using IdentityApi.Models.Shared;
using IdentityApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Endpoints;

public static class IdentityEndpoints {
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/identity");


        group.MapPost("", async ([FromBody] RegistrationUserDto userForRegistration, IdentityService identityService) => {
            var result = await identityService.RegisterUser(userForRegistration);
            if(!result.Succeeded) {
                /* foreach(var error in result.Errors) {
                     ModelState.TryAddModelError(error.Code, error.Description);
                 }*/
                // return Results.BadRequest(ModelState);
                return Results.BadRequest();
            }

            return Results.Created();
        }).WithName("RegisterUser");

        group.MapPost("login", async ([FromBody] AuthenticationUserDto user, IdentityService identityService) => {
            if(!await identityService.ValidateUser(user))
                return Results.Unauthorized();

            var tokenDto = await identityService.CreateToken(populateExp: true);

            return Results.Ok(tokenDto);
        }).WithName("LoginUser");


    }
}
