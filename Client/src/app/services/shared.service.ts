import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private dataSubject = new BehaviorSubject<string>('');
  public sideNavToggleSubject = new BehaviorSubject<any>(null);
  private dataSubject3 = new BehaviorSubject<string>('');
  data$ = this.dataSubject.asObservable();
  filterData$=this.dataSubject3.asObservable();

  setFilterData(data:string){
    this.dataSubject3.next(data);
  }

  setData(data: string): void {
    this.dataSubject.next(data);
  }

  public toggle() {
    return this.sideNavToggleSubject.next(null);
  }
  constructor() { }
}
