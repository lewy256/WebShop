import { Component } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {AuthenticationUserDto, IdentityApiService, TokenDto} from "../../services/identity-api.service";
import {environment} from "../../../environments/environment";
import {Observable} from "rxjs";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  hide:boolean = true;
  identityService:IdentityApiService;
  token:string="";

  constructor(private httpClient:HttpClient,private formBuilder: FormBuilder) {
    this.identityService=new IdentityApiService(this.httpClient,environment.urlAddress);
  }

  loginForm = this.formBuilder.group({
    userName: ['kowalski16', [Validators.required,Validators.minLength(3)]],
    password: ['96RnP9}16XHl',[Validators.required,Validators.minLength(8)]]
  });

  login():void{
    let tokenDto:Observable<TokenDto>=this.identityService.loginUser(
      new AuthenticationUserDto({userName:this.loginForm.value.userName,password:this.loginForm.value.password as string}))
    tokenDto.subscribe(x=>this.token=x.accessToken as string);
  }

}
