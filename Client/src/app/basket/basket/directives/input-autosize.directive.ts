import {AfterViewInit, Directive, ElementRef, HostListener, Input, OnInit, Renderer2} from '@angular/core';

@Directive({
  selector: '[appInputAutosize]'
})

export class InputAutosizeDirective implements AfterViewInit{

  constructor(private el: ElementRef, private ren: Renderer2) {

  }

  @Input() minInputWidth: number = 30;

  @HostListener('input', ['$event.target.value'])
  onInput(value: number): void {
    if(value<1){
      this.el.nativeElement.value = 1;
    } else{
      const inputWidth: number = value.toString().length*20;
      this.el.nativeElement.style.width = `${Math.max(this.minInputWidth, inputWidth)}px`;
    }
  }

  ngAfterViewInit(): void {
    this.el.nativeElement.style.width=`${this.minInputWidth}px`;
    this.ren.setStyle(this.el.nativeElement, 'border', '0');
    this.ren.setStyle(this.el.nativeElement, 'outline', 'none');
  }


}
