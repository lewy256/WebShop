import {Component, signal, ViewChild, WritableSignal} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {CategoryService} from "../../../services/shared/category.service";
import {catchError, throwError} from "rxjs";
import {environment} from "../../../../environments/environment";
import {Product2Service, ProductDto} from "../../../services/api/product2.service";
import {MatSort} from "@angular/material/sort";
import {MatPaginator} from "@angular/material/paginator";
import {BasketDto, BasketItem, BasketApiService, UpdateBasketDto} from "../../../services/api/basket-api.service";
import {BasketSharedService} from "../../../services/shared/basket-shared.service";

@Component({
  selector: 'app-get-products',
  templateUrl: './get-products.component.html',
  styleUrls: ['./get-products.component.css']
})

export class GetProductsComponent {
  products: WritableSignal<ProductDto[]> = signal<ProductDto[]>([]);

  @ViewChild((MatSort) as any) sort: MatSort | undefined;
  @ViewChild(MatPaginator,{static:true}) public paginator!:MatPaginator

  errorMessage:string="";

  private productApiService:Product2Service;

  constructor(private httpClient:HttpClient,
              private categoryService:CategoryService,
              private basketSharedService:BasketSharedService) {
    this.productApiService=new Product2Service(this.httpClient);

    this.updateProductsView();

  }

  private updateProductsView(): void {
    this.categoryService.getCategoryId()
      .subscribe({
        next:(c) => {
          this.productApiService.getProductsForCategory(c)
            .subscribe({
              next:(p) => this.products.set(this.filterData(p)),
              error:(p)=>this.errorMessage=p})

        },
        error:(x)=>this.errorMessage=x
      });
  }

  addToBasket(product:ProductDto):void{
    this.basketSharedService.setProduct(product);
  }

  private filterData(items:ProductDto[]): ProductDto[] {
    const searchQuery:string=this.categoryService.getQuery();
    return searchQuery
      ? items.filter(p=>p.productName?.toLowerCase()
        .includes(searchQuery.trim().toLowerCase()))
      : items;
  }

}
