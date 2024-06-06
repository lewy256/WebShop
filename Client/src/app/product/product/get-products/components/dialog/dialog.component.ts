import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from "@angular/material/dialog";
import {NgForOf, NgOptimizedImage} from "@angular/common";
import {MaterialModule} from "../../../../../material/material.module";

export interface DialogData {
  images:string[];
}

@Component({
  selector: 'app-dialog',
  templateUrl: './dialog.component.html',
  standalone: true,
  imports: [
    MaterialModule,
    NgOptimizedImage,
    MatDialogModule,
    NgForOf
  ],
})

export class DialogComponent {
  constructor(
    public dialogRef: MatDialogRef<DialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
  ) {}

  onClose(): void {
    this.dialogRef.close();
  }
}
