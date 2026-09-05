# DOC-16 — Test cases (employee-profile)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-09-04 | QC (execute) | **Chốt** · St cập nhật (DEC-DLV-017) |
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** employee-profile · **EMP** · [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**.  
**Evidence:** `e2e-full` · `e2e-api-emp-slice-a/b.sh` · `e2e-web` · tc-run 2026-08-29/09-04.

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| EMP-TC-001 | HR tạo NV+org+HĐ | 201 | E2E | Happy | Must | Pass |
| EMP-TC-002 | Unique MNV/CCCD | 409 | API | Unhappy | Must | Pass |
| EMP-TC-003 | Unique email/MST khi có | 409 | API | Unhappy | Must | Pass |
| EMP-TC-004 | Org hiệu lực | Chặn org inactive | API | Unhappy | Must | Pass |
| EMP-TC-005 | HĐ; cảnh báo thiếu HĐ | Warning | E2E | Happy | Must | Pass |
| EMP-TC-006 | HR sửa; cấm đổi LM trên SCR-002 | Chặn | E2E | Unhappy | Must | Pass |
| EMP-TC-007 | Self-service web=mobile | Cùng IAM | E2E | Happy | Must | Pass |
| EMP-TC-008 | Đổi LM một bậc | OK luồng | E2E | Happy | Must | Pass |
| EMP-TC-009 | Đổi LM không mở phiếu lương | Không PAY | E2E | Happy | Must | Pass |
| EMP-TC-010 | Thâm niên master | Catalog | API | Happy | Must | Pass |
| EMP-TC-011 | 403 hồ sơ người khác | 403 | API | Unhappy | Must | Pass |
| EMP-TC-012 | Ẩn màn HR với NV | 403 | E2E | Unhappy | Must | Pass |
| EMP-TC-013 | Đủ EMP-SCR-001…006 | Có màn | E2E | Happy | Must | Pass |
| EMP-TC-014 | Field/HĐ master | Động quy chế | E2E | Happy | Must | Pass |
| EMP-TC-015 | LM 403 phiếu lương | 403 | API | Unhappy | Must | Pass |
| EMP-TC-016 | LM mới org inactive | Chặn | API | Unhappy | Must | Pass |
| EMP-TC-017 | Trình độ học vấn FR-017 | Lưu catalog | API | Happy | Must | Pass |
| EMP-TC-NFR-001 | 403 hồ sơ khác | Pass | API | Unhappy | Must | Pass |
| EMP-TC-NFR-002 | Audit tạo/sửa/duyệt LM | Log | API | Happy | Must | Pass |

## 3. Chi tiết test case

Path: `GET|PATCH /emp/employees/{id}`. Tạo NV / đổi LM / màn HR: EMP-SCR — không bịa URL tạo nếu chưa DOC-12. Không mở PAY.  
**Execute 2026-09-04:** St §2 = SoT (slice A/B + e2e-full).

### EMP-TC-001 — HR tạo NV + org + HĐ

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-001, EMP-AC-001 |
| **Steps** | **H:** JWT HR EMP-SCR-002 lưu định danh + org + HĐ. **N:** JWT NV tạo người khác. |
| **Expected** | **H:** hồ sơ tồn tại; không tính lương. **N:** 403. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-002 — Unique MNV/CCCD

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-002, EMP-AC-002 |
| **Steps** | HR lưu MNV đã có; HR lưu CCCD đã có. |
| **Expected** | Chặn (409 hoặc tương đương). |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-003 — Unique email/MST khi có

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-003, EMP-AC-003 |
| **Steps** | **H1:** lưu không email Cty. **H2:** MST trống. **N:** trùng email khi đã nhập. |
| **Expected** | **H1–H2:** OK. **N:** chặn. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-004 — Org hiệu lực

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-004, EMP-AC-004 |
| **Steps** | HR gắn org không hiệu lực. |
| **Expected** | Chặn. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-005 — HĐ; cảnh báo thiếu HĐ

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-005, EMP-AC-005 |
| **Steps** | **H:** có HĐ TV/chính thức. **N:** không HĐ hiệu lực. |
| **Expected** | **H:** PAY đọc fact HĐ. **N:** cảnh báo; không im lặng 85%. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-006 — HR sửa; cấm đổi LM trên SCR-002

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-006, EMP-AC-006 |
| **Steps** | **H:** HR `PATCH` định danh unique/org OK. **N:** đổi LM trên EMP-SCR-002. |
| **Expected** | **H:** lưu. **N:** chặn; phải SCR-005/006. |
| **Layer / Path** | E2E · Unhappy + Happy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-007 — Self-service web = mobile

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-007, EMP-AC-007 |
| **Steps** | **H:** NV sửa field được phép web + mobile. **N:** NV sửa MNV/CCCD. |
| **Expected** | **H:** lưu; cùng IAM. **N:** 403 hoặc read-only. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-008 — Đổi LM một bậc

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-008, EMP-AC-008 |
| **Steps** | **H:** HR gửi EMP-SCR-005 → duyệt EMP-SCR-006. **N:** ghi LM không duyệt. |
| **Expected** | **H:** LM mới ghi. **N:** chặn. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-009 — Đổi LM không mở phiếu lương

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-009, EMP-AC-009, PAY-BR-007 |
| **Steps** | Sau đổi LM, LM mới mở phiếu cấp dưới từ EMP/PAY. |
| **Expected** | Cấm; không nới PAY. |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-010 — Thâm niên master

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-010, EMP-AC-010 |
| **Steps** | Mở hồ sơ; so với công thức master vs hardcode năm luật. |
| **Expected** | Thâm niên = master. Hardcode → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-011 — 403 hồ sơ người khác

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-011, EMP-AC-011 |
| **Steps** | NV `GET/PATCH` hồ sơ khác; NV tự đổi LM MVP. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-012 — Ẩn màn HR với NV

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-012, EMP-AC-012 |
| **Steps** | NV mở EMP-SCR-001. |
| **Expected** | 403 hoặc không menu. |
| **Layer / Path** | E2E · Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-013 — Đủ EMP-SCR-001…006

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-013, EMP-AC-013 |
| **Steps** | Mở đủ 6 màn. |
| **Expected** | Có màn. Pixel HTML không Must. |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-014 — Field/HĐ master

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-014, EMP-AC-014 |
| **Steps** | So list field UI vs master vs list cứng URD. |
| **Expected** | Động quy chế. List cứng URD → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Pass** — `GET /v1/emp/contract-types` + form master (slice-b). |

### EMP-TC-015 — LM 403 phiếu lương

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-015, EMP-AC-015, IAM-AC-004 |
| **Steps** | JWT LM `GET /pay/payslips/{id}` cấp dưới (từ EMP hoặc PAY). |
| **Expected** | **403**. 200 = Blocker. Chạy cặp IAM-TC-004. |
| **Layer / Path** | API · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** |  **Pass** — emp-b / pay-j LM 403 phiếu. |

### EMP-TC-016 — LM mới org inactive

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-016, EMP-AC-016 |
| **Steps** | Duyệt đổi LM khi org LM mới ngừng. |
| **Expected** | Từ chối ghi LM. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — slice-b approve LM re-check org inactive. |

### EMP-TC-017 — Trình độ học vấn

| Mục | Nội dung |
|-----|----------|
| **Trace** | EMP-FR-017, EMP-AC-017 |
| **Steps** | **H:** HR chọn bậc master SCR-002; xem SCR-003. **N1:** mã không master. **N2:** hardcode THPT/ĐH trên code. |
| **Expected** | **H:** lưu catalog. **N1:** chặn. **N2:** fail AC. Catalog bậc nợ Ban HR. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29 / e2e-full / slice-a. |

### EMP-TC-NFR-001 / 002

**Status:** NFR-001 **Pass** (TC-011) · NFR-002 **Pass** (audit LM).

403 hồ sơ khác. Audit tạo/sửa/duyệt LM — DOC-13.

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.2 | Execute St (DEC-DLV-017) | QC / PGD |
| 0.1 | Chốt catalog (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | §3 chi tiết (DEC-DLV-006) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| PGD | Mr. Dư Hùng | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| PGD | Mr. Dư Hùng | 2026-09-04 | ☑ Execute St v0.2 (DEC-DLV-017) — chờ merge |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
