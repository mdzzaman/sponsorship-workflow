import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatSnackBarModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
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
    const roles = this.auth.currentUser()?.roles ?? [];
    const priority: [string, string][] = [
      ['SystemAdmin', '/admin'],
      ['Manager', '/manager'],
      ['FinanceAdmin', '/finance'],
      ['Requestor', '/requestor'],
    ];
    const match = priority.find(([role]) => roles.includes(role));
    this.router.navigate([match ? match[1] : '/login']);
  }
}
