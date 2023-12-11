using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Exceptions;
using OrderApi.Models;
using OrderApi.Queries;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Handlers;

public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto> {
    private readonly OrderContext _orderContext;
    private readonly IMapper _mapper;

    public GetOrderHandler(OrderContext orderContext, IMapper mapper) {
        _orderContext = orderContext;
        _mapper = mapper;
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
