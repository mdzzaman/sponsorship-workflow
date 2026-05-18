import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-remark-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{data.title}}</h2>
    <mat-dialog-content>
      <mat-form-field appearance="outline" style="width:100%;margin-top:8px">
        <mat-label>Remarks {{data.required ? '*' : '(optional)'}}</mat-label>
        <textarea matInput rows="3" [formControl]="remarks"></textarea>
        <mat-error *ngIf="remarks.hasError('required')">Remarks are required for rejection</mat-error>
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" (click)="confirm()">Confirm</button>
    </mat-dialog-actions>
  `
})
export class RemarkDialogComponent {
  remarks: FormControl;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { title: string; required: boolean },
    private dialogRef: MatDialogRef<RemarkDialogComponent>
  ) {
    this.remarks = new FormControl('', data.required ? Validators.required : []);
  }

  confirm() {
    if (this.remarks.invalid) { this.remarks.markAsTouched(); return; }
    this.dialogRef.close(this.remarks.value || null);
  }
}
