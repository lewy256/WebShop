import { NgModule } from "@angular/core";

import { RouterModule, Routes } from "@angular/router";
import { ProductComponent } from "./product/product.component";
import { OrderComponent } from "./order/order.component";
import { BasketComponent } from "./basket/basket.component";
import {AccountComponent} from "./account/account.component";


const routes: Routes = [
  { path: "", component: ProductComponent },
  { path: "order", component: OrderComponent },
  { path: "basket", component: BasketComponent },
  { path: "account", component: AccountComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]


})
export class AppRoutingModule {
}
