import {Component, EventEmitter, HostBinding, OnInit, Output} from '@angular/core';
import {FormControl} from "@angular/forms";
import {OverlayContainer} from "@angular/cdk/overlay";
import {Observable, startWith} from "rxjs";
import {map} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";
import {Category, ProductApiService} from "../services/product-api.service";
import {environment} from "../../environments/environment";
import {SharedService} from "../services/shared.service";
import {MatSlideToggleChange} from "@angular/material/slide-toggle";

interface User {
  name: string;
}
@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})

export class LayoutComponent implements OnInit {
  private productService: ProductApiService;

  constructor(private overlay: OverlayContainer,private http: HttpClient, private sharedService:SharedService) {
    this.productService=new ProductApiService(this.http,environment.urlAddress);
  }

  setData(category:string){
    this.sharedService.setData(category);
  }

  setFilterData(data:string){
    this.sharedService.setFilterData(data);
  }

  clickMenu() {
    this.sharedService.toggle();
  }


  categories:Category[]= [];

  getCategories(){
    this.productService.getCategories()
      .subscribe(res=>{
        this.categories=res
      })
    // this.sendCategories.emit(this.categories);
  }

  myControl = new FormControl<string | User>('');
  options: User[] = [{name: 'Mary'}, {name: 'Shelley'}, {name: 'Igor'}];
  filteredOptions: Observable<User[]> | undefined;


  displayFn(user: User): string {
    return user && user.name ? user.name : '';
  }

  private _filter(name: string): User[] {
    const filterValue = name.toLowerCase();

    return this.options.filter(option => option.name.toLowerCase().includes(filterValue));
  }

  @Output() sidenavToggle = new EventEmitter();
  toggleControl = new FormControl(false);
  @HostBinding('class') className = '';

  private setTheme():void{
    const darkMode:string = 'darkMode';
    let savedMode:string=localStorage.getItem('mode') as string;

    if(savedMode===darkMode){
      this.overlay.getContainerElement().classList.add(darkMode);
      this.className=darkMode;
      this.toggleControl.setValue(true);
    }

    this.toggleControl.valueChanges.subscribe((isActive:boolean|null):void => {
      this.className = isActive ? darkMode : '';
      if (isActive) {
        this.overlay.getContainerElement().classList.add(darkMode);
        localStorage.setItem('mode', 'darkMode');
      } else{
        this.overlay.getContainerElement().classList.remove(darkMode);
        localStorage.setItem('mode', 'none');
      }
    });

  }

  ngOnInit(): void {
    this.setTheme();
    this.getCategories();

    this.filteredOptions = this.myControl.valueChanges.pipe(
      startWith(''),
      map(value => {
        const name = typeof value === 'string' ? value : value?.name;
        return name ? this._filter(name as string) : this.options.slice();
      }),
    );
  }

}
