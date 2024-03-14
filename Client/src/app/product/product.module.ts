import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ProductRoutingModule} from "./product-routing.module";
import {MaterialModule} from "../material/material.module";
import { GetProductComponent } from './product/get-product/get-product.component';
import { GetProductsComponent } from './product/get-products/get-products.component';
import {MatGridListModule} from "@angular/material/grid-list";
import { GetReviewComponent } from './review/get-review/get-review.component';
import { GetReviewsComponent } from './review/get-reviews/get-reviews.component';
import { CreateReviewComponent } from './review/create-review/create-review.component';
import { DeleteReviewComponent } from './review/delete-review/delete-review.component';
import { UpdateReviewComponent } from './review/update-review/update-review.component';
import { GetPricesHistoryComponent } from './price-history/get-prices-history/get-prices-history.component';
import { GetCategoriesComponent } from './category/get-categories/get-categories.component';
import { CreateProductComponent } from './product/create-product/create-product.component';
import {ReactiveFormsModule} from "@angular/forms";


@NgModule({
  declarations: [
    GetProductComponent,
    GetProductsComponent,
    GetReviewComponent,
    GetReviewsComponent,
    CreateReviewComponent,
    DeleteReviewComponent,
    UpdateReviewComponent,
    GetPricesHistoryComponent,
    GetCategoriesComponent,
    CreateProductComponent
  ],
    imports: [
        CommonModule,
        ProductRoutingModule,
        MaterialModule,
        MatGridListModule,
        ReactiveFormsModule
    ],
  exports:[]
})
export class ProductModule { }
