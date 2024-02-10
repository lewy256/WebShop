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
using System.Security.Claims;

namespace OrderApi.Features.Orders;

public static class GetOrderForCustomer {
    public sealed record Query(int Id) : IRequest<OrderGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, OrderGetResponse> {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<OrderGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orderSummaryDto = await _context.Order
                .AsNoTracking()
                .Where(x => x.CustomerId.Equals(new Guid(userId)))
                .ProjectToType<OrderSummaryDto>()
                .SingleOrDefaultAsync(o => o.OrderId == request.Id);


            if(orderSummaryDto is null) {
                return new NotFoundResponse(request.Id, nameof(Order));
            }

            var statuses = await _context.SpecOrderStatus
                .AsNoTracking()
                .Where(x => x.OrderId == request.Id)
                .Select(x => new StatusDto() {
                    StatusId = x.StatusId,
                    Description = x.Status.Description
                })
                .ToListAsync();

            orderSummaryDto.Statuses.AddRange(statuses);

            var orderItems = await _context.OrderItem.AsNoTracking().Where(x => x.OrderId == request.Id).ProjectToType<OrderItemDto>().ToListAsync();

            orderSummaryDto.OrderItems.AddRange(orderItems);

            return orderSummaryDto;
        }
    }
}

public class GetOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/orders/{id}",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetOrderForCustomer.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(GetOrderForCustomer)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderGetResponse : OneOfBase<OrderSummaryDto, NotFoundResponse> {
}