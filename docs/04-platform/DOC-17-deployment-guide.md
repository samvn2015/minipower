# DOC-17 — Hướng dẫn Triển khai

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (DevOps/SA soạn) | **Chốt** (DEC-DLV-007) |

**Runbook** · DOC-08 §4.4 · ADR-001/003/007 **Accepted** · DOC-15 **Chốt** · DOC-16 **Chốt**.  
**Cổng:** PGD chốt v0.1 (DEC-DLV-007). Sửa runbook đã chốt = CR. Nợ: URL, sản phẩm LBS, **Lark issuer OIDC** (tenant/region), **PostgreSQL version/host prod**, RTO phút, lệnh CI. **Không** khóa K8s. **Không** tự code. **Chưa** `02-baseline/`. Go-live **2027**. Chốt tài liệu ≠ go-live.

---

## 1. Tổng quan

| Mục | Giá trị |
|-----|---------|
| **System / Release** | HRM v1 · 2027 |
| **Deployment type** | **Active/Standby**: cài/smoke trên **Standby** → failover có kiểm soát (ADR-003). Không bắt buộc blue-green. |
| **Maintenance window** | TBD 2027 TZ `Asia/Ho_Chi_Minh` |
| **Rollback decision maker** | PGD (A) + DevOps on-call |

## 2. Môi trường

| Env | URL | Purpose | Infra |
|-----|-----|---------|-------|
| DEV | TBD | Build | 1 DC, không đủ DR |
| UAT | TBD | UAT sau LBS+GW | Gần Prod, DR optional |
| PROD | TBD | 24/7 | DC-Prod Active + DC-DR Standby |
| DR | TBD | Standby | Jobs **OFF** đến promote |

### 2.1 PostgreSQL — Prod (OQ-DLV-003)

| Mục | Giá trị |
|-----|---------|
| **Engine** | PostgreSQL **16+** (ADR-009) |
| **Connection** | `ConnectionStrings:AppDbContext` — User Secrets / vault Prod · **không** commit password |
| **Template** | `../hrm/backend/src/Hrm.Host/appsettings.Production.json` |
| **Format** | `Host={host};Port=5432;Database=hrm;Username={user};Password={secret};Pooling=true;SSL Mode=Require` |
| **Owner cung cấp** | IT/DBA: host, user, password, SSL policy |
| **Migrate** | `AutoMigrate` chỉ DEV — Prod: pipeline migrate riêng (TBD CI) |

Local DEV: Postgres.app · `../hrm/backend/scripts/pg-local.sh` · User Secrets (xem `../hrm/README.md`).

## 3. Điều kiện tiên quyết

| # | Item | Owner | Status |
|---|------|-------|--------|
| 1 | DOC-16 chương trình **Chốt** (DEC-DLV-004) + AC Must Pass | QC / PGD | ☑ DOC-16 · ☐ AC Pass |
| 2 | **Lark** OIDC issuer + JWKS + App credentials (ADR-007 v0.2) | IT | ☐ |
| 3 | Secrets Git/CRM/SMTP vault — không HR | IT | ☐ |
| 4 | LBS health → chỉ Active | DevOps | ☐ |
| 5 | Replicate N DB Prod→DR | DBA | ☐ |
| 6 | Rollback + failover **dry-run** | DevOps | ☐ |
| 7 | Job scheduler disable trên Standby/DR | DevOps | ☐ |
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
                              └── DB-per-service (replicate → DR)
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
| 6 | Job ON chỉ Active mới; OFF cũ | TBD | DevOps | 0 job trên DR |
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
| Job trên DR | Count = 0 | ☐ |
| APM | Không spike 5xx | ☐ |

## 8. Rollback

| Trigger | Action |
|---------|--------|
| Smoke fail | Failover về Active cũ trong RTO **TBD phút** |
| Data lỗi | Stop LBS + restore backup N DB (thứ tự TBD) |

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
| Ngày 1–7 sau go-live 2027 | On-call 24/7 (ADR-003) | PGD · IT |
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
