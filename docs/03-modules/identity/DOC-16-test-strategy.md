# DOC-16 — Test cases (identity)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-09-04 | QC (execute) | **Chốt** · St cập nhật (DEC-DLV-012) |
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** identity · **IAM** · [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**. ADR-002/007 · `GET /iam/me`.  
**Evidence:** `memory/delivery/tc-run-2026-09-04.md` · `e2e-api-iam-rbac.sh` · `e2e-api-pay-slice-j.sh` · `e2e-api-lev-slice-c/d.sh` · DEC-DLV-011 (Lark JWKS bypass DEV).

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| IAM-TC-001 | Không JWT / hết hạn | 401 | API | Unhappy | Must | Partial |
| IAM-TC-002 | Web = mobile | Cùng 401/403 | E2E | Happy | Must | Partial |
| IAM-TC-003 | 5 role MVP | Đúng map | API | Happy | Must | Pass |
| IAM-TC-004 | LM 403 phiếu cấp dưới | 403 | API | Unhappy | Must | Pass |
| IAM-TC-005 | NV 403 dữ liệu người khác | 403 | API | Unhappy | Must | Pass |
| IAM-TC-006 | HR SoT màn Cty | HR vào được | E2E | Happy | Must | Pass |
| IAM-TC-007 | IT 403 PAY | 403 | API | Unhappy | Must | Pass |
| IAM-TC-008 | NV/LM 403 màn HR | 403 | API | Unhappy | Must | Pass |
| IAM-TC-009 | Đổi LM không nới lương | 403 lương giữ | API | Happy | Must | Pass |
| IAM-TC-010 | Disable login; không nút Git | 401; không Git UI | E2E | Happy | Must | Partial |
| IAM-TC-011 | Audit xem phiếu | Log | API | Happy | Must | Pass |
| IAM-TC-012 | Cấm CRM sales | 0 call | E2E | Unhappy | Must | Pass |
| IAM-TC-013 | NV/LM không gán role | 403 | API | Unhappy | Must | Pass |
| IAM-TC-014 | Hợp quyền; thiếu perm lương | 403 lương | API | Unhappy | Must | Pass |
| IAM-TC-015 | LM C1 phép OK | 200 C1 | API | Happy | Must | Pass |
| IAM-TC-016 | LM 403 C2/đột xuất | 403 | API | Unhappy | Must | Pass |
| IAM-TC-017 | Map 1 MNV; disable ≠ xóa EMP | Hồ sơ còn | API | Happy | Must | Pass |
| IAM-TC-018 | Đủ IAM-SCR-001…004 | Có màn | E2E | Happy | Must | Partial |
| IAM-TC-019 | PGD 403 phiếu Cty | 403 trừ policy | API | Unhappy | Must | Pass |
| IAM-TC-NFR-001 | NFR-002…004 | Pass | E2E | Unhappy | Must | Partial |
| IAM-TC-NFR-002 | NFR-005 audit lương | Log | API | Happy | Must | Pass |

## 3. Chi tiết test case

OIDC RP (ADR-002/007). `GET /iam/me`. **Không** `POST /login` password. Không khóa vendor IdP. MFA TBD.  
**Execute 2026-09-04:** St cột §2 = nguồn sự thật; dưới đây ghi evidence ngắn (Partial = còn OIDC Lark / SCR-001).

### IAM-TC-001 — Không JWT / hết hạn

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-001, IAM-AC-001 |
| **Steps** | **H:** login IAM-SCR-001 TK hiệu lực (OIDC). **N1:** API Must không Bearer / hết hạn. **N2:** endpoint HRM public không auth. |
| **Expected** | **H:** có phiên; `GET /iam/me` 200. **N1:** 401. **N2:** fail AC. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | **Partial** — N1: 401 không Bearer (Host). H OIDC Lark hoãn DEC-DLV-011. |

### IAM-TC-002 — Web = mobile

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-002, IAM-AC-002 |
| **Steps** | Cùng role: case 401/403 trên web vs mobile. |
| **Expected** | Giống nhau. Mobile nới quyền → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | **Partial** — cùng API/IAM phiếu web↔mobile (`e2e-api-pay-slice-j`); OIDC màn login chưa. |

### IAM-TC-003 — 5 role MVP

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-003, IAM-AC-003 |
| **Steps** | **H:** gán IAM-ROLE-LM (và đủ 5 role master). **N:** role lạ không master. |
| **Expected** | **H:** role lưu; `GET /iam/me` đúng map. **N:** chặn. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — e2e-full role assign/remove · seed 5 role. |

### IAM-TC-004 — LM 403 phiếu cấp dưới

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-004, IAM-AC-004 |
| **Steps** | JWT LM `GET /pay/payslips/{id}` cấp dưới. |
| **Expected** | **403**. 200 = Blocker. Cặp EMP-TC-015 / PAY-TC-010. |
| **Layer / Path** | API · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** | **Pass** — `e2e-api-pay-slice-j` · `e2e-api-emp-slice-b`. |

### IAM-TC-005 — NV 403 dữ liệu người khác

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-005, IAM-AC-005 |
| **Steps** | NV `GET /emp/employees/{id}` người khác. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — e2e-web / EMP NV guard. |

### IAM-TC-006 — HR SoT màn Cty

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-006, IAM-AC-006 |
| **Steps** | JWT HR mở EMP DS / TIM / PAY kỳ. |
| **Expected** | 200 (IAM). |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Pass** — local-dev HR e2e Must. |

### IAM-TC-007 — IT 403 PAY

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-007, IAM-AC-007 |
| **Steps** | JWT IT không role HR → PAY run / phiếu Cty. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — `e2e-api-iam-rbac.sh`. |

### IAM-TC-008 — NV/LM 403 màn HR

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-008, IAM-AC-008 |
| **Steps** | NV/LM vào TIM-SCR-001 (và màn HR tương đương). |
| **Expected** | 403 hoặc không menu. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — LM TIM import/periods 403 (`e2e-api-iam-rbac`). |

### IAM-TC-009 — Đổi LM không nới lương

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-009, IAM-AC-009 |
| **Steps** | Gán LM kèm quyền lương. |
| **Expected** | Cấm; 403 lương giữ như trước. |
| **Layer / Path** | API · Happy |
| **Status** |  **Pass** — role LM không mở phiếu người khác (cùng 004). |

### IAM-TC-010 — Disable login; không nút Git

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-010, IAM-AC-010 |
| **Steps** | IT vô hiệu IAM-SCR-004 → login lại. Tìm nút Khóa Git trên SCR-004. |
| **Expected** | Login sau → 401. Có nút Git → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** |  **Partial** — API disable + UI 「Không khóa Git/CRM」; login sau disable còn thiếu e2e riêng. |

### IAM-TC-011 — Audit xem phiếu

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-011, IAM-AC-011, NFR-005 |
| **Steps** | HR xem phiếu NV. |
| **Expected** | Có log audit. |
| **Layer / Path** | API · Happy |
| **Status** |  **Pass** — `PayslipViewed` (`e2e-api-iam-rbac`). |

### IAM-TC-012 — Cấm CRM sales

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-012, IAM-AC-012, INT-006 |
| **Steps** | Thử cấp token / gọi CRM bán hàng từ IAM. |
| **Expected** | Cấm; 0 call. Fail = Blocker. |
| **Layer / Path** | E2E · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** |  **Pass** — không endpoint CRM sales trong IAM; LIF/LEV chặn kênh CRM. |

### IAM-TC-013 — NV/LM không gán role

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-013, IAM-AC-013 |
| **Steps** | JWT NV hoặc LM gán role. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — tc-run 2026-08-29. |

### IAM-TC-014 — Hợp quyền; thiếu perm lương

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-014, IAM-AC-014 |
| **Steps** | **H:** user LM+HR. **N:** chỉ LM → phiếu người khác. |
| **Expected** | **H:** quyền HR gồm lương catalog. **N:** 403. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** |  **Pass** — N: LM 403 phiếu; H: local-dev HR+NV. |

### IAM-TC-015 — LM C1 phép OK

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-015, IAM-AC-015 |
| **Steps** | JWT LM `POST /lev/requests/{id}/c1` cấp dưới. |
| **Expected** | 200 C1; không mở phiếu lương. |
| **Layer / Path** | API · Happy |
| **Status** |  **Pass** — `e2e-api-lev-slice-c.sh`. |

### IAM-TC-016 — LM 403 C2 (kể cả đột xuất)

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-016, IAM-AC-016 |
| **Steps** | JWT LM `POST /lev/requests/{id}/c2` (đơn thường hoặc đột xuất đã C1). |
| **Expected** | 403. (AC gốc: C2; catalog thêm đột xuất = cùng 403.) |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — `e2e-api-lev-slice-d.sh` LM C2 403. |

### IAM-TC-017 — Map 1 MNV; disable ≠ xóa EMP

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-017, IAM-AC-017 |
| **Steps** | **H:** 1 TK = 1 MNV login. **N:** disable TK rồi kiểm tra hồ sơ EMP. |
| **Expected** | **H:** đúng hồ sơ. **N:** hồ sơ còn; xóa EMP → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — unit + auto-provision e2e-full. |

### IAM-TC-018 — Đủ IAM-SCR-001…004

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-018, IAM-AC-018 |
| **Steps** | Mở 4 màn. |
| **Expected** | Có màn. Pixel HTML không Must. |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Partial** — SCR-003/004 web; SCR-001 OIDC Lark hoãn DEC-DLV-011. |

### IAM-TC-019 — PGD 403 phiếu Cty

| Mục | Nội dung |
|-----|----------|
| **Trace** | IAM-FR-019, IAM-AC-019 |
| **Preconditions** | JWT PGD **không** gán HR (trừ policy đã chốt khác — không bịa) |
| **Steps** | Mở phiếu lương Cty. |
| **Expected** | 403. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — `e2e-api-iam-rbac` gán PGD → slip 403. |

### IAM-TC-NFR-001 / 002

NFR-002…004 cô lập; NFR-005 audit lương — DOC-13; không bịa %.  
**Status:** NFR-001 **Partial** (DEV); NFR-002 **Pass** (`PayslipViewed` = TC-011).

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.2 | Execute St catalog + §3 (DEC-DLV-012) | QC / PGD |
| 0.1 | Chốt catalog (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | §3 chi tiết (DEC-DLV-006) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| PGD | Mr. Dư Hùng | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| PGD | Mr. Dư Hùng | 2026-09-04 | ☑ Execute St v0.2 (DEC-DLV-012) — chờ xác nhận |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
