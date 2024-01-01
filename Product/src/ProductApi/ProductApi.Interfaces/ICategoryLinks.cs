using Microsoft.AspNetCore.Http;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Categories;

namespace ProductApi.Interfaces;
public interface ICategoryLinks {
    CategoryLinkResponse TryGenerateLinks(IEnumerable<Category> categories, HttpContext httpContext);
}
