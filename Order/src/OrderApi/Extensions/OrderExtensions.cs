using OrderApi.Shared;

namespace OrderApi.Extensions;

public static class OrderExtensions {
    public static decimal CalculateTotalPrice(this List<OrderItemDto> items, decimal discount) {
        decimal totalPrice = items.Sum(item => item.Quantity * item.UnitPrice) - discount;

        return totalPrice;
    }
}
