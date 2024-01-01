namespace OrderApi.Features.Orders;

public class PatchOrder {
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

}
/*[HttpPatch("{id:Guid}")]
public async Task<IActionResult> PatchOrder(Guid id, [FromBody] JsonPatchDocument<UpdateOrderDto> patchDoc) {
    if(patchDoc is null) return BadRequest("patchDoc object sent from client is null.");

    var result = await _sender.Send.PatchOrderAsync(orderId);

    patchDoc.ApplyTo(result.orderDto);

    await await _sender.Send.SaveChangesForPatchAsync(result.orderDto, result.order);

    return NoContent();
}*/