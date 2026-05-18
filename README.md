# Sponsorship Request Approval Workflow

Senior Full Stack .NET Developer Technical Assessment

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 9, ASP.NET Core Web API, Clean Architecture |
| ORM | Entity Framework Core 9, PostgreSQL |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Frontend | Angular 19, Angular Material |
| Containerization | Docker Compose |
| API Docs | Swagger / OpenAPI |

---

## Architecture Overview

```
sponsorship-workflow/
├── backend/
│   ├── SponsorshipWorkflow.Domain/        # Entities, enums, domain events (no dependencies)
│   ├── SponsorshipWorkflow.Application/   # CQRS commands/queries, interfaces (depends on Domain)
│   ├── SponsorshipWorkflow.Infrastructure/# EF Core, JWT, seeding (depends on Application)
│   └── SponsorshipWorkflow.API/           # Controllers, middleware, DI wiring (depends on all)
├── sponsorship-app/                       # Angular frontend
└── docker-compose.yml
```

### Design Patterns Used

| Pattern | Why |
|---|---|
| **Clean Architecture** | Strict layer separation — Domain has zero external dependencies |
| **CQRS with MediatR** | Each workflow action is an explicit Command, each read is a Query |
| **Domain Events** | `RequestStatusChangedEvent` fires on every status change, creating audit records automatically |
| **Repository via IApplicationDbContext** | Abstracts EF Core from Application layer |
| **State Machine (in-code)** | Each command validates the current state before transitioning |
| **JWT + Role-based Authorization** | Policy-based RBAC per endpoint |

### Workflow State Machine

```
Draft ──[Submit]──► PendingManagerApproval
                          │
               [Approve] ─┤─ [Reject] ──► Rejected
                          ▼
                   PendingFinanceReview
                          │
               [Approve] ─┤─ [Reject] ──► Rejected
                          ▼
                       Approved

Any non-final state ──[Cancel]──► Cancelled
```

---

## Running with Docker Compose (Recommended)

> Prerequisites: Docker Desktop installed and running

```bash
# Clone or extract the project
cd sponsorship-workflow

# Build and start all services (PostgreSQL + API + Frontend)
docker-compose up --build

# First run will:
# - Start PostgreSQL
# - Run EF Core migrations automatically
# - Seed all test accounts and sponsorship types
# - Serve Angular at http://localhost:80
# - Serve API at http://localhost:5000
```

**Access points:**
- Frontend: http://localhost
- API Swagger: http://localhost:5000/swagger
- Database (if needed): localhost:5432

---

## Running Locally (Without Docker)

### Prerequisites
- .NET 9 SDK
- Node.js 22+ and npm
- PostgreSQL 14+

### Backend

```bash
# 1. Create the database (PostgreSQL must be running)
# Update connection string in backend/SponsorshipWorkflow.API/appsettings.json if needed

cd backend

# 2. Run migrations + start API
dotnet run --project SponsorshipWorkflow.API

# API runs on http://localhost:5000
# Swagger: http://localhost:5000/swagger
# Migrations run automatically on startup
# Test accounts are seeded automatically
```

### Frontend

```bash
cd sponsorship-app

npm install

# Development (connects to localhost:5000)
ng serve

# Frontend runs on http://localhost:4200
```

---

## Test Login Accounts

| Email | Password | Role | Access |
|---|---|---|---|
| requestor@test.com | Test@123 | Requestor | Submit requests, view own requests, cancel |
| manager@test.com | Test@123 | Manager | Approve/reject pending manager approvals |
| finance@test.com | Test@123 | Finance Admin | Final approve/reject after manager approval |
| admin@test.com | Test@123 | System Admin | View all requests, manage sponsorship types |

---

## API Endpoints Summary

| Method | Endpoint | Role | Description |
|---|---|---|---|
| POST | /api/auth/login | Public | Login |
| GET | /api/sponsorshiprequests/my | Requestor | My requests |
| POST | /api/sponsorshiprequests | Requestor | Create request |
| PUT | /api/sponsorshiprequests/{id} | Requestor | Update draft |
| POST | /api/sponsorshiprequests/{id}/submit | Requestor | Submit for approval |
| POST | /api/sponsorshiprequests/{id}/cancel | Requestor | Cancel request |
| GET | /api/sponsorshiprequests/pending-manager | Manager | Manager queue |
| POST | /api/sponsorshiprequests/{id}/manager-approve | Manager | Approve |
| POST | /api/sponsorshiprequests/{id}/manager-reject | Manager | Reject |
| GET | /api/sponsorshiprequests/pending-finance | FinanceAdmin | Finance queue |
| POST | /api/sponsorshiprequests/{id}/finance-approve | FinanceAdmin | Final approve |
| POST | /api/sponsorshiprequests/{id}/finance-reject | FinanceAdmin | Final reject |
| GET | /api/sponsorshiprequests | SystemAdmin | All requests |
| GET | /api/sponsorshiptypes | All | List types |
| POST | /api/sponsorshiptypes | SystemAdmin | Create type |
| PUT | /api/sponsorshiptypes/{id} | SystemAdmin | Update type |

---

## Architecture Decisions & Tradeoffs

### What was implemented
- Full 4-role RBAC with JWT authentication
- Complete approval workflow: Draft → PendingManagerApproval → PendingFinanceReview → Approved/Rejected/Cancelled
- Audit trail via domain events (WorkflowHistory table)
- Clean Architecture with CQRS (MediatR)
- EF Core migrations with automatic seed data
- Swagger with JWT auth support
- Angular role-based routing with lazy-loaded feature modules
- Docker Compose for one-command startup

### What was deliberately simplified (tradeoffs)
- **File upload**: Optional per assessment; skipped to focus on workflow correctness
- **Email notifications**: Would add via a background service in production
- **Unit/integration tests**: Would add xUnit + Moq + Testcontainers in production
- **Refresh tokens**: Access tokens expire in 8 hours; production would add refresh token rotation
- **Pagination**: List endpoints return all records; production would add cursor-based pagination
- **Audit user lookup**: WorkflowHistory stores actorName as a string to avoid runtime joins
- **IdentityUser extension**: Using claims for FullName rather than extending IdentityUser for simplicity

### Why Clean Architecture over N-Tier
Clean Architecture enforces dependency inversion — the Domain layer has zero external dependencies. This means business rules can be tested without a database, and the Infrastructure can be swapped (e.g., switch from PostgreSQL to SQL Server) without touching domain logic.

### Why CQRS
Each workflow action (Approve, Reject, Submit, Cancel) has different validation rules and authorization requirements. CQRS makes each operation explicit and independently testable. MediatR's pipeline also supports cross-cutting concerns like validation behaviors.

### Why Domain Events for Audit
Rather than manually creating a WorkflowHistory record in every command handler, the `SponsorshipRequest.ChangeStatus()` method fires a `RequestStatusChangedEvent`. This keeps audit logic decoupled from business logic and ensures the audit trail is never missed.
