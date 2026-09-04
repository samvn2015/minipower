# Memory — Delivery

**DOC đích:** 16–17 · **Skill:** `skills/delivery/SKILL.md` · **Tiên quyết:** DOC-06, 07 **Chốt**

## Trạng thái

| Mục | Giá trị |
|-----|---------|
| Test strategy | DOC-16 **Chốt** (7 Must) · execute TC ◐ |
| UAT | Exit khi AC Must Pass · evidence `tc-run-2026-09-04.md` |
| Go-live | DOC-17 **Chốt** · 2027 · JWKS/prod PG còn chặn |
| Code | 7 module Must landed (`main` · #35) · Lark JWKS bypass DEV (DEC-DLV-011) |

## Tóm tắt

- Must API e2e: PRB/TIM/IAM/LEV/PAY/EMP/LIF có script; TC-run 2026-09-04.
- **Không** tự baseline. EVT/RPT không TC.
- DEC-DLV-011: tạm bỏ qua Lark JWKS cho DEV/UAT.

## Tham chiếu

| Loại | Link |
|------|------|
| Docs | [DOC-16 CT](../../docs/04-platform/DOC-16-test-strategy.md) · [DOC-17](../../docs/04-platform/DOC-17-deployment-guide.md) |
| Decision | DEC-DLV-001…**011** |
| TC run | [2026-08-29](tc-run-2026-08-29.md) · [2026-09-04](tc-run-2026-09-04.md) |
| UAT DEV | [uat-checklist-must-2026-09-04](uat-checklist-must-2026-09-04.md) |

## Lịch sử ngắn

- 2026-09-04 — UAT checklist Must DEV; DOC-16 execute 7 module (DEC-DLV-012…018).
- 2026-09-04 — DEC-DLV-011: bypass Lark JWKS DEV/UAT; TC-run sau PR #35.
- 2026-08-26 — DEC-DLV-001: DOC-16 chương trình + DOC-17 Draft.
- 2026-08-26 — DEC-DLV-002: leave DOC-16 Draft.
- 2026-08-26 — DEC-DLV-003: 6 module còn lại DOC-16 Draft.
