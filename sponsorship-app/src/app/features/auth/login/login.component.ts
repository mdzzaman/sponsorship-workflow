import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatProgressSpinnerModule, MatSnackBarModule
  ],
  template: `
    <div class="login-container">
      <mat-card class="login-card">
        <mat-card-header>
          <mat-card-title>Sponsorship Workflow</mat-card-title>
          <mat-card-subtitle>Sign in to your account</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" type="email" placeholder="you@example.com">
              <mat-error *ngIf="form.get('email')?.hasError('required')">Email is required</mat-error>
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput formControlName="password" type="password">
              <mat-error *ngIf="form.get('password')?.hasError('required')">Password is required</mat-error>
            </mat-form-field>
            <button mat-raised-button color="primary" type="submit" [disabled]="loading" class="full-width">
              <mat-spinner diameter="20" *ngIf="loading"></mat-spinner>
              <span *ngIf="!loading">Sign In</span>
            </button>
          </form>
        </mat-card-content>
        <mat-card-footer>
          <div class="test-accounts">
            <small><strong>Test accounts:</strong> requestor&#64;test.com / manager&#64;test.com / finance&#64;test.com / admin&#64;test.com (password: Test&#64;123)</small>
          </div>
        </mat-card-footer>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-container { display:flex; justify-content:center; align-items:center; min-height:100vh; background:#f5f5f5; }
    .login-card { width:400px; padding:16px; }
    .full-width { width:100%; margin-bottom:16px; }
    button { margin-top:8px; }
    .test-accounts { padding:8px 16px; background:#f9f9f9; border-top:1px solid #eee; font-size:12px; color:#666; }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private snack = inject(MatSnackBar);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });
  loading = false;

  onSubmit() {
    if (this.form.invalid) return;
    this.loading = true;
    this.auth.login({ email: this.form.value.email!, password: this.form.value.password! }).subscribe({
      next: () => this.redirectByRole(),
      error: () => {
        this.loading = false;
        this.snack.open('Invalid email or password', 'Close', { duration: 3000 });
      }
    });
  }

  private redirectByRole() {
    const role = this.auth.currentUser()?.role;
    const routes: Record<string, string> = {
      Requestor: '/requestor', Manager: '/manager',
      FinanceAdmin: '/finance', SystemAdmin: '/admin'
    };
    this.router.navigate([routes[role!] ?? '/login']);
  }
}
