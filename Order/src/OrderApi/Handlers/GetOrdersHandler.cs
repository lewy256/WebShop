using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using OrderApi.Queries;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Handlers;

public sealed class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>> {
    private readonly OrderContext _orderContext;
    private readonly IMapper _mapper;

    public GetOrdersHandler(OrderContext orderContext, IMapper mapper) {
        _orderContext = orderContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken) {
        var orders = _orderContext.Order.AsNoTracking();


        var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

        return ordersDto;
    }

    /*    public async Task<(IEnumerable<OrderDto> ordersDto, MetaData metaData)> GetOrderAsync(int customerId,
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
        }*/
}
