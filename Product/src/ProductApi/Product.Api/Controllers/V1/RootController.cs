using Microsoft.AspNetCore.Mvc;
using ProductApi.Model.LinkModels;

namespace ProductApi.Controllers.V1;

[Route("api")]
[Produces("application/vnd.lewy256.apiroot+json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class RootController : ControllerBase {
    private readonly LinkGenerator _linkGenerator;

    public RootController(LinkGenerator linkGenerator) => _linkGenerator = linkGenerator;

    /// <summary>
    /// Retrieves the root information of the API.
    /// </summary>
    /// <param name="mediaType"></param>
    /// <returns>Returns the list of links.</returns>
    [HttpGet(Name = nameof(GetRoot))]
    [ProducesResponseType(typeof(IEnumerable<Link>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType) {
        if(mediaType.Contains("application/vnd.lewy256.apiroot")) {
            var list = new List<Link> {
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
                }
            };

            return Ok(list);
        }

        return NoContent();
    }
}
