import { Component, OnInit, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
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
  imports: [DatePipe, DecimalPipe, MatCardModule, MatButtonModule, MatIconModule, MatDividerModule, MatTableModule, ShellComponent, StatusBadgeComponent],
  templateUrl: './request-detail-view.component.html',
  styleUrl: './request-detail-view.component.scss'
})
export class RequestDetailViewComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly svc = inject(SponsorshipService);

  backRoute = '/';
  navItems: NavItem[] = [];
  request?: SponsorshipRequestDto;
  histCols = ['from', 'to', 'actor', 'remarks', 'date'];

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    const data = this.route.snapshot.data;
    if (data['backRoute']) this.backRoute = data['backRoute'];
    if (data['navItems']) this.navItems = data['navItems'];
    this.svc.getById(id).subscribe(r => this.request = r);
  }

  goBack() { this.router.navigate([this.backRoute]); }
}
