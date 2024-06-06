import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class ProductSharedService {
  private productId: BehaviorSubject<string> = new BehaviorSubject<string>('');

  getProductId(): Observable<string>{
    return this.productId as Observable<string>;
  }

  setProductId(productId:string):void{
    this.productId.next(productId);
  }

}
