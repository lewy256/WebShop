import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderComponent } from './order/order.component';
import { AddressComponent } from './address/address.component';
import { StatusComponent } from './status/status.component';
import { CouponComponent } from './coupon/coupon.component';
import { ShipMethodComponent } from './ship-method/ship-method.component';
import { PaymentMethodComponent } from './payment-method/payment-method.component';
import { OrderStatusComponent } from './order-status/order-status.component';
import {OrderRoutingModule} from "./order-routing.module";
import {MaterialModule} from "../material/material.module";
import {ReactiveFormsModule} from "@angular/forms";
import {BasketModule} from "../basket/basket.module";



@NgModule({
  declarations: [
    OrderComponent,
    AddressComponent,
    StatusComponent,
    CouponComponent,
    ShipMethodComponent,
    PaymentMethodComponent,
    OrderStatusComponent
  ],
    imports: [
        CommonModule,
        OrderRoutingModule,
        MaterialModule,
        ReactiveFormsModule,
        BasketModule,
    ]
})
export class OrderModule { }
