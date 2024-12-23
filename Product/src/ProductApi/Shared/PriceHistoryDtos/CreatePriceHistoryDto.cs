﻿namespace ProductApi.Shared.PriceHistoryDtos;

public record CreatePriceHistoryDto {
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal PriceValue { get; init; }
}