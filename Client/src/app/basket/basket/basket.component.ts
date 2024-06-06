import {Component, Input, OnInit} from '@angular/core';
import {BasketDto, BasketItem, BasketApiService, UpdateBasketDto} from "../../services/api/basket-api.service";
import {HttpClient} from "@angular/common/http";
import {BasketSharedService} from "../../services/shared/basket-shared.service";
import {ProductDto} from "../../services/api/product-api.service";
import {lastValueFrom, take} from "rxjs";
import {LayoutSharedService} from "../../services/shared/layout-shared.service";



@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})

export class BasketComponent implements OnInit{
  private basketApiService:BasketApiService;
  basket: BasketDto;
  private guid:string='57633a10-f144-4430-8acb-9de2a2495014';

  constructor(private httpClient:HttpClient,
              private basketSharedService:BasketSharedService,
              private layoutSharedService: LayoutSharedService,) {
    this.basketApiService=new BasketApiService(this.httpClient)
    this.basket=new BasketDto(this.guid,[]);
  }

  isDarkTheme:boolean=false;

  ngOnInit(): void {
    this.getBasket();
    this.layoutSharedService.checkDarkTheme()
      .subscribe({
      next:(darkTheme: boolean):void => {
        this.isDarkTheme=darkTheme;
      }
    });
  }

  private setTotalBasketItems(basketItems:BasketItem[]): void {
    let itemCount:number=0;
    basketItems.forEach(x=>itemCount+=x.quantity)

    this.basketSharedService.setNumBasketItems(itemCount);
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
            this.setTotalBasketItems(basket.items);
          }
        });

    } else{
      this.basket=basket;
    }
  }


 deleteBasket():void{
    this.basketApiService.deleteBasket(this.guid).subscribe();
    this.basket=new BasketDto(this.guid,[]);
    this.setTotalBasketItems([]);

  }

  deleteBasketItem(itemId:string): void {
    this.basketApiService.getBasket(this.guid).subscribe({
      next:(basket) => {
        let items: BasketItem[] =basket.items.filter(item=>item.id!==itemId);
        this.basketApiService.updateBasket(new UpdateBasketDto(this.guid,items))
          .subscribe({
            next:(x)=>this.basket=x
          });
        this.setTotalBasketItems(items);
      }
    });
  }

  checkoutBasket():void{

  }

}
