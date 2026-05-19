import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
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
  templateUrl: './requestor-dashboard.component.html',
  styleUrl: './requestor-dashboard.component.scss'
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
    this.svc.submit(r.id).subscribe({
      next: () => { this.snack.open('Submitted!', '', { duration: 2000 }); this.load(); },
      error: () => this.snack.open('Error submitting', '', { duration: 3000 })
    });
  }

  cancel(r: SponsorshipRequestDto) {
    if (!confirm('Cancel this request?')) return;
    this.svc.cancel(r.id).subscribe({
      next: () => { this.snack.open('Cancelled', '', { duration: 2000 }); this.load(); },
      error: () => this.snack.open('Error', '', { duration: 3000 })
    });
  }

  viewDetail(r: SponsorshipRequestDto) { this.router.navigate(['/requestor/detail', r.id]); }
}
