using Microsoft.AspNetCore.Http;
using ProductApi.Model.Responses;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Interfaces;
public interface IReviewLinks {
    List<LinkReview> TryGenerateLinks(IEnumerable<ReviewDto> reviewsDto, Guid productId, HttpContext httpContext);
}
