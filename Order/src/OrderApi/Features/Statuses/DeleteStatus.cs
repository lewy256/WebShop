using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;

namespace OrderApi.Features.Statuses;

public static class DeleteStatus {
    public sealed record Command(int Id) : IRequest<StatusDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, StatusDeleteResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            int rows = await _context.Status.Where(p => p.StatusId == request.Id).ExecuteDeleteAsync();

            if(rows == 0) {
                return new NotFoundResponse(request.Id, nameof(Status));
            }

            return new Success();
        }
    }
}

public class DeleteStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/statuses/{id}",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteStatus.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeleteStatus)).WithTags(nameof(Status));
    }
}

[GenerateOneOf]
public partial class StatusDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}