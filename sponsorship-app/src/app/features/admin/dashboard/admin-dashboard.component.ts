import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto, SponsorshipTypeDto } from '../../../core/models/sponsorship.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatTableModule, MatButtonModule, MatIconModule,
    MatCardModule, MatTabsModule, MatFormFieldModule, MatInputModule, MatSlideToggleModule,
    MatSnackBarModule, MatTooltipModule, ShellComponent, StatusBadgeComponent
  ],
  template: `
    <app-shell [navItems]="navItems">
      <mat-tab-group>
        <mat-tab label="All Requests">
          <mat-card style="margin-top:16px">
            <table mat-table [dataSource]="requests" class="full-width">
              <ng-container matColumnDef="title"><th mat-header-cell *matHeaderCellDef>Title</th><td mat-cell *matCellDef="let r">{{r.title}}</td></ng-container>
              <ng-container matColumnDef="requestor"><th mat-header-cell *matHeaderCellDef>Requestor</th><td mat-cell *matCellDef="let r">{{r.requestorName}}</td></ng-container>
              <ng-container matColumnDef="dept"><th mat-header-cell *matHeaderCellDef>Dept</th><td mat-cell *matCellDef="let r">{{r.department}}</td></ng-container>
              <ng-container matColumnDef="amount"><th mat-header-cell *matHeaderCellDef>Amount</th><td mat-cell *matCellDef="let r">MYR {{r.requestedAmount | number:'1.2-2'}}</td></ng-container>
              <ng-container matColumnDef="status"><th mat-header-cell *matHeaderCellDef>Status</th><td mat-cell *matCellDef="let r"><app-status-badge [status]="r.status"></app-status-badge></td></ng-container>
              <ng-container matColumnDef="created"><th mat-header-cell *matHeaderCellDef>Created</th><td mat-cell *matCellDef="let r">{{r.createdAt | date:'dd MMM y'}}</td></ng-container>
              <ng-container matColumnDef="actions"><th mat-header-cell *matHeaderCellDef>History</th>
                <td mat-cell *matCellDef="let r"><button mat-icon-button (click)="router.navigate(['/admin/detail', r.id])" matTooltip="View History"><mat-icon>history</mat-icon></button></td>
              </ng-container>
              <tr mat-header-row *matHeaderRowDef="reqCols"></tr>
              <tr mat-row *matRowDef="let row; columns: reqCols;"></tr>
            </table>
            <p *ngIf="requests.length===0" class="empty">No requests found.</p>
          </mat-card>
        </mat-tab>
        <mat-tab label="Manage Sponsorship Types">
          <div style="margin-top:16px; display:flex; gap:16px">
            <mat-card style="flex:1">
              <mat-card-header><mat-card-title>Add New Type</mat-card-title></mat-card-header>
              <mat-card-content>
                <form [formGroup]="typeForm" (ngSubmit)="createType()">
                  <mat-form-field appearance="outline" style="width:100%;margin-top:8px">
                    <mat-label>Type Name</mat-label>
                    <input matInput formControlName="name">
                    <mat-error>Required</mat-error>
                  </mat-form-field>
                  <button mat-raised-button color="primary" type="submit">Add Type</button>
                </form>
              </mat-card-content>
            </mat-card>
            <mat-card style="flex:2">
              <mat-card-header><mat-card-title>Sponsorship Types</mat-card-title></mat-card-header>
              <mat-card-content>
                <table mat-table [dataSource]="types" style="width:100%">
                  <ng-container matColumnDef="name"><th mat-header-cell *matHeaderCellDef>Name</th><td mat-cell *matCellDef="let t">{{t.name}}</td></ng-container>
                  <ng-container matColumnDef="active"><th mat-header-cell *matHeaderCellDef>Active</th>
                    <td mat-cell *matCellDef="let t"><mat-slide-toggle [checked]="t.isActive" (change)="toggleType(t, $event.checked)"></mat-slide-toggle></td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="['name','active']"></tr>
                  <tr mat-row *matRowDef="let row; columns: ['name','active'];"></tr>
                </table>
              </mat-card-content>
            </mat-card>
          </div>
        </mat-tab>
      </mat-tab-group>
    </app-shell>
  `,
  styles: [`.full-width{width:100%} .empty{padding:24px;text-align:center;color:#999}`]
})
export class AdminDashboardComponent implements OnInit {
  private svc = inject(SponsorshipService);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  router = inject(Router);

  requests: SponsorshipRequestDto[] = [];
  types: SponsorshipTypeDto[] = [];
  reqCols = ['title', 'requestor', 'dept', 'amount', 'status', 'created', 'actions'];
  typeForm = this.fb.group({ name: ['', Validators.required] });
  navItems: NavItem[] = [{ label: 'Admin Dashboard', route: '/admin', icon: 'admin_panel_settings' }];

  ngOnInit() {
    this.svc.getAllRequests().subscribe(r => this.requests = r);
    this.svc.getTypes(false).subscribe(t => this.types = t);
  }

  createType() {
    if (this.typeForm.invalid) return;
    this.svc.createType(this.typeForm.value.name!).subscribe({
      next: t => { this.types = [...this.types, t]; this.typeForm.reset(); this.snack.open('Type created', '', {duration:2000}); },
      error: () => this.snack.open('Error creating type', '', {duration:3000})
    });
  }

  toggleType(t: SponsorshipTypeDto, active: boolean) {
    this.svc.updateType(t.id, t.name, active).subscribe({
      next: updated => { const i = this.types.findIndex(x => x.id === t.id); this.types[i] = updated; this.snack.open('Updated', '', {duration:2000}); },
      error: () => this.snack.open('Error', '', {duration:3000})
    });
  }
}
