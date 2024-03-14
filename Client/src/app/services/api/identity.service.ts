import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {catchError, Observable, throwError} from "rxjs";
import {environment} from "../../../environments/environment";

export class IdentityService {
  private baseUrl:string=environment.urlAddress+'/api/identity/';

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

export class RegistrationUserDto {
  firstName: string;
  lastName: string;
  userName: string;
  password: string;
  email: string;
  phoneNumber: string;
  roles: string[];

  constructor(firstName: string, lastName: string, userName: string, password: string, email: string, phoneNumber: string) {
    this.firstName = firstName;
    this.lastName = lastName;
    this.userName = userName;
    this.password = password;
    this.email = email;
    this.phoneNumber = phoneNumber;
    this.roles = ['Customer'];
  }

}

export class TokenDto {
  accessToken?: string;
  refreshToken?: string;
}

export class AuthenticationUserDto {
  userName: string;
  password: string;
  constructor(userName:string,password:string) {
    this.userName=userName;
    this.password=password;
  }
}

export class ValidationError{
  propertyName?:string;
  errorMessage?:string;
}
export abstract class ApiBaseResponse{
  statusCode:number;
  message:string;

  constructor(statusCode:number,message:string) {
    this.statusCode=statusCode;
    this.message=message;

  }


}

export class UnauthorizedResponse extends ApiBaseResponse{
}

export class ValidationResponse extends ApiBaseResponse{
  errors?:ValidationError[];
  constructor(statusCode:number,message:string,errors:ValidationError[]) {
    super(statusCode,message);
    this.errors=errors;

  }

}
