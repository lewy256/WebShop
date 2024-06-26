import {Component, EventEmitter, HostBinding, OnInit, Output} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {OverlayContainer} from "@angular/cdk/overlay";
import {catchError, Observable, startWith, throwError} from "rxjs";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {LoginSharedService} from "../services/shared/login-shared.service";
import {CategorySharedService} from "../services/shared/category-shared.service";
import {Router} from "@angular/router";
import {BasketSharedService} from "../services/shared/basket-shared.service";
import {Category, ProductApiService} from "../services/api/product-api.service";
import {LayoutSharedService} from "../services/shared/layout-shared.service";
import {BasketApiService} from "../services/api/basket-api.service";

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})

export class LayoutComponent implements OnInit {
  private productService: ProductApiService;
  categories:Category[]= [];
  errorMessage:string="";
  userName:string="";
  itemCount:number=0;

  searchForm:FormGroup = this.formBuilder.group({
    query: ['', []]
  });

  constructor(private overlay: OverlayContainer,
              private http: HttpClient,
              private categoryService:CategorySharedService,
              private loginService:LoginSharedService,
              private formBuilder: FormBuilder,
              private router: Router,
              private basketSharedService:BasketSharedService,
              private basketApiService:BasketApiService,
              private layoutSharedService:LayoutSharedService,) {
    this.productService=new ProductApiService(this.http);

  }

  ngOnInit(): void {
    this.setTheme();
    this.getCategories();
    this.getTotalBasketItems();
  }

  private getCategories():void{
    this.productService.getCategories()
      .pipe(catchError(this.handleError))
      .subscribe({
        next:(x) => {
          this.categories=x
        },
        error:(x)=>{
          this.errorMessage=x
        }
      });
  }

  private getTotalBasketItems():void{
    this.basketSharedService.getNumBasketItems().subscribe({
      next:(x)=>this.itemCount=x
    })
  }

  isAuthenticated():boolean{
    if(this.loginService.getToken()){
      this.userName=this.loginService.getUserName();
      return true;
    } else{
      return false;
    }
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage:string = '';
    if (error.status===422) {
      errorMessage = `Login or password is invalid.`;
    } else {
    }

    return throwError(() => {
      return errorMessage;
    });
  }


  toggleControl:FormControl = new FormControl(false);
  @HostBinding('class') className: string = '';

  private setTheme():void{
    const darkMode:string = 'darkMode';
    let savedMode:string=localStorage.getItem('mode') as string;

    if(savedMode===darkMode){
      this.overlay.getContainerElement().classList.add(darkMode);
      this.className=darkMode;
      this.toggleControl.setValue(true);
    }

    this.toggleControl.valueChanges.subscribe((isActive:boolean):void => {
      this.className = isActive ? darkMode : '';
      if (isActive) {
        this.overlay.getContainerElement().classList.add(darkMode);
        localStorage.setItem('mode', 'darkMode');
        this.layoutSharedService.setDarkTheme(true);
      } else{
        this.overlay.getContainerElement().classList.remove(darkMode);
        localStorage.setItem('mode', 'none');
        this.layoutSharedService.setDarkTheme(false);
      }
    });

  }

  setCategoryId(categoryId:string): void {
    this.categoryService.setCategoryId(categoryId);
  }


  setSearchQuery():void{
    this.categoryService.setQuery(this.searchForm.value.query);
    //this.router.navigateByUrl(`/product`);
  }





}
