namespace OrderApi.Features.Orders;

/*public static class UpdateOrder {
    public sealed record UpdateOrderCommand(Guid Id, UpdateOrderDto Order) : IRequest<UpdateOrderDto>;

    public sealed class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand> {
        private readonly OrderContext _orderContext;
        private readonly IMapper _mapper;

        public UpdateOrderHandler(OrderContext orderContext, IMapper mapper) {
            _orderContext = orderContext;
            _mapper = mapper;
        }

        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken) {
            var order = await _orderContext.Order.SingleOrDefaultAsync(p => p.RowKey.Equals(request.Id));

            if(order is null) {
                throw new OrderNotFoundException(request.Id);
            }

            _mapper.Map(request.Order, order);

            await _orderContext.SaveChangesAsync();
        }

    }

}
[HttpPut("{id:Guid}")]
public async Task<IActionResult> PutOrder(Guid id, [FromBody] UpdateOrderDto order) {

    if(order is null) {
        return BadRequest("UpdateOrderDto object is null");
    }

    await _sender.Send(new UpdateOrderCommand(id, order));

    return NoContent();
}
*/