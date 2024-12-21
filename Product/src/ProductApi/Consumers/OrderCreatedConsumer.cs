using Contracts.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure;

namespace ProductApi.Consumers;

public class OrderCreatedConsumer(ProductContext productContext) : IConsumer<OrderCreated> {
    public async Task Consume(ConsumeContext<OrderCreated> context) {
        OrderCreated message = context.Message;

        var products = await productContext.Product
            .Where(p => message.Products.Keys.Contains(p.Id))
            .ToListAsync();

        foreach(var product in products) {
            product.Stock -= message.Products[product.Id];
        }

        productContext.UpdateRange(products);

        await productContext.SaveChangesAsync();
    }
}