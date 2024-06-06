import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {finalize, Observable} from "rxjs";
import {HttpClient, HttpEvent, HttpEventType, HttpRequest, HttpResponse} from "@angular/common/http";
import {LoginSharedService} from "../../../services/shared/login-shared.service";
import {ErrorMappingService} from "../../../services/shared/error-mapping.service";
import {Category, CreateProductDto, FileDto, ProductApiService} from "../../../services/api/product-api.service";

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.scss']
})

export class CreateProductComponent implements OnInit {
  private productService: ProductApiService;
  message:string="";
  isDataLoaded: boolean=false;

  constructor(private httpClient: HttpClient,
              private formBuilder: FormBuilder,
              private loginService: LoginSharedService,
              private errorMappingService:ErrorMappingService) {
    this.productService = new ProductApiService(this.httpClient);
  }

  productForm:FormGroup = this.formBuilder.group({
    productName: ['pc', [Validators.required]],
    serialNumber: ['123345', [Validators.required]],
    price: ['44.44', [Validators.required]],
    stock: ['44', [Validators.required]],
    description: ['good pc', [Validators.required]],
    color: ['black', [Validators.required]],
    weight: ['23', [Validators.required]],
    size: ['21', [Validators.required]],
    category: ['', Validators.required],
    updateOn: 'blur',
  });


  createProduct(): void {
    const token:string=this.loginService.getToken();

    if(token){
      this.isDataLoaded = true
      this.productService.setAuthToken(token);

      this.productService.createProductForCategory(
        this.productForm.value.category,
        new CreateProductDto(
          this.productForm.value.productName,
          this.productForm.value.serialNumber,
          this.productForm.value.price,
          this.productForm.value.stock,
          this.productForm.value.description,
          this.productForm.value.color,
          this.productForm.value.weight,
          this.productForm.value.size
        )
        )
        .pipe(finalize(() => this.isDataLoaded = false))
        .subscribe({
          next:(x) => {
            this.message+="Product created successfully.";
            if(this.selectedFiles.length>0){
              this.upload(x.id);
            }

            this.productForm.reset();
          },
          error:(x)=>{
            if(x.statusCode===422){
              this.errorMappingService.MappingValidationError(x.errors,this.productForm);
            } else if(x.statusCode===401){
              this.message=x.message;
            } else{
              this.message=x;
            }
          }
        });
    }
    else{
      this.message="Please sign in.";
    }
  }

  categories: Category[]=[];

  ngOnInit(): void {
    this.productService.getCategories()
      .subscribe({
        next:(x) => {
          this.categories=x;
        },
        error:()=>{

        }
      });
  }

  selectedFiles: File[]=[];

  onFileSelected(event:any): void {
    const fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      for (let i = 0; i < fileList.length; i++) {
        this.selectedFiles.push(fileList[i]);
      }
    }
  }

  upload(productId:string): void {
    const formData = new FormData();

    for (let i = 0; i < this.selectedFiles.length; i++) {
      formData.append('files', this.selectedFiles[i], this.selectedFiles[i].name);
    }

    this.productService.uploadImages(formData,productId)
      .subscribe({
        next:(x:FileDto) => {
          this.message+=` Files uploaded successfully: ${x.totalFilesUploaded}.`;

          if(x.notUploadedFiles.length>0){
            for (let i=0;i<x.notUploadedFiles.length;i++){
              this.message+="Files not uploaded successfully: "+x.notUploadedFiles[i];
            }
          }

          this.selectedFiles=[];
        },
        error:()=>{

        }
      });
  }
}


