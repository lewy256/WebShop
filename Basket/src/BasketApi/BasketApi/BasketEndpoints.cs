using BasketApi.Entities;
using BasketApi.Services;

namespace BasketApi;

public static class BasketEndpoints {
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/basket");

        group.MapGet("{id:guid}", async (Guid id, BasketService basketService) => {
            var basket = await basketService.GetBasket(id);

            return Results.Ok(basket);
        }).WithName("GetBasket");

        group.MapPost("", async (Basket basket, BasketService basketService) => {

            var createdBasket = await basketService.CreateBasket(basket);

            return Results.CreatedAtRoute("GetBasket", new { id = createdBasket.Id }, createdBasket);
        }).WithName("PostBasket");

        group.MapPost("{id:guid}", async (Guid id, BasketService basketService, PublisherService messageService) => {
            var basket = await basketService.GetBasket(id);

            var message = new Message() {
                Name = "Order",
                Basket = basket
            };

            await messageService.SendMessage(message);

            await basketService.DeleteBasket(id);

            return Results.Accepted();
        }).WithName("CheckoutBasket");

        group.MapDelete("{id:guid}", async (Guid id, BasketService basketService) => {

            var basket = await basketService.GetBasket(id);

            if(basket is null) {
                return Results.NotFound();
            };

            await basketService.DeleteBasket(id);

            return Results.NoContent();

        }).WithName("DeleteBasket");


    }
}