import { TestBed } from '@angular/core/testing';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import { BasketService } from './basket.service';
import { environment } from '../models/environment';
import { mockGetBasket, mockPostBasket } from '../mocks/mockBasket';


describe('BasketService', () => {
  let service: BasketService;
  let httpController: HttpTestingController;

 const url = `${environment.urlAddress}/api`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(BasketService);
    httpController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });


  it('should call getBasketById and return the appropriate Basket', () => {
    const id = '3fa85f64-5717-4562-b3fc-2c963f66afa6';

    service.getBasketById(id).subscribe((data) => {
      expect(data).toEqual(mockGetBasket);
    });


    const req = httpController.expectOne({
      method: 'GET',
      url: `${url}/basket/${id}`,
    });

    req.flush(mockGetBasket);
  });

/*   it('should call delete Basket and return the basket that was deleted from the API', () => {
    service.deleteBasket(mockBasket.id).subscribe((data) => {
      expect(data).toEqual(mockBasket);
    });

    const req = httpController.expectOne({
      method: 'DELETE',
      url: `${url}/basket/${mockBasket.id}`,
    });

    req.flush(mockBasket);
  });*/

/*  it('should call addBasket and the API should return the basket that was added', () => {
    service.addBasket(mockPostBasket).subscribe((data) => {
      expect(data).toEqual(mockPostBasket);
    });

    const req = httpController.expectOne({
      method: 'POST',
      url: `${url}/basket`,
    });

    req.flush(mockPostBasket);
  });*/

 
});
