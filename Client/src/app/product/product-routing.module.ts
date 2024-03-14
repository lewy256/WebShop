import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {GetProductsComponent} from "./product/get-products/get-products.component";
import {GetReviewComponent} from "./review/get-review/get-review.component";
import {GetPricesHistoryComponent} from "./price-history/get-prices-history/get-prices-history.component";
import {GetCategoriesComponent} from "./category/get-categories/get-categories.component";
import {CreateProductComponent} from "./product/create-product/create-product.component";
import {LoginComponent} from "../account/login/login.component";

// const routes: Routes = [
//   {
//     path: '',
//     component: DashboardComponent,
//     children: [
//       { path: 'update-user', component: UpdateUserComponent },
//       { path: 'add-product', component: AddProductComponent },
//       { path: '', redirectTo: '/admin/update-user', pathMatch: 'full' },
//     ],
//   },
//   { path: '**', component: NotFoundComponent },
// ];
const routes: Routes = [
  {
    path:"",component:GetProductsComponent,
    children:[
      { path: "review", component: GetReviewComponent },
      { path: "price-history", component: GetPricesHistoryComponent},
      { path: "category", component: GetCategoriesComponent }
    ],
  },
  { path: "create-product", component: CreateProductComponent }

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductRoutingModule {}
