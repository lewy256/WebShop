import { NgModule } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import { CreateOrderComponent } from './order/create-order/create-order.component';
import { AddressComponent } from './address/address.component';
import { CouponComponent } from './coupon/coupon.component';
import { ShipMethodComponent } from './ship-method/ship-method.component';
import { PaymentMethodComponent } from './payment-method/payment-method.component';
import {OrderRoutingModule} from "./order-routing.module";
import {MaterialModule} from "../material/material.module";
import {ReactiveFormsModule} from "@angular/forms";
import {BasketModule} from "../basket/basket.module";
import { GetOrdersComponent } from './order/get-orders/get-orders.component';



@NgModule({
  declarations: [
    CreateOrderComponent,
    AddressComponent,
    CouponComponent,
    ShipMethodComponent,
    PaymentMethodComponent,
    GetOrdersComponent
  ],
    imports: [
        CommonModule,
        OrderRoutingModule,
        MaterialModule,
        ReactiveFormsModule,
        BasketModule,
        NgOptimizedImage,
    ]
})
export class OrderModule { }
