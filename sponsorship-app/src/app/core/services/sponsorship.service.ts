import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  ActionRemarkDto,
  CreateRequestDto,
  SponsorshipRequestDto,
  SponsorshipTypeDto
} from '../models/sponsorship.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SponsorshipService {
  private readonly base = `${environment.apiUrl}/sponsorshiprequests`;
  private readonly typesBase = `${environment.apiUrl}/sponsorshiptypes`;

  constructor(private http: HttpClient) {}

  // Types
  getTypes(activeOnly = true) {
    return this.http.get<SponsorshipTypeDto[]>(`${this.typesBase}?activeOnly=${activeOnly}`);
  }
  createType(name: string) {
    return this.http.post<SponsorshipTypeDto>(this.typesBase, { name });
  }
  updateType(id: string, name: string, isActive: boolean) {
    return this.http.put<SponsorshipTypeDto>(`${this.typesBase}/${id}`, { name, isActive });
  }

  // Requestor
  getMyRequests() {
    return this.http.get<SponsorshipRequestDto[]>(`${this.base}/my`);
  }
  getById(id: string) {
    return this.http.get<SponsorshipRequestDto>(`${this.base}/${id}`);
  }
  create(dto: CreateRequestDto) {
    return this.http.post<SponsorshipRequestDto>(this.base, dto);
  }
  update(id: string, dto: CreateRequestDto) {
    return this.http.put<SponsorshipRequestDto>(`${this.base}/${id}`, dto);
  }
  submit(id: string) {
    return this.http.post<void>(`${this.base}/${id}/submit`, {});
  }
  cancel(id: string) {
    return this.http.post<void>(`${this.base}/${id}/cancel`, {});
  }

  // Approver (Manager + Finance — unified endpoint, state machine determines stage)
  getPendingApprovals() {
    return this.http.get<SponsorshipRequestDto[]>(`${this.base}/pending`);
  }
  approve(id: string, dto: ActionRemarkDto) {
    return this.http.post<void>(`${this.base}/${id}/approve`, dto);
  }
  reject(id: string, dto: ActionRemarkDto) {
    return this.http.post<void>(`${this.base}/${id}/reject`, dto);
  }

  // Admin
  getAllRequests() {
    return this.http.get<SponsorshipRequestDto[]>(this.base);
  }
}
