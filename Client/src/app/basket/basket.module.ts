import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasketComponent } from './basket/basket.component';
import {MaterialModule} from "../material/material.module";
import {BasketRoutingModule} from "./basket-routing.module";

@NgModule({
  declarations: [
    BasketComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    BasketRoutingModule
  ]
})
export class BasketModule { }
