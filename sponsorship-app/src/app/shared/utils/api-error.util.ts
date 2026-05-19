import { HttpErrorResponse } from '@angular/common/http';
import { AbstractControl, FormGroup } from '@angular/forms';

export function extractApiError(err: HttpErrorResponse): string {
  if (err.error?.errors) {
    const first = Object.values(err.error.errors as Record<string, string[]>)[0];
    return Array.isArray(first) ? first[0] : String(first);
  }
  return err.error?.error ?? 'An unexpected error occurred. Please try again.';
}

export function applyServerValidationErrors(err: HttpErrorResponse, form: FormGroup): string | null {
  if (err.status === 400 && err.error?.errors) {
    const errors = err.error.errors as Record<string, string[]>;
    let matched = false;
    for (const [key, messages] of Object.entries(errors)) {
      const controlName = key.charAt(0).toLowerCase() + key.slice(1);
      const control: AbstractControl | null = form.get(controlName);
      if (control) {
        control.setErrors({ serverError: messages[0] });
        control.markAsTouched();
        matched = true;
      }
    }
    if (!matched) {
      return extractApiError(err);
    }
    return null;
  }
  return extractApiError(err);
}
