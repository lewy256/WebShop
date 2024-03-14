import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class LoginService {
  private token: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private userName: BehaviorSubject<string> = new BehaviorSubject<string>('');

  setUserName(userName:string):void{
    this.userName.next(userName)
  }
  getUserName():string{
    let userName:string='';
    this.userName.subscribe(x=>userName=x);
    return userName;
  }

  getToken(): string {
    let tokenToReturn:string='';
    this.token.subscribe(x=>tokenToReturn=x);
    return tokenToReturn;
    //return this.token as Observable<string>;
  }

  setToken(token:string):void{
    this.token.next(token);
  }

  clearData():void{
    this.token.next('');
    this.userName.next('')
  }

}
