# DOC-16 — Test cases (lifecycle)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-09-04 | QC (execute) | **Chốt** · St cập nhật (DEC-DLV-018) |
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** lifecycle · **LIF** · [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**. INT-004/005. Job chỉ Active.  
**Evidence:** `e2e-api-lif-slice-a…d.sh` · unit HostRoleGate / early CR · web offboarding.

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| LIF-TC-001 | Checklist on; thiếu Must | Chặn hoàn all | E2E | Unhappy | Must | Pass |
| LIF-TC-002 | Cấp TK lúc on | TK tạo | API | Happy | Must | Pass |
| LIF-TC-003 | N = ngày LV cuối | Đúng N | API | Happy | Must | Pass |
| LIF-TC-004 | NV không kích N+3 | 403 | API | Unhappy | Must | Pass |
| LIF-TC-005 | Khóa Git N+3 lịch | Đúng mốc | API | Happy | Must | Pass |
| LIF-TC-006 | CRM SP cùng mốc | Cùng N+3 | API | Happy | Must | Pass |
| LIF-TC-007 | Khóa trước N+3 | Cấm | API | Unhappy | Must | Pass |
| LIF-TC-008 | HR 403 khóa Git | 403; secret IT | API | Unhappy | Must | Pass |
| LIF-TC-009 | Checklist off | OK | E2E | Happy | Must | Pass |
| LIF-TC-010 | Không CRM sales | 0 call INT-006 | E2E | Unhappy | Must | Pass |
| LIF-TC-011 | Chat theo master | Should | E2E | Happy | Should | Partial |
| LIF-TC-012 | Đủ LIF-SCR-001…006 | Có màn | E2E | Happy | Must | Partial |
| LIF-TC-013 | Hiện N và N+3 | UI đúng | E2E | Happy | Must | Pass |
| LIF-TC-014 | Audit khóa sớm | Log | API | Happy | Must | Partial |
| LIF-TC-015 | NV 403 ghi N job | 403 | API | Unhappy | Must | Pass |
| LIF-TC-016 | Không nút CRM sales | Không UI | E2E | Happy | Must | Pass |
| LIF-TC-NFR-001 | HR không credential Git | Pass | E2E | Unhappy | Must | Pass |
| LIF-TC-NFR-002 | Audit N / khóa Git-CRM | Log | API | Happy | Must | Partial |
| LIF-TC-HA-001 | Job N+3 không chạy DR | Count 0 | HA | Unhappy | Must | Pass |

## 3. Chi tiết test case

Path: onboarding/offboarding API `v1/lif/*`. Job **chỉ Active** (ADR-003 · `IHostRoleGate`). N+3 = N + 3 **ngày lịch**. Secret Git = IT, không HR.  
**Execute 2026-09-04:** St §2 = SoT.

### LIF-TC-001 — Checklist on; thiếu Must

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-001, LIF-AC-001 |
| **Steps** | **H:** tick Must đủ master → HR đóng on LIF-SCR-002. **N:** thiếu tick Must. |
| **Expected** | **H:** cho đóng. **N:** chặn hoàn all. |
| **Layer / Path** | E2E · Unhappy + Happy |
| **Status** |  **Pass** — slice-d close without Must → 400. |

### LIF-TC-002 — Cấp TK lúc on

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-002, LIF-AC-002 |
| **Steps** | Hoàn onboarding. **N:** hẹn cấp Git = N+3. |
| **Expected** | Email/Git/CRM SP/chat đã cấp trên SCR-002. Hẹn N+3 → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — slice-d provision Email/Git/CrmSp/Chat at on. |

### LIF-TC-003 — N = ngày LV cuối

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-003, LIF-AC-003 |
| **Steps** | **H:** HR xác nhận ngày LV cuối LIF-SCR-003. **N:** dùng ngày ký đơn = N. |
| **Expected** | **H:** N lưu. **N:** chặn. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — slice-a confirm N. |

### LIF-TC-004 — NV không kích N+3

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-004, LIF-AC-004 |
| **Steps** | Chỉ NV nhập N (không HR xác nhận). |
| **Expected** | Job N+3 không chạy; không `POST .../locks`. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — NV/LM không IT/PGD khóa (guard). |

### LIF-TC-005 — Khóa Git N+3 lịch

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-005, LIF-AC-005, INT-004 |
| **Preconditions** | N đã HR xác nhận; job Active |
| **Steps** | **H:** ngày hệ thống ≥ N+3 lịch. **N:** chưa đến N+3 (không CR). |
| **Expected** | **H:** Git khóa. **N:** chưa khóa. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — slice-c N+3 Git lock. |

### LIF-TC-006 — CRM SP cùng mốc Git

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-006, LIF-AC-006, INT-005 |
| **Steps** | Quan sát khóa Git vs CRM **SP** (không sales). |
| **Expected** | Cùng lúc. Chỉ khóa Git → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Pass** — slice-c CRM SP cùng mốc. |

### LIF-TC-007 — Cấm khóa trước N+3 (trừ CR)

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-007, LIF-AC-007 |
| **Steps** | **N:** job khóa sớm không CR. **H:** CR an ninh LIF-SCR-006. |
| **Expected** | **N:** cấm. **H:** khóa sớm + audit. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** |  **Pass** — cấm khóa trước N+3 trừ early CR. |

### LIF-TC-008 — HR 403 khóa Git

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-008, LIF-AC-008 |
| **Steps** | JWT HR `POST /lif/cases/{id}/locks` hoặc nút khóa Git. |
| **Expected** | 403 hoặc chỉ ticket IT; **không** credential Git trên HR. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — HR không ApplyLocks (IT/PGD). |

### LIF-TC-009 — Checklist off

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-009, LIF-AC-009 |
| **Steps** | Đóng off thiếu tick Must. |
| **Expected** | Chặn. (Happy đủ Must: cho đóng — cùng SCR off.) |
| **Layer / Path** | E2E · Happy (đủ) + Unhappy |
| **Status** |  **Pass** — slice-b off checklist. |

### LIF-TC-010 — Không CRM sales

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-010, LIF-AC-010, NFR-007, INT-006 |
| **Steps** | Offboarding; bắt webhook/outbound. |
| **Expected** | **0** call CRM bán hàng. Có call = Blocker. |
| **Layer / Path** | E2E · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** |  **Pass** — không CRM sales channel. |

### LIF-TC-011 — Chat theo master (Should)

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-011, LIF-AC-011 |
| **Preconditions** | Master chat ≠ N+3 Git |
| **Steps** | Khóa chat. |
| **Expected** | Mốc đúng master. Im lặng = Git khi master khác → fail. |
| **Layer / Path** | E2E · Happy |
| **Priority** | Should |
| **Status** |  **Partial** — Should Chat provision có; UAT master mỏng. |

### LIF-TC-012 — Đủ LIF-SCR-001…006

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-012, LIF-AC-012 |
| **Steps** | Mở 6 màn. |
| **Expected** | Có màn. Pixel HTML không Must. |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Partial** — on/off pages; chưa đủ 6 SCR tách. |

### LIF-TC-013 — Hiện N và N+3

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-013, LIF-AC-013 |
| **Preconditions** | N đã xác nhận |
| **Steps** | Mở LIF-SCR-001/004. |
| **Expected** | Hiện N và N+3 dự kiến = N + 3 ngày lịch (nháp). |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Pass** — UI hiện N và N+3. |

### LIF-TC-014 — Audit khóa sớm

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-014, LIF-AC-014 |
| **Steps** | **H:** khóa sớm có CR. **N:** khóa sớm không CR. |
| **Expected** | **H:** log CR. **N:** ghi vi phạm. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** |  **Partial** — early CR fields persist; audit EmpLog riêng mỏng. |

### LIF-TC-015 — NV 403 ghi N kích job

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-015, LIF-AC-015 |
| **Steps** | JWT NV lưu N nhằm schedule job. |
| **Expected** | 403 hoặc không schedule. |
| **Layer / Path** | API · Unhappy |
| **Status** |  **Pass** — confirm N HR-only. |

### LIF-TC-016 — Không nút CRM sales

| Mục | Nội dung |
|-----|----------|
| **Trace** | LIF-FR-016, LIF-AC-016 |
| **Steps** | Mở LIF-SCR-004. |
| **Expected** | Không nút gửi CRM sales. Có nút → fail AC. |
| **Layer / Path** | E2E · Happy |
| **Status** |  **Pass** — UI ghi không CRM sales. |

### LIF-TC-NFR-001 / 002 · LIF-TC-HA-001

**Status:** NFR-001 **Pass** · NFR-002 **Partial** · HA-001 **Pass** (HostRoleGate Standby).

HR không credential Git. Audit N / khóa Git-CRM. **HA:** trên Standby/DR, job N+3 count = 0 (không bịa % uptime / kubectl).

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.2 | Execute St (DEC-DLV-018) | QC / PGD |
| 0.1 | Chốt catalog (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | §3 chi tiết (DEC-DLV-006) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| PGD | Mr. Dư Hùng | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| PGD | Mr. Dư Hùng | 2026-09-04 | ☑ Execute St v0.2 (DEC-DLV-018) — chờ merge |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
