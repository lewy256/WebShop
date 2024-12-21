using Mapster;
using OrderApi.Entities;
using OrderApi.Shared;
using OrderApi.Shared.AddressDtos;

namespace OrderApi.Infrastructure;

public static class MappingConfig {
    public static void ConfigureMapster() {
        TypeAdapterConfig.GlobalSettings
            .NewConfig<Address, AddressDto>()
            .Map(dest => dest.Id, src => src.AddressId);
        TypeAdapterConfig.GlobalSettings
           .NewConfig<ShipMethod, ShipMethodDto>()
           .Map(dest => dest.Id, src => src.ShipMethodId);
        TypeAdapterConfig.GlobalSettings
            .NewConfig<PaymentMethod, PaymentMethodDto>()
            .Map(dest => dest.Id, src => src.PaymentMethodId);
    }
}
