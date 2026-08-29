# DOC Registry — {Tên dự án}

| Cập nhật | 2026-08-29 |
|----------|------------|

## Execute artifacts (HRM — không baseline)

| Artifact | Path | Ver | Status | Ghi chú |
|----------|------|-----|--------|---------|
| Backend API | `hrm-backend/` | slice | Active ◐ | IAM + EMP · EF migrate |
| Frontend MVP | `hrm-web/` | 0.1 | Active ◐ | SCR-001…006 · IAM admin · NV profile |
| E2E API | `hrm-backend/scripts/e2e-full.sh` | — | Active | 12 steps |
| E2E web smoke | `hrm-backend/scripts/e2e-web.sh` | — | Active | web + NV + full |
| TC run log | `memory/delivery/tc-run-2026-08-29.md` | 0.1 | Active | partial Pass |
| Prod PG template | `hrm-backend/src/Hrm.Host/appsettings.Production.json` | — | Draft | OQ-DLV-003 host TBD IT |
| Overview rollup | `docs/05-traceability/overview.md` | — | Active | M4g |

Đăng ký DOC theo module. **REQ owner (sign-off → Baseline):** {Tên REQ owner} — phê duyệt **mọi** DOC active ([Luồng phê duyệt tài liệu](../01-project/DOC-02-stakeholder-analysis.md#23-luồng-phê-duyệt-tài-liệu)). Mọi DOC active phải có mục Approval với dòng sign-off REQ owner.

## governance (dùng chung)

| Path | Mô tả | Ver | Người thực hiện tài liệu | Status |
|------|-------|-----|-----------------|--------|
| `00-governance/business-glossary.md` | Business glossary — trạng thái & thuật ngữ (DOC, registry, DOC-16, RACI) | — | BA | Draft |
| `00-governance/doc-versioning.md` | Quy tắc Version, header, Change Log cho mọi DOC | — | BA | Draft |

## Chú giải cột

→ **Trạng thái & thuật ngữ:** [`business-glossary.md`](../00-governance/business-glossary.md)

| Cột registry | Tham chiếu |
|--------------|------------|
| Người thực hiện tài liệu | [Registry — Author](../00-governance/business-glossary.md#5-registry--author) |
| Status | [DOC — Status](../00-governance/business-glossary.md#1-doc--status) |
| Sign-off, Người Sign-off, Ngày sign-off | [Sign-off (registry)](../00-governance/business-glossary.md#2-sign-off-registry) |
| Baseline | [Baseline](../00-governance/business-glossary.md#3-baseline) |
| Highlight vàng | [Sign-off (registry)](../00-governance/business-glossary.md#2-sign-off-registry) *(mục Highlight)* |

---

## project

| Path | DOC | Ver | Người thực hiện tài liệu | Status | Baseline | Sign-off | Người Sign-off | Ngày sign-off |
|------|-----|-----|-----------------|--------|----------|----------|----------------|---------------|
| `01-project/DOC-01-vision-business-case.md` | 01 | — | BA | Draft | — | ☐ | {alias} | — |
| `01-project/DOC-02-stakeholder-analysis.md` | 02 | — | BA | Draft | — | ☐ | {alias} | — |
| `01-project/DOC-03-brd.md` | 03 | — | BA | Draft | — | ☐ | {alias} | — |

## {module-id} ({MOD})

| Path | DOC | Ver | Người thực hiện tài liệu | Status | Baseline | Sign-off | Người Sign-off | Ngày sign-off |
|------|-----|-----|-----------------|--------|----------|----------|----------------|---------------|
| `03-modules/{module-id}/DOC-04-business-rules.md` | 04 | — | BA | Draft | — | ☐ | {alias} | — |
| `03-modules/{module-id}/DOC-05-use-cases.md` | 05 | — | BA | Draft | — | ☐ | {alias} | — |
| `03-modules/{module-id}/DOC-06-srs.md` | 06 | — | BA | Draft | — | ☐ | {alias} | — |
| `03-modules/{module-id}/DOC-07-acceptance-criteria.md` | 07 | — | BA | Draft | — | ☐ | {alias} | — |
| `03-modules/{module-id}/DOC-16-test-strategy.md` | 16 | — | BA | Draft | — | ☐ | {alias} | — |

## platform

| Path | DOC | Ver | Người thực hiện tài liệu | Status | Baseline | Sign-off | Người Sign-off | Ngày sign-off |
|------|-----|-----|-----------------|--------|----------|----------|----------------|---------------|
| `04-platform/DOC-11-data-model.md` | 11 | — | SA | Draft | — | ☐ | {alias} | — |

## Legacy (tham chiếu — không baseline mới)

| Path | Module | Ghi chú |
|------|--------|---------|
| `03-modules/_legacy/{legacy-id}/DOC-04` … `DOC-07` | {legacy-id} | *(mô tả migration)* |

→ Index module: [DOC-03 — Module index](../01-project/DOC-03-brd.md#13-module-index)
