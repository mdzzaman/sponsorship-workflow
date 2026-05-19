import { Component, OnInit, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto, SponsorshipTypeDto } from '../../../core/models/sponsorship.model';
import { extractApiError } from '../../../shared/utils/api-error.util';

@Component({
  selector: 'app-admin-dashboard',
  imports: [
    DatePipe, DecimalPipe, ReactiveFormsModule, MatTableModule, MatButtonModule, MatIconModule,
    MatCardModule, MatTabsModule, MatFormFieldModule, MatInputModule, MatSlideToggleModule,
    MatSnackBarModule, MatTooltipModule, ShellComponent, StatusBadgeComponent
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  private readonly svc = inject(SponsorshipService);
  private readonly snack = inject(MatSnackBar);
  private readonly fb = inject(FormBuilder);
  readonly router = inject(Router);

  requests: SponsorshipRequestDto[] = [];
  types: SponsorshipTypeDto[] = [];
  reqCols = ['title', 'requestor', 'dept', 'amount', 'status', 'created', 'actions'];
  typeForm = this.fb.group({ name: ['', Validators.required] });
  navItems: NavItem[] = [{ label: 'Admin Dashboard', route: '/admin', icon: 'admin_panel_settings' }];

  ngOnInit() {
    this.svc.getAllRequests().subscribe(r => this.requests = r);
    this.svc.getTypes(false).subscribe(t => this.types = t);
  }

  createType() {
    if (this.typeForm.invalid) return;
    this.svc.createType(this.typeForm.value.name!).subscribe({
      next: t => { this.types = [...this.types, t]; this.typeForm.reset(); this.snack.open('Type created', '', { duration: 2000 }); },
      error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
    });
  }

  toggleType(t: SponsorshipTypeDto, active: boolean) {
    this.svc.updateType(t.id, t.name, active).subscribe({
      next: updated => {
        this.types = this.types.map(x => x.id === t.id ? updated : x);
        this.snack.open('Updated', '', { duration: 2000 });
      },
      error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
    });
  }
}
