import {AfterViewInit, Component, OnInit, signal, ViewChild, WritableSignal} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {CategorySharedService} from "../../../services/shared/category-shared.service";
import {catchError, throwError} from "rxjs";
import {environment} from "../../../../environments/environment";
import {ProductApiService, ProductDto} from "../../../services/api/product-api.service";
import {MatSort} from "@angular/material/sort";
import {MatPaginator, PageEvent} from "@angular/material/paginator";
import {BasketDto, BasketItem, BasketApiService, UpdateBasketDto} from "../../../services/api/basket-api.service";
import {BasketSharedService} from "../../../services/shared/basket-shared.service";
import {ProductSharedService} from "../../../services/shared/product-shared.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MatSelectChange} from "@angular/material/select";
import {MatDialog} from "@angular/material/dialog";
import {DialogComponent} from "./components/dialog/dialog.component";
import {MatSidenav} from "@angular/material/sidenav";



@Component({
  selector: 'app-get-products',
  templateUrl: './get-products.component.html',
  styleUrls: ['./get-products.component.scss']
})

export class GetProductsComponent implements OnInit{
  products: ProductDto[] =[];

  deliveryTime: Date = new Date();

  test:boolean=false;

  closeFilter(isClose:boolean):void{
    if(isClose){
      this.test=true;
    } else{
      this.test=false;
    }
  }

  @ViewChild(MatPaginator) paginator!:MatPaginator;

  errorMessage:string="";

  private productApiService:ProductApiService;

  constructor(private httpClient:HttpClient,
              private categoryService:CategorySharedService,
              private basketService:BasketSharedService,
              private priceHistoryService:ProductSharedService,
              private formBuilder: FormBuilder,
              private dialog: MatDialog
  ) {
    this.productApiService=new ProductApiService(this.httpClient);

  }

  ngOnInit(): void {
    this.getAllProducts();
    this.getAllProductByCategory();

    const currentDate: Date = new Date();
    this.deliveryTime.setDate(currentDate.getDate() + 1);
  }

  itemsNumber:number=50;

  private getAllProducts(): void {
    this.productApiService.getAllProducts(undefined,this.itemsNumber,this.loginForm.value.priceTo,this.loginForm.value.priceFrom)
      .subscribe({
        next:(x) => {
          this.filterData(x);
        },
        error:(x)=>this.errorMessage=x
      });
  }

  sortProductsDesc(): void {
    this.products = this.products.sort((a, b) => a.productName.localeCompare(b.productName));
  }

  sortProductsAsc(): void {
    this.products = this.products.sort((a, b) => b.productName.localeCompare(a.productName));
  }

  change(event:PageEvent) {
    this.itemsNumber=event.pageSize;
    this.getAllProducts();
  }


  private getAllProductByCategory(): void {
    this.categoryService.getCategoryId()
      .subscribe({
        next:(c) => {
          this.productApiService.getProductsForCategory(c)
            .subscribe({
              next:(p) => this.filterData(p),
              error:(p)=>this.errorMessage=p})

        },
        error:(x)=>this.errorMessage=x
      });
  }

  private filterData(items:ProductDto[]): void {
    this.categoryService.getQuery().subscribe({
      next:(query:string):void => {
        if(query){
          this.products=(items.filter(p=>p.productName?.toLowerCase().includes(query.trim().toLowerCase())));
        } else{
          this.products=(items);
        }
      },
      error:(p)=>{}});

  }

  loginForm:FormGroup = this.formBuilder.group({
    priceFrom: [0, []],
    priceTo: [50, []]
  });


  addToBasket(product:ProductDto):void{
    this.basketService.setProduct(product);
  }

  setProductId(id:string):void{
    this.priceHistoryService.setProductId(id);
  }


  openDialog(images:string[]): void {
    this.dialog.open(DialogComponent, {
      data: {images},
    });

  }

}
