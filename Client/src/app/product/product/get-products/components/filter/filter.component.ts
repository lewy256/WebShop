import {Component, EventEmitter, Input, Output} from '@angular/core';
import {MatCardModule} from "@angular/material/card";
import {MatIconModule} from "@angular/material/icon";
import {MatInputModule} from "@angular/material/input";
import {MatChipsModule} from "@angular/material/chips";
import {MatButtonToggleModule} from "@angular/material/button-toggle";
import {MatButtonModule} from "@angular/material/button";
import {ReactiveFormsModule} from "@angular/forms";
import {RouterLink} from "@angular/router";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatRadioModule} from "@angular/material/radio";
import {MatSelectModule} from "@angular/material/select";

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss'],
  standalone: true,
  imports: [
    MatCardModule,
    MatIconModule,
    MatInputModule,
    MatChipsModule,
    MatButtonToggleModule,
    MatButtonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCheckboxModule,
    MatRadioModule,
    MatSelectModule
  ]
})

export class FilterComponent {
  @Output() isFilterClose = new EventEmitter<boolean>();

  closeFilter():void{
    this.isFilterClose.emit(false);
  }
}
