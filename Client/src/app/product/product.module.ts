import { NgModule } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import {ProductRoutingModule} from "./product-routing.module";
import {MaterialModule} from "../material/material.module";
import { GetProductComponent } from './product/get-product/get-product.component';
import { GetProductsComponent } from './product/get-products/get-products.component';
import {MatGridListModule} from "@angular/material/grid-list";
import { GetReviewsComponent } from './review/get-reviews/get-reviews.component';
import { CreateReviewComponent } from './review/create-review/create-review.component';
import { DeleteReviewComponent } from './review/delete-review/delete-review.component';
import { UpdateReviewComponent } from './review/update-review/update-review.component';
import { GetPricesHistoryComponent } from './price-history/get-prices-history/get-prices-history.component';
import { GetCategoriesComponent } from './category/get-categories/get-categories.component';
import { CreateProductComponent } from './product/create-product/create-product.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { PaginatorStyleDirective } from './product/get-products/directives/paginator-style.directive';
import {MatDialogModule} from "@angular/material/dialog";
import {DialogComponent} from "./product/get-products/components/dialog/dialog.component";
import { FilterComponent } from './product/get-products/components/filter/filter.component';
import {BarChartModule, LineChartModule} from "@swimlane/ngx-charts";





@NgModule({
  declarations: [
    GetProductComponent,
    GetProductsComponent,
    GetReviewsComponent,
    CreateReviewComponent,
    DeleteReviewComponent,
    UpdateReviewComponent,
    GetPricesHistoryComponent,
    GetCategoriesComponent,
    CreateProductComponent,
    PaginatorStyleDirective
  ],
  imports: [
    CommonModule,
    ProductRoutingModule,
    MaterialModule,
    MatGridListModule,
    ReactiveFormsModule,
    NgOptimizedImage,
    MatDialogModule,
    FormsModule,
    DialogComponent,
    FilterComponent,
    LineChartModule,
    BarChartModule
  ],
  exports:[]
})
export class ProductModule { }
