import { BasketItem } from "./BasketItem";

export interface Basket{
    id:string;
    items:BasketItem[];
    totalPrice:number;
  }
