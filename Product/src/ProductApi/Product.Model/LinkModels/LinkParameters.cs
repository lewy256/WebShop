using Microsoft.AspNetCore.Http;
using ProductApi.Shared.Model;

namespace ProductApi.Model.LinkModels;

public record LinkParameters(ProductParameters ProductParameters, HttpContext Context);
