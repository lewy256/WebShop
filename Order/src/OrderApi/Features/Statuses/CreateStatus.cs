using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;

namespace OrderApi.Features.Statuses;

public static class CreateStatus {
    public class Command : IRequest<StatusCreateResponse> {
        public string Description { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Description)
              .NotEmpty()
              .MaximumLength(10);
        }
    }

    internal sealed class Handler : IRequestHandler<Command, StatusCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext context, IValidator<Command> validator) {
            _context = context;
            _validator = validator;
        }

        public async ValueTask<StatusCreateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {
                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var status = request.Adapt<Status>();

            await _context.Status.AddAsync(status);
            await _context.SaveChangesAsync(cancellationToken);

            var statusDto = status.Adapt<StatusDto>();

            return statusDto;
        }
    }
}

public class CreateStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/statuses",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] StatusRequest request, ISender sender) => {
            var command = request.Adapt<CreateStatus.Command>();

            var results = await sender.Send(command);

            return results.Match(
                status => Results.Created($"{nameof(Status)}/{status.StatusId}", status),
                validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName(nameof(CreateStatus)).WithTags(nameof(Status));
    }
}

[GenerateOneOf]
public partial class StatusCreateResponse : OneOfBase<StatusDto, ValidationResponse> {
}