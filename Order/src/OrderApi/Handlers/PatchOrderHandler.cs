namespace OrderApi.Handlers;

public class PatchOrderHandler {
    /*    public async Task<(UpdateOrderDto orderDto, Order order)> PatchOrderAsync(int orderId) {
            var order = await GetOrderAndCheckIfItExists(orderId, true);


            var orderDto = order.Adapt<UpdateOrderDto>();

            return (ordertDto: orderDto, order: order);
        }

        public async Task SaveChangesForPatchAsync(UpdateOrderDto orderDto, Order order) {
            orderDto.Adapt(order);


            await _orderContext.SaveChangesAsync();
        }*/
}
