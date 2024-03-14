using BasketApi.Services;
using BasketApi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketApi.Endpoints;

public static class BasketEndpoints {
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/basket").WithTags("Basket");

        group.MapGet("{id}",
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        async (Guid id, BasketService basketService) => {
            var results = await basketService.GetBasketAsync(id);

            return results.Match(
                basket => Results.Ok(basket));

        }).WithName("GetBasketAsync");

        group.MapPost("",
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        async ([FromBody] UpdateBasketDto basket, BasketService basketService) => {
            var results = await basketService.UpdateBasketAsync(basket);

            return results.Match(
               basket => Results.Ok(basket),
               validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName("UpdateBasketAsync");

        group.MapPost("{id}",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (Guid id, BasketService basketService) => {
            var results = await basketService.CheckoutBasketAsync(id);
            return results.Match(
                _ => Results.Accepted(),
                isEmpty => Results.BadRequest(isEmpty));

        }).WithName("CheckoutBasketAsync");

        group.MapDelete("{id}",
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        async (Guid id, BasketService basketService) => {
            var results = await basketService.DeleteBasketAsync(id);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName("DeleteBasketAsync");
    }
}