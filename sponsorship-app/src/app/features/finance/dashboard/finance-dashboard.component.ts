import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
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

@Component({
    selector: 'app-finance-dashboard',
    imports: [
        CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatCardModule,
        MatDialogModule, MatSnackBarModule, MatTooltipModule, ShellComponent, StatusBadgeComponent
    ],
    template: `
    <app-shell [navItems]="navItems">
      <h2>Finance Review Queue</h2>
      <mat-card>
        <table mat-table [dataSource]="requests" class="full-width">
          <ng-container matColumnDef="title"><th mat-header-cell *matHeaderCellDef>Title</th><td mat-cell *matCellDef="let r">{{r.title}}</td></ng-container>
          <ng-container matColumnDef="requestor"><th mat-header-cell *matHeaderCellDef>Requestor</th><td mat-cell *matCellDef="let r">{{r.requestorName}}</td></ng-container>
          <ng-container matColumnDef="type"><th mat-header-cell *matHeaderCellDef>Type</th><td mat-cell *matCellDef="let r">{{r.sponsorshipTypeName}}</td></ng-container>
          <ng-container matColumnDef="amount"><th mat-header-cell *matHeaderCellDef>Amount</th><td mat-cell *matCellDef="let r">MYR {{r.requestedAmount | number:'1.2-2'}}</td></ng-container>
          <ng-container matColumnDef="status"><th mat-header-cell *matHeaderCellDef>Status</th><td mat-cell *matCellDef="let r"><app-status-badge [status]="r.status"></app-status-badge></td></ng-container>
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let r">
              <button mat-icon-button (click)="router.navigate(['/finance/detail', r.id])" matTooltip="View"><mat-icon>visibility</mat-icon></button>
              <button mat-raised-button color="primary" (click)="approve(r)" style="margin:0 4px">Approve</button>
              <button mat-raised-button color="warn" (click)="reject(r)">Reject</button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
        </table>
        <p *ngIf="requests.length === 0" class="empty">No items pending finance review.</p>
      </mat-card>
    </app-shell>
  `,
    styles: [`.full-width{width:100%} .empty{padding:24px;text-align:center;color:#999}`]
})
export class FinanceDashboardComponent implements OnInit {
  requests: SponsorshipRequestDto[] = [];
  columns = ['title', 'requestor', 'type', 'amount', 'status', 'actions'];
  navItems: NavItem[] = [{ label: 'Finance Review', route: '/finance', icon: 'account_balance' }];

  constructor(private svc: SponsorshipService, private snack: MatSnackBar, private dialog: MatDialog, public router: Router) {}

  ngOnInit() { this.load(); }
  load() { this.svc.getPendingFinanceReview().subscribe(r => this.requests = r); }

  approve(r: SponsorshipRequestDto) {
    const ref = this.dialog.open(RemarkDialogComponent, { data: { title: 'Final Approval', required: false } });
    ref.afterClosed().subscribe(remarks => {
      if (remarks === undefined) return;
      this.svc.financeApprove(r.id, { remarks }).subscribe({
        next: () => { this.snack.open('Fully Approved!', '', {duration:2000}); this.load(); },
        error: () => this.snack.open('Error', '', {duration:3000})
      });
    });
  }

  reject(r: SponsorshipRequestDto) {
    const ref = this.dialog.open(RemarkDialogComponent, { data: { title: 'Reject Request', required: true } });
    ref.afterClosed().subscribe(remarks => {
      if (!remarks) return;
      this.svc.financeReject(r.id, { remarks }).subscribe({
        next: () => { this.snack.open('Rejected', '', {duration:2000}); this.load(); },
        error: () => this.snack.open('Error', '', {duration:3000})
      });
    });
  }
}
