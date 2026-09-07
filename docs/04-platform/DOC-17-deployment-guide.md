# DOC-17 — Hướng dẫn Triển khai

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (DevOps/SA soạn) | **Chốt** (DEC-DLV-007) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — bỏ DR/DC theo **ADR-010** (DEC-ARC-017/018); thêm backup/restore |

**Runbook** · DOC-08 §4.4 · ADR-001/007/**010** **Accepted** *(ADR-003 §3–5 superseded)* · DOC-15 **Chốt** · DOC-16 **Chốt**.  
**Cổng:** PGD chốt v0.1 (DEC-DLV-007). Sửa runbook đã chốt = CR. Nợ: URL, sản phẩm LBS, **Lark issuer OIDC** (tenant/region), **PostgreSQL version/host prod**, **RTO-failover / RTO-restore**, chu kỳ + nơi lưu backup, lệnh CI. **Không** khóa K8s. **Không** tự code. **Chưa** `02-baseline/`. Go-live **2027**. Chốt tài liệu ≠ go-live.

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
| UAT | TBD | UAT sau LBS+GW | Gần Prod, không bắt buộc cặp A/S |
| PROD | TBD | 24/7 | **Một DC**: Active + Standby cùng DC; jobs chỉ trên Active |

> **Không còn môi trường DR.** Khách hàng yêu cầu bỏ DC dự phòng (ADR-010 · DEC-ARC-018). Mất cả DC → khôi phục bằng **backup/restore** (§2.2), không phải promote site.

### 2.1 PostgreSQL — Prod (OQ-DLV-003)

| Mục | Giá trị |
|-----|---------|
| **Engine** | PostgreSQL **16+** (ADR-009) |
| **Connection** | `ConnectionStrings:AppDbContext` — User Secrets / vault Prod · **không** commit password |
| **Template** | `hrm-backend/src/Hrm.Host/appsettings.Production.json` |
| **Format** | `Host={host};Port=5432;Database=hrm;Username={user};Password={secret};Pooling=true;SSL Mode=Require` |
| **Owner cung cấp** | IT/DBA: host, user, password, SSL policy |
| **Migrate** | `AutoMigrate` chỉ DEV — Prod: pipeline migrate riêng (TBD CI) |

Local DEV: Postgres.app · `hrm-backend/scripts/pg-local.sh` · User Secrets (xem `hrm-backend/README.md`).

### 2.2 Backup & Restore (thay cho DR/DC — ADR-010 §4)

Khách hàng yêu cầu bỏ DC dự phòng (DEC-ARC-018), nên đây là **cơ chế duy nhất** chống thảm họa mất DC.

| Mục | Giá trị |
|-----|---------|
| **Phạm vi** | Toàn bộ DB-per-service + secrets vault + cấu hình hạ tầng |
| **Chu kỳ** | **TBD** — Ops/IT |
| **Nơi lưu** | **Máy Standby, cùng DC** (ADR-010 §4 · DEC-ARC-019) — **không** off-site |
| **Mã hóa at-rest** | Bắt buộc (PII + lương) — thuật toán TBD, xem DOC-13 NFR-S06 |
| **Verify** | **Restore thử định kỳ** — backup chưa restore thử là backup chưa tồn tại. Tần suất TBD |
| **RTO-restore** | **TBD giờ** (NFR-012c) — đo bằng diễn tập, không ước lượng trên giấy |

> ⚠️ **RK-01 (ADR-010 · OQ-ARC-012) — chưa giải:** backup nằm **cùng DC** với Active. Cứu được hỏng ổ đĩa và hỏng dữ liệu logic, **không** cứu được mất cả DC — khi đó backup mất theo. Nghĩa là **NFR-012c hiện không thể đạt** và mất DC = **mất dữ liệu**, không phải ngừng phục vụ tạm thời.
>
> Khách đã ký văn bản xác nhận ở mức *"ngừng phục vụ đến khi restore"*. Nếu PGD chọn phương án (a) — chấp nhận mất dữ liệu — thì **phải xác nhận lại với khách ở đúng mức đó**. Phương án (b) là thêm một bản sao lạnh ngoài DC, rẻ hơn DR site nhiều bậc.

## 3. Điều kiện tiên quyết

| # | Item | Owner | Status |
|---|------|-------|--------|
| 1 | DOC-16 chương trình **Chốt** (DEC-DLV-004) + AC Must Pass | QC / PGD | ☑ DOC-16 · ☐ AC Pass |
| 2 | **Lark** OIDC issuer + JWKS + App credentials (ADR-007 v0.2) | IT | ☐ |
| 3 | Secrets Git/CRM/SMTP vault — không HR | IT | ☐ |
| 4 | LBS health → chỉ Active | DevOps | ☐ |
| 5 | **Backup job** chạy đúng chu kỳ + verify restore thử | DBA | ☐ |
| 6 | Rollback + failover **dry-run** | DevOps | ☐ |
| 7 | Job scheduler disable trên **Standby** (cùng DC) | DevOps | ☐ |
| 8 | Thông báo user | PM | ☐ |

### 3.1 Lark OIDC (INT-001 · DEC-DLV-010)

| Mục | Giá trị |
|-----|---------|
| **IdP** | Lark (Feishu) — mail `@lhqglobal.vn` |
| **Login MVP** | Google · Apple · mail công ty (IT cấu hình federation trên Lark) |
| **HRM cần từ IT** | Tenant Lark · region (CN/Global) · discovery/issuer URL · JWKS · App ID/secret · Audience |
| **Vault** | `Authentication:Jwt:Bearer:Authority` + client secret — **không** commit repo |
| **Map IAM** | JWT `sub` → `iam_identity_account.IdpSubject`; roles từ PostgreSQL |

## 4. Kiến trúc triển khai

```text
Client → LBS (chỉ Active) → GW (OIDC) → MS ×7 + Job + Notif
                              └── DB-per-service (backup định kỳ → backup store)
Job T-15/T-7/N+3: ON chỉ Active
```

Sản phẩm LBS / host **TBD**. Không giả định `kubectl`.

## 5. Các bước triển khai (Prod lần đầu / release)

| Step | Action | Command | Owner | Verify |
|------|--------|---------|-------|--------|
| 1 | Bảo trì / banner (nếu cần) | TBD | DevOps | User thấy |
| 2 | Backup N DB Active | TBD | DBA | Backup ID |
| 3 | Deploy **Standby** (app + migrate) | TBD CI | DevOps | Health Standby |
| 4 | Smoke Standby **nội bộ** (không cắt user) | TC-smoke | QC | Pass |
| 5 | Failover LBS → node mới Active | TBD | DevOps | `/iam/me` 200 |
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
| GW health | 200 | ☐ |
| OIDC `/iam/me` | 200 JWT | ☐ |
| NFR-002 | LM 403 phiếu cấp dưới | ☐ |
| INT-001 | Login **Lark** (Google/Apple/@lhqglobal.vn) | ☐ |
| INT-004/005 | Dry-run lock **UAT** trước Prod | ☐ |
| INT-006 | 0 request CRM sales | ☐ |
| Job trên Standby | Count = 0 | ☐ |
| APM | Không spike 5xx | ☐ |

## 8. Rollback

| Trigger | Action |
|---------|--------|
| Smoke fail | Failover về Active cũ trong **RTO-failover TBD phút** (NFR-012b) |
| Data lỗi | Stop LBS + restore backup N DB (thứ tự TBD) |
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
| IT IdP (Lark) / Git / CRM | SH-006 | TBD |
| BA | Trịnh Yên | TBD |

## 11. Ký duyệt

| Vai trò | Go / No-go | Ngày |
|---------|------------|------|
| Sponsor **(A)** | ☑ Chốt v0.1 (DEC-DLV-007) | 2026-08-26 |
| DevOps | ☐ Runbook khung; lệnh TBD | |
| QC | ☐ Smoke/go-live khi execute | |
| BA | Trịnh Yên 2026-08-26 soạn | |
