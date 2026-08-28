# DOC-15 — Kế hoạch Dự án

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn) | **Chốt** (DEC-PLN-002) |

**IEEE 1058 SPMP**. WBS: [`DOC-14`](../04-platform/DOC-14-wbs-estimate.md) **Chốt**.  
**Cổng:** PGD chốt v0.1 (DEC-PLN-002). **Không** tự DOC-16/17. **Chưa** `02-baseline/`.

---

## 1. Giới thiệu

### 1.1 Tổng quan dự án

HRM nội bộ mInvoice. Sponsor: **Mr. Dư Hùng, PGD**. BA: **Trịnh Yên**. Xây **2026** (~1 tỷ CAPEX); **dùng 2027**.

### 1.2 Mục tiêu

| ID | Objective | Success criteria |
|----|-----------|------------------|
| O-1 | Số hóa vòng đời NS Must 7 module | UAT AC DOC-07; NFR-001 |
| O-2 | Kiến trúc MS+GW+SSO+A/S+DR | Khớp ADR-001/003/007 |
| O-3 | Go-live 2027 24/7 | DOC-17 + ADR-003 |

### 1.3 Tài liệu tham chiếu

| Doc | Version |
|-----|---------|
| DOC-01–03 | Chốt |
| DOC-06/07 7 module | Chốt |
| DOC-08–12 | Chốt khung |
| DOC-13 | Chốt (RTO/RPO phút TBD) |
| DOC-14 | **Chốt** v0.1 |

## 2. Tổ chức dự án

### 2.1 Vai trò & Trách nhiệm

| Role | Name | Responsibility |
|------|------|----------------|
| Sponsor (A) | Mr. Dư Hùng, PGD | Chốt cổng, ngân sách |
| PM | TBD | Lịch, FTE |
| BA | Trịnh Yên | Req, AC |
| SA | TBD | ADR, INT |
| Dev/QC | SH-010 | Code, test |
| IT/IAM | SH-006 | IdP issuer, Git/CRM lock |
| Ban HR | TBD | BO ☐ nợ |

### 2.2 Stakeholder

→ **DOC-02**

## 3. Quy trình quản lý dự án

| Process | Approach |
|---------|----------|
| Scope | 7 module Chốt; EVT/RPT sau SRS; CR DOC-18 |
| Schedule | Wave W0–W5 (DOC-14 §8); sprint khi có team |
| Cost | Trần CAPEX ~1 tỷ 2026; OPEX hosting chưa tách (A-007) |
| Quality | DOC-16 khi mở |
| Risk | §7 |
| Communication | DOC-02 |
| Change | DOC-18 |

## 4. WBS & Lịch trình

### 4.1 Giai đoạn / Cột mốc

| Phase | Milestone | Deliverables | Start | End |
|-------|-----------|--------------|-------|-----|
| Discovery | M1 | DOC-01–03 Chốt | 2026-08 | done (chưa BL) |
| Requirements | M2 | 7× DOC-04–07 + 13 | 2026-08 | done (chưa BL) |
| Architecture | M3 | DOC-08–12 khung | 2026-08 | done (chưa BL) |
| Build W0–W3 | M4 | 7 service + GW | 2026 | TBD |
| HA/UAT | M5 | A/S DR drill; NFR-001 | 2026 | TBD |
| Go-live | M6 | Prod 24/7 | | **2027** |

### 4.2 Lịch trình tóm tắt

| WBS | Task | Duration | Dependency | Owner |
|-----|------|----------|------------|-------|
| 1.1 | Nền tảng OIDC+GW+LBS | TBD | Issuer IT | SA/DevOps |
| 1.2–1.8 | Module Must theo wave | TBD | 1.1 | Dev |
| 1.9 | NFR audit/cô lập | TBD | PAY/IAM | Dev |
| 1.10 | EVT/RPT | TBD | SRS | BA→Dev |
| 1.11 | DOC-16/17 UAT cutover | TBD | M5 | QC/DevOps |

Gantt chi tiết khi có FTE — **không** bịa tuần.

## 5. Kế hoạch nguồn lực

| Role | FTE | Notes |
|------|-----|-------|
| Mọi role | **TBD** | 3 người HR vận hành (DEC-DIS-009) ≠ team build |

## 6. Ngân sách

| Category | Estimated | Actual | Variance |
|----------|-----------|--------|----------|
| CAPEX 2026 | ~1 tỷ (CN-004) | — | — |
| Labor / license / 2 DC | TBD trong 1 tỷ | — | OPEX chưa tách |

## 7. Đăng ký rủi ro

| ID | Risk | Prob | Impact | Mitigation | Owner | Status |
|----|------|------|--------|------------|-------|--------|
| RK-001 | Issuer OIDC chậm | M | H | W0 buffer; IT | SH-006 | Open |
| RK-002 | Ops 2 DC + N service | H | H | A/S runbook DOC-17 | DevOps | Open |
| RK-003 | TIM–PAY saga | M | M | ADR-005 | SA | Open |
| RK-004 | Ban HR chưa ký | M | M | Nợ cổng | PGD | Open |
| RK-005 | EVT chưa SRS | M | M | Wave W5 | BA | Open |
| RK-006 | RTO/RPO phút trống | M | M | OQ-ARC-002 | PGD | Open |

## 8. Đăng ký phụ thuộc

| ID | Dependency | Type | Impact if delayed |
|----|------------|------|-------------------|
| D-01 | **Lark** issuer/JWKS (tenant/region IT) | External IT | Chặn W0 |
| D-02 | Git + CRM sản phẩm API | External IT | Chặn LIF N+3 |
| D-03 | SMTP Cty | External | Mail; in-app vẫn Must |
| D-04 | DOC-16 | Internal | Chặn UAT chính thức |

## 9. Chất lượng & Chấp nhận

| Gate | Criteria | Sign-off |
|------|----------|----------|
| G1 Discovery | DOC-01–03 Chốt | PGD done |
| G2 Architecture | DOC-08–12 Chốt khung | PGD done |
| G3 Planning | DOC-14/15 Chốt | PGD **done** DEC-PLN-002 |
| G4 Build | AC DOC-07 + NFR-001 | QC/PGD |
| G5 Go-live | DOC-17 dry-run + ADR-003 | PGD 2027 |

## 10. Phụ lục

- Training / hypercare: **TBD** gần M6.
- Communication: DOC-02.

## 11. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-PLN-002) · ☐ `02-baseline/` |
| PM | | 2026-08-26 | Soạn → PGD chốt |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
