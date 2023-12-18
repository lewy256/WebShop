using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Interfaces;

public interface IReviewService {
    // Task<(LinkResponse linkResponse, MetaData metaData)> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters);
    Task<ReviewDto> GetReviewByIdAsync(Guid productId, Guid reviewId);
    Task<ReviewDto> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto);
    Task UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto);
    Task DeleteReviewAsync(Guid productId, Guid reviewId);
}
