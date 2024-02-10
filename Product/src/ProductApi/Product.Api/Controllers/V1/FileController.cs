using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Filters;
using ProductApi.Service.Interfaces;

namespace ProductApi.Controllers.V1;

[Route("api/products/{productId}/files")]
[Authorize(Roles = "Administrator")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class FileController : ControllerBase {
    private readonly IFileService _fileService;

    public FileController(IFileService fileService) {
        _fileService = fileService;
    }

    /// <summary>
    /// Uploads new files.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/products/844288e4-630d-42e7-9667-243bc655569f/files
    ///     body: files to upload
    /// </remarks>
    /// <param name="productId">The ID of the category for product.</param>
    /// <returns>The newly created files' information.</returns>
    /// <response code="201">Returns the newly created files names.</response>
    /// <response code="404">If the product with the given ID is not found.</response>
    /// <response code="415">If the header is incorrect.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpPost(Name = nameof(UploadFiles))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ServiceFilter(typeof(MultipartFormDataAttribute))]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> UploadFiles(Guid productId) {
        var results = await _fileService.CreateImagesAsync(productId, HttpContext.Request.Body, Request.ContentType);

        return results.Match<IActionResult>(
            files => CreatedAtAction(nameof(UploadFiles), files),
            notfound => NotFound(notfound));
    }

    /// <summary>
    /// Deletes a specific file by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE api/products/844288e4-630d-42e7-9667-243bc655569f/files
    /// </remarks>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>No content if successful, otherwise returns an error message.</returns>
    /// <response code="204">If the file is successfully deleted.</response>
    /// <response code="404">If the file or product with the given ID is not found.</response>
    /// <response code="401">If the request lacks valid authentication credentials.</response>
    [HttpDelete("{fileId:guid}", Name = nameof(DeleteFile))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFile(Guid productId, Guid fileId) {
        var results = await _fileService.DeleteImageAsync(productId, fileId);

        return results.Match<IActionResult>(
           _ => NoContent(),
           notFound => NotFound(notFound));
    }
}
