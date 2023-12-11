using MediatR;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Queries;

public sealed record GetOrdersQuery() : IRequest<IEnumerable<OrderDto>>;
