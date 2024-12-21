using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Entities;
using ProductApi.Services;

namespace ProductApi.Controllers;

[Route("api/wishlists")]
[Authorize(Policy = "RequireCustomerRole")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
public class WishlistController : BaseController {
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService) {
        _wishlistService = wishlistService;
    }

    [HttpGet(Name = nameof(GetWishlist))]
    [ProducesResponseType(typeof(IEnumerable<WishlistItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWishlist() {
        var results = await _wishlistService.GetWishlistAsync();

        return Ok(results);
    }

    [HttpPost("{productId:guid}", Name = nameof(AddProductToWishlist))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddProductToWishlist(Guid productId) {
        var results = await _wishlistService.AddItemToWishlistAsync(productId);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => Problem(notFound));
    }

    [HttpDelete("{itemId:guid}", Name = nameof(DeleteWishlistProduct))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteWishlistProduct(Guid itemId) {
        var results = await _wishlistService.DeleteWishlistItemAsync(itemId);

        return results.Match<IActionResult>(
            _ => NoContent(),
            notFound => Problem(notFound));
    }
}
