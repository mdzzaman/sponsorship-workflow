import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { ShellComponent, NavItem } from '../../../shared/components/shell.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge.component';
import { SponsorshipService } from '../../../core/services/sponsorship.service';
import { SponsorshipRequestDto } from '../../../core/models/sponsorship.model';

@Component({
  selector: 'app-request-detail',
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatDividerModule, MatTableModule, ShellComponent, StatusBadgeComponent],
  templateUrl: './request-detail.component.html',
  styleUrl: './request-detail.component.scss'
})
export class RequestDetailComponent implements OnInit {
  request?: SponsorshipRequestDto;
  histCols = ['from', 'to', 'actor', 'remarks', 'date'];
  navItems: NavItem[] = [
    { label: 'My Requests', route: '/requestor', icon: 'list' },
    { label: 'New Request', route: '/requestor/new', icon: 'add_circle' }
  ];

  constructor(public router: Router, private route: ActivatedRoute, private svc: SponsorshipService) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.svc.getById(id).subscribe(r => this.request = r);
  }
}
