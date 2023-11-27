import { Component, OnInit, Output, EventEmitter,HostBinding } from '@angular/core';
import {OverlayContainer} from "@angular/cdk/overlay";
import {FormControl} from "@angular/forms";


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  @Output() public sidenavToggle = new EventEmitter();

  toggleControl = new FormControl(false);
  @HostBinding('class') className = '';
  constructor(private overlay: OverlayContainer) { }
  ngOnInit(): void {
    this.toggleControl.valueChanges.subscribe((darkMode) => {
      const darkClassName = 'darkMode';
      this.className = darkMode ? darkClassName : '';
      this.overlay.getContainerElement().classList.add(darkClassName);
   /*   if (darkMode) {
        this.overlay.getContainerElement().classList.add(darkClassName);
      } else {
        this.overlay.getContainerElement().classList.remove(darkClassName);
      }*/
    });
  }
  public onToggleSidenav = () => {
    this.sidenavToggle.emit();
  }
}
