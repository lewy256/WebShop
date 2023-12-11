using MediatR;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Queries;

public sealed record GetOrderQuery(Guid Id) : IRequest<OrderDto>;
