import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {AccountRoutingModule} from "./account-routing.module";
import {MaterialModule} from "../material/material.module";
import { RegisterComponent } from './register/register.component';
import {LoginComponent} from "./login/login.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { LogoutComponent } from './logout/logout.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { TermsOfUseComponent } from './terms-of-use/terms-of-use.component';
import { PrivacyPolicyComponent } from './privacy-policy/privacy-policy.component';



@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    LogoutComponent,
    ResetPasswordComponent,
    TermsOfUseComponent,
    PrivacyPolicyComponent
  ],
    imports: [
        CommonModule,
        MaterialModule,
        AccountRoutingModule,
        ReactiveFormsModule,
        FormsModule

    ],
  exports:[
  ]
})
export class AccountModule { }
