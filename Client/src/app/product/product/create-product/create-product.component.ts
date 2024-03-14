import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {finalize, Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {LoginService} from "../../../services/shared/login.service";
import {ErrorMappingService} from "../../../services/shared/error-mapping.service";
import {Router} from "@angular/router";
import {Product2Service} from "../../../services/api/product2.service";

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.css']
})

export class CreateProductComponent {
  hide: boolean = true;
  private productService: Product2Service;
  errorMessage?:string;
  isDataLoaded: boolean=false;

  constructor(private httpClient: HttpClient,
              private formBuilder: FormBuilder,
              private loginService: LoginService,
              private errorMappingService:ErrorMappingService,
              private router : Router) {
    this.productService = new Product2Service(this.httpClient);
  }

  productForm:FormGroup = this.formBuilder.group({
    productName: ['pc', [Validators.required]],
    serialNumber: ['123345', [Validators.required]],
    price: ['44.44', [Validators.required]],
    stock: ['44', [Validators.required]],
    description: ['good pc', [Validators.required]],
    color: ['black', [Validators.required]],
    weight: ['23', [Validators.required]],
    size: ['21', [Validators.required]],
    updateOn: 'blur',
  });

  // createProduct(): void {
  //   this.isDataLoaded = true
  //
  //   let tokenDto: Observable<TokenDto> = this.productService.loginUser(
  //     new AuthenticationUserDto(
  //       this.loginForm.value.userName,
  //       this.loginForm.value.password
  //     ));
  //
  //   tokenDto
  //     .pipe(finalize(() => this.isDataLoaded = false))
  //     .subscribe({
  //       next:(x) => {
  //         this.loginService.setToken(x.accessToken as string)
  //         this.loginService.setUserName(this.loginForm.value.userName);
  //         this.loginForm.reset();
  //         this.errorMessage='';
  //         this.router.navigate(['product']);
  //       },
  //       error:(x)=>{
  //         if(x.statusCode===422){
  //           this.errorMappingService.MappingValidationError(x.errors,this.loginForm);
  //         } else if(x.statusCode===401){
  //           this.errorMessage=x.message;
  //         } else{
  //           this.errorMessage=x;
  //         }
  //       }
  //     });
  //
  // }

}
