import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule} from "@angular/common/http";
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OrderComponent } from './order/order.component';
import { ProductComponent } from './product/product.component';
import { BasketComponent } from './basket/basket.component';
import { MaterialModule } from './material/material.module';
import { LayoutComponent } from './layout/layout.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { AccountComponent } from './account/account.component'
import {MatCardModule} from "@angular/material/card";
import {MatGridListModule} from "@angular/material/grid-list";

@NgModule({
  declarations: [
    AppComponent,
    OrderComponent,
    ProductComponent,
    BasketComponent,
    LayoutComponent,
    AccountComponent,

  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    MatCardModule,
    MatGridListModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
