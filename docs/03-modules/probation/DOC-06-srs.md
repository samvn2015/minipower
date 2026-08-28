# DOC-06 — Đặc tả Yêu cầu Phần mềm — Probation (PRB)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (SRS PRB · DEC-REQ-057) |

**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 **Chốt** (DEC-REQ-051) · DOC-05 **Chốt** (DEC-REQ-053) · DOC-19 **Chốt khung** (DEC-REQ-055).  
**Cổng:** SRS PRB **đã chốt** (PGD · DEC-REQ-057). Nợ: DOC-07; HTML MCP; Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự AC. DOC-13 **đã chốt** (NFR-002…006).

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **probation** cho BA/Dev/QC. Không gồm tính 85% lương (PAY); ATS; e-sign; SN/lễ (EVT).

### 1.2 Phạm vi

HRM mInvoice — PRB (URD-05, BO-005 phần TV). T-15 / T-7 (ngày lịch); 3 kết quả; LM đề xuất / HR chốt; không CRM sales.

### 1.3 Định nghĩa, viết tắt

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| KT_TV | Ngày kết thúc thử việc trên HĐ EMP |
| T-15 / T-7 | KT_TV − 15 / − 7 **ngày lịch** (kèm chốt BR/UC) |
| 3 mã | Đạt · Gia hạn · Không đạt |
| SoT kết quả | Bản ghi HR chốt trên PRB-SCR-003 — không phải đề xuất LM |
| Phiếu động | Tiêu chí đánh giá từ master; không list cứng trên SRS |

### 1.4 Tài liệu tham chiếu

| ID | Tài liệu |
|----|----------|
| DOC-03 | BRD HRM **Chốt** (v0.7) |
| DOC-04 | PRB-BR-001…012 **Chốt** |
| DOC-05 | PRB-UC-001…004 **Chốt** |
| DOC-19 | PRB-SCR-001…004 **Chốt khung** |
| DOC-13 | NFR-002…006 **Chốt** |

### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI · §5 NFR (DOC-13) · §6 trace.

## 2. Mô tả tổng quan

### 2.1 Bối cảnh sản phẩm

Đọc mốc HĐ từ EMP. Đạt → EMP chính thức (PAY đọc HĐ). Không đạt → LIF off. Không federation CRM sales.

### 2.2 Chức năng sản phẩm

Job T-15 · task T-7 + phiếu đề xuất · HR chốt 3 mã · cảnh báo thiếu mốc.

### 2.3 Phân loại người dùng

| User class | Actor |
|------------|-------|
| NV TV / LM / HR | PRB-ACT-001…003 |
| Hệ thống | PRB-ACT-004 |
| EMP / LIF / PAY | PRB-ACT-005 |

### 2.4 Môi trường vận hành

| Mục | Yêu cầu |
|-----|---------|
| Client | Browser + mobile MVP; cùng identity IAM |
| Server | TBD DOC-08 |
| Network | Nội bộ mInvoice |

### 2.5 Ràng buộc thiết kế & triển khai

| ID | Constraint |
|----|------------|
| PRB-CN-001 | Không hardcode list tiêu chí phiếu / số tháng gia hạn |
| PRB-CN-002 | Không tính 85% trên PRB |
| PRB-CN-003 | HTML MCP không Must pixel |
| PRB-CN-004 | Không date picker KT ảo trên PRB-SCR-004 |
| PRB-CN-005 | Không nút CRM sales trên mọi màn PRB |

### 2.6 Giả định & phụ thuộc

| ID | Mô tả |
|----|-------|
| PRB-A-001 | T-15/T-7 = ngày lịch; ngày công = CR |
| PRB-A-002 | Không LM → task T-7 về HR (DEC-REQ-053) |
| PRB-A-003 | Disable login / khóa Git = IAM / LIF — không FR PRB |

## 3. Yêu cầu chức năng

| FR ID | Mô tả (shall) | Priority | Source | Verify |
|-------|---------------|----------|--------|--------|
| PRB-FR-001 | Hệ thống shall lấy BĐ/KT TV **chỉ** từ HĐ EMP; **cấm** gán ngày mặc định trên PRB | Must | UC-004 · BR-001 | Test |
| PRB-FR-002 | Hệ thống shall gửi cảnh báo T-15 khi ngày hệ thống = KT_TV − 15 ngày lịch | Must | UC-001 · BR-002 | Test |
| PRB-FR-003 | Hệ thống shall tạo task đánh giá T-7 khi ngày hệ thống = KT_TV − 7 ngày lịch | Must | UC-002 · BR-003 | Test |
| PRB-FR-004 | Hệ thống shall chỉ cho phép đề xuất/chốt ∈ {Đạt, Gia hạn, Không đạt}; mã khác → chặn | Must | UC-002,003 · BR-004 | Test |
| PRB-FR-005 | Hệ thống shall khi HR chốt **Đạt** yêu cầu EMP chuyển HĐ chính thức; **cấm** tính 85% trên PRB | Must | UC-003 · BR-005 | Test |
| PRB-FR-006 | Hệ thống shall khi **Gia hạn** cập nhật KT_TV theo thời lượng **master**; cấm ô số tháng tự do | Must | UC-003 · BR-006 | Test |
| PRB-FR-007 | Hệ thống shall khi **Không đạt** mở luồng off LIF; **cấm** xóa im lặng hồ sơ EMP | Must | UC-003 · BR-007 | Test |
| PRB-FR-008 | Hệ thống shall đưa **mọi** NV đang TV (HĐ hiệu lực, đủ mốc) vào hàng T-15/T-7 đúng hạn | Must | UC-001,002 · BR-008 | Test |
| PRB-FR-009 | Hệ thống shall chỉ HR **chốt** SoT trên PRB-SCR-003; LM/NV → **403**; LM chỉ [Lưu đề xuất] | Must | UC-002,003 · BR-009 | Test |
| PRB-FR-010 | Hệ thống shall **không** gửi/notify CRM **bán hàng** từ job hoặc màn PRB | Must | UC-001…003 · BR-010 | Test |
| PRB-FR-011 | Hệ thống shall nhắc T-15/T-7 qua HRM in-app **và** email/app; không bắt buộc vendor cụ thể | Must | UC-001,002 · BR-011 | Test |
| PRB-FR-012 | Hệ thống shall render tiêu chí phiếu từ **master**; cấm list field cứng trên UI/SRS | Must | UC-002 · BR-012 | Test |
| PRB-FR-013 | Hệ thống shall đủ PRB-SCR-001…004; pixel HTML **không** Must | Must | DOC-19 | Test |
| PRB-FR-014 | Hệ thống shall khi **không có LM** gán task T-7 cho HR; HR vẫn chốt được khi LM chưa đề xuất | Must | UC-002 EF-4,5 | Test |
| PRB-FR-015 | Hệ thống shall trên PRB-SCR-004 chỉ cảnh báo + điều hướng EMP; **cấm** date picker KT ảo | Must | UC-004 · DOC-19 | Test |
| PRB-FR-016 | Hệ thống shall sau Gia hạn lập lịch T-15/T-7 theo **KT mới** | Must | UC-003 · BR-002,003 | Test |
| PRB-FR-017 | Hệ thống shall ghi **audit** người/thời điểm chốt SoT = HR | Must | UC-003 · BR-009 | Test |

### PRB-FR-009 — HR SoT (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Nút [Chốt] chỉ HR. LM không có [Chốt chính thức] / [Chuyển HĐ]. |
| **Error handling** | UC-002 EF-1 · UC-003 EF-1 |

### PRB-FR-005 — Đạt không tính lương (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | PAY đọc HĐ EMP sau chuyển chính thức. PRB không xuất hệ số 85%. |
| **Error handling** | UC-003 EF-5 |

## 4. Yêu cầu giao diện bên ngoài

### 4.1 UI

Bắt buộc PRB-SCR-001…004. Không màn PAY 85%. Không màn LIF checklist.

### 4.2 Phần cứng

Không.

### 4.3 Phần mềm

EMP (mốc HĐ) · LIF (off) · PAY (đọc HĐ, không đọc PRB để tính) · IAM (role). **Không** CRM sales.

### 4.4 Truyền thông

Cấm notify/federation sales. Email/app nhắc = kênh nội bộ; vendor TBD DOC-08/10.

## 5. NFR tóm tắt

→ **DOC-13** đã chốt: NFR-002…006.

## 6. Ma trận truy vết (tóm tắt)

| FR ID | UC | BR | AC |
|-------|----|----|-----|
| PRB-FR-001 | 004 | 001 | *(DOC-07)* |
| PRB-FR-002 | 001 | 002 | |
| PRB-FR-003 | 002 | 003 | |
| PRB-FR-004 | 002,003 | 004 | |
| PRB-FR-005 | 003 | 005 | |
| PRB-FR-006 | 003 | 006 | |
| PRB-FR-007 | 003 | 007 | |
| PRB-FR-008 | 001,002 | 008 | |
| PRB-FR-009 | 002,003 | 009 | |
| PRB-FR-010 | 001…003 | 010 | |
| PRB-FR-011 | 001,002 | 011 | |
| PRB-FR-012 | 002 | 012 | |
| PRB-FR-013 | — | — | |
| PRB-FR-014 | 002 | 009 | |
| PRB-FR-015 | 004 | 001 | |
| PRB-FR-016 | 003 | 002,003 | |
| PRB-FR-017 | 003 | 009 | |

### 6.1 BRQ → FR probation

| BRQ | PRB-FR | Kết luận |
|-----|--------|----------|
| BO-005 / URD-05 | 002, 003, 008, 011 | Có (T-15/T-7 + coverage) |
| BRQ-001 | 001…017 | Một phần (PRB) |
| BRQ-005 | 007 | Một phần (mở LIF; Git = LIF) |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-REQ-057) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn Draft |
| Business Owner | Ban HR | | ☐ Nợ |
