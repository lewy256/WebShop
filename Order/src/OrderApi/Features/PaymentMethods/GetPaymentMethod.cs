using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;

namespace OrderApi.Features.PaymentMethods;

public static class GetPaymentMethod {
    public sealed record Query(int Id) : IRequest<PaymentMethodGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, PaymentMethodGetResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<PaymentMethodGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var paymentMethodDto = await _context.PaymentMethod.AsNoTracking().ProjectToType<PaymentMethodDto>().SingleOrDefaultAsync(o => o.PaymentMethodId == request.Id);

            if(paymentMethodDto is null) {
                return new NotFoundResponse(request.Id, nameof(PaymentMethod));
            }

            return paymentMethodDto;
        }
    }
}

public class GetStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/payment-methods/{id}",
                     [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetPaymentMethod.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(GetPaymentMethod)).WithTags(nameof(PaymentMethod));
    }
}

[GenerateOneOf]
public partial class PaymentMethodGetResponse : OneOfBase<PaymentMethodDto, NotFoundResponse> {
}