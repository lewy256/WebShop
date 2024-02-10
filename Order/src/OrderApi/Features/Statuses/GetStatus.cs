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

namespace OrderApi.Features.Statuses;

public static class GetStatus {
    public sealed record Query(int Id) : IRequest<StatusGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, StatusGetResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<StatusGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var statusDto = await _context.Status.AsNoTracking().ProjectToType<StatusDto>().SingleOrDefaultAsync(o => o.StatusId == request.Id);

            if(statusDto is null) {
                return new NotFoundResponse(request.Id, nameof(Status));
            }

            return statusDto;
        }
    }
}

public class GetStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/statuses/{id}",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetStatus.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                status => Results.Ok(status),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(GetStatus)).WithTags(nameof(Status));
    }
}

[GenerateOneOf]
public partial class StatusGetResponse : OneOfBase<StatusDto, NotFoundResponse> {
}