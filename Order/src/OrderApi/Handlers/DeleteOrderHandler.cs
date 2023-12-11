using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Commands;
using OrderApi.Exceptions;
using OrderApi.Models;

namespace OrderApi.Handlers;

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
