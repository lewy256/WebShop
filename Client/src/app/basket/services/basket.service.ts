/*import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../models/environment';
import { catchError, map, tap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BasketService {

  constructor(private http: HttpClient) { }

  public getBasketById = (route: string) => {
    return this.http
    .get(this.createCompleteRoute(route, environment.urlAddress))
    .pipe(catchError(this.handleError<Basket>(`getBasketById id=${route}`)));;
  }
 
  public create = (route: string, body: any) => {
    return this.http.post(this.createCompleteRoute(route, environment.urlAddress), body, this.generateHeaders());
  }
 
  public update = (route: string, body: any) => {
    return this.http.put(this.createCompleteRoute(route, environment.urlAddress), body, this.generateHeaders());
  }
 
  public delete = (route: string) => {
    return this.http.delete(this.createCompleteRoute(route, environment.urlAddress));
  }
 
  private createCompleteRoute = (route: string, envAddress: string) => {
    return `${envAddress}/${route}`;
  }
 
  private generateHeaders = () => {
    return {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    }
  }
}*/

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../models/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Basket } from '../interfaces/Basket';

@Injectable({ providedIn: 'root' })

export class BasketService {
  constructor( private http: HttpClient) { }

  getBasketById(id: string): Observable<Basket> {
    const url = `${environment.urlAddress}/api/basket/${id}`;

    return this.http.get<Basket>(url).pipe(
      tap(_ => console.log(`fetched basket id=${id}`)),
      catchError(this.handleError<Basket>(`getBasket id=${id}`))
    );
  }

  addBasket(basket: Basket): Observable<Basket> {
    const url = `${environment.urlAddress}`;

    return this.http.post<Basket>(url, basket, this.generateHeaders()).pipe(
      tap((newBasket: Basket) => console.log(`added basket w/ id=${newBasket.id}`)),
      catchError(this.handleError<Basket>('addbasket'))
    );
  }

  deleteBasket(id: string): Observable<Basket> {
    const url = `${environment.urlAddress}/${id}`;

    return this.http.delete<Basket>(url, this.generateHeaders()).pipe(
      tap(_ => console.log(`deleted basket id=${id}`)),
      catchError(this.handleError<Basket>('deletebasket'))
    );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.log(`${operation} failed: ${error.message}`);

      return of(result as T);
    };
  }

  private generateHeaders = () => {
    return {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    }
  }
}

