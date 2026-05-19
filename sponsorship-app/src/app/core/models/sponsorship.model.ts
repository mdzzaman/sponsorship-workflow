export enum RequestStatus {
  Draft = 1,
  PendingManagerApproval = 2,
  PendingFinanceReview = 3,
  Approved = 4,
  Rejected = 5,
  Cancelled = 6
}

export const StatusLabels: Record<RequestStatus, string> = {
  [RequestStatus.Draft]: 'Draft',
  [RequestStatus.PendingManagerApproval]: 'Pending Manager Approval',
  [RequestStatus.PendingFinanceReview]: 'Pending Finance Review',
  [RequestStatus.Approved]: 'Approved',
  [RequestStatus.Rejected]: 'Rejected',
  [RequestStatus.Cancelled]: 'Cancelled',
};


export interface WorkflowHistoryDto {
  id: string;
  fromStatus: RequestStatus;
  fromStatusName: string;
  toStatus: RequestStatus;
  toStatusName: string;
  actorName: string;
  remarks: string | null;
  recordedAt: string;
}

export interface SponsorshipRequestDto {
  id: string;
  title: string;
  requestorId: string;
  requestorName: string;
  department: string;
  sponsorshipTypeId: string;
  sponsorshipTypeName: string;
  eventName: string;
  eventDate: string;
  requestedAmount: number;
  justification: string;
  expectedBenefit: string | null;
  remarks: string | null;
  status: RequestStatus;
  statusName: string;
  createdAt: string;
  updatedAt: string;
  workflowHistories: WorkflowHistoryDto[];
}

export interface SponsorshipTypeDto {
  id: string;
  name: string;
  isActive: boolean;
}

export interface CreateRequestDto {
  title: string;
  department: string;
  sponsorshipTypeId: string;
  eventName: string;
  eventDate: string;
  requestedAmount: number;
  justification: string;
  expectedBenefit: string | null;
  remarks: string | null;
}

export interface ActionRemarkDto {
  remarks: string | null;
}
