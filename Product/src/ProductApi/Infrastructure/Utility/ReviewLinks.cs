using Microsoft.Net.Http.Headers;
using ProductApi.Shared.LinkModels;
using ProductApi.Shared.LinkModels.Reviews;
using ProductApi.Shared.ReviewDtos;

namespace ProductApi.Infrastructure.Utility;
public interface IReviewLinks {
    ReviewLinkResponse TryGenerateLinks(IEnumerable<ReviewDto> reviewsDto, Guid productId, HttpContext httpContext);
}

public class ReviewLinks : IReviewLinks {
    private readonly LinkGenerator _linkGenerator;

    public ReviewLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public ReviewLinkResponse TryGenerateLinks(IEnumerable<ReviewDto> reviews, Guid productId, HttpContext httpContext) {

        if(ShouldGenerateLinks(httpContext)) {
            return ReturnLinkdedReviews(reviews, productId, httpContext);
        }

        return new ReviewLinkResponse() { HasLinks = false, Reviews = reviews };
    }

    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private ReviewLinkResponse ReturnLinkdedReviews(IEnumerable<ReviewDto> reviews, Guid productId, HttpContext httpContext) {
        var linkedReviews = new List<LinkedReviews>();

        foreach(var review in reviews) {
            var links = CreateLinksForReview(httpContext, productId, review.Id);

            linkedReviews.Add(new LinkedReviews(review, links));
        }

        var entity = new LinkedReviewEntity() {
            Value = linkedReviews,
            Links = CreateLinksForReviews(httpContext)
        };

        return new ReviewLinkResponse { HasLinks = true, LinkedEntity = entity };
    }

    private List<Link> CreateLinksForReview(HttpContext httpContext, Guid productId, Guid reviewId) {
        var links = new List<Link>{
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetReviewForProduct", values: new { productId,reviewId}),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteReviewForProduct", values: new { productId,reviewId }),
                "delete_review",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateReviewForProduct", values: new { productId,reviewId}),
                "update_review",
                "PUT")
            };
        return links;
    }

    private List<Link> CreateLinksForReviews(HttpContext httpContext) {
        var links = new List<Link>{
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetReviewsForProduct", values: new {}),
                "self",
                "GET")
            };
        return links;
    }
}
