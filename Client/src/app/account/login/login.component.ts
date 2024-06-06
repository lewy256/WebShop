import { Component } from '@angular/core';
import {finalize, Observable} from "rxjs";
import {FormBuilder, FormGroup, Validators,} from "@angular/forms";
import {LoginSharedService} from "../../services/shared/login-shared.service";
import {HttpClient} from "@angular/common/http";
import {ErrorMappingService} from "../../services/shared/error-mapping.service";
import {AuthenticationUserDto, IdentityService, TokenDto} from "../../services/api/identity.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent {
  hide: boolean = true;
  private identityService: IdentityService;
  errorMessage?:string;
  isDataLoaded: boolean=false;

  constructor(private httpClient: HttpClient,
              private formBuilder: FormBuilder,
              private loginService: LoginSharedService,
              private errorMappingService:ErrorMappingService,
              private router : Router) {
    this.identityService = new IdentityService(this.httpClient);
  }

  loginForm:FormGroup = this.formBuilder.group({
    userName: ['kowalski16', [Validators.required]],
    password: ['96RnP9}16XHl', [Validators.required]],
    updateOn: 'blur',
  });

  login(): void {
    this.isDataLoaded = true

    let tokenDto: Observable<TokenDto> = this.identityService.loginUser(
      new AuthenticationUserDto(
        this.loginForm.value.userName,
        this.loginForm.value.password
      ));

    tokenDto
      .pipe(finalize(() => this.isDataLoaded = false))
      .subscribe({
        next:(x) => {
          this.loginService.setToken(x.accessToken as string)
          this.loginService.setUserName(this.loginForm.value.userName);
          this.loginForm.reset();
          this.errorMessage='';
          this.router.navigate(['product']);
        },
        error:(x)=>{
          if(x.statusCode===422){
            this.errorMappingService.MappingValidationError(x.errors,this.loginForm);
          } else if(x.statusCode===401){
            this.errorMessage=x.message;
          } else{
            this.errorMessage=x;
          }
        }
      });

  }

}

