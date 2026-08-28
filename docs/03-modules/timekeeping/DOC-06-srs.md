# DOC-06 — Đặc tả Yêu cầu Phần mềm — Timekeeping (TIM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (SRS timekeeping · DEC-REQ-020) |

**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 **Chốt** (DEC-REQ-017) · DOC-05 **Chốt** (DEC-REQ-018) · DOC-19 **Chốt khung** (DEC-REQ-019).  
**Cổng:** SRS timekeeping **đã chốt** (PGD · DEC-REQ-020). Nợ: DOC-07; DOC-13; HTML MCP; định danh version file (cơ chế kỹ thuật); Ban HR ☐. **Chưa** `02-baseline/`.

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **timekeeping** cho BA/Dev/QC. Không gồm máy CC; tính lương (PAY **đọc** tháng chốt); C1–C2 phép.

### 1.2 Phạm vi

HRM mInvoice — phân hệ công (BRQ-004, BO-003, BR-003 OT trên bảng chốt). Import **một** mẫu Excel động; preview; chốt tháng; phép Đã duyệt; N_thực gồm phép hưởng.

### 1.3 Định nghĩa, viết tắt

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| Version mẫu | Mã template đang **hiệu lực** toàn Cty (một tại một thời điểm) |
| Preview | Kết quả kiểm file **chưa** ghi sổ |
| Commit | Ghi bảng công tháng (Draft) |
| Chốt tháng | Khóa công; PAY được đọc |
| N_thực | Ngày công xuất PAY — **đã gồm** phép hưởng |

### 1.4 Tài liệu tham chiếu

| ID | Tài liệu |
|----|----------|
| DOC-03 | BRD HRM **Chốt** (v0.7) |
| DOC-04 | TIM-BR-001…012 **Chốt** |
| DOC-05 | TIM-UC-001…005 **Chốt** |
| DOC-19 | TIM-SCR-001…006 **Chốt khung** |

### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI (DOC-19) · §5 NFR → DOC-13 · §6 trace + BRQ.

## 2. Mô tả tổng quan

### 2.1 Bối cảnh sản phẩm

Module trong HRM. Phụ thuộc: IAM; master mẫu HR (động); LEV **Đã duyệt**. Đầu ra: bảng công tháng chốt cho PAY.

### 2.2 Chức năng sản phẩm

Công bố mẫu · import/preview · commit · chốt tháng · bỏ chốt (khi PAY chưa chốt).

### 2.3 Phân loại người dùng

| User class | Actor | |
|------------|-------|-|
| NV / LM | TIM-ACT-001/002 | Không màn TIM |
| HR/C&B | TIM-ACT-003 | Mẫu / import / chốt |
| IT | TIM-ACT-004 | Hỗ trợ file mẫu; không chốt trừ IAM HR |

### 2.4 Môi trường vận hành

| Mục | Yêu cầu |
|-----|---------|
| Client | Browser HR (import Excel). Mobile NV **không** Must cho TIM |
| Server | TBD architecture |
| Network | Nội bộ mInvoice |

### 2.5 Ràng buộc thiết kế & triển khai

| ID | Constraint |
|----|------------|
| TIM-CN-001 | Không hardcode danh sách cột Excel trên code URD |
| TIM-CN-002 | Không API / firmware máy CC |
| TIM-CN-003 | HTML MCP không Must pixel-perfect |
| TIM-CN-004 | Không form sửa ô công trên PAY |

### 2.6 Giả định & phụ thuộc

| ID | Mô tả |
|----|-------|
| TIM-A-001 | Cách gắn mã version lên file (checksum / header) = DOC-06 chi tiết kỹ thuật khi implement; FR chỉ **shall** khớp version hiệu lực |
| TIM-A-002 | C&B công bố mẫu trước kỳ import |
| TIM-A-003 | Bỏ chốt TIM khi kỳ PAY đã chốt = cấm (không tự mở lương) |

## 3. Yêu cầu chức năng

| FR ID | Mô tả (shall) | Priority | Source | Verify |
|-------|---------------|----------|--------|--------|
| TIM-FR-001 | Hệ thống shall cho HR công bố **đúng một** version mẫu hiệu lực trên TIM-SCR-002; version cũ **từ chối** import mới | Must | UC-001 · BR-001 | Test |
| TIM-FR-002 | Hệ thống shall lấy cấu trúc cột từ **master HR theo quy chế**; cấm list cột cứng trên SRS/code URD | Must | UC-001 · BR-002 | Test |
| TIM-FR-003 | Hệ thống shall từ chối import nếu file **không khớp** version mẫu đang hiệu lực | Must | UC-002 · BR-003 | Test |
| TIM-FR-004 | Hệ thống shall preview từng dòng (OK / lỗi Must) trên TIM-SCR-003; **cấm** commit im lặng khi còn lỗi Must | Must | UC-002,003 · BR-004 | Test |
| TIM-FR-005 | Hệ thống shall ghi bảng công Draft (TIM-SCR-004) **chỉ khi** hết lỗi Must | Must | UC-003 · BR-004 | Test |
| TIM-FR-006 | Hệ thống shall cho HR chốt tháng trên TIM-SCR-005; PAY chỉ đọc tháng **đã chốt** | Must | UC-004 · BR-005 | Test |
| TIM-FR-007 | Hệ thống shall yêu cầu loại OT 1.5 / 2.0 / 3.0 trên dòng có giờ OT trước khi chốt; không để PAY nhập OT | Must | UC-004 · BR-006 | Test |
| TIM-FR-008 | Hệ thống shall đưa ngày LEV **Đã duyệt** vào bảng công tháng; đơn chờ / từ chối / hủy **không** vào công | Must | UC-004 · BR-007 | Test |
| TIM-FR-009 | Hệ thống shall xuất N_thực **đã gồm** phép hưởng lương; **cấm** tách để PAY cộng lại | Must | UC-004 · BR-008 | Test |
| TIM-FR-010 | Hệ thống shall chỉ nhận **file Excel đúng mẫu**; không kết nối máy CC | Must | UC-002 · BR-009 | Test |
| TIM-FR-011 | Hệ thống shall 403 nếu NV/LM công bố mẫu, import, commit, chốt, hoặc bỏ chốt | Must | UC-001…005 · BR-010,012 | Test |
| TIM-FR-012 | Hệ thống shall cho HR bỏ chốt trên TIM-SCR-006 rồi import lại; **cấm** bỏ chốt nếu kỳ PAY đã chốt; cấm sửa ô trên PAY | Must | UC-005 · BR-011 | Test |
| TIM-FR-013 | Hệ thống shall ẩn TIM-SCR-001…006 với NV/LM | Must | DOC-19 · BR-012 | Test |
| TIM-FR-014 | Hệ thống shall đủ field/luồng TIM-SCR-001…006 (DOC-19); pixel HTML **không** Must | Must | DOC-19 | Test |
| TIM-FR-015 | Hệ thống shall **cấm** hai mẫu cùng hiệu lực cùng lúc | Must | UC-001 EF-1 · BR-001 | Test |
| TIM-FR-016 | Preview chưa commit shall **không** tự ghi khi HR công bố mẫu mới (TIM-UC-001 step 3) | Must | UC-001 | Test |

### TIM-FR-001 — Một mẫu hiệu lực (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Toàn Cty một version import. Công bố mới vô hiệu version cũ với file mới. |
| **Inputs** | File mẫu + mã version + master cột |
| **Processing** | IF hai version Active THEN reject. IF import ≠ Active THEN reject. |
| **Outputs** | Version Active duy nhất |
| **Error handling** | UC-001 EF-1 · UC-002 EF-1 |

### TIM-FR-009 — N_thực gồm phép hưởng (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Payload công cho PAY: N_thực đã gồm ngày phép hưởng. |
| **Preconditions** | LEV Đã duyệt đã merge (FR-008) |
| **Postconditions** | Không xuất N_thực “sạch phép” |
| **Error handling** | Tách phép hưởng = fail FR; PAY-FR-013 cảnh báo nếu lệch |

## 4. Yêu cầu giao diện bên ngoài

### 4.1 UI

Bắt buộc TIM-SCR-001…006. Pixel MCP **không** Must.

### 4.2 Phần cứng

Không (máy CC ngoài scope).

### 4.3 Phần mềm

IAM · master mẫu · LEV Đã duyệt · PAY đọc tháng chốt. API → DOC-12.

### 4.4 Truyền thông

Không webhook CRM bán hàng.

## 5. NFR tóm tắt

→ **DOC-13** (chưa slice).

| NFR ID | Category | Tóm tắt |
|--------|----------|---------|
| *(DOC-13)* | Security | 403 NV/LM; ẩn màn HR |
| *(DOC-13)* | Audit | Log công bố mẫu / import / chốt / bỏ chốt |

## 6. Ma trận truy vết (tóm tắt)

| FR ID | UC | BR | AC | Test |
|-------|----|----|----|------|
| TIM-FR-001 | 001 | 001 | *(DOC-07)* | |
| TIM-FR-002 | 001 | 002 | | |
| TIM-FR-003 | 002 | 003 | | |
| TIM-FR-004 | 002,003 | 004 | | |
| TIM-FR-005 | 003 | 004 | | |
| TIM-FR-006 | 004 | 005 | | |
| TIM-FR-007 | 004 | 006 | | |
| TIM-FR-008 | 004 | 007 | | |
| TIM-FR-009 | 004 | 008 | | |
| TIM-FR-010 | 002 | 009 | | |
| TIM-FR-011 | 001…005 | 010,012 | | |
| TIM-FR-012 | 005 | 011 | | |
| TIM-FR-013 | — | 012 | | |
| TIM-FR-014 | — | — | | |
| TIM-FR-015 | 001 | 001 | | |
| TIM-FR-016 | 001 | — | | |

### 6.1 BRQ (DOC-03) → FR timekeeping

SRS **chỉ** module công.

| BRQ | TIM-FR | Kết luận |
|-----|--------|----------|
| BRQ-001 | 008, 011 (phần công + IAM) | Một phần |
| BRQ-002 | — | Không (payroll) |
| BRQ-003 | — | Không (payroll 85%) |
| BRQ-004 | 001…007, 010, 014, 015 | Có |
| BRQ-005 | — | Không (LIF) |
| BRQ-006 | — | Không kênh xem công NV |
| BRQ-007 | — | Không (payroll PC) |
| BRQ-008 | — | Không (leave) |
| BRQ-009 | 006 | Một phần (công chốt cho UAT lương) |
| BRQ-010 | — | Không (leave) |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-020) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
