import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class LayoutSharedService {
  private darkTheme: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  checkDarkTheme(): Observable<boolean>{
    return this.darkTheme as Observable<boolean>;
  }

  setDarkTheme(darkTheme:boolean):void{
    this.darkTheme.next(darkTheme);
  }

}
