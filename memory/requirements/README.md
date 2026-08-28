# Memory — Requirements

**DOC đích:** 04–07, 13, 19 · **Skill:** `skills/requirements/SKILL.md`

## Trạng thái

| Mục | Giá trị |
|-----|---------|
| Module | **7 module Must: 04+05+06+07+19 khung Chốt** |
| DOC-04 PRB | **Chốt** v0.1 — DEC-REQ-051 |
| DOC-05 PRB | **Chốt** v0.1 — DEC-REQ-053 |
| DOC-19 PRB | **Chốt khung** v0.1 — DEC-REQ-055 |
| DOC-06 PRB | **Chốt** v0.1 — DEC-REQ-057 |
| DOC-07 PRB | **Chốt** v0.1 — DEC-REQ-059 |
| DOC-04 IAM | **Chốt** v0.2 — DEC-REQ-041 |
| DOC-05 IAM | **Chốt** v0.2 — DEC-REQ-043 |
| DOC-19 IAM | **Chốt khung** v0.1 — DEC-REQ-045 |
| DOC-06 IAM | **Chốt** v0.1 — DEC-REQ-047 |
| DOC-07 IAM | **Chốt** v0.1 — DEC-REQ-049 |
| DOC-04 EMP | **Chốt** v0.1 — DEC-REQ-023 |
| DOC-05 EMP | **Chốt** v0.1 — DEC-REQ-026 |
| DOC-19 EMP | **Chốt khung** v0.1 — DEC-REQ-029 |
| DOC-06 EMP | **Chốt** v0.1 — DEC-REQ-033 (EMP-FR-001…017) |
| DOC-07 EMP | **Chốt** v0.1 — DEC-REQ-036 |
| DOC-04 LIF | **Chốt** v0.1 — DEC-REQ-024 |
| DOC-05 LIF | **Chốt** v0.1 — DEC-REQ-027 |
| DOC-19 LIF | **Chốt khung** v0.1 — DEC-REQ-030 |
| DOC-06 LIF | **Chốt** v0.1 — DEC-REQ-034 |
| DOC-07 LIF | **Chốt** v0.1 — DEC-REQ-037 |
| DOC-04 TIM | **Chốt** v0.1 — DEC-REQ-017 |
| DOC-05 TIM | **Chốt** v0.1 — DEC-REQ-018 |
| DOC-19 TIM | **Chốt khung** v0.1 — DEC-REQ-019 |
| DOC-06 TIM | **Chốt** v0.1 — DEC-REQ-020 |
| DOC-07 TIM | **Chốt** v0.1 — DEC-REQ-021 |
| DOC-04 PAY | **Chốt** v0.1 — DEC-REQ-011 |
| DOC-05 PAY | **Chốt** v0.1 — DEC-REQ-012 |
| DOC-19 PAY | **Chốt khung** v0.1 — DEC-REQ-013 |
| DOC-06 PAY | **Chốt** v0.1 — DEC-REQ-014 |
| DOC-07 PAY | **Chốt** v0.1 — DEC-REQ-015 |
| Leave 05–07 / 19 | **Chốt** (không đụng) |
| OQ-010 | Mở (leave) |
| DOC-13 | **Chốt** v0.1 — DEC-REQ-038 |

## Tóm tắt

- Slice req Must **đóng** (DEC-REQ-059). Architecture đã mở: **DOC-08 Draft** (DEC-ARC-001). **Không** tự DOC-16.
- Nợ xuyên suốt: DOC-16; HTML MCP; Ban HR ☐; DOC-13 SLA/RTO; OQ-010 leave; chưa `02-baseline/`.

## Module

| Module ID | Folder | Ghi chú |
|-----------|--------|---------|
| employee-profile | `employee-profile/` | 04+05+06+07+19 khung **Chốt** — DEC-REQ-033 / 036 |
| lifecycle | `lifecycle/` | 04+05+06+07+19 khung **Chốt** — DEC-REQ-034 / 037 |
| payroll | `payroll/` | 04+05+19 khung+06+07 **Chốt** — DEC-REQ-011…015 |
| timekeeping | `timekeeping/` | 04+05+19 khung+06+07 **Chốt** — DEC-REQ-017…021 |
| leave | `leave/` | 04+05+06+07+19 khung **Chốt** |
| identity | `identity/` | 04+05+06+07+19 khung **Chốt** — DEC-REQ-047 / 049 |
| probation | `probation/` | 04+05+06+07+19 khung **Chốt** — DEC-REQ-057 / 059 |

## Tham chiếu

| Loại | Link |
|------|------|
| Docs | [TIM DOC-04](../../docs/03-modules/timekeeping/DOC-04-business-rules.md) |
| Decision | DEC-REQ-001…**059** |

## Lịch sử ngắn

- 2026-08-24 — DEC-REQ-006 chốt khung; sinh DOC-06 Draft.
- 2026-08-24 — DOC-06 v0.2: §6.1 BRQ BRD → LEV-FR.
- 2026-08-24 — DEC-REQ-007: **chốt DOC-06** leave.
- 2026-08-24 — DEC-REQ-008: **chốt DOC-07** AC leave.
- 2026-08-24 — DEC-REQ-009: **chốt DOC-05** UC leave.
- 2026-08-24 — DEC-REQ-010: mở **payroll/DOC-04** Draft.
- 2026-08-24 — DEC-REQ-011: **chốt payroll DOC-04**.
- 2026-08-25 — DEC-REQ-012: **chốt payroll DOC-05**.
- 2026-08-25 — DEC-REQ-013: **chốt khung payroll DOC-19**.
- 2026-08-25 — DEC-REQ-014: **chốt payroll DOC-06**.
- 2026-08-25 — DEC-REQ-015: **chốt payroll DOC-07**.
- 2026-08-25 — DEC-REQ-016: mở **timekeeping/DOC-04** Draft.
- 2026-08-25 — DEC-REQ-017: **chốt timekeeping DOC-04**.
- 2026-08-25 — DEC-REQ-018: **chốt timekeeping DOC-05**.
- 2026-08-25 — DEC-REQ-019: **chốt khung timekeeping DOC-19**.
- 2026-08-25 — DEC-REQ-020: **chốt timekeeping DOC-06**.
- 2026-08-25 — DEC-REQ-021: **chốt timekeeping DOC-07**.
- 2026-08-25 — DEC-REQ-022: mở **EMP + LIF DOC-04** Draft.
- 2026-08-25 — DEC-REQ-023: **chốt EMP DOC-04**.
- 2026-08-25 — DEC-REQ-024: **chốt LIF DOC-04**.
- 2026-08-25 — DEC-REQ-025: mở **EMP + LIF DOC-05** Draft.
- 2026-08-25 — DEC-REQ-026: **chốt EMP DOC-05**.
- 2026-08-25 — DEC-REQ-027: **chốt LIF DOC-05**.
- 2026-08-25 — DEC-REQ-028: mở **EMP + LIF DOC-19** Draft khung.
- 2026-08-25 — DEC-REQ-029: **chốt khung EMP DOC-19**.
- 2026-08-25 — DEC-REQ-030: **chốt khung LIF DOC-19**.
- 2026-08-25 — DEC-REQ-031: mở **EMP + LIF DOC-06** Draft.
- 2026-08-25 — DEC-REQ-032: EMP DOC-06 thêm **EMP-FR-017** trình độ học vấn.
- 2026-08-25 — DEC-REQ-033: **chốt EMP DOC-06** (gồm FR-017).
- 2026-08-25 — DEC-REQ-034: **chốt LIF DOC-06**.
- 2026-08-25 — DEC-REQ-035: mở **EMP + LIF DOC-07** Draft.
- 2026-08-25 — DEC-REQ-036: **chốt EMP DOC-07**.
- 2026-08-25 — DEC-REQ-037: **chốt LIF DOC-07**.
- 2026-08-25 — DEC-REQ-038: **chốt DOC-13** NFR (tạo + chốt; nợ SLA/RTO).
- 2026-08-25 — DEC-REQ-039: mở **identity/DOC-04** Draft.
- 2026-08-25 — DEC-REQ-040: IAM DOC-04 v0.2 ma trận đủ module đã chốt.
- 2026-08-25 — DEC-REQ-041: **chốt identity DOC-04** v0.2.
- 2026-08-25 — DEC-REQ-042: mở **identity/DOC-05** Draft.
- 2026-08-26 — DEC-REQ-043: **chốt identity DOC-05** v0.2 (siết luồng).
- 2026-08-26 — DEC-REQ-044: mở **identity/DOC-19** Draft khung.
- 2026-08-26 — DEC-REQ-045: **chốt khung identity DOC-19**.
- 2026-08-26 — DEC-REQ-046: mở **identity/DOC-06** Draft.
- 2026-08-26 — DEC-REQ-047: **chốt identity DOC-06**.
- 2026-08-26 — DEC-REQ-048: mở **identity/DOC-07** Draft.
- 2026-08-26 — DEC-REQ-049: **chốt identity DOC-07**.
- 2026-08-26 — DEC-REQ-050: mở **probation/DOC-04** Draft.
- 2026-08-26 — DEC-REQ-051: **chốt probation DOC-04**.
- 2026-08-26 — DEC-REQ-052: mở **probation/DOC-05** Draft.
- 2026-08-26 — DEC-REQ-053: **chốt probation DOC-05**.
- 2026-08-26 — DEC-REQ-054: mở **probation/DOC-19** Draft khung.
- 2026-08-26 — DEC-REQ-055: **chốt khung probation DOC-19**.
- 2026-08-26 — DEC-REQ-056: mở **probation/DOC-06** Draft.
- 2026-08-26 — DEC-REQ-057: **chốt probation DOC-06**.
- 2026-08-26 — DEC-REQ-058: mở **probation/DOC-07** Draft.
- 2026-08-26 — DEC-REQ-059: **chốt probation DOC-07**.
