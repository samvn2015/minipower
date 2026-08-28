# DOC-06 — Đặc tả Yêu cầu Phần mềm — Identity (IAM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (SRS identity · DEC-REQ-047) |

**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 **Chốt** v0.2 (DEC-REQ-041) · DOC-05 **Chốt** v0.2 (DEC-REQ-043) · DOC-19 **Chốt khung** (DEC-REQ-045).  
**Cổng:** SRS IAM **đã chốt** (PGD · DEC-REQ-047). Nợ: DOC-07; SSO/MFA (DOC-08); HTML MCP; Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự AC. DOC-13 **đã chốt** (NFR-002…006).

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **identity** cho BA/Dev/QC. Không gồm khóa Git/CRM (LIF); tính lương (PAY); SSO/MFA.

### 1.2 Phạm vi

HRM mInvoice — IAM (URD III, CN-002, BRQ-006). 5 role MVP; cô lập lương; 403 màn HR; disable **login**; không CRM sales.

### 1.3 Định nghĩa, viết tắt

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| Phiên | Session hợp lệ sau login (cơ chế = DOC-08) |
| 5 role MVP | IAM-ROLE-NV / LM / HR / IT / PGD |
| Permission lương | Quyền xem phiếu/số lương **người khác** — **không** nằm trong role LM |
| Disable login | Vô hiệu đăng nhập HRM; ≠ khóa Git |

### 1.4 Tài liệu tham chiếu

| ID | Tài liệu |
|----|----------|
| DOC-03 | BRD HRM **Chốt** (v0.7) |
| DOC-04 | IAM-BR-001…017 **Chốt** v0.2 |
| DOC-05 | IAM-UC-001…005 **Chốt** v0.2 |
| DOC-19 | IAM-SCR-001…004 **Chốt khung** |
| DOC-13 | NFR-002…006 **Chốt** |

### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI · §5 NFR (DOC-13) · §6 trace.

## 2. Mô tả tổng quan

### 2.1 Bối cảnh sản phẩm

Nền tảng mọi module đã chốt. LIF gọi disable login; **không** thực thi khóa Git trên IAM.

### 2.2 Chức năng sản phẩm

Login 2 kênh · gán role · 403 lương/màn HR · disable login · cấm CRM sales.

### 2.3 Phân loại người dùng

| User class | Actor |
|------------|-------|
| NV / LM / HR / IT / PGD | IAM-ACT-001…005 |
| Hệ thống | IAM-ACT-006 |
| LIF | IAM-ACT-007 (gọi disable) |

### 2.4 Môi trường vận hành

| Mục | Yêu cầu |
|-----|---------|
| Client | Browser + mobile MVP; cùng IAM |
| Server | TBD DOC-08 |
| Network | Nội bộ mInvoice |

### 2.5 Ràng buộc thiết kế & triển khai

| ID | Constraint |
|----|------------|
| IAM-CN-001 | Không hardcode list permission nút UI |
| IAM-CN-002 | SSO/MFA không Must trên SRS này |
| IAM-CN-003 | HTML MCP không Must pixel |
| IAM-CN-004 | Không nút khóa Git/CRM trên IAM-SCR-004 |

### 2.6 Giả định & phụ thuộc

| ID | Mô tả |
|----|-------|
| IAM-A-001 | Cookie/JWT = DOC-08 |
| IAM-A-002 | Ma trận DOC-04 §4 = SoT MVP; PRB/EVT/RPT khi có SRS |
| IAM-A-003 | PGD không mặc định permission lương Cty |

## 3. Yêu cầu chức năng

| FR ID | Mô tả (shall) | Priority | Source | Verify |
|-------|---------------|----------|--------|--------|
| IAM-FR-001 | Hệ thống shall yêu cầu phiên cho API/màn Must; không phiên hoặc **hết hạn** → **401**; MVP không HRM public | Must | UC-001 · BR-001 | Test |
| IAM-FR-002 | Hệ thống shall dùng **cùng** identity/role/403 trên IAM-SCR-001 và 002 | Must | UC-001 · BR-002 | Test |
| IAM-FR-003 | Hệ thống shall hỗ trợ đúng 5 role MVP + permission master; role mới = master | Must | UC-002 · BR-003 | Test |
| IAM-FR-004 | Hệ thống shall **403** nếu LM đọc phiếu/số lương NV khác (kể cả cấp dưới) | Must | UC-003 · BR-004 | Test |
| IAM-FR-005 | Hệ thống shall **403** nếu NV đọc hồ sơ/phép/phiếu **người khác** | Must | UC-003 · BR-005 | Test |
| IAM-FR-006 | Hệ thống shall cho HR (catalog) màn SoT: EMP DS, TIM, PAY kỳ, LEV C2, LIF N | Must | BR-006 · §4 | Test |
| IAM-FR-007 | Hệ thống shall **403** IT mở PAY nếu không có role/permission HR | Must | UC-003 · BR-007 | Test |
| IAM-FR-008 | Hệ thống shall ẩn/403 NV/LM: TIM import-chốt, PAY kỳ, EMP-SCR-001, LIF khóa Git | Must | UC-003 · BR-008 | Test |
| IAM-FR-009 | Hệ thống shall **không** cấp permission lương khi đổi/gán LM | Must | UC-002 · BR-009 | Test |
| IAM-FR-010 | Hệ thống shall cho IT (hoặc LIF gọi) **disable login** trên IAM-SCR-004; **cấm** nút khóa Git/CRM trên màn này | Must | UC-004 · BR-010 | Test |
| IAM-FR-011 | Hệ thống shall ghi **audit** khi xem phiếu lương | Must | UC-003 · BR-011 | Test |
| IAM-FR-012 | Hệ thống shall **không** cấp token/federation sang CRM **bán hàng** | Must | UC-005 · BR-012 | Test |
| IAM-FR-013 | Hệ thống shall chỉ HR hoặc IT gán/gỡ role trên IAM-SCR-003; NV/LM → 403 | Must | UC-002 · BR-013 | Test |
| IAM-FR-014 | Hệ thống shall hợp quyền nhiều role; **thiếu** permission lương → 403 phiếu người khác | Must | UC-002 · BR-014 | Test |
| IAM-FR-015 | Hệ thống shall cho LM **C1** đơn phép cấp dưới (200) **không** suy ra lương | Must | UC-003 · BR-015 | Test |
| IAM-FR-016 | Hệ thống shall **403** LM C2 / duyệt đột xuất | Must | UC-003 · BR-016 | Test |
| IAM-FR-017 | Hệ thống shall map login **1–1** MNV EMP hiệu lực; disable login **không** xóa hồ sơ | Must | UC-001,004 · BR-017 | Test |
| IAM-FR-018 | Hệ thống shall đủ IAM-SCR-001…004; pixel HTML **không** Must | Must | DOC-19 | Test |
| IAM-FR-019 | Hệ thống shall **403** PGD xem phiếu Cty nếu chưa gán HR / permission lương | Must | UC-003 · DOC-04 §1 | Test |

### IAM-FR-004 — LM không lương (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | PAY-BR-007 thắng. Role LM không chứa permission lương người khác. |
| **Error handling** | UC-003 EF-1 |

### IAM-FR-010 — Disable login (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Chỉ vô hiệu đăng nhập HRM. Khóa Git/CRM = LIF-UC-003. |
| **Error handling** | UC-004 EF-1 |

## 4. Yêu cầu giao diện bên ngoài

### 4.1 UI

Bắt buộc IAM-SCR-001…004. 403 trên màn PAY/TIM/EMP — không màn IAM riêng.

### 4.2 Phần cứng

Không.

### 4.3 Phần mềm

Mọi module Must đã chốt. LIF gọi disable. **Không** CRM sales. API session → DOC-12/08.

### 4.4 Truyền thông

Cấm federation sales.

## 5. NFR tóm tắt

→ **DOC-13** đã chốt: NFR-002…006.

## 6. Ma trận truy vết (tóm tắt)

| FR ID | UC | BR | AC |
|-------|----|----|-----|
| IAM-FR-001 | 001 | 001 | *(DOC-07)* |
| IAM-FR-002 | 001 | 002 | |
| IAM-FR-003 | 002 | 003 | |
| IAM-FR-004 | 003 | 004 | |
| IAM-FR-005 | 003 | 005 | |
| IAM-FR-006 | — | 006 | |
| IAM-FR-007 | 003 | 007 | |
| IAM-FR-008 | 003 | 008 | |
| IAM-FR-009 | 002 | 009 | |
| IAM-FR-010 | 004 | 010 | |
| IAM-FR-011 | 003 | 011 | |
| IAM-FR-012 | 005 | 012 | |
| IAM-FR-013 | 002 | 013 | |
| IAM-FR-014 | 002 | 014 | |
| IAM-FR-015 | 003 | 015 | |
| IAM-FR-016 | 003 | 016 | |
| IAM-FR-017 | 001,004 | 017 | |
| IAM-FR-018 | — | — | |
| IAM-FR-019 | 003 | — | |

### 6.1 BRQ → FR identity

| BRQ | IAM-FR | Kết luận |
|-----|--------|----------|
| BRQ-001 | 001…019 | Một phần (IAM) |
| BRQ-006 | 002, 018 | Có (mobile cùng IAM) |
| BRQ-005 | 010 | Một phần (disable login; Git = LIF) |
| BRQ-002…004, 007…010 | — | Module khác |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-REQ-047) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
