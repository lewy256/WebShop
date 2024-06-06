import { Component } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
interface Food {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-ship-method',
  templateUrl: './ship-method.component.html',
  styleUrls: ['./ship-method.component.scss']
})
export class ShipMethodComponent {
  constructor(private _formBuilder: FormBuilder) {
  }
  shipFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
  foods: Food[] = [
    {value: 'steak-0', viewValue: 'Steak'},
    {value: 'pizza-1', viewValue: 'Pizza'},
    {value: 'tacos-2', viewValue: 'Tacos'},
  ];
}
