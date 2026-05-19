import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto, RequestStatus } from '../../../core/models/sponsorship.model';
import { extractApiError } from '../../../shared/utils/api-error.util';

@Component({
  selector: 'app-requestor-dashboard',
  imports: [
    DatePipe, DecimalPipe, FormsModule,
    MatTableModule, MatSortModule, MatPaginatorModule,
    MatButtonModule, MatIconModule, MatCardModule,
    MatFormFieldModule, MatInputModule, MatProgressSpinnerModule,
    MatSnackBarModule, MatTooltipModule,
    ShellComponent, StatusBadgeComponent
  ],
  templateUrl: './requestor-dashboard.component.html',
  styleUrl: './requestor-dashboard.component.scss'
})
export class RequestorDashboardComponent implements OnInit, OnDestroy {
  private readonly svc = inject(SponsorshipService);
  private readonly snack = inject(MatSnackBar);
  readonly router = inject(Router);

  requests: SponsorshipRequestDto[] = [];
  totalCount = 0;
  page = 0;
  pageSize = 10;
  sortBy = 'createdAt';
  sortDesc = true;
  search = '';
  loading = false;

  columns = ['title', 'eventName', 'requestedAmount', 'status', 'createdAt', 'actions'];
  navItems: NavItem[] = [
    { label: 'My Requests', route: '/requestor', icon: 'list' },
    { label: 'New Request', route: '/requestor/new', icon: 'add_circle' }
  ];

  private readonly searchInput$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  ngOnInit() {
    this.loadData();
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
    this.svc.getMyRequests({ page: this.page + 1, pageSize: this.pageSize, sortBy: this.sortBy, sortDesc: this.sortDesc, search: this.search || undefined })
      .subscribe({
        next: res => { this.requests = res.items; this.totalCount = res.totalCount; this.loading = false; },
        error: (err: HttpErrorResponse) => { this.snack.open(extractApiError(err), 'Close', { duration: 5000 }); this.loading = false; }
      });
  }

  canCancel(r: SponsorshipRequestDto) {
    return [RequestStatus.Draft, RequestStatus.PendingManagerApproval, RequestStatus.PendingFinanceReview].includes(r.status);
  }

  submit(r: SponsorshipRequestDto) {
    this.svc.submit(r.id).subscribe({
      next: () => { this.snack.open('Submitted successfully!', '', { duration: 2000 }); this.loadData(); },
      error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
    });
  }

  cancel(r: SponsorshipRequestDto) {
    if (!confirm('Are you sure you want to cancel this request?')) return;
    this.svc.cancel(r.id).subscribe({
      next: () => { this.snack.open('Request cancelled.', '', { duration: 2000 }); this.loadData(); },
      error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
    });
  }

  viewDetail(r: SponsorshipRequestDto) { this.router.navigate(['/requestor/detail', r.id]); }
}
