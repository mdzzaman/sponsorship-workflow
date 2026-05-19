import { Component, Input, ViewChild, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../core/services/auth.service';

export interface NavItem { label: string; route: string; icon: string; }

@Component({
  selector: 'app-shell',
  imports: [RouterModule, MatToolbarModule, MatButtonModule, MatIconModule, MatSidenavModule, MatListModule, MatTooltipModule],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss'
})
export class ShellComponent {
  @Input() navItems: NavItem[] = [];
  @ViewChild('sidenav') sidenav!: MatSidenav;

  readonly auth = inject(AuthService);
  private readonly breakpoint = inject(BreakpointObserver);

  readonly isMobile = toSignal(
    this.breakpoint.observe([Breakpoints.XSmall, Breakpoints.Small]).pipe(
      map(r => r.matches)
    ),
    { initialValue: false }
  );

  onNavClick() {
    if (this.isMobile()) this.sidenav.close();
  }

  get userInitials(): string {
    const name = this.auth.currentUser()?.fullName ?? '';
    return name.split(' ').slice(0, 2).map(n => n.charAt(0)).join('').toUpperCase();
  }
}
