import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Basket} from './interfaces/Basket';
import { BasketService } from './services/basket.service';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator'
import { BasketItem } from './interfaces/BasketItem';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.css']
})
export class BasketComponent implements OnInit, AfterViewInit {
  public displayedColumns = ['id'];
  public dataSource = new MatTableDataSource<Basket>();


  @ViewChild((MatSort) as any) sort: MatSort | undefined;
  @ViewChild((MatPaginator) as any) paginator: MatPaginator | undefined;

  constructor(private basketService: BasketService) { }

  ngOnInit() {
    this.getBasket();
  }
  public getBasket = () => {
    this.basketService.getBasketById('3fa85f64-5717-4562-b3fc-2c963f66afa6')
      .subscribe(res => {
        this.dataSource.data=[res];
      });
  }

  public createBasket = (basket:Basket) => {
    this.basketService.addBasket(basket);
  }

    public deleteBasket = (id:string) => {
    this.basketService.deleteBasket(id);
  }


  ngAfterViewInit(): void {
    this.dataSource.sort = ((this.sort) as any);
    this.dataSource.paginator = ((this.paginator) as any);
  }

  public customSort = (event: any) => {
    console.log(event);
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  }




}

