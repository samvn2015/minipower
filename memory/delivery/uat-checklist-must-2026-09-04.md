# UAT checklist — 7 module Must (DEV)

| Ngày | 2026-09-04 |
|------|------------|
| Auth | `/dev/token` · DEC-DLV-011 (không Lark JWKS) |
| Host | `http://localhost:5167` · web `hrm-web` |
| Evidence API | `tc-run-2026-09-04.md` · DOC-16 v0.2 execute St |

**Cách dùng:** HR = `local-dev` · LM = `local-lm` · IT = `it-dev`. Đánh ☑ khi quan sát UI/API khớp Expected. Fail → ghi mã TC DOC-16.

---

## 0. Chuẩn bị

- [ ] Host live + `dotnet run` branch `main` (sau merge stack DOC-16)
- [ ] Web login persona HR / LM / IT lần lượt
- [ ] Không kỳ vọng OIDC Lark / Prod PG

---

## 1. IAM

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 1.1 | `/iam/accounts` gán/gỡ role | HR | OK; NV không vào | 003, 013 |
| 1.2 | IT mở PAY kỳ / run | IT | 403 | 007 |
| 1.3 | LM mở `/tim/imports` | LM | redirect/403 | 008 |
| 1.4 | Xem phiếu mình → audit | HR | `PayslipViewed` | 011 |

---

## 2. EMP

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 2.1 | Tạo NV + HĐ + org | HR | 201; unique 409 | 001–004 |
| 2.2 | Đổi LM SCR-005/006 | HR | duyệt; không đổi LM trên form thường | 006, 008 |
| 2.3 | Contract types từ API | HR | không hardcode | 014 |
| 2.4 | Hồ sơ tôi | NV/HR | `/profile` | 007 |

---

## 3. LEV

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 3.1 | Nộp đơn + bàn giao | NV | Chờ C1 | 001 |
| 3.2 | C1 / C2 | LM → HR | PendingC2 → trừ quỹ C2 | 010–012 |
| 3.3 | `/leave/m` | NV | cùng rule web | 002 Partial |
| 3.4 | ≥3 ngày trễ / ốm file / notify | HR/NV | slice F | 006–009 |

---

## 4. TIM

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 4.1 | Mẫu Active + import Preview | HR | commit sạch | 001–005 |
| 4.2 | Chốt tháng | HR | PAY đọc được | 006 |
| 4.3 | LM import | LM | 403 | 011 |
| 4.4 | (API) punch device / zkteco | HR | 405 / 400 | 010, 016 |

---

## 5. PAY

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 5.1 | Run + close sau TIM Closed | HR | Draft → Closed | 001, 016 |
| 5.2 | Phiếu `/pay/payslips` + `/pay/m/payslips` | HR/NV | cùng data; LM 403 người khác | 010–011 |
| 5.3 | PC tháng / xuất | HR | master; không CC LM | 012, 015 |
| 5.4 | (Script) C&B Δ=0 | — | `e2e-api-pay-slice-k-cb-uat.sh` | 018 |

---

## 6. PRB

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 6.1 | `/prb/cases` + job T-15/T-7 | HR | mốc EMP; reminder | 001–003 |
| 6.2 | Chốt PASS/EXTEND/FAIL | HR | EMP/LIF đúng | 005–007 |
| 6.3 | LM decide | LM | 403 | 009 |
| 6.4 | TV không LM → decide | HR | slice E | 014, 017 |

---

## 7. LIF

| # | Việc | Persona | Expected | TC |
|---|------|---------|----------|-----|
| 7.1 | Onboarding checklist + provision | HR + IT | thiếu Must chặn close | 001–002 |
| 7.2 | Confirm N → thấy N+3 | HR | UI đúng | 003, 013 |
| 7.3 | Early CR lock | IT | trước N+3 có lý do; không CRM sales | 007, 010, 016 |
| 7.4 | (Unit/config) Standby | — | job N+3 không chạy | HA-001 |

---

## Exit UAT (DEV)

| Tiêu chí | |
|----------|--|
| Must UI smoke ☑ trên bảng trên | |
| Không Blocker DOC-16 (403 lương, CRM sales, máy CC…) | |
| Nợ chấp nhận: Lark JWKS Prod · EVT/RPT · Partial SCR · lịch lễ LEV | |
| Go-live 2027 | **chưa** — còn OQ Prod PG / RTO / JWKS |

Ký UAT DEV (tuỳ chọn): PGD __________ ngày ______
