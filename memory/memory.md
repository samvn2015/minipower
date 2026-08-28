# Memory — HRM

> **Index gốc** — chỉ giữ thông tin chung & link tới memory theo chủ đề. **Không** gom toàn bộ context vào file này.

---

## Dự án

| Mục | Giá trị |
|-----|---------|
| **Tên** | HRM |
| **Khách hàng** | *(chưa xác định)* |
| **Phase hiện tại** | delivery |
| **Baseline** | — *(draft)* |
| **Ưu tiên workspace** | **HRM** — CRM **PARKED** (DEC-DIS-008), không trộn DOC |

## Memory theo chủ đề

| Chủ đề | Index | DOC |
|--------|-------|-----|
| Discovery | [discovery/](discovery/README.md) | 01–03 |
| Requirements | [requirements/](requirements/README.md) | 04–07, 13 |
| Architecture | [architecture/](architecture/README.md) | 08–12 |
| Planning | [planning/](planning/README.md) | 14–15 |
| Delivery | [delivery/](delivery/README.md) | 16–17 |
| Change control | [change-control/](change-control/README.md) | 18 |

## Liên kết nhanh

| Folder / file | Vai trò |
|---------------|---------|
| [**`docs/05-traceability/overview.md`**](../docs/05-traceability/overview.md) | **Tổng quan 30s** |
| [`../brainstorm/`](../brainstorm/) | Trao đổi theo ngày |
| [`../FAQ.md`](../FAQ.md) | FAQ thiết lập sẵn |
| [`../assets/`](../assets/) | Tài liệu gốc |
| [`../docs/`](../docs/) | Artifact baseline |

## Ghi chú agent

0. Đọc **`memory/profile.json`**.
1. Tổng thể → [`docs/05-traceability/overview.md`](../docs/05-traceability/overview.md).
2. Mở **memory/{phase}/** theo phase đang làm.
3. Không append dài vào `memory.md` gốc. Quyết định có phương án bị loại → `memory/{phase}/decision-log.md`.
4. User hỏi bước tiếp → `FAQ.md`.
