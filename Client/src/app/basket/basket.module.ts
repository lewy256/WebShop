import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasketComponent } from './basket/basket.component';
import {MaterialModule} from "../material/material.module";
import {BasketRoutingModule} from "./basket-routing.module";
import {MatGridListModule} from "@angular/material/grid-list";

@NgModule({
  declarations: [
    BasketComponent
  ],
  exports: [
    BasketComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    BasketRoutingModule,
    MatGridListModule
  ]
})
export class BasketModule { }
