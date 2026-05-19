import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { of, switchMap } from 'rxjs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { CreateRequestDto, SponsorshipTypeDto } from '../../../core/models/sponsorship.model';
import { applyServerValidationErrors } from '../../../shared/utils/api-error.util';

function futureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const selected = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return selected > today ? null : { futureDate: true };
  };
}

@Component({
  selector: 'app-request-form',
  imports: [
    ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatDatepickerModule, MatNativeDateModule, MatButtonModule, MatCardModule,
    MatSnackBarModule, MatProgressSpinnerModule, ShellComponent
  ],
  templateUrl: './request-form.component.html',
  styleUrl: './request-form.component.scss'
})
export class RequestFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly svc = inject(SponsorshipService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snack = inject(MatSnackBar);

  navItems: NavItem[] = [
    { label: 'My Requests', route: '/requestor', icon: 'list' }
  ];

  form = this.fb.group({
    title:             ['', [Validators.required, Validators.maxLength(200)]],
    department:        ['', [Validators.required, Validators.maxLength(100)]],
    sponsorshipTypeId: ['', Validators.required],
    eventName:         ['', [Validators.required, Validators.maxLength(200)]],
    eventDate:         [null as Date | null, [Validators.required, futureDateValidator()]],
    requestedAmount:   [null as number | null, [Validators.required, Validators.min(0.01)]],
    justification:     ['', [Validators.required, Validators.minLength(20), Validators.maxLength(2000)]],
    expectedBenefit:   ['', Validators.maxLength(2000)],
    remarks:           ['', Validators.maxLength(1000)]
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
          title: r.title,
          department: r.department,
          sponsorshipTypeId: r.sponsorshipTypeId,
          eventName: r.eventName,
          eventDate: new Date(r.eventDate),
          requestedAmount: r.requestedAmount,
          justification: r.justification,
          expectedBenefit: r.expectedBenefit,
          remarks: r.remarks
        });
      });
    }
  }

  getError(controlName: string): string {
    const ctrl = this.form.get(controlName);
    if (!ctrl?.errors || !ctrl.touched) return '';
    const e = ctrl.errors;
    if (e['required'])    return 'This field is required.';
    if (e['minlength'])   return `Minimum ${e['minlength'].requiredLength} characters required.`;
    if (e['maxlength'])   return `Maximum ${e['maxlength'].requiredLength} characters allowed.`;
    if (e['min'])         return 'Amount must be greater than zero.';
    if (e['futureDate'])  return 'Event date must be in the future.';
    if (e['serverError']) return e['serverError'];
    return '';
  }

  charCount(controlName: string): number {
    return (this.form.get(controlName)?.value as string)?.length ?? 0;
  }

  onSave(submit: boolean) {
    this.clearServerErrors();
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.loading = true;
    const raw = this.form.getRawValue();
    const dto = {
      title: raw.title!,
      department: raw.department!,
      sponsorshipTypeId: raw.sponsorshipTypeId!,
      eventName: raw.eventName!,
      eventDate: raw.eventDate!.toISOString(),
      requestedAmount: raw.requestedAmount!,
      justification: raw.justification!,
      expectedBenefit: raw.expectedBenefit || null,
      remarks: raw.remarks || null
    } satisfies CreateRequestDto;

    const save$ = this.requestId ? this.svc.update(this.requestId, dto) : this.svc.create(dto);

    save$.pipe(
      switchMap(res => submit ? this.svc.submit(res.id) : of(void 0))
    ).subscribe({
      next: () => {
        this.snack.open(submit ? 'Submitted!' : 'Saved as draft', '', { duration: 2000 });
        this.router.navigate(['/requestor']);
      },
      error: (err: HttpErrorResponse) => this.handleApiError(err)
    });
  }

  goBack() { this.router.navigate(['/requestor']); }

  private handleApiError(err: HttpErrorResponse) {
    this.loading = false;
    const fallback = applyServerValidationErrors(err, this.form);
    if (fallback) this.snack.open(fallback, 'Close', { duration: 5000 });
  }

  private clearServerErrors() {
    Object.keys(this.form.controls).forEach(key => {
      const ctrl = this.form.get(key)!;
      if (ctrl.errors?.['serverError']) {
        const { serverError, ...rest } = ctrl.errors;
        ctrl.setErrors(Object.keys(rest).length ? rest : null);
      }
    });
  }
}
