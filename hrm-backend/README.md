# Hrm Backend

ASP.NET Core **.NET 9** API — Clean Architecture 5 lớp + [Jarvis](../../jarvis) (clone `hoangnh2412/jarvis`).

Convention tham chiếu: [`ai-skills/fundamentals`](../../ai-skills/fundamentals) (`dotnet-structure`, `dotnet-clean-architecture`, `dotnet-coding-convention`). Chi tiết lớp / nợ kỹ thuật: [`docs/Architecture.md`](docs/Architecture.md). Root có `.editorconfig` tối thiểu.

## Cấu trúc

```text
src/
├── Hrm.Domain.Shared   ← Enums/, Constants/, AssemblyMarker
├── Hrm.Domain
├── Hrm.Application
├── Hrm.Infrastructure
└── Hrm.Host          ← F5 startup project
```

## Chạy local

```bash
cd HRM/hrm-backend/src
# Một lần: User Secrets (không commit password)
dotnet user-secrets set "ConnectionStrings:AppDbContext" \
  "Host=localhost;Port=5432;User ID=<user>;Password=<secret>;Database=hrm;Pooling=true;" \
  --project Hrm.Host
dotnet run --project Hrm.Host
```

Hoặc env: `ConnectionStrings__AppDbContext=...`

- Swagger: https://localhost:7006/swagger
- `GET /api/ping` — không cần DB
- `GET /v1/iam/me` — cần Bearer JWT (`[Authorize]`)
- `GET /v1/iam/accounts` — IAM-SCR-003 (HR/IT)
- `POST /v1/iam/accounts/{id}/roles` · `DELETE .../roles/{code}` — gán/gỡ role
- `POST /v1/iam/accounts/{id}/disable` — IAM-SCR-004 (IT)
- `GET/PATCH /v1/emp/employees/{id}` — EMP skeleton
- `GET /v1/emp/employees` · `POST /v1/emp/employees` — list/tạo NV (HR/IT)
- `/dev/token?sub=local-dev` — JWT dev (Development only)
- `./scripts/e2e-smoke.sh` — smoke E2E local
- `/health/live` — liveness (không cần DB)

## Ghi chú HRM

- JWT OIDC **Lark** (ADR-007 v0.2): Authority thật = **OQ-DLV-001** (IT). **Development:** symmetric key trong `appsettings.Development.json` + `GET /dev/token?sub=...`. Dev seed: `sub=local-dev` (HR+NV), `sub=it-dev` (IT), EMP `MNV-DEV` ↔ `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`.
- PostgreSQL **local DEV:** Postgres.app 16.8 · `scripts/pg-local.sh start|stop|status` · DB `hrm` / user `admin` qua User Secrets. Prod connection vẫn OQ-DLV-003 (IT).
- Host IAM cũ `HRM/src/iam` giữ để tham chiếu; composition root mới là `hrm-backend`.

## Jarvis mapping (clone này)

| Layer | Package (ProjectReference) |
|---|---|
| Domain.Shared | `Jarvis.Domain.Shared` |
| Domain | `Jarvis.Domain` |
| Application | `Jarvis.Application` + Contracts |
| Infrastructure | `Jarvis.EntityFramework` + Caching + BlobStoring |
| Host | Mvc, Auth/Jwt, Swashbuckle, HealthChecks, OpenTelemetry |
