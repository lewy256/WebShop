using Microsoft.Net.Http.Headers;
using ProductApi.Entities;
using ProductApi.Shared.LinkModels;
using ProductApi.Shared.LinkModels.Categories;

namespace ProductApi.Infrastructure.Utility;

public interface ICategoryLinks {
    CategoryLinkResponse TryGenerateLinks(IEnumerable<Category> categories, HttpContext httpContext);
}

public class CategoryLinks : ICategoryLinks {
    private readonly LinkGenerator _linkGenerator;

    public CategoryLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public CategoryLinkResponse TryGenerateLinks(IEnumerable<Category> categories, HttpContext httpContext) {

        if(ShouldGenerateLinks(httpContext)) {
            return ReturnLinkdedCategories(categories, httpContext);
        }

        return new CategoryLinkResponse() { HasLinks = false, Categories = categories };
    }

    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private CategoryLinkResponse ReturnLinkdedCategories(IEnumerable<Category> categories, HttpContext httpContext) {
        var linkedCategories = new List<LinkedCategories>();

        foreach(var category in categories) {
            var links = CreateLinksForCategory(httpContext, category.Id);

            linkedCategories.Add(new LinkedCategories(category, links));
        }

        var entity = new LinkedCategoryEntity() {
            Values = linkedCategories,
            Links = CreateLinksForCategories(httpContext)
        };

        return new CategoryLinkResponse { HasLinks = true, LinkedEntity = entity };
    }

    private List<Link> CreateLinksForCategory(HttpContext httpContext, Guid categoryId) {
        var links = new List<Link>{
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetCategory", values: new { categoryId}),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteCategory", values: new { categoryId }),
                "delete_product",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateCategory", values: new { categoryId}),
                "update_product",
                "PUT")
            };
        return links;
    }

    private List<Link> CreateLinksForCategories(HttpContext httpContext) {
        var links = new List<Link>{
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetCategories", values: new { }),
                "self",
                "GET")
            };
        return links;
    }

}
