using Contracts.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure;

namespace ProductApi.Consumers;

public class OrderDeletedConsumer(ProductContext productContext) : IConsumer<OrderDeleted> {
    public async Task Consume(ConsumeContext<OrderDeleted> context) {
        OrderDeleted message = context.Message;

        var products = await productContext.Product
            .Where(p => message.Products.Keys.Contains(p.Id))
            .ToListAsync();

        foreach(var product in products) {
            product.Stock += message.Products[product.Id];
        }

        productContext.UpdateRange(products);

        await productContext.SaveChangesAsync();
    }
}