# DOC-16 — Test cases (identity)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** identity · **IAM** · [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**. ADR-002/007 · `GET /iam/me`.

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| IAM-TC-001 | Không JWT / hết hạn | 401 | API | Unhappy | Must | |
| IAM-TC-002 | Web = mobile | Cùng 401/403 | E2E | Happy | Must | |
| IAM-TC-003 | 5 role MVP | Đúng map | API | Happy | Must | |
| IAM-TC-004 | LM 403 phiếu cấp dưới | 403 | API | Unhappy | Must | |
| IAM-TC-005 | NV 403 dữ liệu người khác | 403 | API | Unhappy | Must | |
| IAM-TC-006 | HR SoT màn Cty | HR vào được | E2E | Happy | Must | |
| IAM-TC-007 | IT 403 PAY | 403 | API | Unhappy | Must | |
| IAM-TC-008 | NV/LM 403 màn HR | 403 | API | Unhappy | Must | |
| IAM-TC-009 | Đổi LM không nới lương | 403 lương giữ | API | Happy | Must | |
| IAM-TC-010 | Disable login; không nút Git | 401; không Git UI | E2E | Happy | Must | |
| IAM-TC-011 | Audit xem phiếu | Log | API | Happy | Must | |
| IAM-TC-012 | Cấm CRM sales | 0 call | E2E | Unhappy | Must | |
| IAM-TC-013 | NV/LM không gán role | 403 | API | Unhappy | Must | |
| IAM-TC-014 | Hợp quyền; thiếu perm lương | 403 lương | API | Unhappy | Must | |
| IAM-TC-015 | LM C1 phép OK | 200 C1 | API | Happy | Must | |
| IAM-TC-016 | LM 403 C2/đột xuất | 403 | API | Unhappy | Must | |
| IAM-TC-017 | Map 1 MNV; disable ≠ xóa EMP | Hồ sơ còn | API | Happy | Must | |
| IAM-TC-018 | Đủ IAM-SCR-001…004 | Có màn | E2E | Happy | Must | |
| IAM-TC-019 | PGD 403 phiếu Cty | 403 trừ policy | API | Unhappy | Must | |
| IAM-TC-NFR-001 | NFR-002…004 | Pass | E2E | Unhappy | Must | |
| IAM-TC-NFR-002 | NFR-005 audit lương | Log | API | Happy | Must | |

## 3. Chi tiết test case

OIDC RP (ADR-002/007). `GET /iam/me`. **Không** `POST /login` password. Không khóa vendor IdP. MFA TBD.

### IAM-TC-001 — Không JWT / hết hạn

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-001, IAM-AC-001 |
| **Steps** | **H:** login IAM-SCR-001 TK hiệu lực (OIDC). **N1:** API Must không Bearer / hết hạn. **N2:** endpoint HRM public không auth. |
| **Expected** | **H:** có phiên; `GET /iam/me` 200. **N1:** 401. **N2:** fail AC. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | |

### IAM-TC-002 — Web = mobile

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-002, IAM-AC-002 |
| **Steps** | Cùng role: case 401/403 trên web vs mobile. |
| **Expected** | Giống nhau. Mobile nới quyền → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | |

### IAM-TC-003 — 5 role MVP

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-003, IAM-AC-003 |
| **Steps** | **H:** gán IAM-ROLE-LM (và đủ 5 role master). **N:** role lạ không master. |
| **Expected** | **H:** role lưu; `GET /iam/me` đúng map. **N:** chặn. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### IAM-TC-004 — LM 403 phiếu cấp dưới

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-004, IAM-AC-004 |
| **Steps** | JWT LM `GET /pay/payslips/{id}` cấp dưới. |
| **Expected** | **403**. 200 = Blocker. Cặp EMP-TC-015 / PAY-TC-010. |
| **Layer / Path** | API · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** | |

### IAM-TC-005 — NV 403 dữ liệu người khác

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-005, IAM-AC-005 |
| **Steps** | NV `GET /emp/employees/{id}` người khác. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### IAM-TC-006 — HR SoT màn Cty

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-006, IAM-AC-006 |
| **Steps** | JWT HR mở EMP DS / TIM / PAY kỳ. |
| **Expected** | 200 (IAM). |
| **Layer / Path** | E2E · Happy |
| **Status** | |

### IAM-TC-007 — IT 403 PAY

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-007, IAM-AC-007 |
| **Steps** | JWT IT không role HR → PAY run / phiếu Cty. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### IAM-TC-008 — NV/LM 403 màn HR

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-008, IAM-AC-008 |
| **Steps** | NV/LM vào TIM-SCR-001 (và màn HR tương đương). |
| **Expected** | 403 hoặc không menu. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### IAM-TC-009 — Đổi LM không nới lương

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-009, IAM-AC-009 |
| **Steps** | Gán LM kèm quyền lương. |
| **Expected** | Cấm; 403 lương giữ như trước. |
| **Layer / Path** | API · Happy |
| **Status** | |

### IAM-TC-010 — Disable login; không nút Git

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-010, IAM-AC-010 |
| **Steps** | IT vô hiệu IAM-SCR-004 → login lại. Tìm nút Khóa Git trên SCR-004. |
| **Expected** | Login sau → 401. Có nút Git → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | |

### IAM-TC-011 — Audit xem phiếu

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-011, IAM-AC-011, NFR-005 |
| **Steps** | HR xem phiếu NV. |
| **Expected** | Có log audit. |
| **Layer / Path** | API · Happy |
| **Status** | |

### IAM-TC-012 — Cấm CRM sales

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-012, IAM-AC-012, INT-006 |
| **Steps** | Thử cấp token / gọi CRM bán hàng từ IAM. |
| **Expected** | Cấm; 0 call. Fail = Blocker. |
| **Layer / Path** | E2E · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** | |

### IAM-TC-013 — NV/LM không gán role

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-013, IAM-AC-013 |
| **Steps** | JWT NV hoặc LM gán role. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### IAM-TC-014 — Hợp quyền; thiếu perm lương

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-014, IAM-AC-014 |
| **Steps** | **H:** user LM+HR. **N:** chỉ LM → phiếu người khác. |
| **Expected** | **H:** quyền HR gồm lương catalog. **N:** 403. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | |

### IAM-TC-015 — LM C1 phép OK

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-015, IAM-AC-015 |
| **Steps** | JWT LM `POST /lev/requests/{id}/c1` cấp dưới. |
| **Expected** | 200 C1; không mở phiếu lương. |
| **Layer / Path** | API · Happy |
| **Status** | |

### IAM-TC-016 — LM 403 C2 (kể cả đột xuất)

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-016, IAM-AC-016 |
| **Steps** | JWT LM `POST /lev/requests/{id}/c2` (đơn thường hoặc đột xuất đã C1). |
| **Expected** | 403. (AC gốc: C2; catalog thêm đột xuất = cùng 403.) |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### IAM-TC-017 — Map 1 MNV; disable ≠ xóa EMP

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-017, IAM-AC-017 |
| **Steps** | **H:** 1 TK = 1 MNV login. **N:** disable TK rồi kiểm tra hồ sơ EMP. |
| **Expected** | **H:** đúng hồ sơ. **N:** hồ sơ còn; xóa EMP → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### IAM-TC-018 — Đủ IAM-SCR-001…004

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-018, IAM-AC-018 |
| **Steps** | Mở 4 màn. |
| **Expected** | Có màn. Pixel HTML không Must. |
| **Layer / Path** | E2E · Happy |
| **Status** | |

### IAM-TC-019 — PGD 403 phiếu Cty

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-019, IAM-AC-019 |
| **Preconditions** | JWT PGD **không** gán HR (trừ policy đã chốt khác — không bịa) |
| **Steps** | Mở phiếu lương Cty. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### IAM-TC-NFR-001 / 002

NFR-002…004 cô lập; NFR-005 audit lương — DOC-13; không bịa %.

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.1 | Chốt catalog (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | §3 chi tiết (DEC-DLV-006) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| PGD | Mr. Dư Hùng | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
