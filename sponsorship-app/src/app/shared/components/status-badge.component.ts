import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { RequestStatus, StatusLabels } from '../../core/models/sponsorship.model';

@Component({
  selector: 'app-status-badge',
  imports: [NgClass],
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss'
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
