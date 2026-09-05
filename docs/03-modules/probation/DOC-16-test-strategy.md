# DOC-16 — Test cases (probation)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.3 | 2026-09-04 | DEV | **Chốt** · HA-001 + TC-013 (DEC-DLV-020) |
| 0.2 | 2026-09-04 | QC (execute) | **Chốt** · St cập nhật (DEC-DLV-013) |
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** probation · **PRB** · [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**. Job T-15/T-7 chỉ Active.  
**Evidence:** `e2e-api-prb-slice-a…e.sh` · `memory/delivery/tc-run-2026-09-04.md` · unit Probation*.

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| PRB-TC-001 | Không bịa mốc; lấy KT EMP | Đúng HĐ | API | Happy | Must | Pass |
| PRB-TC-002 | T-15 ngày lịch | Có cảnh báo | API | Happy | Must | Pass |
| PRB-TC-003 | Task T-7 | Có task | API | Happy | Must | Pass |
| PRB-TC-004 | Chỉ 3 mã | Chặn mã lạ | API | Unhappy | Must | Pass |
| PRB-TC-005 | Đạt → EMP; không 85% PRB | Không hệ số PRB | API | Happy | Must | Pass |
| PRB-TC-006 | Gia hạn = master | Chặn số tháng tự do | API | Unhappy | Must | Pass |
| PRB-TC-007 | Không đạt → LIF; không xóa im | Mở LIF | API | Happy | Must | Pass |
| PRB-TC-008 | 0 sót coverage TV | Mọi NV TV vào hàng | API | Happy | Must | Pass |
| PRB-TC-009 | LM/NV 403 chốt | 403; LM lưu đề xuất OK | API | Unhappy | Must | Pass |
| PRB-TC-010 | Cấm CRM sales | 0 call | E2E | Unhappy | Must | Pass |
| PRB-TC-011 | In-app + email/app | Đủ 2 kênh | E2E | Happy | Must | Pass |
| PRB-TC-012 | Phiếu động catalog | Không field cứng | E2E | Happy | Must | Pass |
| PRB-TC-013 | Đủ PRB-SCR-001…004 | Có 4 màn; pixel không fail | E2E | Happy | Must | Pass |
| PRB-TC-014 | Không LM → HR; thiếu đề xuất vẫn chốt | HR chốt được | API | Happy | Must | Pass |
| PRB-TC-015 | Không date picker KT ảo | Link EMP | E2E | Unhappy | Must | Pass |
| PRB-TC-016 | T-15/T-7 theo KT mới | Sau gia hạn | API | Happy | Must | Pass |
| PRB-TC-017 | Audit người chốt = HR | Log user HR | API | Happy | Must | Pass |
| PRB-TC-HA-001 | Job T-15/T-7 không chạy DR | Count 0 | HA | Unhappy | Must | Pass |

## 3. Chi tiết test case

Path: `GET /prb/cases`, `POST .../propose` (LM), `POST .../decide` (HR). Job T-15/T-7 **chỉ Active**. 85% = PAY không PRB.  
**Execute 2026-09-04:** St §2 = SoT; Partial/Open ghi rõ nợ.

### PRB-TC-001 — Không bịa mốc; lấy KT EMP

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-001, PRB-AC-001 |
| **Preconditions** | HĐ EMP có ngày KT TV |
| **Steps** | Mở case / job. **N:** job gán KT mặc định. |
| **Expected** | Dùng đúng ngày HĐ. KT mặc định → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — `e2e-api-prb-slice-a` (source EMP.Contract; skip thiếu EndDate). |

### PRB-TC-002 — T-15 ngày lịch

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-002, PRB-AC-002 |
| **Steps** | Ngày hệ thống = KT_TV − 15 **ngày lịch**. **N:** đếm ngày công (không CR). |
| **Expected** | Có cảnh báo T-15. Đếm công → fail AC v0.1. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — `e2e-api-prb-slice-b` asOf T-15. |

### PRB-TC-003 — Task T-7

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-003, PRB-AC-003 |
| **Steps** | Ngày = KT_TV − 7 lịch. |
| **Expected** | Có task đánh giá. Không tạo khi đủ mốc → fail. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — slice-b + slice-e T-7. |

### PRB-TC-004 — Chỉ 3 mã

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-004, PRB-AC-004 |
| **Steps** | **H:** `POST .../decide` Đạt / Gia hạn / Không đạt. **N:** mã “đạt có điều kiện” không master. |
| **Expected** | **H:** OK. **N:** chặn. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | **Pass** — `e2e-api-prb-slice-c` master PASS/EXTEND/FAIL; invalid → 400. |

### PRB-TC-005 — Đạt → EMP; không 85% PRB

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-005, PRB-AC-005 |
| **Steps** | HR chốt Đạt. Kiểm tra output hệ số 85% từ PRB. |
| **Expected** | EMP yêu cầu chuyển HĐ chính thức. PRB xuất 85% → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — `e2e-api-prb-slice-d` PASS → OFFICIAL; không hệ số PRB. |

### PRB-TC-006 — Gia hạn = master

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-006, PRB-AC-006 |
| **Steps** | **H:** chọn thời lượng master. **N:** nhập số tháng tự do. |
| **Expected** | **H:** KT_TV cập nhật. **N:** chặn. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | **Pass** — slice-c/d EXTEND + EXT-1M master. |

### PRB-TC-007 — Không đạt → LIF; không xóa im

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-007, PRB-AC-007 |
| **Steps** | HR chốt Không đạt. Kiểm tra hồ sơ EMP. |
| **Expected** | Mở luồng off LIF. Xóa im lặng EMP → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — slice-d FAIL → LIF open; EMP còn. |

### PRB-TC-008 — 0 sót coverage TV

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-008, PRB-AC-008 |
| **Steps** | Liệt kê mọi NV TV hiệu lực vs hàng T-15/T-7. |
| **Expected** | Không sót. Bỏ một NV → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — `GET /prb/cases` Active TV từ EMP (slice-a). |

### PRB-TC-009 — LM/NV 403 chốt; LM đề xuất OK

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-009, PRB-AC-009 |
| **Steps** | **H:** HR `POST .../decide` PRB-SCR-003. **N:** JWT LM hoặc NV `/decide`. **H2:** LM `POST .../propose`. |
| **Expected** | **H:** 200 SoT. **N:** 403. **H2:** lưu đề xuất; không đổi HĐ. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | **Pass** — `e2e-api-prb-slice-c` LM decide 403. |

### PRB-TC-010 — Cấm CRM sales

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-010, PRB-AC-010, INT-006 |
| **Steps** | Job/màn PRB; bắt notify. |
| **Expected** | 0 call CRM bán hàng. Có = Blocker. |
| **Layer / Path** | E2E · Unhappy |
| **Severity nếu fail** | Blocker |
| **Status** | **Pass** — slice-b channel `hrm-inapp+email`; assert không CRM. |

### PRB-TC-011 — In-app + email/app

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-011, PRB-AC-011 |
| **Steps** | T-15 và T-7. |
| **Expected** | Đủ in-app HRM **và** email/app. Chỉ in-app → fail. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | **Pass** — slice-b reminder channel inapp+email. |

### PRB-TC-012 — Phiếu động catalog

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-012, PRB-AC-012 |
| **Steps** | Render phiếu; so field cứng UI. |
| **Expected** | Tiêu chí master. List cứng → fail AC. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | **Pass** — masters outcomes/criteria/extend (slice-c); UI select từ API. |

### PRB-TC-013 — Đủ PRB-SCR-001…004

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-013, PRB-AC-013 |
| **Steps** | Mở 4 màn. |
| **Expected** | Có 4 màn. Pixel HTML MCP không fail AC. |
| **Layer / Path** | E2E · Happy |
| **Status** | **Pass** — `/prb/cases` (001) · `/prb/cases/:id/evaluate` (002) · `/prb/cases/:id/decide` (003) · `/prb/incomplete` (004). Pixel HTML không Must. |

### PRB-TC-014 — Không LM → HR; thiếu đề xuất vẫn chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-014, PRB-AC-014 |
| **Steps** | **H1:** NV không LM → task T-7. **H2:** LM chưa đề xuất; HR decide. |
| **Expected** | **H1:** task gán HR. **H2:** HR chốt được. |
| **Layer / Path** | API · Happy |
| **Status** | **Pass** — `e2e-api-prb-slice-e`. |

### PRB-TC-015 — Không date picker KT ảo

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-015, PRB-AC-015 |
| **Steps** | PRB-SCR-004; thử nhập KT trên PRB. |
| **Expected** | Cảnh báo + link EMP; không date picker KT. Nhập KT ảo → fail AC. |
| **Layer / Path** | E2E · Unhappy + Happy |
| **Status** | **Pass** — `PrbMyMilestonesPage` read-only; source EMP.Contract. |

### PRB-TC-016 — T-15/T-7 theo KT mới

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-016, PRB-AC-016 |
| **Preconditions** | Sau Gia hạn; KT mới |
| **Steps** | Quan sát lịch nhắc. |
| **Expected** | Tính theo KT mới. Nhắc KT cũ → fail. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — slice-d EXTEND cập nhật KT; reminders derive từ EMP KT. |

### PRB-TC-017 — Audit người chốt = HR

| Mục | Nội dung |
|-----|----------|
| **Trace** | PRB-FR-017, PRB-AC-017 |
| **Steps** | HR chốt; đọc audit. |
| **Expected** | Log user HR + thời điểm. SoT không audit → fail. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | **Pass** — `ProbationDecided` (`e2e-api-prb-slice-e`). |

### PRB-TC-HA-001 — Job không chạy DR

| Mục | Nội dung |
|-----|----------|
| **Trace** | ADR-003 |
| **Steps** | Standby/DR: đếm job T-15/T-7. |
| **Expected** | Count 0. Không bịa % / kubectl. |
| **Layer / Path** | HA · Unhappy |
| **Status** | **Pass** — `RunProbationRemindersCommandHandler` + `IHostRoleGate` Standby → `BadRequest`, không tạo reminder (unit `HandleAsync_StandbyHost_Throws_NoReminders`). |

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.3 | HA-001 + TC-013 Pass — Standby gate + tách SCR-001…004 (DEC-DLV-020) | DEV |
| 0.2 | Execute St catalog + §3 (DEC-DLV-013) | QC / PGD |
| 0.1 | Chốt catalog (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | §3 chi tiết (DEC-DLV-006) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| PGD | Mr. Dư Hùng | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| PGD | Mr. Dư Hùng | 2026-09-04 | ☑ Execute St v0.2 (DEC-DLV-013) — chờ xác nhận merge |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
