import { Component } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";

@Component({
  selector: 'app-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.scss']
})
export class AddressComponent {
  constructor(private _formBuilder: FormBuilder) {
  }
  addressFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
}
