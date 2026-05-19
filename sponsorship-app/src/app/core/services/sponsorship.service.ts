import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  ActionRemarkDto,
  CreateRequestDto,
  PagedRequest,
  PagedResult,
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
  getMyRequests(params?: PagedRequest) {
    return this.http.get<PagedResult<SponsorshipRequestDto>>(`${this.base}/my`, { params: toHttpParams(params) });
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
  getPendingApprovals(params?: PagedRequest) {
    return this.http.get<PagedResult<SponsorshipRequestDto>>(`${this.base}/pending`, { params: toHttpParams(params) });
  }
  approve(id: string, dto: ActionRemarkDto) {
    return this.http.post<void>(`${this.base}/${id}/approve`, dto);
  }
  reject(id: string, dto: ActionRemarkDto) {
    return this.http.post<void>(`${this.base}/${id}/reject`, dto);
  }

  // Admin
  getAllRequests(params?: PagedRequest) {
    return this.http.get<PagedResult<SponsorshipRequestDto>>(this.base, { params: toHttpParams(params) });
  }
}

function toHttpParams(p?: PagedRequest): HttpParams {
  let params = new HttpParams();
  if (!p) return params;
  if (p.page     != null) params = params.set('page',     p.page);
  if (p.pageSize != null) params = params.set('pageSize', p.pageSize);
  if (p.sortBy)           params = params.set('sortBy',   p.sortBy);
  if (p.sortDesc != null) params = params.set('sortDesc', p.sortDesc);
  if (p.search)           params = params.set('search',   p.search);
  return params;
}
