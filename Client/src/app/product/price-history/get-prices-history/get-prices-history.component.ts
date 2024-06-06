import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {HttpClient, HttpResponse} from "@angular/common/http";
import {MetaData, PriceHistoryDto, ProductApiService} from "../../../services/api/product-api.service";
import {ProductSharedService} from "../../../services/shared/product-shared.service";
import {MatTableDataSource} from "@angular/material/table";
import {MatPaginator, PageEvent} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {formatDate} from "@angular/common";


@Component({
  selector: 'app-get-prices-history',
  templateUrl: './get-prices-history.component.html',
  styleUrls: ['./get-prices-history.component.scss']
})

export class GetPricesHistoryComponent implements OnInit,AfterViewInit {
  private productService:ProductApiService

  metaData!:MetaData;

  prices?:any;

  constructor(private httpClient: HttpClient, private priceHistoryService:ProductSharedService) {
   // Object.assign(this, { multi });
    this.productService=new ProductApiService(this.httpClient);
    this.metaData=new MetaData(1,1,0,0,false,false);
  }

  ngOnInit(): void {
    this.priceHistoryService
      .getProductId()
      .subscribe({
        next:(id:string):void => {
          this.getProductPrices(id);
        },
        error:():void=>{
        }
      });

  }

  ngAfterViewInit(): void {

  }

  private getProductPrices(productId:string):void{
  //   this.productService.getPricesHistoryForProduct(productId,this.metaData.currentPage,this.metaData.pageSize)
  //     .subscribe({
  //       next:(x: HttpResponse<any>):void => {
  //         const paginationHeader:string=x.headers.get('x-pagination') as string;
  //         this.metaData=JSON.parse(paginationHeader) as MetaData;
  //
  //         const prices: PriceHistoryDto[] =x.body as PriceHistoryDto[];
  //
  //         const chartData = {
  //           name: "Price",
  //           series: prices.map(item => ({
  //             name: item.startDate,
  //             value: item.priceValue
  //           }))
  //         };
  //
  //         this.test=chartData;
  //
  //
  //       },
  //       error:():void=>{
  //       }
  //     });
  // }

    this.productService.getPricesHistoryForProduct(productId).subscribe({
        next:(prices: PriceHistoryDto[]):void => {

          this.prices = {
            name: "Price",
            series: prices.map((item: PriceHistoryDto) => ({
              value: item.priceValue,
              name: formatDate(item.endDate,'yyyy-MM-dd','en-US'),
            }))
        };




        },
        error:():void=>{}
        });
  }



  view: any = [700, 300];
  legend: boolean = false;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = true;
  xAxisLabel: string = 'Date';
  yAxisLabel: string = 'Price';
  timeline: boolean = true;

  protected readonly multi = multi;
}

export var multi = [
  {
    "name": "Germany",
    "series": [
      {
        "name": "1990",
        "value": 62000000
      },
      {
        "name": "2010",
        "value": 73000000
      },
      {
        "name": "2011",
        "value": 89400000
      }
    ]
  }
];
