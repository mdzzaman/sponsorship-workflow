import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
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
    CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatCardModule,
    MatDialogModule, MatSnackBarModule, MatTooltipModule, ShellComponent, StatusBadgeComponent
  ],
  templateUrl: './finance-dashboard.component.html',
  styleUrl: './finance-dashboard.component.scss'
})
export class FinanceDashboardComponent implements OnInit {
  requests: SponsorshipRequestDto[] = [];
  columns = ['title', 'requestor', 'type', 'amount', 'status', 'actions'];
  navItems: NavItem[] = [{ label: 'Finance Review', route: '/finance', icon: 'account_balance' }];

  constructor(private svc: SponsorshipService, private snack: MatSnackBar, private dialog: MatDialog, public router: Router) {}

  ngOnInit() { this.load(); }
  load() { this.svc.getPendingApprovals().subscribe(r => this.requests = r); }

  approve(r: SponsorshipRequestDto) {
    this.dialog.open(RemarkDialogComponent, { data: { title: 'Final Approval', required: false } })
      .afterClosed().subscribe((remarks: string | undefined) => {
        if (remarks === undefined) return;
        this.svc.approve(r.id, { remarks }).subscribe({
          next: () => { this.snack.open('Fully Approved!', '', { duration: 2000 }); this.load(); },
          error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
        });
      });
  }

  reject(r: SponsorshipRequestDto) {
    this.dialog.open(RemarkDialogComponent, { data: { title: 'Reject Request', required: true } })
      .afterClosed().subscribe((remarks: string | undefined) => {
        if (!remarks) return;
        this.svc.reject(r.id, { remarks }).subscribe({
          next: () => { this.snack.open('Rejected', '', { duration: 2000 }); this.load(); },
          error: (err: HttpErrorResponse) => this.snack.open(extractApiError(err), 'Close', { duration: 5000 })
        });
      });
  }
}
