# Architecture — Hrm Backend

Nguồn convention: `ai-skills/fundamentals/` (`dotnet-clean-architecture`, `dotnet-structure`, `dotnet-ddd`, `dotnet-coding-convention`).

## Layers

| Layer | Responsibility |
|---|---|
| Domain.Shared | Constants, shared enums/value types (`Enums/`, `Constants/`, `AssemblyMarker`) |
| Domain | Entities, domain services, `IAppUnitOfWork` |
| Application | CQRS handlers via `AddCoreApplication` |
| Infrastructure | EF `AppDbContext`, Caching, BlobStoring |
| Host | HTTP, JWT, Swagger, Health, OTEL — composition root |

## Dependency / project refs (khớp fundamentals)

```text
Host → Application, Infrastructure (+ Jarvis Host packages)
Application → Domain (+ Jarvis.Application*)
Infrastructure → Domain (+ Jarvis.EntityFramework / Caching / BlobStoring)
Domain → Domain.Shared (+ Jarvis.Domain)
Domain.Shared → (Jarvis.Domain.Shared — shared kernel framework)
```

Composition root: `Program.cs` → `AddHostLayer()` → `AddApplicationLayer()` rồi `AddInfrastructureLayer()` (trong Infra: `AddDomainLayer` trước persistence).

## Jarvis

Monorepo ProjectReference tới `../../jarvis` (repo root packages, không dùng path `frameworks/`, không `Jarvis.DDD.*` cũ).

Thứ tự DI bắt buộc: `AddJarvisCaching()` → `AddEntityFramework()` → `AddCoreDbContext<AppDbContext>`.

## Decisions aligned

- ADR-001: .NET 9 + microservices (Host này là composition root đầu tiên; fan-out service sau).
- ADR-007: không password login API — chỉ `GET /v1/iam/me` + Bearer.
- ADR-009: PostgreSQL SoT — connection string qua User Secrets / env (`ConnectionStrings:AppDbContext`); không commit password. Chốt host thật: OQ-DLV-003.

## Nợ kỹ thuật (đối chiếu fundamentals — chưa bắt buộc sửa ngay)

| Mục | Trạng thái | Ghi chú |
|---|---|---|
| Cây `src/` + `tests/` + `*LayerExtension` | Khớp | Đúng mục 5.1–5.7 / 5.11 |
| Host không inject repo concrete | Khớp | Controller mỏng (`Ping`, `Me`) |
| Domain.Shared chỉ BCL | Lệch nhẹ | Có `ProjectReference` `Jarvis.Domain.Shared` (chấp nhận theo stack Jarvis) |
| Application / Infrastructure `FrameworkReference` AspNetCore | Lệch nhẹ | Cần `IHostApplicationBuilder`; có thể thu hẹp package Hosting.Abstr* sau |
| Folder `Entities/`, `Commands/`, `Features/`… | Nợ scaffold | Thêm khi có use case nghiệp vụ thật |
| `IAppUnitOfWork` ở Domain | Lệch nhẹ vs CA (UoW thường Application) | Giữ vì kế thừa `Jarvis.Domain.Repositories.IUnitOfWork` |
| NetArchTest dependency rule | Chưa có | Optional khi solution phình |
| Migrate DB / OIDC Authority thật | Blocked | OQ-DLV-003 / OQ-DLV-001 |
