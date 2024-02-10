using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using OrderApi.Shared;

namespace OrderApi.Features.Statuses;

public static class GetStatuses {
    public sealed record Query : IRequest<StatusGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, StatusGetAllResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<StatusGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var statusDtos = await _context.Status.AsNoTracking().ProjectToType<StatusDto>().ToListAsync();

            return new StatusGetAllResponse(statusDtos);
        }
    }
}

public class GetStatusesEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/statuses",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(IEnumerable<StatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (ISender sender) => {
            var query = new GetStatuses.Query();

            var results = await sender.Send(query);

            return Results.Ok(results.Statuses);

        }).WithName(nameof(GetStatuses)).WithTags(nameof(Status));
    }
}

public record StatusGetAllResponse(IEnumerable<StatusDto> Statuses);
