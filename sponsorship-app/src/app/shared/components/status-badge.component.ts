import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RequestStatus, StatusLabels } from '../../core/models/sponsorship.model';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `<span class="badge" [ngClass]="cssClass">{{label}}</span>`,
  styles: [`
    .badge { padding:3px 10px; border-radius:12px; font-size:12px; font-weight:600; white-space:nowrap; }
    .draft     { background:#e0e0e0; color:#616161; }
    .pending   { background:#fff3e0; color:#e65100; }
    .finance   { background:#e8eaf6; color:#283593; }
    .approved  { background:#e8f5e9; color:#2e7d32; }
    .rejected  { background:#ffebee; color:#c62828; }
    .cancelled { background:#f5f5f5; color:#9e9e9e; }
  `]
})
export class StatusBadgeComponent {
  @Input() status!: RequestStatus;

  get label() { return StatusLabels[this.status] ?? this.status; }

  get cssClass() {
    const map: Record<RequestStatus, string> = {
      [RequestStatus.Draft]: 'draft',
      [RequestStatus.PendingManagerApproval]: 'pending',
      [RequestStatus.PendingFinanceReview]: 'finance',
      [RequestStatus.Approved]: 'approved',
      [RequestStatus.Rejected]: 'rejected',
      [RequestStatus.Cancelled]: 'cancelled',
    };
    return map[this.status];
  }
}
