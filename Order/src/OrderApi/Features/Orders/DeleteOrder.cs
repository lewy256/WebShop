namespace OrderApi.Features.Orders;

/*public static class DeleteOrder {
    public record DeleteOrderCommand(Guid Id) : IRequest;
    public sealed class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand> {
        private readonly OrderContext _orderContext;

        public DeleteOrderHandler(OrderContext orderContext) {
            _orderContext = orderContext;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken) {
            var order = await _orderContext.Order.AsNoTracking().SingleOrDefaultAsync(p => p.RowKey.Equals(request.Id));

            if(order is null) {
                throw new OrderNotFoundException(request.Id);
            }

            _orderContext.Order.Remove(order);

            await _orderContext.SaveChangesAsync();

        }

    }
}
[HttpDelete("{id:Guid}")]
public async Task<IActionResult> DeleteOrder(Guid id) {
    await _sender.Send(new DeleteOrderCommand(id));

    return NoContent();
}*/