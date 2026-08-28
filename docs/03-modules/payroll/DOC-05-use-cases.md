# DOC-05 — Kịch bản sử dụng — Payroll (PAY)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (UC payroll · DEC-REQ-012) |

**Module:** payroll · **MOD:** PAY · **Tiên quyết:** DOC-04 **Chốt** (DEC-REQ-011).  
**Phạm vi UC:** BRQ-002, 003, 007, 009 (phần lương) · PAY-BR-001…012.  
**Không:** import/sửa công (TIM); nộp/duyệt phép (LEV); N+3 (LIF); list mã PC cứng; chạy lương bởi NV/LM.  
**Cổng:** DOC-05 **đã chốt** (PGD · DEC-REQ-012). Nợ: DOC-19 khung; DOC-06/07; làm tròn → DOC-07; Ban HR ☐. **Chưa** `02-baseline/`. FR ID = sau DOC-06.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| PAY-ACT-001 | Nhân viên | Xem phiếu **của mình** (web + mobile); không chạy kỳ | Primary |
| PAY-ACT-002 | Line Manager | Không xem lương cấp dưới — **không UC riêng** (PAY-BR-007 / UC-004 EF) | Secondary |
| PAY-ACT-003 | HR / C&B | Tính/chốt kỳ; nhập PC tháng; xuất phiếu hàng loạt | Primary |
| PAY-ACT-004 | IT Admin | Không xem số lương trừ IAM HR — **không UC riêng** | Secondary |
| PAY-ACT-005 | Hệ thống | Tính N_tính / OT / BH-TNCN; phát phiếu / email | System |
| PAY-ACT-006 | TIM + LEV | Cung cấp công **đã chốt** + ngày phép **Đã duyệt** | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| PAY-UC-001 | Tính kỳ lương | PAY-ACT-003 | Must | PAY-BR-001, 003, 004, 005, 006, 011 · BRQ-002, 003, 007 |
| PAY-UC-002 | Chốt kỳ lương | PAY-ACT-003 | Must | PAY-BR-002, 008, 009, 010 |
| PAY-UC-003 | Nhập PC/thưởng theo tháng | PAY-ACT-003 | Must | PAY-BR-005 |
| PAY-UC-004 | Xem phiếu lương của mình | PAY-ACT-001 | Must | PAY-BR-007, 012 · BRQ-006, 009 |
| PAY-UC-005 | Xuất phiếu hàng loạt (PDF/email) | PAY-ACT-003 | Must | PAY-BR-007, 010 |

## 3. Sơ đồ use case

```text
[HR/C&B] ──► (PAY-UC-003 PC tháng)
[HR/C&B] ──► (PAY-UC-001 Tính kỳ) ──include──► TIM chốt + LEV Đã duyệt + HĐ + master
                    │
                    ▼
[HR/C&B] ──► (PAY-UC-002 Chốt kỳ) ──► phiếu sẵn
[HR/C&B] ──► (PAY-UC-005 Xuất hàng loạt)
[NV]     ──► (PAY-UC-004 Xem phiếu mình)  web | mobile
[LM]     ──x  xem lương cấp dưới (403)
```

## 4. Đặc tả use case (Fully Dressed)

### PAY-UC-001 — Tính kỳ lương

| Mục | Nội dung |
|-----|----------|
| **ID** | PAY-UC-001 |
| **Actor chính** | PAY-ACT-003 |
| **Actor phụ** | PAY-ACT-005, PAY-ACT-006 |
| **Mục tiêu** | Sinh bảng lương kỳ từ công đã chốt + HĐ + master quy chế (chưa phát hành phiếu) |
| **Preconditions** | Actor = HR/C&B; tháng TIM **đã chốt**; master tỷ lệ BH/TNCN + catalog PC **hiệu lực kỳ** |
| **Postconditions (success)** | Bản tính kỳ (draft); chưa mở phiếu NV |
| **Postconditions (failure)** | Không lưu bản tính; không đổi phiếu đã phát hành kỳ khác |
| **Trigger** | HR mở “Tính lương” cho kỳ |
| **Frequency** | 1 lần / kỳ (tính lại được trước khi chốt) |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Chọn kỳ / công ty |
| 2 | Hệ thống | Đọc N_thực, OT (loại 1.5/2.0/3.0), N_KHL từ TIM chốt + ngày LEV Đã duyệt |
| 3 | Hệ thống | Đọc HĐ (lương, PC cố định), master PC, tỷ lệ BH/TNCN **tại kỳ** |
| 4 | Hệ thống | N_tính = N_thực − N_KHL; **không** cộng phép hưởng (đã trong N_thực) |
| 5 | Hệ thống | Áp trần ngày công chuẩn tháng; hệ số 85% nếu HĐ thử việc; OT; PC hai kênh; BH/TNCN tạm |
| 6 | HR | Xem bảng preview (từng NV / từng dòng) |

#### Luồng thay thế

| ID | Điều kiện | Steps |
|----|-----------|-------|
| AF-1 | Tính lại cùng kỳ chưa chốt | Ghi đè bản draft; không đụng phiếu kỳ đã chốt |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | TIM tháng chưa chốt | Chặn step 2 | Không tính |
| EF-2 | NV hoặc LM gọi tính kỳ | 403 | PAY-BR-010 |
| EF-3 | N_tính > ngày công chuẩn tháng | Cảnh báo / chặn theo PAY-BR-002 (chặn **chốt** ở UC-002) | Preview vẫn xem được lỗi |
| EF-4 | TIM tách phép hưởng khỏi N_thực (A-001 sai) | Không im lặng cộng lại | Cảnh báo CR; không tự sửa công |
| EF-5 | HR sửa ngày công / OT / phép trên màn PAY | Cấm | Sửa ở TIM rồi chốt lại |
| EF-6 | Hardcode % BH từ URD | Cấm | Dùng tỷ lệ kỳ |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PAY-BR-001, 011 | 4 |
| PAY-BR-003, 004, 005, 006 | 5 |
| PAY-BR-009 | EF-5 |
| PAY-BR-010 | EF-2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 payroll chưa có |

---

### PAY-UC-002 — Chốt kỳ lương

| Mục | Nội dung |
|-----|----------|
| **ID** | PAY-UC-002 |
| **Actor chính** | PAY-ACT-003 |
| **Mục tiêu** | Khóa kỳ; phát hành phiếu theo IAM |
| **Preconditions** | Có bản tính UC-001 cho kỳ; TIM vẫn chốt |
| **Postconditions (success)** | Kỳ Chốt; NV xem được phiếu mình; không tính đè im lặng |
| **Postconditions (failure)** | Kỳ vẫn draft |
| **Trigger** | HR bấm Chốt kỳ |
| **Frequency** | 1 lần / kỳ (mở lại = CR / quy trình hủy chốt — **ngoài MVP** trừ khi DOC-06 nói khác) |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Chọn kỳ draft |
| 2 | Hệ thống | Kiểm tra N_tính ≤ ngày công chuẩn; không còn lỗi A-001 im lặng |
| 3 | HR | Xác nhận chốt |
| 4 | Hệ thống | Đánh dấu Chốt; mở PAY-UC-004 / UC-005 |

#### Luồng thay thế

| ID | Điều kiện | Steps |
|----|-----------|-------|
| AF-1 | UAT kỳ mẫu | So từng dòng vs bảng tay = 0 đồng sau làm tròn quy chế (chi tiết DOC-07) |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | N_tính > chuẩn tháng | Chặn step 3 | PAY-BR-002 |
| EF-2 | NV/LM chốt | 403 | |
| EF-3 | TIM bỏ chốt sau khi tính | Chặn | Tính lại UC-001 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PAY-BR-002 | 2, EF-1 |
| PAY-BR-008 | AF-1 |
| PAY-BR-010 | EF-2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### PAY-UC-003 — Nhập PC/thưởng theo tháng

| Mục | Nội dung |
|-----|----------|
| **ID** | PAY-UC-003 |
| **Actor chính** | PAY-ACT-003 |
| **Mục tiêu** | Ghi dòng PC/thưởng **nhập tháng** (kênh 2); kênh HĐ cố định không nhập tại đây |
| **Preconditions** | Role HR/C&B; mã nằm trên master kỳ |
| **Postconditions (success)** | Dòng tháng gắn NV + kỳ; UC-001 đọc được |
| **Trigger** | HR mở nhập PC tháng |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Chọn kỳ, NV, mã master, số tiền / đơn vị |
| 2 | Hệ thống | Kiểm tra mã ∈ master hiệu lực kỳ |
| 3 | Hệ thống | Lưu dòng tháng (không phải sửa công) |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Mã không có trên master kỳ | Chặn | PAY-BR-005 |
| EF-2 | NV/LM nhập | 403 | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PAY-BR-005 | 1–3 |
| PAY-BR-009 | Dòng PC ≠ sửa công |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### PAY-UC-004 — Xem phiếu lương của mình

| Mục | Nội dung |
|-----|----------|
| **ID** | PAY-UC-004 |
| **Actor chính** | PAY-ACT-001 |
| **Actor phụ** | PAY-ACT-003 (HR xem theo IAM) |
| **Mục tiêu** | NV xem phiếu kỳ đã chốt **của mình** |
| **Preconditions** | Kỳ đã chốt; IAM đăng nhập |
| **Postconditions (success)** | Hiển thị phiếu chủ sở hữu |
| **Postconditions (failure)** | 403; không lộ dòng lương người khác |
| **Trigger** | Menu phiếu lương (web hoặc mobile) |
| **Frequency** | Theo nhu cầu |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | NV | Mở phiếu kỳ đã chốt |
| 2 | Hệ thống | Chỉ trả phiếu `user = chủ phiếu` |

#### Luồng thay thế

| ID | Điều kiện | Steps |
|----|-----------|-------|
| AF-1 | Kênh mobile | Cùng IAM / cùng dữ liệu web (PAY-BR-012) |
| AF-2 | HR/C&B được cấp quyền | Xem theo IAM (không mở cho LM) |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | NV xem phiếu người khác | 403 | PAY-BR-007 |
| EF-2 | LM xem lương cấp dưới | 403 | Policy URD = **không** |
| EF-3 | Kỳ chưa chốt | Không hiện phiếu NV | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PAY-BR-007 | 2, EF-1, EF-2 |
| PAY-BR-012 | AF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### PAY-UC-005 — Xuất phiếu hàng loạt (PDF/email)

| Mục | Nội dung |
|-----|----------|
| **ID** | PAY-UC-005 |
| **Actor chính** | PAY-ACT-003 |
| **Mục tiêu** | Phát PDF/email **đúng người nhận** sau khi kỳ chốt |
| **Preconditions** | Kỳ Chốt; actor HR/C&B |
| **Postconditions (success)** | File/email gắn đúng NV |
| **Trigger** | HR xuất hàng loạt |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Chọn kỳ đã chốt + kênh PDF và/hoặc email |
| 2 | Hệ thống | Mỗi phiếu chỉ gắn đúng chủ / địa chỉ đã khai |
| 3 | Hệ thống | Ghi nhận đã phát hành |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | NV tự xuất hàng loạt | 403 | PAY-BR-010 |
| EF-2 | Gửi nhầm người / CC LM | Cấm | PAY-BR-007 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PAY-BR-007 | 2, EF-2 |
| PAY-BR-010 | 1, EF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

## 5. Tóm tắt

| UC ID | Actor | Mô tả 1 câu |
|-------|-------|-------------|
| PAY-UC-001 | HR | Tính kỳ từ công chốt + HĐ + master; không sửa công. |
| PAY-UC-002 | HR | Chốt kỳ khi N_tính hợp lệ; mở phiếu. |
| PAY-UC-003 | HR | Nhập PC/thưởng tháng theo master. |
| PAY-UC-004 | NV | Xem phiếu mình; LM 403. |
| PAY-UC-005 | HR | Xuất PDF/email đúng người. |

UC-001…005 đủ dressed. PAY-BR-008 (UAT 0 đồng) = AF-1 của UC-002 → DOC-07.

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-012) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
