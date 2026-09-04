# DOC-16 — Test cases (timekeeping)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-09-04 | QC (execute) | **Chốt** · St cập nhật (DEC-DLV-015) |
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** timekeeping · **TIM** · [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**. INT-003. NFR-001 đo sau GW.  
**Evidence:** `e2e-api-tim-slice-a…f.sh` · `e2e-api-iam-rbac` (LM TIM 403) · PAY run presupposes TIM Closed.

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| TIM-TC-001 | Một version mẫu hiệu lực | 1 Active | API | Happy | Must | Pass |
| TIM-TC-002 | Cột từ master | Không hardcode | E2E | Happy | Must | Pass |
| TIM-TC-003 | Import khớp version | Sai version chặn | API | Unhappy | Must | Pass |
| TIM-TC-004 | Preview; cấm commit còn lỗi | Không ghi công | API | Unhappy | Must | Pass |
| TIM-TC-005 | Commit hết lỗi Must | OK | API | Happy | Must | Pass |
| TIM-TC-006 | Chốt tháng; PAY đọc | PAY thấy kỳ | API | Happy | Must | Pass |
| TIM-TC-007 | OT loại trước chốt | OK | API | Happy | Must | Pass |
| TIM-TC-008 | Chỉ phép đã duyệt vào công | Đúng | UT | Happy | Must | Partial |
| TIM-TC-009 | N_thực gồm phép hưởng | Không cộng lại | UT | Happy | Must | Pass |
| TIM-TC-010 | Chỉ Excel mẫu; không máy CC | Chặn protocol máy | API | Unhappy | Must | Pass |
| TIM-TC-011 | NV/LM import/chốt | 403 | API | Unhappy | Must | Pass |
| TIM-TC-012 | Bỏ chốt; cấm nếu PAY đã chốt | Chặn | API | Unhappy | Must | Pass |
| TIM-TC-013 | Ẩn màn HR NV/LM | 403 | E2E | Unhappy | Must | Pass |
| TIM-TC-014 | Đủ TIM-SCR-001…006 | Có màn; pixel không fail | E2E | Happy | Must | Partial |
| TIM-TC-015 | Hai mẫu Active | Cấm | API | Unhappy | Must | Pass |
| TIM-TC-016 | Đổi mẫu không tự commit | Preview only | API | Happy | Must | Pass |
| TIM-TC-NFR-001 | 403 NV/LM DOC-13 | Pass | API | Unhappy | Must | Pass |
| TIM-TC-NFR-002 | Audit công bố/import/chốt | Log | API | Happy | Must | Partial |

## 3. Chi tiết test case

E2E sau LBS+GW. Path: `POST /tim/imports`, `POST /tim/periods/{ym}/close`. Commit/bỏ chốt/công bố mẫu: UI TIM-SCR — không bịa URL. NFR-001 **không** đo localhost. INT-003 = Excel mẫu.  
**Execute 2026-09-04:** St §2 = SoT. Partial = phép↔TIM UT mỏng / SCR chưa đủ 6 màn / audit chưa đủ e2e.

### TIM-TC-001 — Một version mẫu hiệu lực

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-001, TIM-AC-001 |
| **Steps** | **H:** HR TIM-SCR-002 công bố V2. **N:** JWT NV công bố. |
| **Expected** | **H:** V2 hiệu lực; import file V1 từ chối. **N:** 403. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — slice-a publish Active; NV 403 template. |

### TIM-TC-002 — Cột từ master

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-002, TIM-AC-002 |
| **Steps** | HR đổi tên cột master; import đúng version. So với cột hardcode URD. |
| **Expected** | Preview mapping master. Hardcode URD → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Pass** — columns from template master (slice-a/b). |

### TIM-TC-003 — Import khớp version

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-003, TIM-AC-003 |
| **Preconditions** | Mẫu hiệu lực = V2 |
| **Steps** | `POST /tim/imports` file V1. |
| **Expected** | Từ chối; không ghi sổ. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — wrong versionCode rejected (slice-b/c). |

### TIM-TC-004 — Preview; cấm commit còn lỗi

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-004, TIM-AC-004 |
| **Preconditions** | File đúng version; dòng thiếu NV |
| **Steps** | 1. Preview TIM-SCR-003. 2. HR commit khi còn lỗi Must. |
| **Expected** | Lỗi Must hiện; chưa commit. Commit → chặn; không ghi công. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — Preview + commit blocked when errors (slice-c). |

### TIM-TC-005 — Commit hết lỗi Must

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-005, TIM-AC-005 |
| **Preconditions** | Preview sạch |
| **Steps** | HR ghi TIM-SCR-004. |
| **Expected** | Bảng công Draft; chưa chốt. |
| **Layer / Path** | API · Happy |
| **Status** |  **Pass** — commit clean → Draft (slice-c/d). |

### TIM-TC-006 — Chốt tháng; PAY đọc

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-006, TIM-AC-006, PAY-FR-001 |
| **Steps** | **H:** đã commit hết lỗi → `POST /tim/periods/{ym}/close` / TIM-SCR-005. **N:** PAY `POST /pay/periods/{ym}/run` khi TIM chưa chốt. |
| **Expected** | **H:** tháng Chốt; PAY đọc được. **N:** PAY chặn (chạy trước PAY-TC-001). |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — close period; PAY run sau TIM Closed (pay-a). |

### TIM-TC-007 — OT loại trước chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-007, TIM-AC-007 |
| **Steps** | **N:** có giờ OT không loại → chốt. **H:** loại 1.5/2.0/3.0 → chốt. |
| **Expected** | **N:** chặn. **H:** chốt nếu hết lỗi khác. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** |  **Pass** — OT classification before close (slice-d/e). |

### TIM-TC-008 — Chỉ phép Đã duyệt vào công

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-008, TIM-AC-008 |
| **Steps** | **H:** đơn phép Đã duyệt trong tháng → chốt. **N:** đơn Chờ C1 → chốt. |
| **Expected** | **H:** ngày phép trên bảng công. **N:** không vào công. |
| **Layer / Path** | UT · Happy + Unhappy |
| **Status** |  **Partial** — approved leave in TIM path có; e2e riêng đơn Chờ C1 còn mỏng. |

### TIM-TC-009 — N_thực gồm phép hưởng

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-009, TIM-AC-009 |
| **Steps** | Xuất N_thực cho PAY. |
| **Expected** | Đã gồm ngày phép hưởng; không tách để PAY cộng. N_thực “sạch phép” → fail AC. |
| **Layer / Path** | UT · Happy + Unhappy |
| **Status** |  **Pass** — N_thực gồm phép hưởng; PAY A-001 không cộng lại (pay-i). |

### TIM-TC-010 — Chỉ Excel mẫu; không máy CC

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-010, TIM-AC-010, INT-003 |
| **Steps** | **H:** import Excel version hiệu lực. **N:** gọi protocol/API máy chấm công. |
| **Expected** | **H:** vào preview. **N:** ngoài scope / cấm. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** |  **Pass** — `e2e-api-tim-slice-f` 405 device + reject zkteco. |

### TIM-TC-011 — NV/LM 403 import/chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-011, TIM-AC-011 |
| **Steps** | JWT LM `POST /tim/imports`. JWT NV chốt tháng. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — `e2e-api-iam-rbac` LM TIM import/periods 403. |

### TIM-TC-012 — Bỏ chốt; cấm nếu PAY đã chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-012, TIM-AC-012 |
| **Steps** | **H:** TIM Chốt; PAY Draft → TIM-SCR-006 bỏ chốt. **N1:** PAY đã chốt → bỏ chốt TIM. **N2:** sửa ô công trên PAY. |
| **Expected** | **H:** TIM Draft; import lại được. **N1:** cấm. **N2:** cấm. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** |  **Pass** — unclose blocked when PAY Closed (slice-e). |

### TIM-TC-013 — Ẩn màn HR NV/LM

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-013, TIM-AC-013 |
| **Steps** | NV/LM mở TIM-SCR-001. |
| **Expected** | 403 hoặc không menu. |
| **Layer / Path** | E2E · Unhappy |
| **Status** |  **Pass** — RequireHr routes `/tim/imports|periods`. |

### TIM-TC-014 — Đủ TIM-SCR-001…006

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-014, TIM-AC-014 |
| **Steps** | Đi luồng mẫu → import → commit → chốt → bỏ chốt. |
| **Expected** | Có 6 màn. Pixel HTML MCP không fail Must. |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Partial** — templates/imports/periods UI; chưa đủ 6 SCR tách. |

### TIM-TC-015 — Cấm hai mẫu Active

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-015, TIM-AC-015 |
| **Steps** | HR để V1 và V2 cùng hiệu lực. |
| **Expected** | Cấm. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — publish demotes prior Active (slice-a). |

### TIM-TC-016 — Đổi mẫu không tự commit

| Mục | Nội dung |
|-----|----------|
| **Trace** | TIM-FR-016, TIM-AC-016 |
| **Preconditions** | Preview chưa commit |
| **Steps** | HR công bố mẫu mới. |
| **Expected** | Preview cũ không tự commit. |
| **Layer / Path** | API · Happy |
| **Status** |  **Pass** — slice-f Preview intact after publish. |

### TIM-TC-NFR-001 / 002

403 NV/LM DOC-13. Audit: công bố / import / chốt / bỏ chốt — không bịa SLA.  
**Status:** NFR-001 **Pass** (TC-011). NFR-002 **Partial** — chưa e2e audit đủ mọi thao tác.

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.2 | Execute St catalog + §3 (DEC-DLV-015) | QC / PGD |
| 0.1 | Chốt catalog (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | §3 chi tiết (DEC-DLV-006) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| PGD | Mr. Dư Hùng | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| PGD | Mr. Dư Hùng | 2026-09-04 | ☑ Execute St v0.2 (DEC-DLV-015) — chờ merge |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
