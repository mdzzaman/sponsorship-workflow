import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
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
    DatePipe, DecimalPipe, FormsModule, ReactiveFormsModule,
    MatTableModule, MatSortModule, MatPaginatorModule,
    MatButtonModule, MatIconModule, MatCardModule, MatTabsModule,
    MatFormFieldModule, MatInputModule, MatProgressSpinnerModule,
    MatSlideToggleModule, MatSnackBarModule, MatTooltipModule,
    ShellComponent, StatusBadgeComponent
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  private readonly svc = inject(SponsorshipService);
  private readonly snack = inject(MatSnackBar);
  private readonly fb = inject(FormBuilder);
  readonly router = inject(Router);

  requests: SponsorshipRequestDto[] = [];
  types: SponsorshipTypeDto[] = [];
  totalCount = 0;
  page = 0;
  pageSize = 10;
  sortBy = 'createdAt';
  sortDesc = true;
  search = '';
  loading = false;

  reqCols = ['title', 'requestor', 'dept', 'requestedAmount', 'status', 'created', 'actions'];
  typeForm = this.fb.group({ name: ['', Validators.required] });
  navItems: NavItem[] = [{ label: 'Admin Dashboard', route: '/admin', icon: 'admin_panel_settings' }];

  private readonly searchInput$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  ngOnInit() {
    this.loadData();
    this.svc.getTypes(false).subscribe(t => this.types = t);
    this.searchInput$.pipe(debounceTime(350), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => { this.page = 0; this.loadData(); });
  }

  ngOnDestroy() { this.destroy$.next(); this.destroy$.complete(); }

  onSearchChange(value: string) { this.searchInput$.next(value); }

  onSort(sort: Sort) {
    this.sortBy = sort.active;
    this.sortDesc = sort.direction === 'desc';
    this.page = 0;
    this.loadData();
  }

  onPage(event: PageEvent) {
    this.page = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.svc.getAllRequests({ page: this.page + 1, pageSize: this.pageSize, sortBy: this.sortBy, sortDesc: this.sortDesc, search: this.search || undefined })
      .subscribe({
        next: res => { this.requests = res.items; this.totalCount = res.totalCount; this.loading = false; },
        error: (err: HttpErrorResponse) => { this.snack.open(extractApiError(err), 'Close', { duration: 5000 }); this.loading = false; }
      });
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
      next: updated => { this.types = this.types.map(x => x.id === t.id ? updated : x); this.snack.open('Updated', '', { duration: 2000 }); },
      error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
    });
  }
}
