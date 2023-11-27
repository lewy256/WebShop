using OrderApi.Models;
using OrderApi.Shared;

namespace OrderApi.Services;

public class ProcessService {
    private readonly IConfiguration _configuration;


    public ProcessService(IConfiguration configuration) {
        _configuration = configuration;
    }

    public async Task Process(Message message) {

        using var context = new MessageContext(_configuration.GetConnectionString("TestDatabase"));


        var order = new Order() {
            OrderId = 42332,
            CustomerId = 1,
            OrderDate = DateTime.UtcNow,
            PaymentMethodId = 1,
            AddressId = 1,
            ShipMethodId = 1,
            TotalPrice = message.Basket.TotalPrice,
        };

        await context.Order.AddAsync(order);


        /*      int count = 2313131;

              foreach (var item in message.Basket.Items) {
                  var orderItem = new OrderItem() {
                      OrderItemId = count++,
                      Price = item.Price,
                      Quantity = item.Quantity,
                      OrderId = 42332,
                      ProductId = item.Id
                  };
                  await context.OrderItem.AddAsync(orderItem);
              }*/


        await context.SaveChangesAsync();
    }
}

