import { Component, Inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-remark-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './remark-dialog.component.html',
  styleUrl: './remark-dialog.component.scss'
})
export class RemarkDialogComponent {
  remarks: FormControl;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { title: string; required: boolean },
    private dialogRef: MatDialogRef<RemarkDialogComponent>
  ) {
    this.remarks = new FormControl(
      '',
      data.required
        ? [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]
        : [Validators.maxLength(1000)]
    );
  }

  getError(): string {
    const e = this.remarks.errors;
    if (!e) return '';
    if (e['required'])   return 'Remarks are required for rejection.';
    if (e['minlength'])  return `Minimum ${e['minlength'].requiredLength} characters required.`;
    if (e['maxlength'])  return `Maximum ${e['maxlength'].requiredLength} characters allowed.`;
    return '';
  }

  confirm() {
    if (this.remarks.invalid) { this.remarks.markAsTouched(); return; }
    this.dialogRef.close(this.remarks.value || null);
  }
}
