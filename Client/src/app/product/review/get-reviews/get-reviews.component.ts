import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ProductApiService, ReviewDto} from "../../../services/api/product-api.service";
import {ProductSharedService} from "../../../services/shared/product-shared.service";

@Component({
  selector: 'app-get-reviews',
  templateUrl: './get-reviews.component.html',
  styleUrls: ['./get-reviews.component.scss']
})

export class GetReviewsComponent implements OnInit {
  private productService:ProductApiService;
  //reviews:ReviewDto[];

  constructor(private httpClient: HttpClient, private productSharedService: ProductSharedService) {
    this.productService=new ProductApiService(this.httpClient);
  }

  ngOnInit(): void {
    this.productSharedService.getProductId()
      .subscribe({
        next:(id:string):void => {
          this.getReviews(id);
        },
        error:():void=>{
        }
      });
  }

  private getReviews(productId:string):void {
    this.productService.getReviewsForProduct(productId)
      .subscribe({
        next:(x):void => {
          //this.getReviews(id);
        },
        error:():void=>{
        }
      });
  }

}
