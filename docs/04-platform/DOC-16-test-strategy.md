# DOC-16 — Chiến lược Kiểm thử (chương trình HRM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA soạn) | **Chốt** (DEC-DLV-004) |
| 0.2 | 2026-09-18 | soạn nháp QC (trợ lý) | **Chốt** (DEC-ARC-035 · PGD) — theo **ADR-013** (một host, không GW/DB-per-service), **ADR-012** (login HRM — TC SSO bỏ, TC password thêm), **ADR-010** (không DR); ghi thực trạng test đã có trong `hrm/` (doc-review pass 3 B2) |

**ISTQB** levels/types. Phạm vi: 7 module Must đã có DOC-07 **Chốt**.  
**Gói DOC-16 Chốt:** chương trình + 7 file module (DEC-DLV-004). EVT/RPT chưa SRS — không TC.  
**Cổng:** PGD chốt v0.1 (DEC-DLV-004) · v0.2 (DEC-ARC-035). Sửa catalog/chiến lược đã chốt = CR. **Không** tự code. **Chưa** `02-baseline/`. EVT/RPT: chưa SRS → chưa TC Must.

> **Thực trạng 2026-09-18** (soi `hrm/`): unit **163** (Domain 11 · Application 143 · **Architecture 9**) · Playwright autotest API + UI (login DEV, luồng đăng nhập → dashboard) · script e2e API theo module (`memory/delivery/tc-run-*`). **Chưa có CI** — test chạy tay. Module `identity/DOC-16` còn TC OIDC Lark (Partial) → **phải sửa theo ADR-012** (ngoài slice này, owner QC).

---

## 1. Mục đích

Khóa **cách test** và **quy tắc trace** AC→TC. Mỗi AC Must trên DOC-07 phải có ≥1 TC (happy + negative đã có trên AC) trước UAT sign-off module.

Nguồn: 7× DOC-06/07 Chốt · DOC-12 v0.4 · DOC-10 v0.2 · DOC-13 v0.3.2 (NFR-S07…S13) · ADR-010 HA · ADR-012 login · ADR-013 ranh giới.

## 2. Phạm vi & ngoài phạm vi

| In | Out |
|----|-----|
| IAM, EMP, LEV, TIM, PAY, PRB, LIF | EVT, RPT (chưa SRS) |
| INT-002…005 *(adapter chưa code — test tới outbox)*, **cấm** INT-006 | Pixel HTML MCP · ~~INT-001~~ (bỏ, ADR-012) |
| NFR-001 (1000 dòng sau **LBS**), 403 lương, **hàng rào DB** (role không đọc chéo schema), **login password** NFR-S07…S11 | % uptime bịa; RTO phút chưa chốt; mobile (chưa code) |

## 3. Mức kiểm thử (ISTQB)

| Level | Owner | Mục |
|-------|-------|-----|
| Unit | Dev | Domain + Application (`unittest/`) — 154 test |
| **Architecture** | Dev | `Hrm.Architecture.Tests` — 9 test: context không reference chéo, allowlist closure `emp`/audit (ADR-011 W2). **Phải vào CI** (NFR-M03) |
| Integration | Dev/QC | API DOC-12 trên một host + **role test DB**: `psql -U hrm_app_lev` đọc `pay.*` → permission denied (OQ-ARC-011, 8/8) |
| System / E2E | QC | Client → LBS → `Hrm.Host` (Playwright `autotest/`); DEV dùng `/dev/login`, UAT/Prod dùng `POST /v1/iam/auth/login` (chưa code) |
| UAT | Ban HR + PGD | AC Must; NFR-001 sau LBS |
| Security | QC + IAM | 401 token giả/không `sub`; 403 lương; **login negative** (S07 blocklist, S08 khoá, S10 429, S11 reset hết hạn, một thông báo chung); `/dev/*` → 404 Prod; NFR-002/004/006/007 |
| HA | DevOps | Failover A/S **một DC**; job trên Standby → 422 (`Hrm:HostRole`); restore thử từ backup |

## 4. Quy tắc catalog (khi mở file module)

| Cột | Giá trị |
|-----|---------|
| TC ID | `{MOD}-TC-nnn` |
| Trace | FR + AC bắt buộc |
| Layer | UT / API / E2E |
| Path | Happy / Unhappy |
| Priority | Must theo AC Must |
| Trạng thái | trống đến khi chạy |

**Smoke go-live (DOC-17 §7):** `POST /v1/iam/auth/login` → `GET /v1/iam/me` 200; `POST /dev/login` → **404**; 1 đơn phép; 1 phiếu **của mình**; PRB 403 LM chốt; `/health` 7 `db-*` Healthy; role LEV không đọc `pay.*`; probe INT-006 = 0 call.

## 5. Ma trận phủ (khung)

| Module | DOC-07 | File TC module | Coverage |
|--------|--------|----------------|----------|
| IAM | Chốt | [identity/DOC-16](../03-modules/identity/DOC-16-test-strategy.md) **Chốt** — **còn TC OIDC Lark, phải sửa theo ADR-012** | catalog ◐; RBAC đã chạy 2026-09-04 (Partial) |
| EMP | Chốt | [employee-profile/DOC-16](../03-modules/employee-profile/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| LEV | Chốt | [leave/DOC-16](../03-modules/leave/DOC-16-test-strategy.md) **Chốt** | catalog AC Must; chưa chạy |
| TIM | Chốt | [timekeeping/DOC-16](../03-modules/timekeeping/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| PAY | Chốt | [payroll/DOC-16](../03-modules/payroll/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| PRB | Chốt | [probation/DOC-16](../03-modules/probation/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| LIF | Chốt | [lifecycle/DOC-16](../03-modules/lifecycle/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| INT-006 | NFR-007 | TC trên LEV/LIF/PRB/IAM | catalog ◐; chưa chạy |
| NFR-002 DB | ADR-011 W1 | role test `psql` (OQ-ARC-011) | **đã chạy** 2026-09-07/15 — 8/8 |
| Ranh giới code | ADR-011 W2 | `Hrm.Architecture.Tests` | **đã chạy** — 9/9 |
| NFR-S07…S11 login | DOC-13 v0.3.2 | identity/DOC-16 delta — **chưa có** | chờ CR-002 |

✅ chỉ khi file module có TC map đủ AC Must.

## 6. Môi trường

| Env | Mục đích |
|-----|----------|
| Dev | Unit / Architecture / API (Postgres.app `trust` — **không** kiểm được mật khẩu role; role test chỉ chứng minh GRANT) |
| UAT | E2E + UAT business — **sau** LBS, `pg_hba` scram, `ASPNETCORE_ENVIRONMENT != Development` |
| Prod | Smoke cutover only |

NFR-001 **không** đo localhost.

## 7. Entry / exit

| Gate | Entry | Exit |
|------|-------|------|
| Test module | DOC-07 Chốt; API path DOC-12 | Mọi AC Must có TC Pass |
| UAT chương trình | 7 module TC Pass; login password UAT (CR-002 đã code) | PGD + Ban HR |
| Go-live | DOC-17 dry-run + rollback | M6 2027 |

## 8. Defect

Theo glossary DOC-16 (Blocker/Major/Minor). Blocker 403 lương / role đọc chéo schema / CRM sales / job trên Standby / `/dev/login` 200 trên Prod → **chặn** go-live.

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-09-18 | ☑ **Chốt v0.2** (DEC-ARC-035) |
| QC | | | Catalog Chốt; chưa execute |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
