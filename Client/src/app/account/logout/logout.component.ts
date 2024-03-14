import { Component } from '@angular/core';
import {LoginService} from "../../services/shared/login.service";

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent {
  constructor(private loginService:LoginService) {
    this.loginService.clearData();
  }
}
