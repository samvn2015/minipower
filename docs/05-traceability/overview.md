# Overview — HRM

> **Đọc trong ~30 giây** — rollup tiến độ dự án. Chi tiết requirement → [`trace-matrix.md`](trace-matrix.md) · từng DOC → [`doc-registry.md`](doc-registry.md).

| Meta | Giá trị |
|------|---------|
| **Cập nhật** | 2026-08-29 |
| **Người rollup** | Dư Hùng (PGD) · soạn trợ lý |
| **Nguồn sync** | DOC-03 · memory · DOC-08–12 Chốt · DOC-14/15 **Chốt** · DOC-16/17 **Chốt** · `hrm-backend` IAM+EMP slice · `hrm-web` MVP · **Lark IdP** (DEC-DLV-010) |

---

## Snapshot

| Chỉ số | Giá trị |
|--------|---------|
| **Phase hiện tại** | delivery |
| **Baseline** | chưa (`02-baseline/` trống; DOC-01–15 + DOC-16/17 **Chốt**, chưa BL) |
| **Module (in scope)** | 9 (8 Must + RPT Should) |
| **Module req 04–07 Chốt** | 7 (LEV, PAY, TIM, EMP, LIF, IAM, PRB) |
| **Module chưa SRS** | EVT, RPT |
| **FR đã baseline** | 0 |
| **FR đang phân tích** | 0 |
| **Blocker / nợ mở** | 6 (xem dưới) |
| **Code slice (execute)** | IAM ◐ + EMP ◐ + **hrm-web** ◐ — backend: IAM admin · EMP CRUD + org/HĐ · SCR-005/006 LM workflow · SaveChanges fix · `e2e-full.sh` · frontend MVP (SCR-001/002/005/006) · **Lark JWKS** nợ IT |

---

## Module × pipeline

Ký hiệu: `—` chưa · `◐` đang · `✓` xong (Chốt, chưa BL) · `BL` đã baseline.

| Module | Owner | Discovery | Req (04–07) | Arch slice | Plan | Delivery | Sign-off | Ghi chú |
|--------|-------|-----------|-------------|------------|------|----------|----------|---------|
| leave | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | OQ-010 Skip MVP; TC chưa chạy |
| payroll | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | 85% = PAY; TC chưa chạy |
| timekeeping | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | 1 mẫu Excel; TC chưa chạy |
| employee-profile | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | code ◐ org/HĐ/LM + `hrm-web` SCR-001/002/005/006; TC chưa chạy |
| lifecycle | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | Git/CRM N+3; job không DR |
| identity | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | ADR-007 · IdP Lark (DEC-DLV-010); code ◐ admin API + dev JWT; Lark JWKS nợ IT |
| probation | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | 85% không PRB; job không DR |
| events | BA | ✓ | — | ◐ | — | — | — | chưa SRS |
| hr-analytics | BA | ✓ | — | ◐ | — | — | — | Should; chưa SRS |

**Đạt từng cột khi:**

| Cột | Điều kiện |
|-----|-----------|
| Discovery | Có trong [`DOC-03`](../01-project/DOC-03-brd.md), in scope |
| Req | DOC-04–07 có FR Must + AC tương ứng |
| Arch slice | DOC-08/10/12 có phần liên quan module |
| Plan | FR Must đã vào DOC-14 |
| Delivery | DOC-16 module **Chốt** (catalog + §3); DOC-17 chương trình **Chốt** |
| Sign-off | `doc-registry` = Baseline hoặc có trong `02-baseline/` manifest |

Arch slice = `✓` khung vì DOC-08/10/11/12 **Chốt**. Delivery 7 Must = `✓` tài liệu (chưa execute TC, chưa BL).

---

## Công việc & milestone

### Milestones (tóm tắt)

| ID | Milestone | Deliverable | Target | Trạng thái |
|----|-----------|-------------|--------|------------|
| M1 | Discovery | DOC-01–03 Chốt | 2026-08-24 | done (chưa BL) |
| M2 | Requirements Must (7 module) | DOC-04–07 + khung 19 + DOC-13 | 2026-08-26 | done (chưa BL) |
| M3 | Architecture khung | DOC-08–12 | 2026-08-26 | **done** Chốt (chưa BL) |
| M4 | Planning | DOC-14/15 | 2026-08-26 | **done** Chốt (chưa BL) |
| M4b | Delivery docs | DOC-16 (7 Must) + DOC-17 | 2026-08-26 | **done** Chốt (chưa execute / chưa BL) |
| M4c | First code slice | IAM `hrm-backend` Host + Application + EF IAM | 2026-08-28 | **done** ◐ |
| M4d | IAM admin + EMP + dev E2E | Admin API · EMP CRUD slice · dev JWT · smoke script | 2026-08-28 | **done** ◐ (pushed) |
| M4e | EMP org/HĐ + LM workflow | OrgUnit · Contract · SCR-005/006 API · `e2e-full.sh` · SaveChanges fix | 2026-08-29 | **done** ◐ (local E2E OK) |
| M4f | Frontend MVP (`hrm-web`) | SCR-001/002/005/006 · dev login · proxy HTTPS | 2026-08-29 | **done** ◐ (local) |
| M5 | Go-live | Prod 24/7 | 2027 | planned |

→ Chi tiết: [`DOC-15`](../00-governance/DOC-15-project-plan.md) **Chốt** · WBS: [`DOC-14`](../04-platform/DOC-14-wbs-estimate.md) **Chốt**

### Việc 1–2 tuần tới

| Việc | Owner | Module | Due | Trạng thái |
|------|-------|--------|-----|------------|
| Chốt DOC-16 + DOC-17 | PGD | platform | 2026-08-26 | **done** (DEC-DLV-004, 007) |
| RTO/RPO phút (không bịa %) | SA / PGD | NFR-012 | | mở |
| Ban HR ký BO | Ban HR | all | 2026-08-26 | **done** (PGD) |
| EVT/RPT SRS nếu vào Must | BA / PGD | EVT, RPT | | chưa |
| Slice IAM `hrm-backend` (Jarvis + Application + EF) | DEV | identity | 2026-08-28 | **done** ◐ |
| IAM admin API (SCR-003/004) | DEV | identity | 2026-08-28 | **done** ◐ (accounts · roles · disable) |
| IAM persistence PostgreSQL (roles SoT) | DEV | identity | 2026-08-28 | **done** *(local)* · prod OQ-DLV-003 |
| EMP list/create/get/patch + unique guard | DEV | employee-profile | 2026-08-28 | **done** ◐ |
| EMP org/HĐ + SCR-005/006 LM workflow | DEV | employee-profile | 2026-08-29 | **done** ◐ (`e2e-full.sh` OK) |
| Frontend MVP `hrm-web` (SCR-001/002/005/006) | DEV | employee-profile | 2026-08-29 | **done** ◐ *(local)* |
| Dev JWT + E2E smoke/full | DEV | platform | 2026-08-29 | **done** *(local)* |
| Lark OIDC Issuer + JWKS (JWT 200 thật) | IT / SA | identity | | **mở** — OQ-DLV-001 · DEC-DLV-010 |
| **Không** `02-baseline/` / fan-out 6 MS | — | — | — | một slice |

---

## Blocker / TBD

| ID | Module / FR | Vấn đề | Owner | ETA | Tham chiếu |
|----|-------------|--------|-------|-----|------------|
| BLK-001 | LEV | OQ-010: LM/HR hủy hộ đơn chờ C1/C2? | PGD | | `memory/requirements/open-questions.md` |
| BLK-002 | NFR-012 | SLA / RTO / RPO — không bịa % | SA / PGD | | DOC-13 · DOC-08 §2 AG-010 |
| BLK-003 | EVT, RPT | Chưa SRS | BA | | DOC-03 index |
| BLK-004 | repo | Chưa `02-baseline/` | PGD | | discovery/req |
| BLK-005 | Ban HR | Đã ký BO (2026-08-26) | Ban HR | | DEC-DLV-008 |
| BLK-006 | IAM | ~~IdP SSO sản phẩm TBD~~ → **Lark** (DEC-DLV-010); Google/Apple/@lhqglobal.vn qua Lark | IT | | ADR-007 · OQ-ARC-004 |
| BLK-007 | IAM | Lark **issuer + JWKS + Audience** (JWT thật; tenant/region TBD) | IT | | OQ-DLV-001 · DEC-DLV-010 |

---

## Quy tắc cập nhật

| Ai | Cập nhật phần | Khi nào |
|----|---------------|---------|
| **PM** | Snapshot, milestones, 2 tuần tới | Sync định kỳ (~15 phút) |
| **BA (owner module)** | Dòng pipeline module mình, blocker | Cuối phiên requirements |
| **SA** | Cột Arch, TBD platform | Khi có slice API / integration |
| **Bất kỳ** | Đếm FR từ trace-matrix | Sau distill vào `docs/` |

**Không** ghi chi tiết FR, transcript, SRS dài vào file này — dùng `trace-matrix`, `03-modules/`, `brainstorm/`.
