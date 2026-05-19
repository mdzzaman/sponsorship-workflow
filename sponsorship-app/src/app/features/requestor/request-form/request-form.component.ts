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
  templateUrl: './request-form.component.html',
  styleUrl: './request-form.component.scss'
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
        this.form.patchValue({ ...r, eventDate: new Date(r.eventDate) } as any);
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

  goBack() { this.router.navigate(['/requestor']); }

  private handleError() {
    this.loading = false;
    this.snack.open('An error occurred', 'Close', { duration: 3000 });
  }
}
