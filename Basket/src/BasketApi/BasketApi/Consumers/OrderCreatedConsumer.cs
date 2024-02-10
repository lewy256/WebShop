using BasketApi.Consumers.Messages;
using BasketApi.Services;
using MassTransit;

namespace BasketApi.Consumers;

public class OrderCreatedConsumer(BasketService basketService) : IConsumer<OrderCreated> {
    public async Task Consume(ConsumeContext<OrderCreated> context) {
        OrderCreated message = context.Message;

        await basketService.DeleteBasketAsync(message.BasketId);
    }
}