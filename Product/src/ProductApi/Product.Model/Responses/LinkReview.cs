using ProductApi.Model.LinkModels;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Model.Responses;
public class LinkReview {
    public ReviewDto Review { get; set; }
    public List<Link> Links { get; set; }
}
