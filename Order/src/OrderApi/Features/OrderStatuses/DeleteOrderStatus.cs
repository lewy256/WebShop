using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;

namespace OrderApi.Features.OrderStatus;

public static class DeleteOrderStatus {
    public sealed record Command(int Id) : IRequest<OrderStatusDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, OrderStatusDeleteResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<OrderStatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            int rows = await _context.SpecOrderStatus.Where(p => p.SpecOrderStatusId == request.Id).ExecuteDeleteAsync();

            if(rows == 0) {
                return new NotFoundResponse(request.Id, nameof(Status));
            }

            return new Success();
        }
    }
}

public class DeleteSpecOrderStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/order-statuses/{id}",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteOrderStatus.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeleteOrderStatus)).WithTags(nameof(SpecOrderStatus));
    }
}

[GenerateOneOf]
public partial class OrderStatusDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}