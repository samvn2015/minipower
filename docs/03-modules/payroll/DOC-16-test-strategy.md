# DOC-16 — Test cases (payroll)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** payroll · **PAY** · Trace [DOC-07](DOC-07-acceptance-criteria.md) **Chốt**. E2E sau LBS+GW. **Không** tự code. 85% = PAY không PRB.

## 2. Catalog

| TC ID | Mô tả | Expected | Layer | Path | Pri | St |
|-------|-------|----------|-------|------|-----|-----|
| PAY-TC-001 | Tính kỳ khi TIM đã chốt | OK | API | Happy | Must | |
| PAY-TC-002 | N_tính = N_thực − N_KHL | Đúng CT | UT | Happy | Must | |
| PAY-TC-003 | TV 85% quy chế | Hệ số PAY | UT | Happy | Must | |
| PAY-TC-004 | OT từ công chốt | Đúng | API | Happy | Must | |
| PAY-TC-005 | PC HĐ+tháng master | Catalog | API | Happy | Must | |
| PAY-TC-006 | BH/TNCN tỷ lệ kỳ | Không hardcode | API | Happy | Must | |
| PAY-TC-007 | N_tính > chuẩn | Chặn chốt | API | Unhappy | Must | |
| PAY-TC-008 | Sửa công trên PAY | Cấm | API | Unhappy | Must | |
| PAY-TC-009 | NV/LM tính/chốt/xuất | 403 | API | Unhappy | Must | |
| PAY-TC-010 | Cô lập phiếu | LM 403 cấp dưới | E2E | Unhappy | Must | |
| PAY-TC-011 | Mobile phiếu = IAM | Cùng 403 | E2E | Happy | Must | |
| PAY-TC-012 | Xuất đúng người; không CC LM | OK | E2E | Happy | Must | |
| PAY-TC-013 | Không cộng im lặng | A-001 | UT | Unhappy | Must | |
| PAY-TC-014 | Preview cột DOC-19 | Đủ cột | E2E | Happy | Must | |
| PAY-TC-015 | Nhập PC tháng | OK | E2E | Happy | Must | |
| PAY-TC-016 | Tính lại Draft; không hủy chốt | Chặn hủy | API | Unhappy | Must | |
| PAY-TC-017 | Ẩn màn HR NV/LM | 403 | E2E | Unhappy | Must | |
| PAY-TC-018 | UAT 0đ sau làm tròn | Pass | E2E | Happy | Must | |
| PAY-TC-NFR-001 | Cô lập + 403 DOC-13 | Pass | E2E | Unhappy | Must | |
| PAY-TC-NFR-002 | Audit tính/chốt/xuất/xem | Log | API | Happy | Must | |

## 3. Chi tiết test case

E2E sau LBS+GW+OIDC. Path khung: `POST /pay/periods/{ym}/run`, `GET /pay/payslips/me`, `GET /pay/payslips/{id}`. Chốt/xuất/PC tháng: UI PAY-SCR + API HR — không bịa URL. 85% chỉ PAY (HĐ TV), không PRB. Không bịa số làm tròn.

### PAY-TC-001 — Tính kỳ khi TIM đã chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-001, PAY-AC-001 |
| **Preconditions** | JWT HR/C&B; master kỳ có |
| **Steps** | **H:** tháng TIM Chốt → PAY-SCR-002 / `POST /pay/periods/{ym}/run`. **N:** TIM chưa chốt → cùng thao tác. |
| **Expected** | **H:** Draft; đọc N_thực, OT, N_KHL, HĐ, master. **N:** chặn; không Draft. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### PAY-TC-002 — N_tính = N_thực − N_KHL

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-002, PAY-AC-002 |
| **Preconditions** | Fixture: N_thực gồm phép hưởng; N_KHL = K |
| **Steps** | 1. Tính kỳ. 2. Kiểm tra nếu engine cộng N_phép_hưởng vào N_tính. |
| **Expected** | N_tính = N_thực − K; không + phép hưởng. Cộng phép → fail; không chốt kỳ. |
| **Layer / Path** | UT · Happy + Unhappy |
| **Status** | |

### PAY-TC-003 — Hệ số TV 85% trên PAY

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-003, PAY-AC-003 |
| **Preconditions** | **H1:** HĐ thử việc tại kỳ. **H2:** hết TV tại kỳ. |
| **Steps** | Tính lương thời gian. |
| **Expected** | **H1:** hệ số 0,85 (quy chế PAY). **H2:** 100%. Không lấy hệ số từ PRB. |
| **Layer / Path** | UT · Happy |
| **Status** | |

### PAY-TC-004 — OT từ công chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-004, PAY-AC-004 |
| **Preconditions** | TIM Chốt; OT loại 1.5/2.0/3.0 |
| **Steps** | **H:** tính kỳ. **N:** HR nhập OT tay trên PAY. |
| **Expected** | **H:** OT đúng loại. **N:** cấm; không ô nhập OT. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### PAY-TC-005 — PC HĐ + tháng master

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-005, PAY-AC-005 |
| **Steps** | **H:** PC HĐ + mã tháng ∈ master kỳ → tính. **N:** gắn mã lạ. |
| **Expected** | **H:** cả hai kênh trên preview. **N:** chặn ghi / chặn chốt. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### PAY-TC-006 — BH/TNCN tỷ lệ kỳ

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-006, PAY-AC-006 |
| **Preconditions** | % BH/TNCN master kỳ ≠ số URD cũ |
| **Steps** | Tính kỳ; đối chiếu không hardcode URD. |
| **Expected** | Dùng % kỳ. Hardcode URD → fail AC. |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### PAY-TC-007 — Chặn chốt khi N_tính > chuẩn

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-007, PAY-AC-007 |
| **Steps** | **N:** N_tính > ngày công chuẩn lịch Cty → chốt PAY-SCR-003. **H:** N_tính ≤ chuẩn → chốt. |
| **Expected** | **N:** chặn chốt; preview vẫn xem cảnh báo. **H:** Chốt nếu hết lỗi khác. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | |

### PAY-TC-008 — Cấm sửa công trên PAY

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-008, PAY-AC-008 |
| **Steps** | HR sửa N_thực/OT/phép trên PAY-SCR-002. |
| **Expected** | Cấm; gợi ý sửa TIM rồi chốt lại. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### PAY-TC-009 — NV/LM 403 tính/chốt/xuất

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-009, PAY-AC-009 |
| **Steps** | **N1:** JWT NV → tính / chốt / PC tháng / xuất hàng loạt. **N2:** JWT LM → chốt kỳ. |
| **Expected** | 403 cả hai. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### PAY-TC-010 — Cô lập phiếu

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-010, PAY-AC-010, NFR-002 |
| **Preconditions** | Kỳ Chốt (trừ nhánh Draft) |
| **Steps** | **H:** NV `GET /pay/payslips/me` / PAY-SCR-005. **U1:** NV phiếu người khác. **U2:** LM `GET /pay/payslips/{id}` cấp dưới. **U3:** kỳ Draft, NV mở phiếu. |
| **Expected** | **H:** 200 phiếu mình. **U1–U2:** **403**. **U3:** NV không thấy phiếu. |
| **Layer / Path** | E2E · Unhappy (+ Happy NV) |
| **Severity nếu fail** | Blocker (LM 200 lương) |
| **Status** | |

### PAY-TC-011 — Mobile phiếu = IAM web

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-011, PAY-AC-011 |
| **Steps** | Cùng user PAY-SCR-006 vs web. Mobile mở phiếu người khác. |
| **Expected** | Dữ liệu = web; không nới quyền; 403 giống web. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | |

### PAY-TC-012 — Xuất đúng người; không CC LM

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-012, PAY-AC-012 |
| **Preconditions** | Kỳ Chốt; JWT HR; PAY-SCR-007 |
| **Steps** | **H:** xuất PDF/email. **N:** CC LM hoặc gửi nhầm. |
| **Expected** | **H:** mỗi file/email đúng NV. **N:** cấm. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | |

### PAY-TC-013 — A-001 không cộng im lặng

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-013, PAY-AC-013 |
| **Preconditions** | N_thực **không** gồm phép hưởng (TIM lệch) |
| **Steps** | Tính kỳ. |
| **Expected** | Cảnh báo CR; không im lặng cộng lại; không tự sửa công. |
| **Layer / Path** | UT · Unhappy |
| **Status** | |

### PAY-TC-014 — Preview cột DOC-19

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-014, PAY-AC-014 |
| **Steps** | Tính thành công → PAY-SCR-002. |
| **Expected** | Có N_thực, N_KHL, N_tính, hệ số TV, OT, PC HĐ, PC tháng, BH, TNCN tạm, thực lĩnh. Pixel HTML không fail Must. |
| **Layer / Path** | E2E · Happy |
| **Status** | |

### PAY-TC-015 — Nhập PC tháng

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-015, PAY-AC-015 |
| **Steps** | **H:** PAY-SCR-004 mã ∈ master → lưu tiền. **N:** mã không master. |
| **Expected** | **H:** dòng tháng vào tính kỳ. **N:** chặn lưu. |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | |

### PAY-TC-016 — Tính lại Draft; không hủy chốt

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-016, PAY-AC-016 |
| **Steps** | **H:** kỳ Draft → tính lại. **N1:** kỳ Chốt → tính lại im lặng. **N2:** tìm nút hủy chốt PAY-SCR-003. |
| **Expected** | **H:** ghi đè Draft. **N1:** cấm. **N2:** không có nút MVP. |
| **Layer / Path** | API · Unhappy + Happy |
| **Status** | |

### PAY-TC-017 — Ẩn màn HR với NV/LM

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-017, PAY-AC-017 |
| **Steps** | NV/LM mở PAY-SCR-001. NV mở phiếu mình. |
| **Expected** | DS kỳ: 403 hoặc không menu. NV chỉ PAY-SCR-005/006. |
| **Layer / Path** | E2E · Unhappy |
| **Status** | |

### PAY-TC-018 — UAT 0 đồng sau làm tròn

| Mục | Nội dung |
|-----|----------|
| **Trace** | PAY-FR-018, PAY-AC-018 |
| **Preconditions** | Master làm tròn quy chế kỳ UAT (PAY-A-004); bảng tay C&B. **Không** hardcode số chữ số trên TC. |
| **Steps** | So từng dòng + tổng vs hệ thống. |
| **Expected** | Lệch = 0 đồng. Lệch ≠ 0 → fail UAT kỳ đó; không go-live kỳ đó. |
| **Layer / Path** | E2E · Happy |
| **Status** | |

### PAY-TC-NFR-001 / 002

Cô lập + 403 DOC-13 (trùng TC-010). Audit: log tính / chốt / xuất / xem phiếu — không bịa SLA.

## 4. Trace

PAY-TC-nnn → PAY-AC-nnn / PAY-FR-nnn. NFR → DOC-13.

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
