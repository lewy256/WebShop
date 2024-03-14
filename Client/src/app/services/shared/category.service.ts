import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class CategoryService {
  private categoryId: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private searchQuery: BehaviorSubject<string> = new BehaviorSubject<string>('');

  getCategoryId(): Observable<string> {
    return this.categoryId as Observable<string>;
  }

  setCategoryId(categoryId:string):void{
    this.categoryId.next(categoryId);
  }

  setQuery(data:string): void {
    this.searchQuery.next(data);
  }

  getQuery():string{
    let query:string='';
    this.searchQuery.subscribe(x=>query=x);
    return query
  }

}
