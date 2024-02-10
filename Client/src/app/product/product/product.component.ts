import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator'
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {CategoryDto, ProductApiService, ProductDto, ReviewDto} from "../../services/product.api.service";
import {MatDrawer} from "@angular/material/sidenav";
import {environment} from "../../../environments/environment";
import {SharedService} from "../../services/shared.service";
import {AfterViewInit, Component, OnInit, ViewChild} from "@angular/core";
@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit, AfterViewInit {
  public dataSource = new MatTableDataSource<ProductDto>();

  @ViewChild((MatSort) as any) sort: MatSort | undefined;
  // @ViewChild((MatPaginator) as any) paginator: MatPaginator | undefined;

  private api: ProductApiService;

  constructor(private http: HttpClient, private sharedService:SharedService) {
    this.api=new ProductApiService(this.http,environment.urlAddress);

    this.sharedService.data$.subscribe((data)=>
      this.getAllProducts(data));

    this,sharedService.filterData$.subscribe((data)=>
      this.doFilter(data));

  }

  @ViewChild('drawer') public drawer!: MatDrawer;
  ngOnInit() {
    this.sharedService.sideNavToggleSubject.subscribe(()=> {
      if(!this.drawer.opened){
        this.drawer.toggle();
      }

    });
  }

  public cardData:ProductDto[]=[];

  public getAllProducts = (categoryId:string) => {
    this.api.getProductsForCategory(categoryId)
      .subscribe(res => {
        this.dataSource.data=res as ProductDto[]
        this.cardData=res;
      })

  }
  @ViewChild(MatPaginator,{static:true}) public paginator!:MatPaginator
  ngAfterViewInit(): void {
    this.cardData.sort = ((this.sort) as any);
    //this.cardData.paginator = ((this.paginator) as any);
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



  /*  public redirectToDelete = (id: string) => {
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
    }*/

}
