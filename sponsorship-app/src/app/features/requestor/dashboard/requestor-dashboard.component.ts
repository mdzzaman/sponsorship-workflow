import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto, RequestStatus } from '../../../core/models/sponsorship.model';

@Component({
    selector: 'app-requestor-dashboard',
    imports: [
        CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatCardModule,
        MatSnackBarModule, MatTooltipModule, MatDialogModule,
        ShellComponent, StatusBadgeComponent
    ],
    template: `
    <app-shell [navItems]="navItems">
      <div class="page-header">
        <h2>My Requests</h2>
        <button mat-raised-button color="primary" (click)="router.navigate(['/requestor/new'])">
          <mat-icon>add</mat-icon> New Request
        </button>
      </div>
      <mat-card>
        <table mat-table [dataSource]="requests" class="full-width">
          <ng-container matColumnDef="title">
            <th mat-header-cell *matHeaderCellDef>Title</th>
            <td mat-cell *matCellDef="let r">{{r.title}}</td>
          </ng-container>
          <ng-container matColumnDef="eventName">
            <th mat-header-cell *matHeaderCellDef>Event</th>
            <td mat-cell *matCellDef="let r">{{r.eventName}}</td>
          </ng-container>
          <ng-container matColumnDef="amount">
            <th mat-header-cell *matHeaderCellDef>Amount</th>
            <td mat-cell *matCellDef="let r">MYR {{r.requestedAmount | number:'1.2-2'}}</td>
          </ng-container>
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let r"><app-status-badge [status]="r.status"></app-status-badge></td>
          </ng-container>
          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef>Created</th>
            <td mat-cell *matCellDef="let r">{{r.createdAt | date:'dd MMM y'}}</td>
          </ng-container>
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let r">
              <button mat-icon-button (click)="viewDetail(r)" matTooltip="View"><mat-icon>visibility</mat-icon></button>
              <button mat-icon-button *ngIf="r.status === 1" (click)="router.navigate(['/requestor/edit', r.id])" matTooltip="Edit"><mat-icon>edit</mat-icon></button>
              <button mat-icon-button *ngIf="r.status === 1" (click)="submit(r)" matTooltip="Submit" color="primary"><mat-icon>send</mat-icon></button>
              <button mat-icon-button *ngIf="canCancel(r)" (click)="cancel(r)" matTooltip="Cancel" color="warn"><mat-icon>cancel</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns;"></tr>
        </table>
        <p *ngIf="requests.length === 0" class="empty">No requests yet. Create your first one!</p>
      </mat-card>
    </app-shell>
  `,
    styles: [`.page-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:16px;} .full-width{width:100%;} .empty{padding:24px;text-align:center;color:#999;}`]
})
export class RequestorDashboardComponent implements OnInit {
  requests: SponsorshipRequestDto[] = [];
  columns = ['title', 'eventName', 'amount', 'status', 'createdAt', 'actions'];
  navItems: NavItem[] = [
    { label: 'My Requests', route: '/requestor', icon: 'list' },
    { label: 'New Request', route: '/requestor/new', icon: 'add_circle' }
  ];

  constructor(public router: Router, private svc: SponsorshipService, private snack: MatSnackBar) {}

  ngOnInit() { this.load(); }
  load() { this.svc.getMyRequests().subscribe(r => this.requests = r); }

  canCancel(r: SponsorshipRequestDto) {
    return [RequestStatus.Draft, RequestStatus.PendingManagerApproval, RequestStatus.PendingFinanceReview].includes(r.status);
  }

  submit(r: SponsorshipRequestDto) {
    this.svc.submit(r.id).subscribe({ next: () => { this.snack.open('Submitted!', '', {duration:2000}); this.load(); }, error: () => this.snack.open('Error submitting', '', {duration:3000}) });
  }

  cancel(r: SponsorshipRequestDto) {
    if (!confirm('Cancel this request?')) return;
    this.svc.cancel(r.id).subscribe({ next: () => { this.snack.open('Cancelled', '', {duration:2000}); this.load(); }, error: () => this.snack.open('Error', '', {duration:3000}) });
  }

  viewDetail(r: SponsorshipRequestDto) { this.router.navigate(['/requestor/detail', r.id]); }
}
