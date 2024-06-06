import { NgModule } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import { BasketComponent } from './basket/basket.component';
import {MaterialModule} from "../material/material.module";
import {BasketRoutingModule} from "./basket-routing.module";
import {MatGridListModule} from "@angular/material/grid-list";
import { InputAutosizeDirective } from './basket/directives/input-autosize.directive';

@NgModule({
  declarations: [
    BasketComponent,
    InputAutosizeDirective
  ],
  exports: [
    BasketComponent
  ],
    imports: [
        CommonModule,
        MaterialModule,
        BasketRoutingModule,
        MatGridListModule,
        NgOptimizedImage
    ]
})
export class BasketModule { }
