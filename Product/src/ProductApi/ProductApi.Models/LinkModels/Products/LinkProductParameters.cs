using Microsoft.AspNetCore.Http;
using ProductApi.Shared.Model.ProductDtos.V2;

namespace ProductApi.Model.LinkModels.Products;

public record LinkProductParameters(ProductParameters ProductParameters, HttpContext Context);
