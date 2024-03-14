import { Injectable } from '@angular/core';
import {catchError, Observable, throwError} from "rxjs";
import {UnauthorizedResponse, ValidationResponse} from "./identity.service";
import {environment} from "../../../environments/environment";
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})

export class BasketApiService {
  private baseUrl:string=environment.urlAddress+'/api/basket';

  constructor(private httpClient:HttpClient) { }

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      "Accept": "application/json"
    }),
  };

  updateBasket(basketDto: UpdateBasketDto): Observable<BasketDto> {
    return this.httpClient.post<BasketDto>(this.baseUrl, basketDto,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  getBasket(basketId:string): Observable<BasketDto> {
    return this.httpClient.get<BasketDto>(this.baseUrl+'/'+basketId,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  deleteBasket(basketId:string): Observable<void>  {
    return this.httpClient.delete<void>(this.baseUrl+'/'+basketId,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  checkoutBasket(basketId:string): void {
    this.httpClient.post(this.baseUrl+'/'+basketId,this.httpOptions)
      .pipe(catchError(this.handleError));
  }


  private handleError(error: HttpErrorResponse) {
    let customError:any;
    if (error.status===422) {
      customError= error.error as ValidationResponse;
    }
    else if (error.status===401) {
      const errorMessage:string='Login or password is invalid.';
      customError = new UnauthorizedResponse(error.status,errorMessage);
    }
    else {
      customError='Something went wrong.\n Please try again later.';
    }

    return throwError(() => {
      return customError;
    });
  }
}
export class UpdateBasketDto {
  id: string;
  items: BasketItem[];

  constructor(id: string, items: BasketItem[]) {
    this.id = id;
    this.items = items;
  }
}
export class BasketItem {
  id:string;
  productId: string;
  productName:string;
  imageUrl:string;
  quantity: number;
  price: number;

  constructor(id: string, productId: string, productName: string, imageUrl: string, quantity: number, price: number) {
    this.id = id;
    this.productId = productId;
    this.productName = productName;
    this.imageUrl = imageUrl;
    this.quantity = quantity;
    this.price = price;
  }




}

export class BasketDto {
  id: string;
  items: BasketItem[];
  totalPrice?: number;

  constructor(id: string, items: BasketItem[]) {
    this.id = id;
    this.items = items;
  }
}
