using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Commands;
using OrderApi.Exceptions;
using OrderApi.Models;

namespace OrderApi.Handlers;

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
