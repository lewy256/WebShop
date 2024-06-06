import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {IdentityService, RegistrationUserDto,} from "../../services/api/identity.service";
import {HttpClient} from "@angular/common/http";
import {finalize} from "rxjs";
import {ErrorMappingService} from "../../services/shared/error-mapping.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

export class RegisterComponent {
  hide: boolean = true;
  private identityService: IdentityService;
  errorMessage:string="";
  isDataLoaded: boolean=false;

  constructor(private httpClient: HttpClient,
              private formBuilder: FormBuilder,
              private errorMappingService:ErrorMappingService,
              private router : Router) {
    this.identityService = new IdentityService(this.httpClient);
  }
  registerForm:FormGroup = this.formBuilder.group({
    firstName:['Jan',[Validators.required]],
    lastName:['Kowalski',[Validators.required]],
    email:['kowalski423@wp.pl',[Validators.required,Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$')]],
    phoneNumber:['+48444444444',[Validators.required,Validators.pattern('^\\+\\d{1,3}\\s?\\d{8,14}$')]],
    userName: ['kowalski44', [Validators.required, Validators.pattern('^[a-zA-Z0-9]+$')]],
    password: ['96RnP9}16XHl', [Validators.required, Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,}$')]],
    policy:[false, [Validators.pattern('true')]],
    newsletter:['', []],
    updateOn: 'blur',
  });

  register(): void {
    this.isDataLoaded = true

    this.identityService.registerUser(
      new RegistrationUserDto(
        this.registerForm.value.firstName,
        this.registerForm.value.lastName,
        this.registerForm.value.userName,
        this.registerForm.value.password,
        this.registerForm.value.email,
        this.registerForm.value.phoneNumber,
      ))
      .pipe(finalize(() => this.isDataLoaded = false))
      .subscribe({
        next:()=>{
          this.registerForm.reset();
          this.errorMessage='';
          this.router.navigate(['/login']);
        },
        error:(x)=> {
        if(x.statusCode===422){
          this.errorMappingService.MappingValidationError(x.errors,this.registerForm);
        }else{
          this.errorMessage=x;
        }
      }
    });
  }
}


