import {HttpHeaders} from "@angular/common/http";

export class ApiBase {

  protected httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      "Accept": "application/json",
    }),
  };

  setAuthToken(token: string):void {
    this.httpOptions={headers: this.httpOptions.headers.append('Authorization', `Bearer ${token}`)};

  }

  setContentType(type:string):void{
    this.httpOptions={headers: this.httpOptions.headers.set('Content-Type', type)};
  }

  removeContentType():void{
    this.httpOptions={headers: this.httpOptions.headers.delete('Content-Type')};
  }
}
