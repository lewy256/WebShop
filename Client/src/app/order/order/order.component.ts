import { Component } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
interface Food {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})

export class OrderComponent {
  addressFormGroup = this._formBuilder.group({
    firstCtrl: ['', Validators.required],
  });
  paymentFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
  basketFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
  firstFormGroup = this._formBuilder.group({
    firstCtrl: ['', Validators.required],
  });
  secondFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
  foods: Food[] = [
    {value: 'steak-0', viewValue: 'Steak'},
    {value: 'pizza-1', viewValue: 'Pizza'},
    {value: 'tacos-2', viewValue: 'Tacos'},
  ];
  constructor(private _formBuilder: FormBuilder) {}
}
