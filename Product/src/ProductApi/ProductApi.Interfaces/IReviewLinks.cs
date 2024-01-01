using Microsoft.AspNetCore.Http;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Interfaces;
public interface IReviewLinks {
    ReviewLinkResponse TryGenerateLinks(IEnumerable<ReviewDto> reviewsDto, Guid productId, HttpContext httpContext);
}
