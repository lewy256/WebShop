import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {RegisterComponent} from "./register/register.component";
import {LoginComponent} from "./login/login.component";
import {LogoutComponent} from "./logout/logout.component";
import {ResetPasswordComponent} from "./reset-password/reset-password.component";
import {TermsOfUseComponent} from "./terms-of-use/terms-of-use.component";
import {PrivacyPolicyComponent} from "./privacy-policy/privacy-policy.component";

const routes: Routes = [
  { path:"login",component: LoginComponent },
  { path:"register",component: RegisterComponent},
  { path:"logout",component: LogoutComponent},
  { path:"policy",component: PrivacyPolicyComponent},
  { path:"terms",component: TermsOfUseComponent},
  { path:"reset-password",component: ResetPasswordComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
