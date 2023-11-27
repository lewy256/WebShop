import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Product } from './interfaces/Product';
import { ProductService } from './services/product.service';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator'

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit, AfterViewInit {
  public displayedColumns = ['id','name','price','description','details','update','delete'];
  public dataSource = new MatTableDataSource<Product>();

  @ViewChild((MatSort) as any) sort: MatSort | undefined;
  @ViewChild((MatPaginator) as any) paginator: MatPaginator | undefined;

  constructor(private productService: ProductService) { }

  ngOnInit() {
    this.getAllProducts();
  }
  public getAllProducts = () => {
    this.productService.getData('api/Product')
      .subscribe(res => {
        this.dataSource.data = res as Product[];
      })
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



  public redirectToDetails = (id: string) => {
    
  }
  public redirectToUpdate = (id: string) => {
    
  }

  public redirectToBasket = (id: string) => {
    
  }



  public redirectToDelete = (id: string) => {
    this.productService.delete(`api/Product/${id}`)
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
