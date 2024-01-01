namespace OrderApi.Features.Orders;

/*public class GetOrders {
    public sealed record GetOrdersQuery() : IRequest<IEnumerable<OrderDto>>;
    public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto> {
        private readonly OrderContext _orderContext;

        public GetOrderHandler(OrderContext orderContext) {
            _orderContext = orderContext;
        }

        public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken) {
            var order = _orderContext.Order.AsNoTracking().SingleOrDefaultAsync(p => p.RowKey.Equals(request.Id));

            if(order is null) {
                throw new OrderNotFoundException(request.Id);
            }

            var orderDto = _mapper.Map<OrderDto>(order);

            return orderDto;
        }
    }
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetOrders(int id, [FromQuery] OrderParameters orderParameters) {
        var pagedResult = await _sender.Send(new GetOrdersQuery());


        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult);
    }*/