using MediatR;

namespace OrderApi.Commands;

public record DeleteOrderCommand(Guid Id) : IRequest;
