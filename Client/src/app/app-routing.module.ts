import { NgModule } from "@angular/core";

import { RouterModule, Routes } from "@angular/router";
import { HomeComponent } from "./home/home.component";
import { ProductComponent } from "./product/product.component";
import { OrderComponent } from "./order/order.component";
import { BasketComponent } from "./basket/basket.component";


const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "product", component: ProductComponent },
  { path: "order", component: OrderComponent },
  { path: "basket", component: BasketComponent }
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
