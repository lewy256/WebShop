using OrderApi.Models;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Intefaces;

public interface IOrderService
{
    Task<(IEnumerable<OrderDto> ordersDto, MetaData metaData)> GetOrderAsync(int customerId, OrderParameters orderParameters);
    Task<Order> GetOrderByIdAsync(int orderId);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto);
    Task UpdateOrderAsync(int orderId, UpdateOrderDto orderDto);
    Task DeleteOrderAsync(int orderId);
    Task<(UpdateOrderDto orderDto, Order order)> PatchOrderAsync(int orderId);
    Task SaveChangesForPatchAsync(UpdateOrderDto orderDto, Order order);
}