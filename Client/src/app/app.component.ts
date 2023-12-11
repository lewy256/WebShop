import {Component, HostBinding} from '@angular/core';
import {FormControl} from "@angular/forms";
import {OverlayContainer} from "@angular/cdk/overlay";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = 'WebShop';

/*  events: string[] = [];
  opened: any;

  fillerNav = ["Queries", "Events", "Disks", "Memory"];
  toggleControl = new FormControl(false);
  @HostBinding('class') className = '';


  ngOnInit(): void {
    this.toggleControl.valueChanges.subscribe((darkMode) => {
      const darkClassName = 'darkMode darkChart';
      this.className = darkMode ? darkClassName: '';


      if (darkMode) {
        this.overlay.getContainerElement().classList.add(darkClassName);
      } else {
        this.overlay.getContainerElement().classList.remove(darkClassName);
      }
    });




  }*/
 /* constructor(private overlay: OverlayContainer) {
    if (localStorage.getItem('hideButton') === null) {
      this.hideButton = true;
      localStorage.setItem('hideButton', 'true');
    }
  }

  hideButton = localStorage.getItem('hideButton') === 'true';
  hideButtonFunction(hide:boolean) {
    this.hideButton= hide;
    localStorage.setItem('hideButton', hide.toString());
  }*/
}
