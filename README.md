# SmeOpsHub

A small **modular monolith** built with **ASP.NET Core MVC + EF Core** to demonstrate real-world skills: feature modules, clean boundaries, soft delete, authentication/authorization, audit trail, and admin tooling.

## Tech Stack
- ASP.NET Core MVC
- Entity Framework Core (SQL Server)
- ASP.NET Core Identity (roles + policies)
- Serilog (request + domain/audit logging)

## Key Features
- **Modular architecture**: modules are discovered at startup and plug into the app (CRM, Projects, HR)
- **Soft delete everywhere**: global query filters hide deleted records; restore supported
- **Authentication & Authorization**: Identity + roles (Admin/Manager/User), policies for restricted actions
- **Audit trail**: DB-backed audit events (soft delete/restore + approvals) with admin audit pages
- **Admin Recycle Bin**: view and restore deleted Companies/Projects/Employees

## Modules
- **CRM**: Companies, Contacts, Activities
- **Projects**: Projects + Tasks (optional link to CRM contacts)
- **HR**: Employees + Leave Requests (approve/reject workflow)

## Solution Structure
- `SmeOpsHub.Web` — MVC app (composition root, routing, UI)
- `SmeOpsHub.SharedKernel` — shared abstractions (base entities, value objects, soft delete, security)
- `SmeOpsHub.Modules.Crm` — CRM module (domain + EF configs + services + MVC area)
- `SmeOpsHub.Modules.Projects` — Projects module
- `SmeOpsHub.Modules.Hr` — HR module
- `SmeOpsHub.Infrastructure` — EF Core DbContext, migrations, interceptors, identity persistence