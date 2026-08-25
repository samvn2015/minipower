# DOC-04 — Quy tắc nghiệp vụ — Payroll (PAY)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-24 | Trịnh Yên (BA) | **Chốt** (cổng BR · payroll · DEC-REQ-011) |

**Module:** payroll · **MOD:** PAY · **Phạm vi:** BRQ-002, 003, 007, 009 (phần lương).  
**Không:** import Excel công (TIM); quỹ phép / C1–C2 (LEV — PAY chỉ nhận **ngày phép Đã duyệt**); N+3 (LIF); danh mục PC cứng trên BR.

**Cổng:** DEC-REQ-011 **đã chốt** 2026-08-24. Nợ: làm tròn → DOC-07; master PC/BH = động (014); Ban HR ☐. **Chưa** `02-baseline/`. Mở: DOC-05 · DOC-19 (không tự viết).

---

## 1. Mục đích & phạm vi

Tính lương tháng từ bảng công **đã chốt** + hợp đồng + master quy chế: N_tính, thử việc 85%, OT, phụ cấp/thưởng hai kênh, BH/TNCN tạm, phiếu lương cô lập (web/mobile/PDF/email). UAT sai số **0 đồng** vs kiểm tra tay.

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| PAY-BR-001 | N_tính | N_tính = N_thực − N_KHL; **không** + phép hưởng lương | Calculation | Must | BRQ-002 · BR-001 | SH-002 |
| PAY-BR-002 | Trần ngày công | N_tính ≤ ngày công chuẩn tháng (lịch Cty) | Validation | Must | BR-001 | SH-002 |
| PAY-BR-003 | Thử việc 85% | Lương thời gian × **85%** khi đang TV (quy chế) | Calculation | Must | BRQ-003 · BR-002 | SH-002 |
| PAY-BR-004 | OT | 1.5 / 2.0 / 3.0; giờ từ bảng công **đã chốt** | Calculation | Must | BR-003 | SH-002 |
| PAY-BR-005 | PC/thưởng | Kênh HĐ cố định **và** nhập tháng; danh mục master quy chế | Calculation | Must | BRQ-007 | SH-002 |
| PAY-BR-006 | BH / TNCN | Tỷ lệ **tại kỳ lương** theo luật/quy chế; không hardcode URD | Calculation | Must | CN-001 · D-002 | SH-002 |
| PAY-BR-007 | Phiếu lương | Chỉ người đó + HR/C&B theo IAM; Manager không xem lương cấp dưới trừ policy khác = **không** (URD) | Authorization | Must | BRQ-009 · CN-002 | SH-002 |
| PAY-BR-008 | UAT 0 đồng | Kỳ mẫu: kết quả hệ thống = bảng tay (làm tròn theo quy chế) | Validation | Must | BRQ-009 | SH-002 |
| PAY-BR-009 | Đầu vào | Chỉ TIM **chốt tháng** + ngày LEV **Đã duyệt**; không tự sửa công/phép | Validation | Must | TIM · LEV | SH-002 |
| PAY-BR-010 | Ai chạy lương | Chỉ HR/C&B; NV xem phiếu mình | Authorization | Must | IAM | SH-002 |
| PAY-BR-011 | N_thực | Ngày công CC **đã gồm** ngày phép hưởng lương (A-001) | Inference | Must | DOC-03 A-001 | SH-002 |
| PAY-BR-012 | Mobile phiếu | Self-service phiếu **của mình** cùng IAM web | Authorization | Must | BRQ-006 | SH-004 |

**Loại:** Validation · Calculation · Authorization · Inference.

## 3. Chi tiết quy tắc

### PAY-BR-001 — N_tính không cộng kép phép hưởng

| Mục | Nội dung |
|-----|----------|
| **Statement** | Ngày công tính lương **N_tính = N_thực − N_phép_không_lương**. Không cộng thêm số ngày phép hưởng lương (đã nằm trong N_thực). |
| **Condition** | IF công thức cộng N_phép_hưởng vào N_tính |
| **Action** | THEN **cấm** (sai lương) |
| **Exception** | IF N_KHL **không** nằm trong N_thực (A-001 sai) THEN N_tính = N_thực — **CR**, không im lặng |
| **Source** | DEC-DIS-002 · DOC-03 BR-001 |
| **Trace** | BRQ-002 |

### PAY-BR-002 — Trần ngày công chuẩn tháng

| Mục | Nội dung |
|-----|----------|
| **Statement** | N_tính không vượt số **ngày công chuẩn** của tháng trên lịch Cty (động, D-004). |
| **Condition** | IF N_tính > ngày công chuẩn tháng |
| **Action** | THEN chặn chốt lương / báo lỗi C&B |
| **Exception** | — |
| **Source** | DOC-03 BR-001 |
| **Trace** | D-004 |

### PAY-BR-003 — Thử việc 85%

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV trong thời gian thử việc: phần lương theo thời gian (ngày công) nhân **85%** theo quy chế mInvoice. |
| **Condition** | IF trạng thái HĐ = thử việc tại kỳ |
| **Action** | THEN hệ số 0,85 trên lương thời gian |
| **Exception** | Hết TV / chính thức: 100%. Tỷ lệ khác = đổi quy chế + master, không sửa code URD. |
| **Source** | C-001 DOC-01 · BRQ-003 |
| **Trace** | PRB (cảnh báo TV — module khác) |

### PAY-BR-004 — OT từ công đã chốt

| Mục | Nội dung |
|-----|----------|
| **Statement** | OT nhân 1.5 / 2.0 / 3.0 theo **loại OT trên bảng công đã chốt** (URD). PAY không nhập OT tay ngoài TIM. |
| **Condition** | IF bảng công chưa chốt |
| **Action** | THEN không tính OT / không chốt phiếu |
| **Exception** | Hệ số OT đổi = quy chế / catalog TIM, không hardcode vĩnh viễn trên BR |
| **Source** | DOC-03 BR-003 |
| **Trace** | TIM |

### PAY-BR-005 — Phụ cấp / thưởng hai kênh

| Mục | Nội dung |
|-----|----------|
| **Statement** | Có dòng từ **HĐ (cố định)** và dòng **nhập theo tháng**. Danh mục mã/số tiền = master C&B theo quy chế (động). |
| **Condition** | IF mã không có trên master kỳ lương |
| **Action** | THEN không ghi nhận / chặn chốt |
| **Exception** | Không đóng list mã trên DOC-04 |
| **Source** | BRQ-007 · DEC-DIS-014 |
| **Trace** | D-003 |

### PAY-BR-006 — BHXH/BHYT/BHTN và TNCN tạm

| Mục | Nội dung |
|-----|----------|
| **Statement** | Tạm tính BH + TNCN theo **tỷ lệ hiệu lực tại kỳ lương** (luật / quy chế). |
| **Condition** | IF hardcode % từ URD cũ |
| **Action** | THEN **cấm** |
| **Exception** | Nộp BH nhà nước / sổ cái KT = **ngoài scope** (DOC-03 4.2) |
| **Source** | CN-001 · D-002 |
| **Trace** | BRQ-001 PAY |

### PAY-BR-007 — Cô lập phiếu lương

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV chỉ xem phiếu của mình. Line Manager **không** xem lương cấp dưới. HR/C&B xem theo IAM. PDF/email chỉ đúng người nhận. |
| **Condition** | IF actor ≠ chủ phiếu AND không phải HR được cấp quyền |
| **Action** | THEN 403 |
| **Exception** | — |
| **Source** | URD III · BRQ-009 · G-007 DOC-01 |
| **Trace** | IAM |

### PAY-BR-008 — Sai số 0 đồng

| Mục | Nội dung |
|-----|----------|
| **Statement** | UAT kỳ mẫu: tổng và từng dòng so bảng kiểm tra tay = **0 đồng** (sau làm tròn quy chế cùng kỳ). |
| **Condition** | IF lệch ≠ 0 |
| **Action** | THEN fail UAT; không go-live kỳ đó |
| **Exception** | Quy tắc làm tròn = master quy chế, ghi vào AC DOC-07 |
| **Source** | BRQ-009 · G-004 |
| **Trace** | DOC-07 (sau) |

### PAY-BR-009 — Không tự sửa công / phép

| Mục | Nội dung |
|-----|----------|
| **Statement** | PAY đọc N_thực, OT, phép không lương từ TIM chốt và ngày LEV Đã duyệt. Không mở form sửa công trong PAY. |
| **Condition** | IF C&B sửa ngày công trên màn PAY |
| **Action** | THEN **cấm** (sửa ở TIM rồi chốt lại) |
| **Exception** | Dòng PC nhập tháng (PAY-BR-005) không phải sửa công |
| **Source** | DOC-03 4.1 PAY/TIM/LEV |
| **Trace** | TIM · LEV |

### PAY-BR-010 — Quyền chạy kỳ lương

| Mục | Nội dung |
|-----|----------|
| **Statement** | Chỉ HR/C&B tạo/chốt kỳ, xuất phiếu hàng loạt. NV không chạy payroll. |
| **Condition** | IF NV/LM bấm chốt kỳ |
| **Action** | THEN 403 |
| **Exception** | IT không xem số lương trừ khi IAM HR |
| **Source** | IAM |
| **Trace** | PAY-BR-007 |

### PAY-BR-011 — N_thực gồm phép hưởng

| Mục | Nội dung |
|-----|----------|
| **Statement** | N_thực từ CC đã **bao gồm** ngày nghỉ phép hưởng lương. |
| **Condition** | IF TIM tách phép hưởng khỏi N_thực |
| **Action** | THEN phải CR công thức PAY-BR-001 (không cộng lại) |
| **Exception** | A-001 |
| **Source** | DOC-03 thuật ngữ |
| **Trace** | TIM |

### PAY-BR-012 — Mobile phiếu mình

| Mục | Nội dung |
|-----|----------|
| **Statement** | App mobile MVP hiển thị phiếu lương **của user đăng nhập**, cùng PAY-BR-007. |
| **Condition** | IF mobile |
| **Action** | THEN không nới quyền xem |
| **Exception** | — |
| **Source** | BRQ-006 |
| **Trace** | LEV-BR-012 (cùng nguyên tắc kênh) |

## 4. Bảng quyết định — N_tính

| N_thực đã gồm phép hưởng | N_KHL nằm trong N_thực | N_tính |
|--------------------------|-------------------------|--------|
| Có (A-001) | Có | N_thực − N_KHL |
| Có | Không | N_thực (cảnh báo A-001; CR) |
| Không | — | **Cấm im lặng** — CR TIM/PAY |

## 5. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | PAY-BR-001…012 | Distill DOC-03 BRQ-002/003/007/009 + DEC-DIS-014 | — |
| 0.1 | — | **Chốt cổng BR** (DEC-REQ-011) | — |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-24 | **Chốt** v0.1 (DEC-REQ-011) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-24 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
