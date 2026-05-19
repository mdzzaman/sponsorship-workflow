import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
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
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { RemarkDialogComponent } from '../../../shared/components/remark-dialog.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto } from '../../../core/models/sponsorship.model';
import { extractApiError } from '../../../shared/utils/api-error.util';

@Component({
  selector: 'app-finance-dashboard',
  imports: [
    DecimalPipe, FormsModule,
    MatTableModule, MatSortModule, MatPaginatorModule,
    MatButtonModule, MatIconModule, MatCardModule,
    MatFormFieldModule, MatInputModule, MatProgressSpinnerModule,
    MatDialogModule, MatSnackBarModule, MatTooltipModule,
    ShellComponent, StatusBadgeComponent
  ],
  templateUrl: './finance-dashboard.component.html',
  styleUrl: './finance-dashboard.component.scss'
})
export class FinanceDashboardComponent implements OnInit, OnDestroy {
  private readonly svc = inject(SponsorshipService);
  private readonly snack = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);
  readonly router = inject(Router);

  requests: SponsorshipRequestDto[] = [];
  totalCount = 0;
  page = 0;
  pageSize = 10;
  sortBy = 'createdAt';
  sortDesc = true;
  search = '';
  loading = false;

  columns = ['title', 'requestor', 'type', 'requestedAmount', 'status', 'actions'];
  navItems: NavItem[] = [{ label: 'Finance Review', route: '/finance', icon: 'account_balance' }];

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
    this.svc.getPendingApprovals({ page: this.page + 1, pageSize: this.pageSize, sortBy: this.sortBy, sortDesc: this.sortDesc, search: this.search || undefined })
      .subscribe({
        next: res => { this.requests = res.items; this.totalCount = res.totalCount; this.loading = false; },
        error: (err: HttpErrorResponse) => { this.snack.open(extractApiError(err), 'Close', { duration: 5000 }); this.loading = false; }
      });
  }

  approve(r: SponsorshipRequestDto) {
    this.dialog.open(RemarkDialogComponent, { data: { title: 'Final Approval', required: false } })
      .afterClosed().subscribe((remarks: string | undefined) => {
        if (remarks === undefined) return;
        this.svc.approve(r.id, { remarks }).subscribe({
          next: () => { this.snack.open('Fully Approved!', '', { duration: 2000 }); this.loadData(); },
          error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
        });
      });
  }

  reject(r: SponsorshipRequestDto) {
    this.dialog.open(RemarkDialogComponent, { data: { title: 'Reject Request', required: true } })
      .afterClosed().subscribe((remarks: string | undefined) => {
        if (!remarks) return;
        this.svc.reject(r.id, { remarks }).subscribe({
          next: () => { this.snack.open('Rejected', '', { duration: 2000 }); this.loadData(); },
          error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
        });
      });
  }
}
