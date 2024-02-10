﻿namespace OrderApi.Shared;

public class BasketItem {
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}