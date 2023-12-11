using MediatR;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Commands;

public sealed record CreateOrderCommand(CreateOrderDto Order) : IRequest<OrderDto>;
