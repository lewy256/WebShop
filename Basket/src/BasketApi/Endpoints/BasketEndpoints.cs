using BasketApi.Services;
using BasketApi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketApi.Endpoints;

public static class BasketEndpoints {
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/basket").WithTags("Basket");

        group.MapGet(String.Empty,
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async (BasketService basketService) => {
            var results = await basketService.GetBasketAsync();

            return results.Match(
                basket => Results.Ok(basket),
                notFound => Results.Problem(notFound));

        }).WithName("GetBasketAsync");

        group.MapPost(String.Empty,
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async ([FromBody] UpsertBasketDto basket, BasketService basketService) => {
            var results = await basketService.UpsertBasketAsync(basket);

            return results.Match(
               basket => Results.Ok(basket),
               notFound => Results.Problem(notFound),
               validationFailed => Results.Problem(validationFailed));

        }).WithName("UpsertBasketAsync");

        group.MapDelete(String.Empty,
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async (BasketService basketService) => {
            var results = await basketService.DeleteBasketAsync();

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.Problem(notfound));

        }).WithName("DeleteBasketAsync");

        group.MapMethods(String.Empty, ["OPTIONS"],
        [ProducesResponseType(StatusCodes.Status200OK)]
        (HttpContext context, LinkGenerator linkGenerator) => {
            context.Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE");

            var links = new List<Link>{
                new Link(linkGenerator.GetUriByName(context, "GetBasketAsync", values: new {})!,"get_basket","GET"),
                new Link(linkGenerator.GetUriByName(context, "UpsertBasketAsync", values: new {})!,"upsert_basket","POST"),
                new Link(linkGenerator.GetUriByName(context, "DeleteBasketAsync", values: new {})!, "delete_basket","DELETE")
            };

            return Results.Ok(links);
        });
    }
}