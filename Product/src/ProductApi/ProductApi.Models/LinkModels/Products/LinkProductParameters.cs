using Microsoft.AspNetCore.Http;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Model.LinkModels.Products;

public record LinkProductParameters(ProductParameters ProductParameters, HttpContext Context);
