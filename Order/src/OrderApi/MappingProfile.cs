using AutoMapper;
using OrderApi.Models;
using OrderApi.Shared.OrderDtos;

namespace OrderApi;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<Order, OrderDto>();

        CreateMap<OrderDto, Order>();

        CreateMap<CreateOrderDto, Order>();

        CreateMap<UpdateOrderDto, Order>();
    }
}
