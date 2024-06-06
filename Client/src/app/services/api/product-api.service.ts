import {environment} from "../../../environments/environment";
import {HttpClient, HttpErrorResponse, HttpEvent, HttpHeaders, HttpRequest, HttpResponse} from "@angular/common/http";
import {catchError, Observable, throwError} from "rxjs";
import {
  AuthenticationUserDto,
  RegistrationUserDto,
  TokenDto,
  UnauthorizedResponse,
  ValidationResponse
} from "./identity.service";
import {ApiBase} from "./api-base";


export class ProductApiService extends ApiBase{

  private baseUrl:string=environment.urlAddress+'/api';

  constructor(private httpClient:HttpClient) {
    super();
  }



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



  createProductForCategory(categoryId:string,productDto: CreateProductDto): Observable<ProductDto> {
    return this.httpClient.post<ProductDto>(this.baseUrl+'/categories/'+categoryId+'/products',productDto,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  uploadImages(formData: FormData,productId:string): Observable<FileDto> {
    this.removeContentType();

    return this.httpClient.post<FileDto>(this.baseUrl+ '/products/'+productId+'/files', formData,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  getAllProducts(pageNumber?: number,
                         pageSize?: number, maxPrice?: number,
                         minPrice?: number, searchTerm?: string,
                         orderBy?: string): Observable<ProductDto[]> {
    let url = this.baseUrl + `/products?`;

    if (pageSize !== undefined)
      url += "PageSize=" + pageSize + "&";
    if (maxPrice !== undefined)
      url += "MaxPrice=" + maxPrice + "&";
    if (minPrice !== undefined)
      url += "MinPrice=" + minPrice + "&";
    if (searchTerm !== undefined)
      url += "SearchTerm=" + searchTerm + "&";
    if (orderBy !== undefined)
      url += "OrderBy=" + orderBy + "&";
    //url = url.replace(/[?&]$/, "");

    return this.httpClient.get<ProductDto[]>(url,this.httpOptions)
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

    return this.httpClient.get<ProductDto[]>(url,this.httpOptions)
      .pipe(catchError(this.handleError));
  }

  getReviewsForProduct(productId: string, pageNumber?: number,
                             pageSize?: number, maxPrice?: number,
                             minPrice?: number, orderBy?: string): Observable<HttpResponse<any>> {

    let url = this.baseUrl + `/products/${productId}/reviews?`;

    if (pageSize !== undefined)
      url += "PageSize=" + + pageSize + " ";
    if (maxPrice !== undefined)
      url += "MaxPrice=" + maxPrice + " ";
    if (minPrice !== undefined)
      url += "MinPrice=" + minPrice + " ";
    if (orderBy !== undefined)
      url += "OrderBy=" + orderBy + " ";

    return this.httpClient.get<HttpResponse<any>>(url,{observe: 'response'})
      .pipe(catchError(this.handleError));
  }


  getPricesHistoryForProduct(productId: string): Observable<PriceHistoryDto[]> {
    let url = this.baseUrl + `/products/${productId}/prices-history`;

    return this.httpClient.get<PriceHistoryDto[]>(url,this.httpOptions)
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

export class MetaData{
  currentPage:number;
  pageSize:number;
  totalCount:number;
  totalPages:number;
  hasPrevious:boolean;
  hasNext:boolean;

  constructor(currentPage: number, pageSize: number, totalCount: number, totalPages: number, hasPrevious: boolean, hasNext: boolean) {
    this.currentPage = currentPage;
    this.pageSize = pageSize;
    this.totalCount = totalCount;
    this.totalPages = totalPages;
    this.hasPrevious = hasPrevious;
    this.hasNext = hasNext;
  }
}


export class ReviewDto{
  id:string;
  userName:string;
  description:string;
  rating:number;
  reviewDate:string;

  constructor(id: string, userName: string, description: string, rating: number, reviewDate: string) {
    this.id = id;
    this.userName = userName;
    this.description = description;
    this.rating = rating;
    this.reviewDate = reviewDate;
  }
}

export class PriceHistoryDto {
  id: string;
  startDate: string;
  endDate: string;
  priceValue: number;

  constructor(id: string, startDate: string, endDate: string, priceValue: number) {
    this.id = id;
    this.startDate = startDate;
    this.endDate = endDate;
    this.priceValue = priceValue;
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

export class FileDto {
  totalFilesUploaded:number;
  totalSizeUploaded:string;
  fileNames:string[];
  notUploadedFiles:string[];

  constructor(totalFilesUploaded: number, totalSizeUploaded: string, fileNames: string[], notUploadedFiles: string[]) {
    this.totalFilesUploaded = totalFilesUploaded;
    this.totalSizeUploaded = totalSizeUploaded;
    this.fileNames = fileNames;
    this.notUploadedFiles = notUploadedFiles;
  }
}
