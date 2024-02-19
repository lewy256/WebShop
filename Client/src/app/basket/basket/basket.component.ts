import { Component } from '@angular/core';
import {BasketApiService, BasketItem, CreateBasketDto} from "../../services/basket-api.service";
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {IdentityApiService} from "../../services/identity-api.service";

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})

export class BasketComponent {
  private basketService:BasketApiService;

  constructor(private httpClient: HttpClient) {
    this.basketService=new BasketApiService(httpClient,environment.urlAddress);
  }

  createBasket(item: CreateBasketDto):void {
    this.basketService.createBasket();

  }

  getBasket() {
    //let items=this.basketService.getBasket();
    //return items;
  }

  checkoutBasket(id:string){
    this.basketService.checkoutBasket(id);
  }
  deleteBasket(id:string):void{
    this.basketService.deleteBasket(id);
  }
}
