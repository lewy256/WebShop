import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewComponent } from './review/review.component';
import { PriceHistoryComponent } from './price-history/price-history.component';
import { ProductComponent } from './product/product.component';
import { CategoryComponent } from './category/category.component';
import {MaterialModule} from "../material/material.module";



@NgModule({
  declarations: [
    ReviewComponent,
    PriceHistoryComponent,
    ProductComponent,
    CategoryComponent
  ],
    imports: [
        CommonModule,
        MaterialModule
    ]
})
export class ProductModule { }
