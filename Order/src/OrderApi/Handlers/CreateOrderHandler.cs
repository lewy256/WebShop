using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Exceptions;
using OrderApi.Models;
using OrderApi.Notifications;

namespace OrderApi.Handlers;

public sealed class CreateOrderHandler : INotificationHandler<OrderCreatedNotification> {
    private readonly OrderContext _orderContext;
    private readonly IMapper _mapper;

    public CreateOrderHandler(OrderContext orderContext, IMapper mapper) {
        _orderContext = orderContext;
        _mapper = mapper;
    }

    public async Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken) {
        var order = await _orderContext.Order.AsNoTracking().SingleOrDefaultAsync(p => p.RowKey.Equals(notification.Id));

        if(order is null) {
            throw new OrderNotFoundException(notification.Id);
        }

        var orderDto = _mapper.Map<Order>(notification.Order);

        await _orderContext.AddAsync(order);
        await _orderContext.SaveChangesAsync();


    }

}
