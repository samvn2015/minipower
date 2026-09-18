# DOC-17 — Hướng dẫn Triển khai

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (DevOps/SA soạn) | **Chốt** (DEC-DLV-007) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — bỏ DR/DC theo **ADR-010** (DEC-ARC-017/018); thêm backup/restore |
| 0.3 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — thêm §2.3 bộ lập lịch job theo **ADR-005** (DEC-ARC-026) |
| 0.4 | 2026-09-16 | soạn nháp SA (trợ lý) | **Chốt** (DEC-ARC-031 · PGD) — **ADR-013**: một host, một DB 8 schema / 7 role, không Gateway; **ADR-012**: bỏ Lark, secret ký JWT, kiểm `/dev/*` đóng trên Prod |
| 0.4.1 | 2026-09-18 | soạn nháp SA (trợ lý) | **Chốt** *(sửa lỗi, không đổi quyết định)* — doc-review pass 3: **M1** key JWT đúng tên Jarvis, **xoá** `Authority` khỏi template Prod; **M11** bỏ cột `idempotencyKey` không tồn tại; **M10** thêm `Hrm:HostRole`; Minor path `/v1` |
| 0.4.2 | 2026-09-18 | soạn nháp SA (trợ lý) | **Chốt** *(sửa lỗi)* — pass 4 M3: Standby từ chối job = **400** |

**Runbook** · DOC-08 §4.4 v0.4 · **ADR-013** · **ADR-012** · **ADR-010** · ADR-009 · DOC-15 **Chốt** · DOC-16 **Chốt**.  
**Cổng:** PGD chốt v0.1 (DEC-DLV-007). Sửa runbook đã chốt = CR. Nợ: URL, sản phẩm LBS, **PostgreSQL host prod**, **RTO-failover / RTO-restore**, chu kỳ + nơi lưu backup, **lệnh CI / Dockerfile (chưa có)**, **hosting SPA**, **OTEL collector**, **luồng login Prod chưa code** (CR-002). **Không** khóa K8s. **Không** tự code. **Chưa** `02-baseline/`. Go-live **2027**. Chốt tài liệu ≠ go-live.

---

## 1. Tổng quan

| Mục | Giá trị |
|-----|---------|
| **System / Release** | HRM v1 · 2027 |
| **Deployment type** | **Active/Standby**: cài/smoke trên **Standby** → failover có kiểm soát (ADR-010 §2). Không bắt buộc blue-green. |
| **Maintenance window** | TBD 2027 TZ `Asia/Ho_Chi_Minh` |
| **Rollback decision maker** | PGD (A) + DevOps on-call |

## 2. Môi trường

| Env | URL | Purpose | Infra |
|-----|-----|---------|-------|
| DEV | TBD | Build | Node đơn |
| UAT | TBD | UAT sau LBS | Gần Prod, không bắt buộc cặp A/S |
| PROD | TBD | 24/7 | **Một DC**: Active + Standby cùng DC; jobs chỉ trên Active |

> **Không còn môi trường DR.** Khách hàng yêu cầu bỏ DC dự phòng (ADR-010 · DEC-ARC-018). Mất cả DC → khôi phục bằng **backup/restore** (§2.2), không phải promote site.

### 2.1 PostgreSQL — Prod (OQ-DLV-003)

| Mục | Giá trị |
|-----|---------|
| **Engine** | PostgreSQL **16+** (ADR-009) |
| **Connection** | **8 chuỗi**, không phải một (ADR-011 W1 · ADR-013): `ConnectionStrings:{Iam,Emp,Lev,Tim,Pay,Prb,Lif}DbContext` mỗi cái một role `hrm_app_*`, + `AppDbContext` bằng `hrm_migrator` (chỉ migrate). Vault Prod · **không** commit password |
| ⚠️ **Bẫy** | `AddContextConnection` **rơi về chuỗi `AppDbContext`** nếu thiếu chuỗi của context → app chạy bằng `hrm_migrator`, **hàng rào NFR-002 biến mất im lặng**. Prod phải có **guard khởi động** từ chối thiếu chuỗi (chưa code — nợ) và checklist §3 kiểm đủ 8 |
| **Role / GRANT** | DBA chạy `hrm/backend/scripts/w1-roles.sql` **sau khi đổi mọi `CHANGE_ME_*`**; `pg_hba.conf` = `scram-sha-256` (local dev dùng `trust` — **không** mang lên Prod) |
| **Template** | `hrm/backend/src/Hrm.Host/appsettings.Production.json` — hiện chỉ có `AppDbContext` + 3 placeholder TBD; **chưa chạy được** |
| **Format** | `Host={host};Port=5432;Database=hrm;Username={user};Password={secret};Pooling=true;SSL Mode=Require` |
| **Owner cung cấp** | IT/DBA: host, user, password, SSL policy |
| **Migrate** | `AutoMigrate` chỉ DEV — Prod: `dotnet ef database update --context AppDbContext` bằng `hrm_migrator`, chạy trên Standby trước failover (§5). Pipeline TBD CI |

Local DEV: Postgres.app · `hrm/backend/scripts/pg-local.sh` · User Secrets id `4d509ed6-…` (8 chuỗi — `hrm/backend/README` mục *Kết nối*). *(Path tính từ gốc workspace `Học AI/`, không phải từ `docs/04-platform/`.)*

### 2.2 Backup & Restore (thay cho DR/DC — ADR-010 §4)

Khách hàng yêu cầu bỏ DC dự phòng (DEC-ARC-018), nên đây là **cơ chế duy nhất** chống thảm họa mất DC.

| Mục | Giá trị |
|-----|---------|
| **Phạm vi** | **Một** database `hrm` (8 schema — backup một lần, không có thứ tự N DB) + secrets vault (chuỗi kết nối, `IssuerSigningKeys`) + cấu hình hạ tầng |
| **Chu kỳ** | **TBD** — Ops/IT |
| **Nơi lưu** | **Máy Standby, cùng DC** (ADR-010 §4 · DEC-ARC-019) — **không** off-site |
| **Mã hóa at-rest** | Bắt buộc (PII + lương) — thuật toán TBD, xem DOC-13 NFR-S06 |
| **Verify** | **Restore thử định kỳ** — backup chưa restore thử là backup chưa tồn tại. Tần suất TBD |
| **RTO-restore** | **TBD giờ** (NFR-012c) — đo bằng diễn tập, không ước lượng trên giấy |

> ⚠️ **RK-01 (ADR-010 · OQ-ARC-012) — chưa giải:** backup nằm **cùng DC** với Active. Cứu được hỏng ổ đĩa và hỏng dữ liệu logic, **không** cứu được mất cả DC — khi đó backup mất theo. Nghĩa là **NFR-012c hiện không thể đạt** và mất DC = **mất dữ liệu**, không phải ngừng phục vụ tạm thời.
>
> Khách đã ký văn bản xác nhận ở mức *"ngừng phục vụ đến khi restore"*. Nếu PGD chọn phương án (a) — chấp nhận mất dữ liệu — thì **phải xác nhận lại với khách ở đúng mức đó**. Phương án (b) là thêm một bản sao lạnh ngoài DC, rẻ hơn DR site nhiều bậc.

### 2.3 Bộ lập lịch job (ADR-005 · RK-06)

Backend **không có** `BackgroundService`/`IHostedService` nào (soi code 2026-09-07). Job chạy bằng endpoint, do một bộ lập lịch **bên ngoài** gọi:

| Job | Endpoint | Nghiệp vụ |
|---|---|---|
| PRB nhắc T-15 / T-7 | `POST /v1/prb/jobs/reminders/run` | NFR-009 — 0 sót 0 trễ |
| LIF khoá N+3 | `POST /v1/lif/offboarding/jobs/nplus3-locks` | INT-004/005 |

| Ràng buộc | Giá trị |
|---|---|
| **Chỉ chạy trên Active** | Bộ lập lịch trỏ vào **LBS**, LBS chỉ bơm vào Active (ADR-010 §5) — không trỏ thẳng node |
| **Idempotent theo ngày** | Gọi lại cùng ngày **không** được sinh nhắc trùng. **PRB**: `ExistsAsync` + unique `(EmployeeId, Kind, ProbationEndDate)` trên `prb_probation_reminder`, có unit test lần 2 → `SkippedAlreadyExists` — **RK-07 đóng**. **LIF N+3**: bỏ qua case đã `GitLockedAtUtc && CrmSpLockedAtUtc` — *không* có cột `idempotencyKey` (v0.3–0.4 ghi sai) |
| **Sản phẩm lập lịch** | **TBD** — cron OS / Jenkins / Jarvis worker (`OQ-ARC-017`) |
| **Tần suất** | **TBD** |
| **Giám sát** | **TBD** — job không chạy phải có cảnh báo, nếu không NFR-009 im lặng hỏng |

> ⚠️ Đây là **thành phần ngoài hệ thống** nhưng NFR-009 phụ thuộc vào nó. Chưa chốt ba dòng TBD trên thì cảnh báo TV/SN/lễ chưa có gì bảo đảm.

## 3. Điều kiện tiên quyết

| # | Item | Owner | Status |
|---|------|-------|--------|
| 1 | DOC-16 chương trình **Chốt** (DEC-DLV-004) + AC Must Pass | QC / PGD | ☑ DOC-16 · ☐ AC Pass |
| 2 | **JWT Prod** — trong `Authentication:Jwt:Bearer:*` (tên key theo `Jarvis.Authentication.Jwt` — `AuthenticationJwtOption`): `IssuerSigningKeys` (mảng, ≥ 1 chuỗi ≥ 32 byte, sinh mới, **không** phải `DevAuthController.DefaultSigningKey`), `ValidIssuers`, `ValidAudiences`, `ValidateIssuerSigningKey/Issuer/Audience = true`. **`Authority` phải XOÁ / rỗng** — Jarvis thấy `Authority` là bỏ qua `IssuerSigningKeys` và validate qua metadata OIDC (`AuthenticationBuilderExtension.cs:141-163`). `Audience` (số ít) chỉ có tác dụng kèm `Authority` — không dùng | IT | ☐ |
| 2a | `ASPNETCORE_ENVIRONMENT=Production` trên **mọi** node — sai là `POST /dev/login` / `GET /dev/token` mở với mật khẩu plaintext (ADR-012 §5 · RK-09) | DevOps | ☐ |
| 2b | **8** connection string + `Cors` origin Prod + OTEL endpoint trong vault. `appsettings.Production.json` hiện còn **`Authority: TBD-LARK-OIDC-ISSUER` + `Audience`** — hai dòng này **xoá**, không "thay"; thay `TBD-PROD-HOST/TBD` bằng vault (**Dev sửa template — nợ**) | DevOps + Dev | ☐ |
| 2e | **`Hrm:HostRole`** = `Standby` trên node Standby, `Active` trên Active. Mặc định code là **Active** khi rỗng (`HostRoleGate.cs`) → node Standby quên key sẽ chạy job song song. Failover phải **lật** key trên cả hai node | DevOps | ☐ |
| 2c | Luồng login Prod **đã code** (CR-002) — chính sách DOC-13 S07…S10 chốt, IAM DOC-06/07 có | Dev / PGD | ☐ |
| 2d | Hosting **SPA** frontend (static + reverse proxy `/v1` → host) — sản phẩm TBD | DevOps | ☐ |
| 3 | Secrets Git/CRM/SMTP vault — không HR | IT | ☐ |
| 4 | LBS health → chỉ Active | DevOps | ☐ |
| 5 | **Backup job** chạy đúng chu kỳ + verify restore thử | DBA | ☐ |
| 6 | Rollback + failover **dry-run** | DevOps | ☐ |
| 7 | Job scheduler disable trên **Standby** (cùng DC) | DevOps | ☐ |
| 8 | Thông báo user | PM | ☐ |

### 3.1 Xác thực (ADR-012 — thay cho Lark OIDC)

| Mục | Giá trị |
|-----|---------|
| **Cơ chế** | Username/password **HRM tự quản** → JWT **HRM ký**. Không IdP, không JWKS, không `Authority` |
| **Vault** | `Authentication:Jwt:Bearer:IssuerSigningKeys[]` · `ValidIssuers[]` · `ValidAudiences[]` — **không** commit repo. **Không có `Authority`.** Xoay khoá = mọi token cũ hết hạn (chấp nhận; TTL/logout/xoay → nợ NFR-S13 DOC-13) |
| **Map IAM** | JWT `sub` → `iam_identity_account.IdpSubject` (tên cột giữ); roles từ schema `iam` |
| **Cấm trên Prod** | `POST /dev/login` · `GET /dev/token` — phải trả **404** (kiểm §7). Không bao giờ dùng `DevAuth:Accounts` ngoài Development |
| **Chưa có** | `POST /v1/iam/auth/login` / `change-password` / `reset-password` — CR-002. **Không go-live được** khi chưa có |

## 4. Kiến trúc triển khai

```text
Client (SPA tĩnh + mobile) → LBS/TLS (chỉ Active) → Hrm.Host ×1 (Active) [+ Standby nóng]
                                                       └── PostgreSQL `hrm`: 8 schema, 7 role app + migrator
                                                            └── backup định kỳ → backup store (cùng DC — RK-01)
Bộ lập lịch (ngoài) → LBS → POST /v1/…/jobs/…   (chỉ Active — ADR-010 §5)
```

**Một** deploy unit (`Hrm.Host`) + SPA tĩnh. **Không** Gateway (ADR-013). Sản phẩm LBS / host **TBD**. Không giả định `kubectl`. Scale ngang = thêm bản `Hrm.Host` sau LBS (JWT stateless).

## 5. Các bước triển khai (Prod lần đầu / release)

| Step | Action | Command | Owner | Verify |
|------|--------|---------|-------|--------|
| 1 | Bảo trì / banner (nếu cần) | TBD | DevOps | User thấy |
| 2 | Backup DB `hrm` trên Active (một DB) | TBD | DBA | Backup ID |
| 3 | Deploy **Standby**: `Hrm.Host` + `dotnet ef database update --context AppDbContext` bằng `hrm_migrator`; nếu GRANT đổi → chạy `w1-roles.sql` phần thay đổi | TBD CI | DevOps / DBA | `/health` readiness 7 `db-*` Healthy |
| 4 | Smoke Standby **nội bộ** (không cắt user) | TC-smoke | QC | Pass |
| 5 | Failover LBS → node mới Active; **lật `Hrm:HostRole`** (mới = Active, cũ = Standby) | TBD | DevOps | `GET /v1/iam/me` 200 · job trên node cũ trả **400** `Host Standby` |
| 6 | Job ON chỉ Active mới; OFF cũ | TBD | DevOps | 0 job trên Standby |
| 7 | Smoke Prod: phép, phiếu mình, 403 lương LM, 0 INT-006 | DOC-16 smoke | QC | Pass |
| 8 | Tắt bảo trì | TBD | DevOps | |

## 6. Di chuyển dữ liệu

| Step | Action | Reconcile | Rollback |
|------|--------|-----------|----------|
| 1 | Load master quy chế / mẫu CC | Count catalog | Restore backup |
| 2 | EMP unique CCCD/email/MST | Report trùng | Restore |

Quy tắc as-is **động** (DEC-DIS-014) — không đóng file nguồn trên runbook.

## 7. Xác minh sau triển khai

| Check | Expected | Pass |
|-------|----------|------|
| `/health` liveness + readiness | 200, **7** check `db-iam…db-lif` Healthy | ☐ |
| Login `POST /v1/iam/auth/login` → `GET /v1/iam/me` | 200 JWT *(khi CR-002 đã code)* | ☐ |
| **`POST /dev/login` · `GET /dev/token`** | **404** — nếu 200/400 là sai `ASPNETCORE_ENVIRONMENT`, **dừng go-live** | ☐ |
| Token giả / không `sub` | 401 | ☐ |
| NFR-002 (app) | LM 403 phiếu cấp dưới | ☐ |
| NFR-002 (DB) | `psql -U hrm_app_lev -c 'select 1 from pay.pay_line limit 1'` → *permission denied*; `pg_stat_activity` thấy **7** role `hrm_app_*`, **không** thấy `hrm_migrator` từ app | ☐ |
| INT-004/005 | Dry-run lock **UAT** trước Prod | ☐ |
| INT-006 | 0 request CRM sales | ☐ |
| Job trên Standby | `POST /v1/prb/jobs/reminders/run` vào node Standby → **400** `Host Standby` (`BadRequestException`); `pg_stat_activity` không có job từ Standby | ☐ |
| APM | Không spike 5xx | ☐ |

## 8. Rollback

| Trigger | Action |
|---------|--------|
| Smoke fail | Failover về Active cũ trong **RTO-failover TBD phút** (NFR-012b) |
| Data lỗi | Stop LBS + restore backup DB `hrm` (một DB — không có thứ tự) |
| **Mất cả DC** | Không còn site để promote, và backup nằm cùng DC (RK-01) → **hiện không có đường khôi phục**. Chờ PGD quyết OQ-ARC-012 trước khi viết runbook |

| Step | Action | Owner |
|------|--------|-------|
| 1 | LBS về phiên bản/node trước | DevOps |
| 2 | Job ON đúng Active cũ | DevOps |
| 3 | Restore DB nếu đã migrate | DBA |
| 4 | Smoke | QC |
| 5 | Báo PGD / HR | PM |

## 9. Hypercare

| Period | Support | Escalation |
|--------|---------|------------|
| Ngày 1–7 sau go-live 2027 | On-call 24/7 (ADR-010 §1) | PGD · IT |
| Roster | TBD | |

## 10. Liên hệ

| Role | Name | Kênh |
|------|------|------|
| Sponsor | Mr. Dư Hùng, PGD | TBD |
| DevOps on-call | TBD | TBD |
| IT Git / CRM / vault (không còn IdP — ADR-012) | SH-006 | TBD |
| BA | Trịnh Yên | TBD |

## 11. Ký duyệt

| Vai trò | Go / No-go | Ngày |
|---------|------------|------|
| Sponsor **(A)** | ☑ Chốt v0.1 (DEC-DLV-007) | 2026-08-26 |
| Sponsor **(A)** | ☑ **Chốt v0.4** (DEC-ARC-031) | 2026-09-16 |
| DevOps | ☐ Runbook khung; lệnh TBD | |
| QC | ☐ Smoke/go-live khi execute | |
| BA | Trịnh Yên 2026-08-26 soạn | |
