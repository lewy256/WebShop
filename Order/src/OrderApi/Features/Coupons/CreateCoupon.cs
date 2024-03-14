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

namespace OrderApi.Features.Coupons;

public static class CreateCoupon {
    public class Command : IRequest<CouponCreateResponse> {
        public string Code { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Description)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, CouponCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext context, IValidator<Command> validator) {
            _context = context;
            _validator = validator;
        }

        public async ValueTask<CouponCreateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var coupon = request.Adapt<Coupon>();

            await _context.Coupon.AddAsync(coupon);
            await _context.SaveChangesAsync(cancellationToken);

            var couponDto = coupon.Adapt<CouponDto>();

            return couponDto;

        }
    }
}

public class CreateCouponEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/coupons",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] CouponRequest request, ISender sender) => {
            var command = request.Adapt<CreateCoupon.Command>();

            var results = await sender.Send(command);

            return results.Match(
                coupon => Results.Created($"{nameof(Coupon)}/{coupon.CouponId}", coupon),
                validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName(nameof(CreateCoupon)).WithTags(nameof(Coupon));
    }
}

[GenerateOneOf]
public partial class CouponCreateResponse : OneOfBase<CouponDto, ValidationResponse> {
}