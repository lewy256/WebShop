using Mapster;
using Microsoft.EntityFrameworkCore;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service;

public class ReviewService : IReviewService {
    private readonly ProductContext _productContext;

    public ReviewService(ProductContext productContext) {
        _productContext = productContext;
    }


    public async Task<ReviewDto> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto) {
        throw new NotImplementedException();
    }

    public async Task DeleteReviewAsync(Guid productId, Guid reviewId) {
        throw new NotImplementedException();
    }

    public async Task<ReviewDto> GetReviewByIdAsync(Guid productId, Guid reviewId) {
        var product = await _productContext.Product.SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            //return new ProductNotFoundException(productId);
        }

        var review = await _productContext.Review.SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(review is null) {
            //return new ReviewNotFoundException(reviewId);
        }

        var reviewDto = review.Adapt<ReviewDto>();
        return reviewDto;
    }

    /*    public async Task<(LinkResponse linkResponse, MetaData metaData)> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters) {
            var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(productId));
            if(product is null) {
                //return new NotFoundResponse(productId, nameof(product));
            }
            var reviews = await _productContext.Review
                .Where(p => p.ProductId.Equals(productId))
                .Skip((linkParameters.ReviewParameters.PageNumber - 1) * linkParameters.ReviewParameters.PageSize)
                .Take(linkParameters.ReviewParameters.PageSize)
                .ToListAsync();

            var reviewsDto = reviews.Adapt<IEnumerable<ReviewDto>>();
            var count = await _productContext.Review.Where(p => p.ProductId.Equals(productId)).CountAsync();
            var links = _reviewLinks.TryGenerateLinks(reviewsDto, productId, linkParameters.Context);

            return (linkResponse: links, metaData: new MetaData() {
                CurrentPage = linkParameters.ReviewParameters.PageNumber,
                PageSize = linkParameters.ReviewParameters.PageSize,
                TotalCount = count
            });
        }*/

    public async Task UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto) {
        throw new NotImplementedException();
    }

}