import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {OrderComponent} from "./order/order.component";
import {CouponComponent} from "./coupon/coupon.component";
import {PaymentMethodComponent} from "./payment-method/payment-method.component";
import {OrderStatusComponent} from "./order-status/order-status.component";
import {ShipMethodComponent} from "./ship-method/ship-method.component";
import {StatusComponent} from "./status/status.component";
import {AddressComponent} from "./address/address.component";

const routes: Routes = [{
    path:"",component:OrderComponent,
    children:[
      { path: "coupon", component: CouponComponent },
      { path: "payment-method", component: PaymentMethodComponent},
      { path: "order-status", component: OrderStatusComponent },
      { path: "ship-method", component: ShipMethodComponent },
      { path: "status", component: StatusComponent},
      { path: "address", component: AddressComponent},
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrderRoutingModule {}
