import {Component, EventEmitter, HostBinding, OnInit, Output} from '@angular/core';
import {FormControl} from "@angular/forms";
import {OverlayContainer} from "@angular/cdk/overlay";
import {Observable, startWith} from "rxjs";
import {map} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";
import {CategoryDto, ProductApiService} from "../services/product.api.service";
import {environment} from "../product/environment/environment";
import {SharedService} from "../services/shared.service";
interface User {
  name: string;
}
@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})

export class LayoutComponent implements OnInit {
 // @Output() public sidenavToggle = new EventEmitter();

  toggleControl = new FormControl(false);
  @HostBinding('class') className = '';



  private api: ProductApiService;

  constructor(private overlay: OverlayContainer,private http: HttpClient, private sharedService:SharedService) {
    this.api=new ProductApiService(this.http,environment.urlAddress);
  }

  public setData(category:string){
    this.sharedService.setData(category);
  }

  public setFilterData(data:string){
    this.sharedService.setFilterData(data);
  }

  clickMenu() {
    this.sharedService.toggle();
  }


  categories:CategoryDto[]= [];

  public getCategories(){
    this.api.getCategories()
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
  ngOnInit(): void {
    this.getCategories();
 /*   if(localStorage.getItem('mode') === 'darkMode'){
      const darkClassName = 'darkMode';
      this.className =  'darkMode'
      this.overlay.getContainerElement().classList.add(darkClassName);
    }*/

    this.toggleControl.valueChanges.subscribe((darkMode) => {
      const darkClassName = 'darkMode';


      this.className = darkMode ? darkClassName : '';
      if (darkMode) {
        this.overlay.getContainerElement().classList.add(darkClassName);
        //localStorage.setItem('mode', 'darkMode')
      } else {
        this.overlay.getContainerElement().classList.remove(darkClassName);
      }
    });
    this.filteredOptions = this.myControl.valueChanges.pipe(
      startWith(''),
      map(value => {
        const name = typeof value === 'string' ? value : value?.name;
        return name ? this._filter(name as string) : this.options.slice();
      }),
    );
  }
/*  public onToggleSidenav = () => {
    this.sidenavToggle.emit();
  }*/

}
