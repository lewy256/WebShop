import {BehaviorSubject, Observable} from "rxjs";
import {Injectable} from "@angular/core";
import {ProductDto} from "../api/product2.service";
import {BasketDto, BasketItem} from "../api/basket-api.service";
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})

export class BasketSharedService {
  private totalBasketItems: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  private basketItems: BasketItem[] = [];
 // private customer: Subject<Object> = new ReplaySubject<Object>(1);

  getBasketItems(): BasketItem[] {
    let items: BasketItem[] =this.basketItems;
    this.basketItems=[];
    return items;
  }

  setProduct(product:ProductDto):void{
    const itemId:string = uuidv4();
    this.basketItems.push(new BasketItem(itemId,product.id,product.productName,"dupa",1,product.price))
    this.totalBasketItems.next(this.totalBasketItems.value+1);
  }


  getNumBasketItems(): Observable<number> {
    return this.totalBasketItems as Observable<number>;
  }

  setNumBasketItems(items: number):void{
    this.totalBasketItems.next(items);
  }

}
