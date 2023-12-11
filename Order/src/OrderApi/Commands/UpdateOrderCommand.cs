using MediatR;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Commands;

public sealed record UpdateOrderCommand(Guid Id, UpdateOrderDto Order) : IRequest;
