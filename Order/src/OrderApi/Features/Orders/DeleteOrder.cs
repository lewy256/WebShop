using Carter;
using Contracts.Messages;
using MassTransit;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Responses;

namespace OrderApi.Features.Orders;

public static class DeleteOrder {
    public sealed record Command(int Id) : IRequest<OrderDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, OrderDeleteResponse> {
        private readonly OrderContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(OrderContext context, IPublishEndpoint publishEndpoint) {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async ValueTask<OrderDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            var order = await _context.Order.SingleOrDefaultAsync(p => p.OrderId.Equals(request.Id));

            if(order is null) {
                return new NotFoundResponse(request.Id.ToString(), nameof(Order));
            }

            _context.Order.Remove(order);

            await _context.SaveChangesAsync(cancellationToken);

            var messagePayload = await _context.OrderItem
                 .Where(x => x.OrderId == order.OrderId)
                 .ToDictionaryAsync(p => p.ProductId, o => o.Quantity);

            await _publishEndpoint.Publish(new OrderDeleted {
                Products = messagePayload
            }, cancellationToken);

            return new Success();
        }
    }
}

public class DeleteOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/orders/{id}",
        [Authorize(Policy = "RequireAdministratorRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteOrder.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.Problem(notfound));

        }).WithName(nameof(DeleteOrder)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}