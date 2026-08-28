# DOC-04 — Quy tắc nghiệp vụ — Timekeeping (TIM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (cổng BR · timekeeping · DEC-REQ-017) |

**Module:** timekeeping · **MOD:** TIM · **Phạm vi:** BRQ-004, BO-003, BR-003 (OT từ công chốt), CN-003, D-001, A-001.  
**Không:** máy CC phần cứng; sửa công trên PAY; quỹ phép / C1–C2 (LEV); tính lương (PAY chỉ **đọc** tháng đã chốt).

**Cổng:** DEC-REQ-017 **đã chốt** 2026-08-25. Nợ: cột Excel = master HR; Ban HR ☐. **Chưa** `02-baseline/`. Mở: DOC-05 + khung DOC-19. **Không** tự viết.

---

## 1. Mục đích & phạm vi

Import **một** mẫu Excel công toàn Cty tại một thời điểm (nội dung **động theo quy chế**), preview lỗi, chốt tháng, ghi OT, đưa ngày phép **Đã duyệt** vào bảng công sao cho N_thực **đã gồm** phép hưởng lương.

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| TIM-BR-001 | Một mẫu tại một thời điểm | Toàn Cty dùng **đúng một** version template đang công bố | Validation | Must | BRQ-004 · CN-003 | SH-002 |
| TIM-BR-002 | Master mẫu động | Cột/file = khai báo HR theo quy chế; không hardcode list cột URD | Inference | Must | D-001 · DEC-DIS-014 | SH-002 |
| TIM-BR-003 | Khớp version | File import phải đúng version mẫu đang hiệu lực | Validation | Must | BRQ-004 | SH-006 |
| TIM-BR-004 | Preview lỗi | Hiện lỗi từng dòng; **cấm** commit im lặng khi còn lỗi Must | Validation | Must | BO-003 · DOC-03 4.1 TIM | SH-002 |
| TIM-BR-005 | Chốt tháng | Chỉ tháng **đã chốt** mới là đầu vào PAY | Validation | Must | PAY-BR-009 | SH-002 |
| TIM-BR-006 | OT trên bảng công | Loại 1.5 / 2.0 / 3.0 nằm trên file/bảng đã import; TIM không để PAY nhập OT | Calculation | Must | BR-003 | SH-002 |
| TIM-BR-007 | Phép Đã duyệt | Ngày LEV **Đã duyệt** vào bảng công kỳ; đơn chờ không tính | Inference | Must | DOC-03 4.1 · LEV | SH-002 |
| TIM-BR-008 | N_thực gồm phép hưởng | Ngày phép hưởng lương **nằm trong** N_thực (không tách để PAY cộng lại) | Inference | Must | A-001 · PAY-BR-011 | SH-002 |
| TIM-BR-009 | Không phần cứng | Chỉ import file; không tích hợp máy CC | Constraint | Must | DOC-03 4.2 | SH-006 |
| TIM-BR-010 | Ai import / chốt | Chỉ HR/C&B (IT hỗ trợ mẫu, không chốt công trừ IAM HR) | Authorization | Must | IAM | SH-002 |
| TIM-BR-011 | Bỏ chốt / import lại | HR bỏ chốt TIM rồi import lại; PAY không sửa công | Authorization | Must | PAY-FR-008 | SH-002 |
| TIM-BR-012 | NV/LM không import | NV/LM không upload Excel công Cty | Authorization | Must | TIM-BR-010 | SH-002 |

**Loại:** Validation · Calculation · Authorization · Inference · Constraint.

## 3. Chi tiết quy tắc

### TIM-BR-001 — Một mẫu toàn Cty

| Mục | Nội dung |
|-----|----------|
| **Statement** | Tại một thời điểm chỉ **một** template Excel công được dùng cho import toàn Cty. |
| **Condition** | IF HR công bố version mới |
| **Action** | THEN version cũ **không** nhận import (trừ file đang preview chưa commit) |
| **Exception** | Hai mẫu song song = **cấm** (CN-003) |
| **Source** | BRQ-004 · CN-003 |
| **Trace** | D-001 |

### TIM-BR-002 — Cột động theo quy chế

| Mục | Nội dung |
|-----|----------|
| **Statement** | Tên cột, bắt buộc/optional, mapping N_thực/OT = master HR theo quy chế. DOC-04 **không** đóng danh sách cột. |
| **Condition** | IF Dev hardcode cột theo URD cũ |
| **Action** | THEN **cấm** |
| **Exception** | Đổi cột = đổi master + version mẫu, không sửa BR URD |
| **Source** | DEC-DIS-014 · CN-006 |
| **Trace** | BRQ-004 |

### TIM-BR-003 — File đúng version

| Mục | Nội dung |
|-----|----------|
| **Statement** | File upload phải khớp version mẫu đang hiệu lực (checksum / mã version trên file — cách kỹ thuật TBD DOC-06). |
| **Condition** | IF sai version |
| **Action** | THEN từ chối import; không ghi bảng công |
| **Source** | BRQ-004 |
| **Trace** | SH-006 |

### TIM-BR-004 — Preview trước khi ghi

| Mục | Nội dung |
|-----|----------|
| **Statement** | HR xem preview: dòng OK / lỗi (thiếu NV, sai OT, lệch cột). Commit chỉ khi hết lỗi Must. |
| **Condition** | IF còn lỗi Must AND HR bấm ghi |
| **Action** | THEN chặn |
| **Exception** | Cảnh báo Should không chặn commit — **không** dùng MVP trừ khi DOC-06 nói khác; MVP = lỗi Must chặn hết |
| **Source** | DOC-03 4.1 TIM · BO-003 |
| **Trace** | UAT import |

### TIM-BR-005 — Chốt tháng

| Mục | Nội dung |
|-----|----------|
| **Statement** | HR chốt tháng sau import hợp lệ. PAY chỉ đọc tháng chốt. |
| **Condition** | IF PAY tính kỳ khi TIM chưa chốt |
| **Action** | THEN PAY chặn (đã có PAY-FR-001) |
| **Exception** | — |
| **Source** | PAY-BR-009 |
| **Trace** | PAY-UC-001 |

### TIM-BR-006 — OT 1.5 / 2.0 / 3.0

| Mục | Nội dung |
|-----|----------|
| **Statement** | Giờ OT và **loại hệ số** đi trên bảng công đã import/chốt. Đổi hệ số = quy chế / catalog TIM, không hardcode vĩnh viễn. |
| **Condition** | IF thiếu loại OT trên dòng có giờ OT |
| **Action** | THEN lỗi preview (Must) |
| **Exception** | Hệ số khác 1.5/2.0/3.0 = đổi master, không bịa trên BR |
| **Source** | DOC-03 BR-003 |
| **Trace** | PAY-BR-004 |

### TIM-BR-007 — Phép Đã duyệt vào công

| Mục | Nội dung |
|-----|----------|
| **Statement** | Ngày LEV trạng thái **Đã duyệt** trong tháng được phản ánh trên bảng công. Đơn Chờ C1/C2 / Từ chối / Đã hủy **không** vào công. |
| **Condition** | IF chỉ có đơn chờ |
| **Action** | THEN không tính ngày phép vào N_thực / N_KHL |
| **Exception** | — |
| **Source** | DOC-03 4.1 TIM · LEV |
| **Trace** | PAY-BR-009 |

### TIM-BR-008 — N_thực gồm phép hưởng

| Mục | Nội dung |
|-----|----------|
| **Statement** | N_thực xuất cho PAY **đã gồm** ngày nghỉ phép hưởng lương. Không xuất N_thực “sạch phép” để PAY cộng lại. |
| **Condition** | IF tách phép hưởng khỏi N_thực |
| **Action** | THEN vi phạm A-001; PAY cảnh báo CR (PAY-FR-013) |
| **Exception** | — |
| **Source** | DOC-03 A-001 |
| **Trace** | PAY-BR-011 |

### TIM-BR-009 — Không máy CC

| Mục | Nội dung |
|-----|----------|
| **Statement** | Hệ thống nhận **file** (Excel đúng mẫu). Không API máy chấm công MVP. |
| **Condition** | IF yêu cầu kết nối firmware máy |
| **Action** | THEN ngoài scope |
| **Source** | DOC-03 4.2 |
| **Trace** | — |

### TIM-BR-010 — Quyền import / chốt

| Mục | Nội dung |
|-----|----------|
| **Statement** | Chỉ HR/C&B import, preview, chốt tháng. IT cấu hình mẫu, không xem/sửa công trừ IAM HR. |
| **Condition** | IF NV hoặc LM upload/chốt |
| **Action** | THEN 403 |
| **Source** | IAM |
| **Trace** | TIM-BR-012 |

### TIM-BR-011 — Bỏ chốt và import lại

| Mục | Nội dung |
|-----|----------|
| **Statement** | Sửa công sau khi chốt TIM: HR **bỏ chốt** tháng rồi import/preview lại. Không form sửa ô công trên PAY. |
| **Condition** | IF tháng đang chốt AND HR cần sửa |
| **Action** | THEN bỏ chốt → import → chốt lại; PAY phải tính lại Draft |
| **Exception** | Kỳ lương PAY **đã chốt** = ngoài MVP TIM (không tự mở khóa lương) |
| **Source** | PAY-FR-016 · PAY-BR-009 |
| **Trace** | PAY |

### TIM-BR-012 — NV/LM không import

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV/LM không được import Excel công toàn Cty. |
| **Condition** | IF actor ≠ HR/C&B |
| **Action** | THEN 403 trên chức năng import/chốt |
| **Exception** | Xem công mình = **không** Must trên BRD kênh NV (BRQ-006 không liệt kê); không mở UC xem công trừ DOC-05 sau |
| **Source** | BRQ-006 (không gồm công) |
| **Trace** | DOC-05 (sau) |

## 4. Bảng quyết định — Import

| File đúng version | Preview hết lỗi Must | Commit | Chốt tháng |
|-------------------|----------------------|--------|------------|
| Không | — | Cấm | Cấm |
| Có | Không | Cấm | Cấm |
| Có | Có | Được | HR chốt sau |

## 5. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | TIM-BR-001…012 | Distill DOC-03 BRQ-004 + TIM 4.1 + A-001 + BR-003 | — |
| 0.1 | — | **Chốt cổng BR** (DEC-REQ-017) | — |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-017) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
