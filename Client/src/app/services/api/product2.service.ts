import {environment} from "../../../environments/environment";
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {catchError, Observable, throwError} from "rxjs";
import {
  AuthenticationUserDto,
  RegistrationUserDto,
  TokenDto,
  UnauthorizedResponse,
  ValidationResponse
} from "./identity.service";
import {ICreateProductDto} from "./product-api.service";


export class Product2Service {
  private baseUrl:string=environment.urlAddress+'/api';

  constructor(private httpClient:HttpClient) { }

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      "Accept": "application/json"
    }),
  };

  loginUser(authenticationUserDto: AuthenticationUserDto): Observable<TokenDto> {
    return this.httpClient.post<TokenDto>(this.baseUrl+'login', authenticationUserDto,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  registerUser(registrationUserDto: RegistrationUserDto): Observable<void> {
    return this.httpClient.post<void>(this.baseUrl, registrationUserDto,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  getCategories(): Observable<Category[]> {
    return this.httpClient.get<Category[]>(this.baseUrl+'/categories',this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  createProductForCategory(){
    return this.httpClient.get<Category[]>(this.baseUrl+'/categories',this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  getProductsForCategory(categoryId: string, pageNumber?: number,
                         pageSize?: number, maxPrice?: number,
                         minPrice?: number, searchTerm?: string,
                         orderBy?: string): Observable<ProductDto[]> {
    let url = this.baseUrl + `/categories/${categoryId}/products?`;

    if (pageSize !== undefined)
      url += "PageSize=" + + pageSize + " ";
    if (maxPrice !== undefined)
      url += "MaxPrice=" + maxPrice + " ";
    if (minPrice !== undefined)
      url += "MinPrice=" + minPrice + " ";
    if (searchTerm !== undefined)
      url += "SearchTerm=" + searchTerm + "";
    if (orderBy !== undefined)
      url += "OrderBy=" + orderBy + " ";
    //url = url.replace(/[?&]$/, "");

    return this.httpClient.get<ProductDto[]>(url,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let customError:any;
    if (error.status===422) {
      customError= error.error as ValidationResponse;
    }
    else if (error.status===401) {
      const errorMessage:string='Login or password is invalid.';
      customError = new UnauthorizedResponse(error.status,errorMessage);
    }
    else {
      customError='Something went wrong.\n Please try again later.';
    }

    return throwError(() => {
      return customError;
    });
  }
}

export class Category {
  id: string;
  categoryName: string;

  constructor(id: string, categoryName: string) {
    this.id = id;
    this.categoryName = categoryName;
  }
}

export class ProductDto {
  id: string;
  productName: string;
  serialNumber: string;
  imageUris: string[];
  price: number;
  stock: number;
  description: string;
  color: string;
  weight: number;
  size: string;

  constructor(id: string, productName: string, serialNumber: string, imageUris: string[], price: number, stock: number, description: string, color: string, weight: number, size: string) {
    this.id = id;
    this.productName = productName;
    this.serialNumber = serialNumber;
    this.imageUris = imageUris;
    this.price = price;
    this.stock = stock;
    this.description = description;
    this.color = color;
    this.weight = weight;
    this.size = size;
  }
}
export class CreateProductDto {
  productName: string;
  serialNumber: string;
  price: number;
  stock: number;
  description: string;
  color: string;
  weight: number;
  size: string;

  constructor(productName: string, serialNumber: string, price: number, stock: number, description: string, color: string, weight: number, size: string) {
    this.productName = productName;
    this.serialNumber = serialNumber;
    this.price = price;
    this.stock = stock;
    this.description = description;
    this.color = color;
    this.weight = weight;
    this.size = size;
  }
}
