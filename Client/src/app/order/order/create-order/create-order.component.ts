import {Component, OnInit} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {BasketApiService, BasketDto, BasketItem, UpdateBasketDto} from "../../../services/api/basket-api.service";
import {HttpClient} from "@angular/common/http";
import {BasketSharedService} from "../../../services/shared/basket-shared.service";
import {LayoutSharedService} from "../../../services/shared/layout-shared.service";

@Component({
  selector: 'app-create-order',
  templateUrl: './create-order.component.html',
  styleUrls: ['./create-order.component.scss']
})

export class CreateOrderComponent implements OnInit{
  private basketApiService:BasketApiService;
  basket: BasketDto;
  private guid:string='57633a10-f144-4430-8acb-9de2a2495014';

  constructor(private httpClient:HttpClient,private _formBuilder: FormBuilder, private basketSharedService:BasketSharedService) {
    this.basketApiService=new BasketApiService(this.httpClient)
    this.basket=new BasketDto(this.guid,[]);
  }

  ngOnInit(): void {
    this.getBasket();
  }

  private getBasket():void{
    this.basketApiService.getBasket(this.guid)
      .subscribe({
        next:(basket) => {
          this.updateBasket(basket);
        }
      });
  }

  private updateBasket(basket:BasketDto):void{
    let items: BasketItem[] = this.basketSharedService.getBasketItems();
    if(items.length){
      basket.items.push(...items);
      this.basketApiService.updateBasket(new UpdateBasketDto(this.guid,basket.items))
        .subscribe({
          next:(x)=>{
            this.basket=x;
          }
        });

    } else{
      this.basket=basket;
    }
  }



}
