import { Component } from '@angular/core';
import {FormControl, Validators} from "@angular/forms";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  hide = true;
  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  roles: any= [
    {value: 'Administrator', viewValue: 'Administrator'},
    {value: 'Customer', viewValue: 'Customer'}
  ];
}
