# DOC-04 — Quy tắc nghiệp vụ — Probation (PRB)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (cổng BR · PRB · DEC-REQ-051) |

**Module:** probation · **MOD:** PRB · **Phạm vi:** URD-05, BO-005 (phần TV), DOC-03 §4.1 PRB (T-15 / T-7; Đạt / gia hạn / không đạt).  
**Không:** 85% lương (PAY-BR); ATS; form đánh giá cứng (master); SN/lễ (EVT); list field phiếu đánh giá.

**Cổng BR đóng.** Sửa BR = CR. Nợ kèm chốt: T-15/T-7 = **ngày lịch** (chưa vs ngày công); ai đề xuất vs HR chốt = DOC-05/IAM; Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự DOC-05.

---

## 1. Mục đích & phạm vi

Theo dõi thử việc: cảnh báo **T-15** và task **T-7**; chốt **Đạt / Gia hạn / Không đạt**; 0 sót 0 trễ cảnh báo TV (BO-005 phần này).

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| PRB-BR-001 | Mốc TV = HĐ EMP | Ngày BĐ/KT thử việc lấy từ hồ sơ HĐ; PRB không tự bịa mốc | Inference | Must | EMP-BR-006 | SH-002 |
| PRB-BR-002 | Cảnh báo T-15 | Cảnh báo **15 ngày** trước ngày KT TV (nháp: **ngày lịch**) | Inference | Must | BO-005 · URD-05 | SH-002 |
| PRB-BR-003 | Task T-7 | Task/nhắc đánh giá **7 ngày** trước KT TV (nháp: ngày lịch) | Inference | Must | URD-05 | SH-002 |
| PRB-BR-004 | 3 kết quả | Kết quả Must ∈ {Đạt, Gia hạn, Không đạt}; không trạng thái lạ trên BR | Validation | Must | DOC-03 4.1 | SH-002 |
| PRB-BR-005 | Đạt → chính thức | Đạt: EMP chuyển HĐ chính thức (fact PAY hết 85%) — PRB không tính lương | Inference | Must | PAY-BR-003 | SH-002 |
| PRB-BR-006 | Gia hạn | Gia hạn: kéo KT TV theo **master quy chế**; không hardcode số tháng trên BR | Inference | Must | DEC-DIS-014 | SH-002 |
| PRB-BR-007 | Không đạt | Không đạt: không im lặng; kích hồ sơ off / LIF theo quy chế (chi tiết DOC-05) | Inference | Must | LIF | SH-002 |
| PRB-BR-008 | 0 sót 0 trễ | Job cảnh báo T-15/T-7 không bỏ NV TV đang hiệu lực | Validation | Must | BO-005 | SH-002 |
| PRB-BR-009 | HR SoT kết quả | Chỉ HR (IAM) **chốt** Đạt/Gia hạn/Không đạt. LM đề xuất = catalog/DOC-05 | Authorization | Must | IAM | SH-002 |
| PRB-BR-010 | Không CRM sales | Cảnh báo TV không bắn CRM bán hàng | Constraint | Must | DEC-DIS-001 | SH-002 |
| PRB-BR-011 | Kênh nhắc | Nhắc T-15/T-7 qua HRM + email/app (DOC-03 4.3) — không bịa vendor | Inference | Must | 4.3 | SH-002 |
| PRB-BR-012 | Phiếu đánh giá động | Nội dung tiêu chí đánh giá = master; không đóng list trên BR | Inference | Must | DEC-DIS-014 | SH-002 |

## 3. Chi tiết quy tắc

### PRB-BR-001 — Mốc

| Mục | Nội dung |
|-----|----------|
| **Statement** | Nguồn ngày TV = HĐ EMP. Thiếu mốc → cảnh báo, không bịa. |
| **Source** | EMP |
| **Trace** | PAY 85% đọc HĐ không đọc PRB |

### PRB-BR-002 — T-15

| Mục | Nội dung |
|-----|----------|
| **Statement** | Cảnh báo khi ngày hệ thống = KT_TV − 15 **ngày lịch** (nháp). |
| **Condition** | IF bỏ NV TV |
| **Action** | THEN fail BO-005 |
| **Exception** | Ngày công vs lịch = **TBD** (giống nợ N+3); nháp lịch |
| **Source** | Brainstorm URD: 15 ngày |

### PRB-BR-003 — T-7

| Mục | Nội dung |
|-----|----------|
| **Statement** | Task đánh giá tại KT_TV − 7 ngày lịch (nháp). |
| **Source** | URD task 7 ngày |
| **Trace** | DOC-05: giao ai (LM/HR) |

### PRB-BR-004 — 3 giá trị

| Mục | Nội dung |
|-----|----------|
| **Statement** | Chốt kết quả chỉ 3 mã. |
| **Condition** | IF lưu “đạt có điều kiện” không master |
| **Action** | THEN chặn trừ CR |

### PRB-BR-005 / 006 / 007

Đạt → EMP chính thức. Gia hạn → master. Không đạt → luồng off (LIF), không xóa im lặng hồ sơ.

### PRB-BR-008 — Coverage

Mọi NV đang TV (HĐ TV hiệu lực) phải vào hàng T-15/T-7 đúng hạn.

### PRB-BR-009 — Ai chốt

HR chốt. LM **không** tự Đạt một mình trừ IAM catalog (DOC-05).

### PRB-BR-010…012

Không CRM sales. Kênh in-app/email. Tiêu chí phiếu = master.

## 4. Bảng quyết định — Kết quả

| Kết quả | EMP HĐ | PAY 85% | LIF |
|---------|--------|---------|-----|
| Đạt | Chính thức | Hết hệ số TV (kỳ sau theo HĐ) | Không bắt buộc off |
| Gia hạn | TV kéo KT | Vẫn TV | Không |
| Không đạt | Theo quy chế off | PAY theo HĐ/kỳ | Mở off |

## 5. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | PRB-BR-001…012 | Distill URD-05 + BO-005 TV; T-15/T-7 ngày lịch kèm chốt | — |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | ☑ Chốt v0.1 — DEC-REQ-051 |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn Draft |
| Business Owner | Ban HR | | ☐ Nợ |
