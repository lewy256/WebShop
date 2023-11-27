export interface Order{
  orderId:number,
  customerId:number,
  orderDate:string,
  specOrderStatusId:number,
  paymentMethodId:number,
  addressId:number,
  shipMethodId:number,
  totalPrice:number
}
