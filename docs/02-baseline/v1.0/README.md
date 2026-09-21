# Baseline v1.0 — READ ONLY

**BL-1.0 · 2026-09-21 · scope: `04-platform`** — ký PGD Dư Hùng (DEC-ARC-037).

Snapshot của DOC-08 SAD v0.4.3 · DOC-09 ADR-001…013 · DOC-10 v0.2.1 · DOC-11 v0.3.2 · DOC-12 v0.4.3 + `openapi.yaml` (sinh từ runtime 2026-09-21) · DOC-13 v0.3.3 · DOC-14 v0.2.1 · DOC-16 v0.2.1 · DOC-17 v0.4.2 — kiến trúc theo **ADR-013** (modular monolith, một DB nhiều schema, không Gateway), **ADR-012** (login HRM tự quản), **ADR-010** (một DC).

**Không thuộc BL-1.0:** `01-project`, `03-modules` (7 module DOC-04…07/16), `00-governance` — baseline ở v1.1+.

**Quy tắc:**
- Không sửa file trong folder này trực tiếp.
- Thay đổi sau baseline → CR (`06-changes/`) → sửa bản working → approve → tạo `v1.1/`.
- Nợ mở khi baseline (không chặn tài liệu, chặn go-live): R-008 backup cùng DC (RK-01) · R-009 văn bản khách (RK-08) · R-012 login chưa code · R-015 adapter ra · R-016 xác thực scheduler · R-018 audit atomic 5 context · `X-Total-Count` chưa trong OAS · pass 4 Major có owner: M8 Prod lần đầu (DBA), M9 replication (DevOps), M14 forwarded headers.

Chi tiết file + sha256: `manifest.yaml`.
