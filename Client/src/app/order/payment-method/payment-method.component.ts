import { Component } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
interface Food {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-payment-method',
  templateUrl: './payment-method.component.html',
  styleUrls: ['./payment-method.component.scss']
})
export class PaymentMethodComponent {
  constructor(private _formBuilder: FormBuilder) {
  }
  paymentFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
  foods: Food[] = [
    {value: 'steak-0', viewValue: 'Steak'},
    {value: 'pizza-1', viewValue: 'Pizza'},
    {value: 'tacos-2', viewValue: 'Tacos'},
  ];
}
