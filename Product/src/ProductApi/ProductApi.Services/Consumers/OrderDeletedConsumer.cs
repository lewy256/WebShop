using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductApi.Model;
using ProductApi.Service.Consumers.Messages;

namespace ProductApi.Service.Consumers;

public class OrderDeletedConsumer(ProductContext productContext) : IConsumer<OrderDeleted> {
    public async Task Consume(ConsumeContext<OrderDeleted> context) {
        OrderDeleted message = context.Message;

        var products = await productContext.Product
            .Where(p => message.Orders.Select(o => o.ProductId)
            .Contains(p.Id))
            .ToArrayAsync();

        for(int i = 0; i < products.Length; i++) {
            products[i].Stock += message.Orders[i].Quantity;
        }

        productContext.UpdateRange(products);

        await productContext.SaveChangesAsync();
    }
}