using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ProductApi.Entities;
using ProductApi.Extensions;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Utility;
using ProductApi.Shared;
using ProductApi.Shared.LinkModels.Reviews;
using ProductApi.Shared.Responses;
using ProductApi.Shared.ReviewDtos;
using System.Security.Claims;

namespace ProductApi.Services;

public interface IReviewService {
    Task<ReviewCreateResponse> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto);
    Task<ReviewDeleteResponse> DeleteReviewAsync(Guid productId, Guid reviewId);
    Task<ReviewGetResponse> GetReviewByIdAsync(Guid productId, Guid reviewId);
    Task<ReviewsGetAllResponse> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters);
    Task<ReviewUpdateResponse> UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto);
}

public class ReviewService : IReviewService {
    private readonly ProductContext _productContext;
    private readonly IReviewLinks _reviewLinks;
    private readonly IValidator<CreateReviewDto> _createValidator;
    private readonly IValidator<UpdateReviewDto> _updateValidator;
    private readonly IValidator<ReviewParameters> _parametersValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReviewService(ProductContext productContext, IReviewLinks reviewLinks,
        IValidator<CreateReviewDto> createValidator,
        IValidator<UpdateReviewDto> updateValidator,
        IValidator<ReviewParameters> parametersValidator,
        IHttpContextAccessor httpContextAccessor) {
        _productContext = productContext;
        _reviewLinks = reviewLinks;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _parametersValidator = parametersValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ReviewsGetAllResponse> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.ReviewParameters);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var query = _productContext.Review
        .AsNoTracking()
        .Where(p => p.ProductId.Equals(productId));

        var reviewsDto = await query
            .SortReviews(linkParameters.ReviewParameters.OrderBy)
            .Skip((linkParameters.ReviewParameters.PageNumber - 1) * linkParameters.ReviewParameters.PageSize)
            .Take(linkParameters.ReviewParameters.PageSize)
            .ProjectToType<ReviewDto>()
            .ToListAsync();

        var count = await query.CountAsync();

        var links = _reviewLinks.TryGenerateLinks(reviewsDto, productId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.ReviewParameters.PageNumber,
            PageSize = linkParameters.ReviewParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ReviewGetResponse> GetReviewByIdAsync(Guid productId, Guid reviewId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var reviewDto = await _productContext.Review.AsNoTracking().ProjectToType<ReviewDto>().SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(reviewDto is null) {
            return new NotFoundResponse(reviewId, nameof(Review));
        }

        return reviewDto;
    }

    public async Task<ReviewCreateResponse> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto) {
        var validationResult = await _createValidator.ValidateAsync(reviewDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var review = reviewDto.Adapt<Review>();

        review.Id = Guid.NewGuid();
        review.ProductId = product.Id;
        review.UserName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        review.ReviewDate = DateTime.UtcNow;
        review.Discriminator = nameof(Review);

        await _productContext.AddAsync(review);
        await _productContext.SaveChangesAsync();

        var reviewToReturn = review.Adapt<ReviewDto>();

        return reviewToReturn;
    }

    public async Task<ReviewUpdateResponse> UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto) {
        var validationResult = await _updateValidator.ValidateAsync(reviewDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }


        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var review = await _productContext.Review.SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(review is null) {
            return new NotFoundResponse(reviewId, nameof(Review));
        }

        reviewDto.Adapt(review);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<ReviewDeleteResponse> DeleteReviewAsync(Guid productId, Guid reviewId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var review = await _productContext.Review.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(review is null) {
            return new NotFoundResponse(reviewId, nameof(Review));
        }

        _productContext.Review.Remove(review);

        await _productContext.SaveChangesAsync();

        return new Success();
    }


}

[GenerateOneOf]
public partial class ReviewsGetAllResponse : OneOfBase<(ReviewLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ReviewUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ReviewCreateResponse : OneOfBase<ReviewDto, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ReviewDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ReviewGetResponse : OneOfBase<ReviewDto, NotFoundResponse> {
}
