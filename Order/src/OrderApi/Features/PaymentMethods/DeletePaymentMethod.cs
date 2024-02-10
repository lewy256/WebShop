using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;

namespace OrderApi.Features.PaymentMethods;

public static class DeletePaymentMethod {
    public sealed record Command(int Id) : IRequest<PaymentMethodDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, PaymentMethodDeleteResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<PaymentMethodDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            var rows = await _context.PaymentMethod.Where(p => p.PaymentMethodId == request.Id).ExecuteDeleteAsync();

            if(rows == 0) {
                return new NotFoundResponse(request.Id, nameof(PaymentMethod));
            }

            return new Success();
        }

    }
}

public class DeletePaymentMethodEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/payment-methods/{id}",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeletePaymentMethod.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeletePaymentMethod)).WithTags(nameof(PaymentMethod));
    }
}

[GenerateOneOf]
public partial class PaymentMethodDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}