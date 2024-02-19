import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {ProductComponent} from "./product/product.component";
import {ReviewComponent} from "./review/review.component";
import {PriceHistoryComponent} from "./price-history/price-history.component";
import {CategoryComponent} from "./category/category.component";

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
    path:"",component:ProductComponent,
    children:[
      { path: "review", component: ReviewComponent },
      { path: "price-history", component: PriceHistoryComponent},
      { path: "category", component: CategoryComponent }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductRoutingModule {}
