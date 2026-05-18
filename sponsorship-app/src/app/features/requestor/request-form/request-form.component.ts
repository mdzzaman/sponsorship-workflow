import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipTypeDto } from '../../../core/models/sponsorship.model';

@Component({
    selector: 'app-request-form',
    imports: [
        CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule,
        MatSelectModule, MatDatepickerModule, MatNativeDateModule, MatButtonModule,
        MatCardModule, MatSnackBarModule, MatProgressSpinnerModule
    ],
    template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{isEdit ? 'Edit' : 'New'}} Sponsorship Request</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="onSave(false)" class="form-grid">
          <mat-form-field appearance="outline">
            <mat-label>Request Title *</mat-label>
            <input matInput formControlName="title">
            <mat-error>Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Department *</mat-label>
            <input matInput formControlName="department">
            <mat-error>Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Sponsorship Type *</mat-label>
            <mat-select formControlName="sponsorshipTypeId">
              <mat-option *ngFor="let t of types" [value]="t.id">{{t.name}}</mat-option>
            </mat-select>
            <mat-error>Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Event / Organisation Name *</mat-label>
            <input matInput formControlName="eventName">
            <mat-error>Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Event Date *</mat-label>
            <input matInput [matDatepicker]="picker" formControlName="eventDate">
            <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
            <mat-datepicker #picker></mat-datepicker>
            <mat-error>Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Requested Amount (MYR) *</mat-label>
            <input matInput type="number" formControlName="requestedAmount">
            <mat-error>Required, must be > 0</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline" class="full-span">
            <mat-label>Purpose / Justification *</mat-label>
            <textarea matInput rows="3" formControlName="justification"></textarea>
            <mat-error>Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline" class="full-span">
            <mat-label>Expected Business Benefit</mat-label>
            <textarea matInput rows="2" formControlName="expectedBenefit"></textarea>
          </mat-form-field>
          <mat-form-field appearance="outline" class="full-span">
            <mat-label>Remarks</mat-label>
            <textarea matInput rows="2" formControlName="remarks"></textarea>
          </mat-form-field>
        </form>
      </mat-card-content>
      <mat-card-actions align="end">
        <button mat-button type="button" (click)="goBack()">Cancel</button>
        <button mat-stroked-button type="button" (click)="onSave(false)" [disabled]="loading">Save as Draft</button>
        <button mat-raised-button color="primary" type="button" (click)="onSave(true)" [disabled]="loading">Save & Submit</button>
      </mat-card-actions>
    </mat-card>
  `,
    styles: [`
    mat-card { max-width:900px; margin:0 auto; }
    .form-grid { display:grid; grid-template-columns:1fr 1fr; gap:16px; margin-top:16px; }
    .full-span { grid-column:1/-1; }
    mat-form-field { width:100%; }
  `]
})
export class RequestFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private svc = inject(SponsorshipService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private snack = inject(MatSnackBar);

  form = this.fb.group({
    title: ['', Validators.required],
    department: ['', Validators.required],
    sponsorshipTypeId: ['', Validators.required],
    eventName: ['', Validators.required],
    eventDate: [null as Date | null, Validators.required],
    requestedAmount: [null as number | null, [Validators.required, Validators.min(1)]],
    justification: ['', Validators.required],
    expectedBenefit: [''],
    remarks: ['']
  });
  types: SponsorshipTypeDto[] = [];
  loading = false;
  isEdit = false;
  requestId?: string;

  ngOnInit() {
    this.svc.getTypes().subscribe(t => this.types = t);
    this.requestId = this.route.snapshot.paramMap.get('id') ?? undefined;
    if (this.requestId) {
      this.isEdit = true;
      this.svc.getById(this.requestId).subscribe(r => {
        this.form.patchValue({
          ...r,
          eventDate: new Date(r.eventDate)
        } as any);
      });
    }
  }

  onSave(submit: boolean) {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    const dto = { ...this.form.value, eventDate: this.form.value.eventDate?.toISOString() };
    const action = this.requestId
      ? this.svc.update(this.requestId, dto as any)
      : this.svc.create(dto as any);

    action.subscribe({
      next: (res: any) => {
        if (submit) {
          this.svc.submit(res.id).subscribe({
            next: () => { this.snack.open('Submitted!', '', { duration: 2000 }); this.router.navigate(['/requestor']); },
            error: () => this.handleError()
          });
        } else {
          this.snack.open('Saved as draft', '', { duration: 2000 });
          this.router.navigate(['/requestor']);
        }
      },
      error: () => this.handleError()
    });
  }

  private handleError() {
    this.loading = false;
    this.snack.open('An error occurred', 'Close', { duration: 3000 });
  }

  goBack() { this.router.navigate(['/requestor']); }
}
