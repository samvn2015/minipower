# Overview — HRM

> **Đọc trong ~30 giây** — rollup tiến độ dự án. Chi tiết requirement → [`trace-matrix.md`](trace-matrix.md) · từng DOC → [`doc-registry.md`](doc-registry.md).

| Meta | Giá trị |
|------|---------|
| **Cập nhật** | 2026-09-04 |
| **Người rollup** | Dư Hùng (PGD) · soạn trợ lý |
| **Nguồn sync** | DOC-03 · memory/delivery · DOC-08–17 **Chốt** · 7 Must code · TC-run · DOC-16 St v0.2 execute · **UAT DEV Must §1–§7 Pass** · DEC-DLV-010/011 · PR #34–#41 |

---

## Snapshot

| Chỉ số | Giá trị |
|--------|---------|
| **Phase hiện tại** | delivery |
| **Baseline** | chưa (`02-baseline/` trống; DOC-01–17 **Chốt**, chưa BL) |
| **Module (in scope)** | 9 (8 Must + RPT Should) |
| **Module req 04–07 Chốt** | 7 (LEV, PAY, TIM, EMP, LIF, IAM, PRB) |
| **Module chưa SRS** | EVT, RPT |
| **FR đã baseline** | 0 |
| **FR đang phân tích** | 0 |
| **Blocker / nợ mở** | 5 (xem dưới) — JWKS **không** chặn DEV/UAT (DEC-DLV-011) |
| **Code / UAT** | 7 Must trên `main` · e2e · DOC-16 St v0.2 · **UAT DEV Must Pass** (checklist) · **Prod:** Lark JWKS + PG host + RTO còn mở |

---

## Module × pipeline

Ký hiệu: `—` chưa · `◐` đang · `✓` xong (Chốt, chưa BL) · `BL` đã baseline.

| Module | Owner | Discovery | Req (04–07) | Arch slice | Plan | Delivery | Sign-off | Ghi chú |
|--------|-------|-----------|-------------|------------|------|----------|----------|---------|
| leave | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §3 Pass · OQ-010 Skip · lịch lễ MVP |
| payroll | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §5 Pass · C&B Δ=0 |
| timekeeping | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §4 Pass · cấm máy CC |
| employee-profile | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §2 Pass |
| lifecycle | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §7 Pass · HA Standby nợ · SCR Partial |
| identity | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §1 Pass · OIDC Lark Partial (DEV bypass) |
| probation | BA | ✓ | ✓ | ✓ | ✓ | ✓ | — | UAT §6 Pass · HA job Open · SCR Partial |
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

Arch = `✓` khung. Delivery 7 Must = tài liệu + execute + **UAT DEV** (chưa BL / chưa go-live / chưa ký UAT formal).

---

## Công việc & milestone

### Milestones (tóm tắt)

| ID | Milestone | Deliverable | Target | Trạng thái |
|----|-----------|-------------|--------|------------|
| M1 | Discovery | DOC-01–03 Chốt | 2026-08-24 | done (chưa BL) |
| M2 | Requirements Must (7 module) | DOC-04–07 + khung 19 + DOC-13 | 2026-08-26 | done (chưa BL) |
| M3 | Architecture khung | DOC-08–12 | 2026-08-26 | **done** Chốt (chưa BL) |
| M4 | Planning | DOC-14/15 | 2026-08-26 | **done** Chốt (chưa BL) |
| M4b | Delivery docs | DOC-16 (7 Must) + DOC-17 | 2026-08-26 | **done** Chốt |
| M4c–g | IAM→web + TC early | slices + e2e-web | 2026-08-29 | **done** ◐ |
| M4h | 7 Must code + Must gaps | LEV…LIF · PR #32–#35 | 2026-09-04 | **done** ◐ (`main`) |
| M4i | DOC-16 execute St | St v0.2 · TC-run · PR #36–#39 | 2026-09-04 | **done** ◐ |
| M4j | UAT DEV Must | checklist §1–§7 · PR #40–#41 · persona fixes | 2026-09-04 | **done** ◐ (API+§1 UI; ký formal tuỳ chọn) |
| M5 | Go-live | Prod 24/7 | 2027 | planned — JWKS · PG host · RTO/RPO |

→ Chi tiết: [`DOC-15`](../00-governance/DOC-15-project-plan.md) **Chốt** · WBS: [`DOC-14`](../04-platform/DOC-14-wbs-estimate.md) **Chốt**  
→ Evidence: [`tc-run-2026-09-04.md`](../../memory/delivery/tc-run-2026-09-04.md) · [`uat-checklist-must-2026-09-04.md`](../../memory/delivery/uat-checklist-must-2026-09-04.md)

### Việc 1–2 tuần tới

| Việc | Owner | Module | Due | Trạng thái |
|------|-------|--------|-----|------------|
| UAT DEV Must (checklist) | QC / PGD | all Must | 2026-09-04 | **done** ◐ — §1–§7 Pass; ký formal ☐ |
| Overview sync | PM | platform | 2026-09-04 | **done** (rollup này) |
| RTO/RPO phút (không bịa %) | SA / PGD | NFR-012 | | mở |
| Prod PostgreSQL host (OQ-DLV-003) | IT | platform | | mở |
| Lark issuer + JWKS Prod | IT / SA | identity | | **mở** — DEV bypass DEC-DLV-011 |
| EVT/RPT SRS nếu vào Must | BA / PGD | EVT, RPT | | chưa |
| `02-baseline/` | PGD | all | | **chưa** |
| Siết Partial SCR / HA Standby | DEV | PRB · LIF | | nợ mỏng |

---

## Blocker / TBD

| ID | Module / FR | Vấn đề | Owner | ETA | Tham chiếu |
|----|-------------|--------|-------|-----|------------|
| BLK-001 | LEV | OQ-010 hủy hộ → **Skip MVP** | PGD | | DOC-16 LEV-TC-019 |
| BLK-002 | NFR-012 | SLA / RTO / RPO — không bịa % | SA / PGD | | DOC-13 · DOC-08 |
| BLK-003 | EVT, RPT | Chưa SRS | BA | | DOC-03 |
| BLK-004 | repo | Chưa `02-baseline/` | PGD | | |
| BLK-005 | Ban HR | Đã ký BO (2026-08-26) | Ban HR | | DEC-DLV-008 |
| BLK-006 | IAM | IdP = **Lark** (DEC-DLV-010) | IT | | ADR-007 |
| BLK-007 | IAM | Lark JWKS Prod — **không chặn DEV/UAT** (DEC-DLV-011) | IT | | OQ-DLV-001 |

---

## Quy tắc cập nhật

| Ai | Cập nhật phần | Khi nào |
|----|---------------|---------|
| **PM** | Snapshot, milestones, 2 tuần tới | Sync định kỳ (~15 phút) |
| **BA (owner module)** | Dòng pipeline module mình, blocker | Cuối phiên requirements |
| **SA** | Cột Arch, TBD platform | Khi có slice API / integration |
| **Bất kỳ** | Đếm FR từ trace-matrix | Sau distill vào `docs/` |

**Không** ghi chi tiết FR, transcript, SRS dài vào file này — dùng `trace-matrix`, `03-modules/`, `brainstorm/`.
