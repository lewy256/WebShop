using Microsoft.AspNetCore.Mvc;
using ProductApi.Model.LinkModels;

namespace ProductApi.Controllers;

[Route("api")]
[ApiController]
public class RootController : ControllerBase {
    private readonly LinkGenerator _linkGenerator;

    public RootController(LinkGenerator linkGenerator) => _linkGenerator = linkGenerator;

    [HttpGet(Name = "GetRoot")]
    public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType) {
        if(mediaType.Contains("application/vnd.lewy256.apiroot")) {
            var list = new List<Link>
                {
                    new Link
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
                        Rel = "self",
                        Method = "GET"
                    },
                    new Link
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, "GetCategories", new {}),
                        Rel = "categories",
                        Method = "GET"
                    },
                    new Link
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, "CreateCategory", new {}),
                        Rel = "create_category",
                        Method = "POST"
                    }
                };

            return Ok(list);
        }

        return NoContent();
    }
}
