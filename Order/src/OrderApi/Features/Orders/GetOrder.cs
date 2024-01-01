using Carter;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared.OrderDtos;
using static OrderApi.Features.Orders.GetOrder;

namespace OrderApi.Features.Orders;

public class GetOrder {
    public sealed record GetOrderQuery(Guid Id) : IRequest<OrderGetResponse>;
    public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderGetResponse> {
        private readonly OrderContext _context;

        public GetOrderHandler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<OrderGetResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken) {
            var order = await _context.Order.AsNoTracking().SingleOrDefaultAsync(o => o.RowKey.Equals(request.Id));

            if(order is null) {
                return new NotFoundResponse(request.Id, nameof(order));
            }

            var orderDto = order.Adapt<OrderDto>();

            return orderDto;
        }
    }
}

public class GetOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/orders{id}", async (Guid id, ISender sender) => {
            var query = new GetOrderQuery(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.NotFound(notfound));

        }).WithName("GetOrderById");
    }
}

[GenerateOneOf]
public partial class OrderGetResponse : OneOfBase<OrderDto, NotFoundResponse> {
}