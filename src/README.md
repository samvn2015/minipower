# HRM source — nháp thực thi

**Composition root mới:** [`../hrm-backend`](../hrm-backend) — Clean Architecture 5 lớp + Jarvis (ProjectReference).

**Slice IAM cũ (tham chiếu):** `iam/` — thin Host trước khi scaffold đầy đủ.

- Không 7 service cùng lúc. Không login password.
- JarvisRoot: `/Users/Hung/Documents/Học AI/jarvis`
- OIDC Authority: OQ-DLV-001 · PostgreSQL connection: OQ-DLV-003

```bash
cd HRM/hrm-backend/src
dotnet run --project Hrm.Host
```

- Swagger: https://localhost:7006/swagger
- `GET /api/ping` → 200
- `GET /v1/iam/me` không Bearer → **401**
