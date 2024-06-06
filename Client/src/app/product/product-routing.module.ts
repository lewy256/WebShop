import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {GetProductsComponent} from "./product/get-products/get-products.component";
import {GetPricesHistoryComponent} from "./price-history/get-prices-history/get-prices-history.component";
import {GetCategoriesComponent} from "./category/get-categories/get-categories.component";
import {CreateProductComponent} from "./product/create-product/create-product.component";
import {CreateReviewComponent} from "./review/create-review/create-review.component";
import {GetReviewsComponent} from "./review/get-reviews/get-reviews.component";
import {DeleteReviewComponent} from "./review/delete-review/delete-review.component";
import {UpdateReviewComponent} from "./review/update-review/update-review.component";

const routes: Routes = [
  { path:"",component:GetProductsComponent},
  { path: "review", component: GetReviewsComponent },
  { path: "price-history", component: GetPricesHistoryComponent},
  { path: "category", component: GetCategoriesComponent },
  { path: "create-product", component: CreateProductComponent },
  { path: "create-review", component: CreateReviewComponent },
  { path: "delete-review", component: DeleteReviewComponent },
  { path: "update-review", component: UpdateReviewComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})

export class ProductRoutingModule {}
