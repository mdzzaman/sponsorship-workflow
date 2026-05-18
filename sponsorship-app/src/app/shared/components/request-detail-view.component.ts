import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { ShellComponent, NavItem } from './shell.component';
import { StatusBadgeComponent } from './status-badge.component';
import { SponsorshipService } from '../../core/services/sponsorship.service';
import { SponsorshipRequestDto } from '../../core/models/sponsorship.model';

@Component({
  selector: 'app-request-detail-view',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatDividerModule, MatTableModule, ShellComponent, StatusBadgeComponent],
  template: `
    <app-shell [navItems]="navItems">
      <div *ngIf="request">
        <div class="page-header">
          <h2>{{request.title}}</h2>
          <button mat-button (click)="router.navigate([backRoute])"><mat-icon>arrow_back</mat-icon> Back</button>
        </div>
        <mat-card>
          <mat-card-content>
            <div class="detail-grid">
              <div class="field"><label>Status</label><app-status-badge [status]="request.status"></app-status-badge></div>
              <div class="field"><label>Requestor</label><span>{{request.requestorName}}</span></div>
              <div class="field"><label>Department</label><span>{{request.department}}</span></div>
              <div class="field"><label>Type</label><span>{{request.sponsorshipTypeName}}</span></div>
              <div class="field"><label>Event</label><span>{{request.eventName}}</span></div>
              <div class="field"><label>Event Date</label><span>{{request.eventDate | date:'dd MMM yyyy'}}</span></div>
              <div class="field"><label>Amount</label><span>MYR {{request.requestedAmount | number:'1.2-2'}}</span></div>
              <div class="field full-span"><label>Justification</label><span>{{request.justification}}</span></div>
              <div class="field full-span" *ngIf="request.expectedBenefit"><label>Expected Benefit</label><span>{{request.expectedBenefit}}</span></div>
            </div>
            <mat-divider style="margin:24px 0"></mat-divider>
            <h3>Workflow History</h3>
            <table mat-table [dataSource]="request.workflowHistories" style="width:100%" *ngIf="request.workflowHistories.length">
              <ng-container matColumnDef="from"><th mat-header-cell *matHeaderCellDef>From</th><td mat-cell *matCellDef="let h">{{h.fromStatusName}}</td></ng-container>
              <ng-container matColumnDef="to"><th mat-header-cell *matHeaderCellDef>To</th><td mat-cell *matCellDef="let h">{{h.toStatusName}}</td></ng-container>
              <ng-container matColumnDef="actor"><th mat-header-cell *matHeaderCellDef>By</th><td mat-cell *matCellDef="let h">{{h.actorName}}</td></ng-container>
              <ng-container matColumnDef="remarks"><th mat-header-cell *matHeaderCellDef>Remarks</th><td mat-cell *matCellDef="let h">{{h.remarks ?? '-'}}</td></ng-container>
              <ng-container matColumnDef="date"><th mat-header-cell *matHeaderCellDef>Date</th><td mat-cell *matCellDef="let h">{{h.actionedAt | date:'dd MMM y HH:mm'}}</td></ng-container>
              <tr mat-header-row *matHeaderRowDef="histCols"></tr>
              <tr mat-row *matRowDef="let r; columns: histCols;"></tr>
            </table>
            <p *ngIf="!request.workflowHistories.length" style="color:#999">No workflow actions yet.</p>
          </mat-card-content>
        </mat-card>
      </div>
    </app-shell>
  `,
  styles: [`.page-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:16px;}.detail-grid{display:grid;grid-template-columns:1fr 1fr;gap:16px;}.field{display:flex;flex-direction:column;gap:4px;}.field label{font-size:12px;color:#666;font-weight:600;}.full-span{grid-column:1/-1;}`]
})
export class RequestDetailViewComponent implements OnInit {
  @Input() backRoute = '/';
  @Input() navItems: NavItem[] = [];
  request?: SponsorshipRequestDto;
  histCols = ['from', 'to', 'actor', 'remarks', 'date'];

  constructor(public router: Router, private route: ActivatedRoute, private svc: SponsorshipService) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    const data = this.route.snapshot.data;
    if (data['backRoute']) this.backRoute = data['backRoute'];
    if (data['navItems']) this.navItems = data['navItems'];
    this.svc.getById(id).subscribe(r => this.request = r);
  }
}
