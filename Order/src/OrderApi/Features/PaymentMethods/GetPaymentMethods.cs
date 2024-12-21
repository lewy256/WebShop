using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Shared;

namespace OrderApi.Features.PaymentMethods;

public static class GetPaymentMethods {
    public sealed record Query : IRequest<PaymentMethodsGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, PaymentMethodsGetAllResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<PaymentMethodsGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var paymentMethodDtos = await _context.PaymentMethod.AsNoTracking().ProjectToType<PaymentMethodDto>().ToListAsync();

            return new PaymentMethodsGetAllResponse(paymentMethodDtos);
        }
    }
}

public class GetPaymentMethodsEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/payment-methods",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(IEnumerable<PaymentMethodDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (ISender sender) => {
            var query = new GetPaymentMethods.Query();

            var results = await sender.Send(query);

            return Results.Ok(results.PaymentMethods);

        }).WithName(nameof(GetPaymentMethods)).WithTags(nameof(PaymentMethod));
    }
}


public record PaymentMethodsGetAllResponse(IEnumerable<PaymentMethodDto> PaymentMethods);