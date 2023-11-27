import { Basket } from "../interfaces/Basket";

const mockGetBasket: Basket = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    items: [
      {id:'1',price:5,quantity:4},
      {id:'2',price:5,quantity:4},
      {id:'3',price:5,quantity:4}
    ],
    totalPrice: 32, 
};

const mockPostBasket: Basket = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa7',
    items: [
      {id:'1',price:5,quantity:4},
      {id:'2',price:5,quantity:4},
      {id:'3',price:5,quantity:4}
    ],
    totalPrice: 32, 
};

export { mockGetBasket, mockPostBasket };

