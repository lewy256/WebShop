using Mapster;
using Microsoft.EntityFrameworkCore;
using OrderApi.Exceptions;
using OrderApi.Intefaces;
using OrderApi.Models;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Services;

public class OrderService : IOrderService {
    private readonly OrderContext _orderContext;


    public OrderService(OrderContext orderContext) {
        _orderContext = orderContext;
    }

    public async Task<(IEnumerable<OrderDto> ordersDto, MetaData metaData)> GetOrderAsync(int customerId,
        OrderParameters orderParameters) {


        await CheckIfCustomerExists(customerId);

        var orders = await _orderContext.Order
            .Where(p => p.CustomerId.Equals(customerId))
            .Skip((orderParameters.PageNumber - 1) * orderParameters.PageSize)
            .Take(orderParameters.PageSize)
            .ToListAsync();

        var ordersDto = orders.Adapt<IEnumerable<OrderDto>>();

        var count = await _orderContext.Order
            .Where(p => p.CustomerId.Equals(customerId))
            .CountAsync();


        return (ordersDto: ordersDto, metaData: new MetaData() {
            CurrentPage = orderParameters.PageNumber,
            PageSize = orderParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<Order> GetOrderByIdAsync(int orderId) {
        var order = await GetOrderAndCheckIfItExists(orderId, false);

        return order;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto) {
        var order = orderDto.Adapt<Order>();

        var lastId = _orderContext.Order.MaxBy(x => x.OrderId).OrderId;

        order.OrderId = ++lastId;

        await _orderContext.AddAsync(order);
        await _orderContext.SaveChangesAsync();

        var orderToReturn = order.Adapt<OrderDto>();

        return orderToReturn;
    }

    public async Task UpdateOrderAsync(int orderId, UpdateOrderDto orderDto) {
        var order = await GetOrderAndCheckIfItExists(orderId, true);

        orderDto.Adapt(order);

        await _orderContext.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(int orderId) {
        var order = await GetOrderAndCheckIfItExists(orderId, false);

        _orderContext.Order.Remove(order);

        await _orderContext.SaveChangesAsync();
    }


    public async Task<(UpdateOrderDto orderDto, Order order)> PatchOrderAsync(int orderId) {
        var order = await GetOrderAndCheckIfItExists(orderId, true);


        var orderDto = order.Adapt<UpdateOrderDto>();

        return (ordertDto: orderDto, order: order);
    }


    public async Task SaveChangesForPatchAsync(UpdateOrderDto orderDto, Order order) {
        orderDto.Adapt(order);


        await _orderContext.SaveChangesAsync();
    }

    private async Task<Order> GetOrderAndCheckIfItExists(int orderId, bool trackChanges) {
        var order = trackChanges
            ? await _orderContext.Order
                .SingleOrDefaultAsync(p => p.OrderId.Equals(orderId))
            : await _orderContext.Order
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.OrderId.Equals(orderId));

        if(order is null) throw new OrderNotFoundException(orderId);

        return order;
    }

    private async Task CheckIfCustomerExists(int customerId) {
        var category = await _orderContext.Customer.SingleOrDefaultAsync(c => c.CustomerId == customerId);

        if(category is null) throw new CustomerNotFoundException(customerId);
    }
}