# Platform — Cross-cutting artifacts

Tài liệu **không thuộc một module** hoặc **ảnh hưởng nhiều module**. **Baseline BL-1.0 (2026-09-21)** — snapshot ở [`02-baseline/v1.0/04-platform/`](../02-baseline/v1.0/). Sửa sau baseline = CR (`06-changes/`).

| File / Folder | DOC | Template |
|---------------|-----|----------|
| `DOC-08-sad.md` | 08 | [DOC-08](../../templates/DOC-08-sad.md) · **v0.4.3 BL-1.0** — ADR-012/013 |
| `DOC-09-adr/` | 09 | [DOC-09](../../templates/DOC-09-adr.md) — 1 file / quyết định · ADR-001…013, **BL-1.0** |
| `DOC-10-integration-specification.md` | 10 | [DOC-10](../../templates/DOC-10-integration-specification.md) · **v0.2.1 BL-1.0** — DEC-ARC-035 |
| `DOC-11-data-model/` | 11 | [DOC-11](../../templates/DOC-11-data-model.md) · **v0.3.2 BL-1.0** — DEC-ARC-031 |
| `DOC-12-api-spec/` | 12 | [DOC-12](../../templates/DOC-12-api-specification.md) · **v0.4.3 BL-1.0** — DEC-ARC-031; `openapi.yaml` sinh từ runtime 2026-09-21 |
| `DOC-13-nfr.md` | 13 | [DOC-13](../../templates/DOC-13-nfr.md) · **v0.3.3 BL-1.0** — DEC-ARC-031/033/037 |
| `DOC-14-wbs-estimate.md` | 14 | [DOC-14](../../templates/DOC-14-wbs-estimate.md) · **v0.2.1 BL-1.0** — DEC-ARC-035 |
| `DOC-16-test-strategy.md` | 16 | **v0.2.1 BL-1.0** — DEC-ARC-035 · TC chi tiết per-module (`03-modules/*/DOC-16`, identity còn TC OIDC — nợ QC) |
| `DOC-17-deployment-guide.md` | 17 | [DOC-17](../../templates/DOC-17-deployment-guide.md) · **v0.4.2 BL-1.0** — DEC-ARC-031 |

**Gợi ý:** `DOC-12-api-spec/` — một file OpenAPI hoặc tách `{module-id}.yaml` theo module.
