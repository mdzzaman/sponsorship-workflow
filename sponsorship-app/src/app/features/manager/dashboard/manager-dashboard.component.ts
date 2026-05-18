import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { RemarkDialogComponent } from '../../../shared/components/remark-dialog.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto } from '../../../core/models/sponsorship.model';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatTableModule, MatButtonModule, MatIconModule,
    MatCardModule, MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSnackBarModule, MatTooltipModule, ShellComponent, StatusBadgeComponent
  ],
  template: `
    <app-shell [navItems]="navItems">
      <h2>Pending Manager Approvals</h2>
      <mat-card>
        <table mat-table [dataSource]="requests" class="full-width">
          <ng-container matColumnDef="title"><th mat-header-cell *matHeaderCellDef>Title</th><td mat-cell *matCellDef="let r">{{r.title}}</td></ng-container>
          <ng-container matColumnDef="requestor"><th mat-header-cell *matHeaderCellDef>Requestor</th><td mat-cell *matCellDef="let r">{{r.requestorName}}</td></ng-container>
          <ng-container matColumnDef="dept"><th mat-header-cell *matHeaderCellDef>Department</th><td mat-cell *matCellDef="let r">{{r.department}}</td></ng-container>
          <ng-container matColumnDef="amount"><th mat-header-cell *matHeaderCellDef>Amount</th><td mat-cell *matCellDef="let r">MYR {{r.requestedAmount | number:'1.2-2'}}</td></ng-container>
          <ng-container matColumnDef="status"><th mat-header-cell *matHeaderCellDef>Status</th><td mat-cell *matCellDef="let r"><app-status-badge [status]="r.status"></app-status-badge></td></ng-container>
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let r">
              <button mat-icon-button (click)="viewDetail(r)" matTooltip="View"><mat-icon>visibility</mat-icon></button>
              <button mat-raised-button color="primary" (click)="approve(r)" style="margin:0 4px">Approve</button>
              <button mat-raised-button color="warn" (click)="reject(r)">Reject</button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
        </table>
        <p *ngIf="requests.length === 0" class="empty">No pending approvals.</p>
      </mat-card>
    </app-shell>
  `,
  styles: [`.full-width{width:100%} .empty{padding:24px;text-align:center;color:#999}`]
})
export class ManagerDashboardComponent implements OnInit {
  requests: SponsorshipRequestDto[] = [];
  columns = ['title', 'requestor', 'dept', 'amount', 'status', 'actions'];
  navItems: NavItem[] = [{ label: 'Pending Approvals', route: '/manager', icon: 'approval' }];

  constructor(private svc: SponsorshipService, private snack: MatSnackBar, private dialog: MatDialog, public router: Router) {}

  ngOnInit() { this.load(); }
  load() { this.svc.getPendingManagerApprovals().subscribe(r => this.requests = r); }

  approve(r: SponsorshipRequestDto) {
    const ref = this.dialog.open(RemarkDialogComponent, { data: { title: 'Approve Request', required: false } });
    ref.afterClosed().subscribe(remarks => {
      if (remarks === undefined) return;
      this.svc.managerApprove(r.id, { remarks }).subscribe({
        next: () => { this.snack.open('Approved!', '', {duration:2000}); this.load(); },
        error: () => this.snack.open('Error', '', {duration:3000})
      });
    });
  }

  reject(r: SponsorshipRequestDto) {
    const ref = this.dialog.open(RemarkDialogComponent, { data: { title: 'Reject Request', required: true } });
    ref.afterClosed().subscribe(remarks => {
      if (!remarks) return;
      this.svc.managerReject(r.id, { remarks }).subscribe({
        next: () => { this.snack.open('Rejected', '', {duration:2000}); this.load(); },
        error: () => this.snack.open('Error', '', {duration:3000})
      });
    });
  }

  viewDetail(r: SponsorshipRequestDto) { this.router.navigate(['/manager/detail', r.id]); }
}
