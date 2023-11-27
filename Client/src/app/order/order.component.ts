import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { OrderService } from './services/order.service';
import { Order } from './interfaces/order';


@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit, AfterViewInit {

  public displayedColumns = ['orderId','customerId','orderDate','totalPrice'];
  public ordersData = new MatTableDataSource<Order>();

  @ViewChild((MatSort) as any) sort: MatSort | undefined;
  @ViewChild((MatPaginator) as any) paginator: MatPaginator | undefined;

  constructor(private orderService: OrderService) { }

  ngOnInit() {
    this.getOrderByCustomer();
  }
  public getOrderByCustomer = () => {
    this.orderService.getData('api/Order/1')
      .subscribe(res => {
        this.ordersData.data = res as Order[];
      })
  }
  ngAfterViewInit(): void {
    this.ordersData.sort = ((this.sort) as any);
    this.ordersData.paginator = ((this.paginator) as any);
  }

  public customSort = (event: any) => {
    console.log(event);
  }

  public doFilter = (value: string) => {
    this.ordersData.filter = value.trim().toLocaleLowerCase();
  }



  public redirectToDetails = (id: string) => {
    
  }
  public redirectToUpdate = (id: string) => {
    
  }


  public redirectToDelete = (id: string) => {
    this.orderService.delete(`api/Order/${id}`)
    .subscribe(
      (val) => {
          console.log("DELETE call successful value returned in body", 
                      val);
      },
      response => {
          console.log("DELETE call in error", response);
      },
      () => {
          console.log("The DELETE observable is now completed.");
      });;
  }

}
