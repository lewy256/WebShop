using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ProductApi.Entities;
using ProductApi.Infrastructure;
using ProductApi.Shared.Responses;
using System.Security.Claims;

namespace ProductApi.Services;

public interface IWishlistService {
    Task<OneOf<Success, NotFoundResponse>> AddItemToWishlistAsync(Guid productId);
    Task<OneOf<Success, NotFoundResponse>> DeleteWishlistItemAsync(Guid itemId);
    Task<IEnumerable<WishlistItem>> GetWishlistAsync();
}

public class WishlistService : IWishlistService {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ProductContext _productContext;

    public WishlistService(IHttpContextAccessor httpContextAccessor, ProductContext productContext) {
        _httpContextAccessor = httpContextAccessor;
        _productContext = productContext;
    }

    public async Task<IEnumerable<WishlistItem>> GetWishlistAsync() {
        var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var items = await _productContext.WishlistItem.AsNoTracking().Where(c => c.UserId.Equals(userId)).ToListAsync();

        return items;
    }

    public async Task<OneOf<Success, NotFoundResponse>> AddItemToWishlistAsync(Guid productId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        int count = await _productContext.WishlistItem.CountAsync(w => w.Details.ProductId.Equals(productId));

        if(count == 0) {
            var wishlistItem = new WishlistItem() {
                Id = Guid.NewGuid(),
                UserId = userId,
                Details = new ProductDetails() {
                    ProductId = productId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Stock = product.Stock,
                    Brand = product.Brand,
                    Image = product.Files.Select(f => f.URI).FirstOrDefault()
                }
            };

            await _productContext.AddAsync(wishlistItem);

            await _productContext.SaveChangesAsync();
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFoundResponse>> DeleteWishlistItemAsync(Guid itemId) {
        var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var wishlistItem = await _productContext.WishlistItem.AsNoTracking().SingleOrDefaultAsync(w => w.UserId.Equals(userId) && w.Id.Equals(itemId));

        if(wishlistItem is null) {
            return new NotFoundResponse(itemId, nameof(WishlistItem));
        }

        _productContext.WishlistItem.Remove(wishlistItem);

        await _productContext.SaveChangesAsync();

        return new Success();
    }
}
