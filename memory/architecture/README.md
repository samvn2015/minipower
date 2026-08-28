# Memory — Architecture

**DOC đích:** 08–12 · **Skill:** `skills/architecture/SKILL.md` · **Tiên quyết:** DOC-06, 13 **Chốt** (chưa baseline)

## Trạng thái

| Mục | Giá trị |
|-----|---------|
| DOC-08 SAD | **Chốt** v0.1 — DEC-ARC-005 |
| DOC-09 ADR | 001 · 002 · 003 · **007** **Accepted** |
| DOC-10 / 11 / 12 | cả ba **Chốt** khung · DEC-ARC-006/008/010 |
| Integration map | DOC-10 + DOC-08 §4.0 |

## Tóm tắt

- Slice vừa chốt: **ADR-007 v0.2 Lark IdP** + **DOC-12 OAS** (DEC-ARC-009/010/015).
- **Không** tự DOC-17. Nợ: issuer URL (IT), RTO/RPO phút, LBS, MFA, engine DB.

## ADR / quyết định

| ID | Chủ đề | Trạng thái |
|----|--------|------------|
| ADR-001 | MS + GW + LBS + SSO | **Accepted** |
| ADR-002 | Token SSO GW + IAM | **Accepted** |
| ADR-003 | 24/7 + A/S + DR/DC | **Accepted** |
| ADR-004 | Mã hóa at-rest | Proposed |
| ADR-005 | Job broker / saga | Proposed |
| ADR-006 | MFA | Proposed |
| ADR-007 | **Lark** IdP + OIDC-only MVP | **Accepted** v0.2 |

## Tham chiếu

| Loại | Link |
|------|------|
| Docs | [`DOC-08-sad.md`](../../docs/04-platform/DOC-08-sad.md) |
| Decision | DEC-ARC-001…**010** |
| Req | [`../requirements/README.md`](../requirements/README.md) |

## Lịch sử ngắn

- 2026-08-26 — DEC-ARC-001: mở DOC-08 Draft; rollup overview.
- 2026-08-26 — DEC-ARC-003: **chốt ADR-001** gói F Accepted.
- 2026-08-26 — DEC-ARC-004: **chốt ADR-003** Accepted.
- 2026-08-26 — DEC-ARC-005: **chốt DOC-08** SAD khung.
- 2026-08-26 — DEC-ARC-006: **chốt DOC-10** INT-001…006.
- 2026-08-26 — DEC-ARC-007: **chốt ADR-002** token SSO.
- 2026-08-26 — DEC-ARC-008: **chốt DOC-11** khung ER.
- 2026-08-28 — DEC-ARC-015: **ADR-007 v0.2** IdP **Lark** (Google/Apple/@lhqglobal.vn).
- 2026-08-26 — DEC-ARC-009: **chốt ADR-007** IdP OIDC.
- 2026-08-26 — DEC-ARC-010: **chốt DOC-12** khung OpenAPI.
