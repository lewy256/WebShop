using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Shared.OrderDtos;
using System.Security.Claims;
using System.Text.Json;

namespace OrderApi.Features.Orders;

public static class GetOrders {
    public record Query(OrderParameters OrderParameters) : IRequest<OrdersGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, OrdersGetAllResponse> {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<OrdersGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int pageNumber = request.OrderParameters.PageNumber is null ? 1 : (int)request.OrderParameters.PageNumber;
            int pageSize = request.OrderParameters.PageSize is null ? 50 : (int)request.OrderParameters.PageSize;

            var query = _context.Order
                .AsNoTracking()
                .Where(x => x.CustomerId.Equals(userId))
                .ProjectToType<OrderDto>();

            var pagedOrders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            var count = await query.CountAsync();

            return new OrdersGetAllResponse(Orders: pagedOrders, MetaData: new MetaData() {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = count
            });
        }
    }
}

public class GetOrdersEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/orders",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (ISender sender, HttpContext context, [AsParameters] OrderParameters orderParameters) => {
            var query = new GetOrders.Query(orderParameters);

            var results = await sender.Send(query);

            context.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(results.MetaData));

            return Results.Ok(results.Orders);

        }).WithName(nameof(GetOrders)).WithTags(nameof(Order));
    }
}

public record OrdersGetAllResponse(IEnumerable<OrderDto> Orders, MetaData MetaData);