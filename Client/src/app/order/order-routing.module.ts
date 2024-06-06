import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {CreateOrderComponent} from "./order/create-order/create-order.component";
import {CouponComponent} from "./coupon/coupon.component";
import {PaymentMethodComponent} from "./payment-method/payment-method.component";
import {ShipMethodComponent} from "./ship-method/ship-method.component";
import {AddressComponent} from "./address/address.component";

const routes: Routes = [{
    path:"",component:CreateOrderComponent,
    children:[
      { path: "coupon", component: CouponComponent },
      { path: "payment-method", component: PaymentMethodComponent},
      { path: "ship-method", component: ShipMethodComponent },
      { path: "address", component: AddressComponent},
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrderRoutingModule {}
