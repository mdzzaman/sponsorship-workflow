import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { AuthService } from '../../core/services/auth.service';

export interface NavItem { label: string; route: string; icon: string; }

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterModule, MatToolbarModule, MatButtonModule, MatIconModule, MatSidenavModule, MatListModule],
  template: `
    <mat-sidenav-container class="shell-container">
      <mat-sidenav mode="side" opened class="sidenav">
        <div class="sidenav-header">
          <mat-icon>business_center</mat-icon>
          <span>Sponsorship<br>Workflow</span>
        </div>
        <mat-nav-list>
          <a mat-list-item *ngFor="let item of navItems" [routerLink]="item.route" routerLinkActive="active-link">
            <mat-icon matListItemIcon>{{item.icon}}</mat-icon>
            <span matListItemTitle>{{item.label}}</span>
          </a>
        </mat-nav-list>
        <div class="sidenav-footer">
          <small>{{auth.currentUser()?.fullName}}</small><br>
          <small class="role-badge">{{auth.currentUser()?.role}}</small>
        </div>
      </mat-sidenav>
      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span class="spacer"></span>
          <button mat-icon-button (click)="auth.logout()" title="Sign out">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>
        <div class="content">
          <ng-content></ng-content>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .shell-container { height:100vh; }
    .sidenav { width:220px; background:#1a237e; color:white; display:flex; flex-direction:column; }
    .sidenav-header { padding:24px 16px; display:flex; gap:12px; align-items:center; font-size:14px; font-weight:600; border-bottom:1px solid rgba(255,255,255,0.1); }
    mat-nav-list a { color:rgba(255,255,255,0.85); }
    mat-nav-list a.active-link { background:rgba(255,255,255,0.15); border-left:3px solid #fff; }
    .sidenav-footer { margin-top:auto; padding:16px; font-size:12px; color:rgba(255,255,255,0.6); border-top:1px solid rgba(255,255,255,0.1); }
    .role-badge { background:rgba(255,255,255,0.2); padding:2px 8px; border-radius:8px; }
    .spacer { flex:1 1 auto; }
    .content { padding:24px; }
    mat-toolbar { background:#283593; }
  `]
})
export class ShellComponent {
  @Input() navItems: NavItem[] = [];
  constructor(public auth: AuthService) {}
}
