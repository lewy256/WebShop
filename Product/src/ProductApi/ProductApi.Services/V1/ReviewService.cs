using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Service.Extensions;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.Responses;
using ProductApi.Shared.Model.ReviewDtos;
using System.Security.Claims;

namespace ProductApi.Service.V1;

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

    public async Task<ReviewtGetAllResponse> GetReviewsAsync(Guid productId, ReviewParameters reviewParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(reviewParameters);

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
            .SortReviews(reviewParameters.OrderBy)
            .Skip((reviewParameters.PageNumber - 1) * reviewParameters.PageSize)
            .Take(reviewParameters.PageSize)
            .ProjectToType<ReviewDto>()
            .ToListAsync();

        var count = await query.CountAsync();

        return (reviews: reviewsDto, metaData: new MetaData() {
            CurrentPage = reviewParameters.PageNumber,
            PageSize = reviewParameters.PageSize,
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