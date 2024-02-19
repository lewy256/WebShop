import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator'
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {
  Category,
  CreateCategoryDto,
  ProductApiService,
  ProductDto,
  ReviewDto
} from "../../services/product-api.service";
import {MatDrawer} from "@angular/material/sidenav";
import {environment} from "../../../environments/environment";
import {SharedService} from "../../services/shared.service";
import {AfterViewInit, Component, OnInit, ViewChild} from "@angular/core";
import {
  AuthenticationUserDto,
  IAuthenticationUserDto,
  IdentityApiService,
  TokenDto
} from "../../services/identity-api.service";
import {Observable} from "rxjs";
@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit, AfterViewInit {
  public dataSource = new MatTableDataSource<ProductDto>();

  @ViewChild((MatSort) as any) sort: MatSort | undefined;
  // @ViewChild((MatPaginator) as any) paginator: MatPaginator | undefined;

  private productService: ProductApiService;

  public accessToken:string | null | undefined="";

  constructor(private httpClient: HttpClient, private sharedService:SharedService) {
    this.productService=new ProductApiService(this.httpClient,environment.urlAddress);

    // this.identityApi.loginUser(new AuthenticationUserDto({userName:"kowalski16",password:"96RnP9}16XHl"}))
    //   .subscribe(x=>this.accessToken=x.accessToken);

    // if(typeof this.accessToken==="string"){
    //   this.api.setAuthToken(this.accessToken);
    // }

    this.sharedService.data$.subscribe((data)=>
      this.getAllProducts(data));

    this,sharedService.filterData$.subscribe((data)=>
      this.doFilter(data));

  }

  public getAllProducts(categoryId:string):void  {
    this.productService.getProductsForCategory(categoryId)
      .subscribe(res => {
        this.dataSource.data=res as ProductDto[]
        this.cardData=res;
      })
  }

  public getAllProductsByCategoryId(categoryId:string):void  {
    this.productService.getProductsForCategory(categoryId)
      .subscribe(res => {
        this.dataSource.data=res as ProductDto[]
        this.cardData=res;
      })
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

  public myFunc():void{
      console.log(this.accessToken)
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


}
