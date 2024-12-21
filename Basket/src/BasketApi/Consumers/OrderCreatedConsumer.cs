using Contracts.Messages;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;

namespace BasketApi.Consumers;

public class OrderCreatedConsumer(IDistributedCache cache) : IConsumer<OrderCreated> {
    public async Task Consume(ConsumeContext<OrderCreated> context) {
        OrderCreated message = context.Message;

        await cache.RemoveAsync(message.UserId);
    }
}