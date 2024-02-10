using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderApi.Consumers.Messages;
using OrderApi.Models;
using Serilog;

namespace OrderApi.Consumers;

public class BasketCreatedConsumer(OrderContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<BasketCreated> {
    public async Task Consume(ConsumeContext<BasketCreated> context) {
        BasketCreated message = context.Message;

        var order = await dbContext.Order
            .AsNoTracking()
            .Where(c => c.CustomerId.Equals(message.Basket.UserId))
            .OrderByDescending(x => x.OrderDate)
            .FirstOrDefaultAsync();

        if(order is null) {
            Log.Error($"The order with user id: {message.Basket.UserId} doesn't exist in the database.");
        }

        var checkItems = await dbContext.OrderItem
            .AsNoTracking()
            .AnyAsync(c => c.OrderId.Equals(order.OrderId));

        if(checkItems) {
            Log.Error($"The order with id: {order.OrderId} has items.");
        }

        List<OrderItem> orderItems = new List<OrderItem>();

        foreach(var item in message.Basket.Items) {
            orderItems.Add(new OrderItem {
                Price = item.Price,
                Quantity = item.Quantity,
                OrderId = order.OrderId,
                ProductId = item.ProductId
            });
        }

        var orderStatus = new SpecOrderStatus() {
            OrderId = order.OrderId,
            StatusId = 1,
            StatusDate = DateTime.UtcNow
        };

        await dbContext.SpecOrderStatus.AddAsync(orderStatus);

        await dbContext.OrderItem.AddRangeAsync(orderItems);

        await dbContext.SaveChangesAsync();

        await publishEndpoint.Publish(new OrderCreated() {
            BasketId = message.Basket.Id,
            Orders = orderItems
            .Select(x => new OrderPayload() {
                ProductId = x.ProductId,
                Quantity = x.Quantity
            })
            .ToList()
        });
    }
}