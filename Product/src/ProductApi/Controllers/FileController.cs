using FileApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.Filters;
using ProductApi.Infrastructure.ModelBinders;
using ProductApi.Shared.FilesDtos;

namespace ProductApi.Controllers;

[Route("api/products/{productId}/files")]
[Authorize(Policy = "RequireAdministratorRole")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class FileController : BaseController {
    private readonly IFileService _fileService;

    public FileController(IFileService fileService) {
        _fileService = fileService;
    }

    [HttpPost(Name = nameof(UploadFilesForProduct))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ServiceFilter(typeof(MultipartFormDataAttribute))]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> UploadFilesForProduct(Guid productId) {
        var fileParameters = new FileParameters(HttpContext, Request);

        var results = await _fileService.UploadFilesAsync(productId, fileParameters);

        return results.Match<IActionResult>(
            files => CreatedAtAction(nameof(UploadFilesForProduct), files),
            notfound => Problem(notfound));
    }

    /// <summary>
    /// Deletes specific files for the product.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/products/de985cbb-15cc-4d9a-b82f-09c630131943/files/collection/(06f35f8b-89d1-4abf-82a1-e7ebbecc1c59,941539bb-8e65-47df-9f7a-e90808183e48)
    /// </remarks>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    [HttpDelete("collection/{fileIds}", Name = nameof(DeleteFilesForProduct))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteFilesForProduct(Guid productId,
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> fileIds) {
        var results = await _fileService.DeleteFilesAsync(productId, fileIds);

        return results.Match<IActionResult>(
           _ => NoContent(),
           notFound => Problem(notFound));
    }

}
