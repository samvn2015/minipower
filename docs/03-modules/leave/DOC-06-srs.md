# DOC-06 — Đặc tả Yêu cầu Phần mềm — Leave (LEV)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-24 | Trịnh Yên (BA) | **Chốt** (SRS leave · DEC-REQ-007) |

**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 Chốt · DOC-19 **Chốt khung** (DEC-REQ-006) · DOC-03 **Chốt**.  
**Cổng:** SRS leave **đã chốt** (PGD). Nợ: OQ-REQ-010 (chỉ NV hủy); DOC-13; HTML MCP. AC: **DOC-07 Chốt** (DEC-REQ-008). **Chưa** `02-baseline/`.

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **leave** cho BA/Dev/QC. Không gồm TIM import, PAY tính lương (chỉ nhận ngày phép đã duyệt).

### 1.2 Phạm vi

HRM mInvoice — phân hệ phép (URD-02 + DEC-DIS-003/004). Web + mobile MVP. Thông báo Email/App/HRM — không CRM bán hàng.

### 1.3 Định nghĩa, viết tắt

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| C1 / C2 | Duyệt LM / duyệt HR chính thức |
| 3 NLĐ | Ba ngày làm việc trên lịch công ty |
| Ngày công chuẩn liền | ≥ 3 ngày công liên tiếp kích hoạt hạn nộp (LEV-BR-007) |
| Nghỉ đột xuất | Cờ đơn; chỉ HR đóng ngoại lệ + C2 |

### 1.4 Tài liệu tham chiếu

| ID | Tài liệu |
|----|----------|
| DOC-03 | BRD HRM **Chốt** (v0.7) |
| DOC-04 | LEV-BR-001…016 Chốt |
| DOC-05 | LEV-UC-001…006 |
| DOC-19 | LEV-SCR-001…008 Chốt khung |

### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI (DOC-19) · §5 NFR tóm tắt → DOC-13 · §6 trace (UC/BR + **BRQ BRD**).

## 2. Mô tả tổng quan

### 2.1 Bối cảnh sản phẩm

Module trong HRM. Phụ thuộc: IAM (role), lịch ngày công (HR), catalog mẫu file ốm (HR/IT). Đầu ra TIM: ngày phép **Đã duyệt**.

### 2.2 Chức năng sản phẩm

Nộp đơn · C1 · C2 + trừ quỹ năm · hủy trước C2 · xem quỹ · cấu hình trần loại (tuỳ chọn).

### 2.3 Phân loại người dùng

| User class | Actor | |
|------------|-------|-|
| NV | LEV-ACT-001 | Form + quỹ + hủy chờ |
| LM | LEV-ACT-002 | C1 cấp dưới |
| HR/C&B | LEV-ACT-003 | C2, catalog |
| IT | LEV-ACT-004 | Luồng; không C2 |

### 2.4 Môi trường vận hành

| Mục | Yêu cầu |
|-----|---------|
| Client | Browser + app mobile MVP; cùng rule |
| Server | TBD architecture |
| Network | Nội bộ mInvoice |

### 2.5 Ràng buộc thiết kế & triển khai

| ID | Constraint |
|----|------------|
| LEV-CN-001 | Không hardcode số ngày luật; trần catalog có thể trống |
| LEV-CN-002 | Không gọi CRM bán hàng |
| LEV-CN-003 | HTML wireframe không bắt buộc triển khai pixel-perfect so với MCP (chưa có) |

### 2.6 Giả định & phụ thuộc

| ID | Mô tả |
|----|-------|
| LEV-A-001 | OQ-REQ-010: chỉ NV hủy đơn mình |
| LEV-A-002 | MIME/mẫu file ốm: HR cung cấp mẫu; kiểm tra “đúng mẫu” theo quy định IT TBD |
| LEV-A-003 | Lịch ngày công D-004 có trước go-live |

## 3. Yêu cầu chức năng

| FR ID | Mô tả (shall) | Priority | Source | Verify |
|-------|---------------|----------|--------|--------|
| LEV-FR-001 | Hệ thống shall cho NV tạo đơn trên LEV-SCR-002 với 6 loại phép, Từ–Đến, nhãn Cả ngày/Sáng/Chiều, lý do, 1 người bàn giao active ≠ mình | Must | UC-001 · BR-001,002,004 | Test |
| LEV-FR-002 | Hệ thống shall áp **cùng** validation LEV-FR-* trên LEV-SCR-003 (mobile) | Must | UC-001 AF-3 · BR-012 | Test |
| LEV-FR-003 | Hệ thống shall chặn submit khi overlap ngày/buổi với đơn Chờ C1, Chờ C2, Đã duyệt | Must | UC-001 EF-1 · BR-013 | Test |
| LEV-FR-004 | Hệ thống shall chặn phép năm nếu ngày đơn > quỹ còn (submit **và** C2) | Must | UC-001 EF-3 · BR-005 | Test |
| LEV-FR-005 | Hệ thống shall **không** trừ/chặn quỹ năm cho ốm, KHL, kết hôn, tang, chế độ | Must | BR-005 | Test |
| LEV-FR-006 | Hệ thống shall kích hoạt hạn 3 NLĐ chỉ khi đơn có ≥ 3 **ngày công chuẩn liền**; trễ và không cờ đột xuất → chặn + gợi ý cờ | Must | UC-001 EF-2 · BR-007 | Test |
| LEV-FR-007 | Hệ thống shall cho submit đơn có cờ Nghỉ đột xuất khi trễ 3 NLĐ; C1 không trừ quỹ | Must | UC-001 AF-2 · BR-008 | Test |
| LEV-FR-008 | Hệ thống shall bắt buộc file **đúng mẫu Cty** cho ốm/BHXH trước submit và trước C2 | Must | UC-001 AF-1 · BR-003 | Test |
| LEV-FR-009 | Hệ thống shall gửi thông báo Email và/hoặc App/HRM khi nộp, C1, C2, từ chối, hủy; **không** CRM bán hàng | Must | UC-001 · BR-011 | Test |
| LEV-FR-010 | Hệ thống shall chỉ cho LM của NV Phê duyệt/Từ chối C1 (lý do bắt buộc khi từ chối); không Matrix; không tự C1 đơn mình | Must | UC-002 · BR-009,014 | Test |
| LEV-FR-011 | Hệ thống shall **không** cho C2 nếu C1 chưa duyệt (kể cả đột xuất) | Must | UC-003 EF-1 · BR-010 | Test |
| LEV-FR-012 | Khi HR duyệt C2, hệ thống shall atomic: Đã duyệt + trừ quỹ năm nếu loại phép năm; đột xuất: ngoại lệ + C2 cùng thao tác | Must | UC-003 · BR-006,008 | Test |
| LEV-FR-013 | Hệ thống shall cho NV hủy đơn mình khi Chờ C1 hoặc Chờ C2; Đã hủy không chiếm ngày; được nộp lại cùng ngày | Must | UC-004 · BR-016 | Test |
| LEV-FR-014 | Hệ thống shall từ chối hủy/thu hồi/hoàn quỹ sau C2 (MVP) | Must | UC-004 EF-1 · BR-015 | Test |
| LEV-FR-015 | Hệ thống shall cho NV xem **quỹ phép năm của mình** trên LEV-SCR-007; không xem quỹ người khác | Must | UC-005 | Test |
| LEV-FR-016 | Hệ thống shall cho HR cấu hình trần ngày theo loại; trống = không trần; chỉ chặn khi đã có số | Should | UC-006 · BR-001 | Test |
| LEV-FR-017 | Hệ thống shall ẩn C2 với Manager; IT không C2 trừ khi gán role HR | Must | UC-003 EF-2 · BR-014 | Test |
| LEV-FR-018 | Hệ thống shall không trừ quỹ ở C1 | Must | UC-002 · BR-006 | Test |

### LEV-FR-006 — Hạn 3 NLĐ (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Đếm ngày công chuẩn giao khoảng nghỉ; chuỗi ≥ 3 ngày công liền (lễ/CN không phải ngày công không đếm, không phá chuỗi). Sáng/Chiều vẫn = 1 ngày công. |
| **Inputs** | Từ–Đến, nhãn ngày, lịch công ty, thời điểm submit |
| **Processing** | IF chuỗi ≥ 3 AND NLĐ tới ngày bắt đầu &lt; 3 AND không cờ đột xuất THEN reject |
| **Outputs** | Lỗi + CTA đánh dấu đột xuất |
| **Error handling** | EF-2 UC-001 |

### LEV-FR-012 — C2 atomic

| Mục | Nội dung |
|-----|----------|
| **Description** | Duyệt chính thức và trừ quỹ năm (nếu áp dụng) không tách transaction |
| **Preconditions** | C1 duyệt; file ốm còn đúng mẫu; quỹ đủ nếu phép năm |
| **Postconditions** | Đã duyệt; quỹ đã trừ hoặc không đổi (loại không trừ năm) |
| **Error handling** | EF-3, EF-4 UC-003 |

## 4. Yêu cầu giao diện bên ngoài

### 4.1 UI

Bắt buộc đủ field/luồng DOC-19 LEV-SCR-001…008. Pixel HTML MCP **không** Must.

### 4.2 Phần cứng

Không.

### 4.3 Phần mềm

IAM roles · lịch công ty · (sau) TIM đọc đơn Đã duyệt. API → DOC-12.

### 4.4 Truyền thông

Thông báo in-app/email; không webhook CRM bán hàng.

## 5. NFR tóm tắt

→ **DOC-13** (chưa slice). MVP leave: cùng IAM; phiếu lương **không** thuộc module này.

| NFR ID | Category | Tóm tắt |
|--------|----------|---------|
| *(DOC-13)* | Security | NV không xem đơn/quỹ người khác (trừ LM cấp dưới / HR) |
| *(DOC-13)* | Audit | Log C1/C2/hủy |

## 6. Ma trận truy vết (tóm tắt)

| FR ID | UC | BR | AC | Test |
|-------|----|----|----|------|
| LEV-FR-001 | 001 | 001,002,004 | *(DOC-07)* | |
| LEV-FR-003 | 001 | 013 | | |
| LEV-FR-006 | 001 | 007 | | |
| LEV-FR-007 | 001 | 008 | | |
| LEV-FR-008 | 001 | 003 | | |
| LEV-FR-010 | 002 | 009 | | |
| LEV-FR-012 | 003 | 006 | | |
| LEV-FR-013 | 004 | 016 | | |
| LEV-FR-014 | 004 | 015 | | |
| LEV-FR-015 | 005 | — | | |
| LEV-FR-016 | 006 | 001 | | |

### 6.1 BRQ (DOC-03) → FR leave

SRS **chỉ** module phép. BRQ lương/công/LIF **không** có FR ở đây — đúng chỗ DOC-06 module khác.

| BRQ | LEV-FR | Kết luận |
|-----|--------|----------|
| BRQ-001 | 001…018 (phần phép + role C1/C2) | Một phần. 7 phân hệ kia + IAM đầy đủ → SRS module khác. |
| BRQ-002 | — | Không (payroll) |
| BRQ-003 | — | Không (payroll) |
| BRQ-004 | — | Không (timekeeping) |
| BRQ-005 | — | Không (lifecycle) |
| BRQ-006 | 002; 001, 015 | Có kênh phép (mobile + đơn/quỹ). Hồ sơ/phiếu lương → module khác. |
| BRQ-007 | — | Không (payroll) |
| BRQ-008 | 006, 007 | Có |
| BRQ-009 | — | Không (UAT lương/công/cảnh báo/phiếu) |
| BRQ-010 | 007, 010, 011, 012, 017, 018 | Có |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-24 | **Chốt** v0.2 (DEC-REQ-007) · ☐ repo `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-24 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ — không chặn cổng A |
