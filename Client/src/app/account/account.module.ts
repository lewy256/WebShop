import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {AccountRoutingModule} from "./account-routing.module";
import {MaterialModule} from "../material/material.module";
import { RegisterComponent } from './register/register.component';
import {LoginComponent} from "./login/login.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";


@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent
  ],
    imports: [
        CommonModule,
        MaterialModule,
        AccountRoutingModule,
        ReactiveFormsModule,
        FormsModule

    ]
})
export class AccountModule { }
