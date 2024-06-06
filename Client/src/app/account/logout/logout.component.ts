import { Component } from '@angular/core';
import {LoginSharedService} from "../../services/shared/login-shared.service";

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss']
})
export class LogoutComponent {
  constructor(private loginService:LoginSharedService) {
    this.loginService.clearData();
  }
}
