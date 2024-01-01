using Mediator;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Notifications;

public sealed record OrderCreatedNotification(Guid Id, CreateOrderDto Order) : INotification;
