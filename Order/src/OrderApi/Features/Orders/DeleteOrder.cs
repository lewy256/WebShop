using Carter;
using MassTransit;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Consumers.Messages;
using OrderApi.Models;
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
            var order = await _context.Order.AsNoTracking().SingleOrDefaultAsync(p => p.OrderId == request.Id);

            if(order is null) {
                return new NotFoundResponse(request.Id, nameof(Order));
            }

            var productIds = await _context.OrderItem.Where(x => x.OrderId == order.OrderId).Select(x => x.ProductId).ToListAsync();

            await _publishEndpoint.Publish(new OrderDeleted {
                ProductIds = productIds
            }, cancellationToken);

            return new Success();
        }
    }
}

public class DeleteOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/orders/{id}",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteOrder.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeleteOrder)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}