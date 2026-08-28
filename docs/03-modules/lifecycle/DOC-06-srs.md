# DOC-06 — Đặc tả Yêu cầu Phần mềm — Lifecycle (LIF)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (SRS lifecycle · DEC-REQ-034) |

**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 **Chốt** (DEC-REQ-024) · DOC-05 **Chốt** (DEC-REQ-027) · DOC-19 **Chốt khung** (DEC-REQ-030).  
**Cổng:** SRS LIF **đã chốt** (PGD · DEC-REQ-034). Nợ: DOC-07; DOC-13; HTML MCP; tick list master; **N+3 = 3 ngày lịch** (nháp); Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự AC.  
CRM SP khóa in scope; notify CRM **bán hàng** ngoài.

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **lifecycle** cho BA/Dev/QC. Không gồm ATS; khóa lương PAY; e-sign HĐLĐ; notify sales CRM.

### 1.2 Phạm vi

HRM mInvoice — on/off (BRQ-005, BR-006, A-003). Checklist động; cấp TK lúc on; khóa Git + CRM sản phẩm tại N+3; N = ngày LV cuối.

### 1.3 Định nghĩa, viết tắt

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| N | Ngày **làm việc cuối** đã HR xác nhận — không phải ngày ký đơn |
| N+3 | N cộng **3 ngày lịch** (nháp; TBD vs ngày công chuẩn) |
| CRM SP | CRM **sản phẩm** (khóa quyền) — không phải CRM sales/pipeline |
| Checklist | Mục tick từ master HR/IT, không list cứng |

### 1.4 Tài liệu tham chiếu

| ID | Tài liệu |
|----|----------|
| DOC-03 | BRD HRM **Chốt** (v0.7) |
| DOC-04 | LIF-BR-001…012 **Chốt** |
| DOC-05 | LIF-UC-001…005 **Chốt** |
| DOC-19 | LIF-SCR-001…006 **Chốt khung** |

### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI (DOC-19) · §5 NFR → DOC-13 · §6 trace + BRQ.

## 2. Mô tả tổng quan

### 2.1 Bối cảnh sản phẩm

Module trong HRM. Phụ thuộc: EMP hồ sơ; IAM/IT Git & CRM SP. HR xác nhận N; job/IT khóa. Không SSH Git từ HR.

### 2.2 Chức năng sản phẩm

Onboarding checklist + cấp TK · xác nhận N · job khóa Git/CRM SP · off checklist · chat/CR an ninh.

### 2.3 Phân loại người dùng

| User class | Actor | |
|------------|-------|-|
| NV | LIF-ACT-001 | Không xác nhận N đủ kích job |
| HR/C&B | LIF-ACT-002 | Checklist; xác nhận N |
| IT/IAM | LIF-ACT-003 | Cấp TK; khóa Git/CRM |
| Hệ thống | LIF-ACT-004 | Job N+3 |

### 2.4 Môi trường vận hành

| Mục | Yêu cầu |
|-----|---------|
| Client | Browser HR/IT |
| Server | Job lịch N+3; TBD architecture |
| Network | Nội bộ + connector Git/CRM SP (DOC-12) |

### 2.5 Ràng buộc thiết kế & triển khai

| ID | Constraint |
|----|------------|
| LIF-CN-001 | Không hardcode tên tick checklist trên SRS |
| LIF-CN-002 | Không webhook / ticket phép sang CRM **sales** |
| LIF-CN-003 | HTML MCP không Must pixel-perfect |
| LIF-CN-004 | N+3 đơn vị ngày: **lịch** trừ CR |

### 2.6 Giả định & phụ thuộc

| ID | Mô tả |
|----|-------|
| LIF-A-001 | Connector Git/CRM SP = architecture DOC-12 |
| LIF-A-002 | EMP unique email khi LIF đã cấp |
| LIF-A-003 | Khóa chat ≠ bắt buộc = N+3 Git nếu master khác |

## 3. Yêu cầu chức năng

| FR ID | Mô tả (shall) | Priority | Source | Verify |
|-------|---------------|----------|--------|--------|
| LIF-FR-001 | Hệ thống shall hiển thị checklist on trên LIF-SCR-002 từ master; **cấm** đóng on nếu thiếu tick Must | Must | UC-001 · BR-005,012 | Test |
| LIF-FR-002 | Hệ thống shall hỗ trợ **cấp** email @Cty, Git, CRM SP, chat **lúc on** — cấm trì hoãn cấp Git đến N+3 | Must | UC-001 · BR-007 | Test |
| LIF-FR-003 | Hệ thống shall cho HR xác nhận N trên LIF-SCR-003 là **ngày LV cuối**; cấm dùng ngày ký đơn làm N | Must | UC-002 · BR-001,009 | Test |
| LIF-FR-004 | Hệ thống shall **không** kích job N+3 nếu chỉ NV tự nhập N (chưa HR xác nhận) | Must | UC-002 · BR-009 | Test |
| LIF-FR-005 | Hệ thống shall khóa Git khi ngày hệ thống ≥ N+3 (**3 ngày lịch**, nháp) và N đã xác nhận | Must | UC-003 · BR-002 | Test |
| LIF-FR-006 | Hệ thống shall khóa CRM **sản phẩm** **cùng mốc** Git (N+3); cấm lệch chỉ khóa một bên | Must | UC-003 · BR-003 | Test |
| LIF-FR-007 | Hệ thống shall **cấm** job khóa Git/CRM SP **trước** N+3 trừ CR/an ninh (LIF-SCR-006) | Must | UC-003,005 · BR-004 | Test |
| LIF-FR-008 | Hệ thống shall 403 HR khóa Git trực tiếp; HR chỉ xem LIF-SCR-004 + ticket IT | Must | UC-003 · BR-008 | Test |
| LIF-FR-009 | Hệ thống shall checklist off trên LIF-SCR-005 từ master; cấm đóng off thiếu tick Must | Must | UC-004 · BR-006 | Test |
| LIF-FR-010 | Hệ thống shall **không** gửi sự kiện LIF/phép sang CRM bán hàng | Must | UC-003 EF-4 · BR-010 | Test |
| LIF-FR-011 | Hệ thống shall khóa chat theo mốc **master** (LIF-SCR-006); cấm mặc định im lặng = N+3 Git nếu quy chế khác | Should | UC-005 · BR-011 | Test |
| LIF-FR-012 | Hệ thống shall đủ luồng LIF-SCR-001…006; pixel HTML **không** Must | Must | DOC-19 | Test |
| LIF-FR-013 | Hệ thống shall hiển thị N và N+3 dự kiến trên LIF-SCR-001/004 | Must | DOC-19 · UC-002 | Test |
| LIF-FR-014 | Hệ thống shall audit khóa sớm có CR; khóa sớm không CR = vi phạm (ghi log) | Must | UC-005 · BR-004 | Test |
| LIF-FR-015 | Hệ thống shall 403 NV ghi N đủ để chạy job | Must | UC-002 · BR-009 | Test |
| LIF-FR-016 | Hệ thống shall **không** có nút “gửi CRM sales” trên LIF-SCR-004 | Must | DOC-19 · BR-010 | Test |

### LIF-FR-003 — N = ngày LV cuối (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | N trên SCR-003. Nhãn UI cấm nhầm ngày ký đơn. |
| **Inputs** | Ngày LV cuối do HR |
| **Processing** | IF lấy ngày ký = N THEN reject. IF NV set THEN không schedule job. |
| **Outputs** | N confirmed; lịch N+3 |
| **Error handling** | UC-002 EF-1…3 |

### LIF-FR-005 — Khóa Git N+3 (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Job/IT khóa repo Cty tại N+3 ngày lịch. |
| **Preconditions** | FR-003 confirmed |
| **Postconditions** | Git khóa; CRM SP cùng lúc (FR-006) |
| **Error handling** | Khóa trước = FR-007; HR SSH = FR-008 |

## 4. Yêu cầu giao diện bên ngoài

### 4.1 UI

Bắt buộc LIF-SCR-001…006. Pixel MCP **không** Must.

### 4.2 Phần cứng

Không.

### 4.3 Phần mềm

IAM · Git · CRM SP · chat. EMP hồ sơ. **Không** CRM sales. API/connector → DOC-12.

### 4.4 Truyền thông

Job nội bộ; **cấm** notify pipeline sales.

## 5. NFR tóm tắt

→ **DOC-13** (chưa slice).

| NFR ID | Category | Tóm tắt |
|--------|----------|---------|
| *(DOC-13)* | Security | HR không credential Git; 403 khóa tay |
| *(DOC-13)* | Audit | Xác nhận N; khóa Git/CRM; CR an ninh |

## 6. Ma trận truy vết (tóm tắt)

| FR ID | UC | BR | AC | Test |
|-------|----|----|----|------|
| LIF-FR-001 | 001 | 005,012 | *(DOC-07)* | |
| LIF-FR-002 | 001 | 007 | | |
| LIF-FR-003 | 002 | 001,009 | | |
| LIF-FR-004 | 002 | 009 | | |
| LIF-FR-005 | 003 | 002 | | |
| LIF-FR-006 | 003 | 003 | | |
| LIF-FR-007 | 003,005 | 004 | | |
| LIF-FR-008 | 003 | 008 | | |
| LIF-FR-009 | 004 | 006 | | |
| LIF-FR-010 | 003 | 010 | | |
| LIF-FR-011 | 005 | 011 | | |
| LIF-FR-012 | — | — | | |
| LIF-FR-013 | 002 | — | | |
| LIF-FR-014 | 005 | 004 | | |
| LIF-FR-015 | 002 | 009 | | |
| LIF-FR-016 | — | 010 | | |

### 6.1 BRQ (DOC-03) → FR lifecycle

SRS **chỉ** module LIF.

| BRQ | LIF-FR | Kết luận |
|-----|--------|----------|
| BRQ-001 | — | Không (hồ sơ EMP) |
| BRQ-002 | — | Không (payroll) |
| BRQ-003 | — | Không (payroll) |
| BRQ-004 | — | Không (TIM) |
| BRQ-005 | 001…009, 012…015 | Có |
| BRQ-006 | — | Không (hồ sơ/mobile EMP) |
| BRQ-007 | — | Không (PC) |
| BRQ-008 | — | Không (leave) |
| BRQ-009 | — | Không (UAT lương) |
| BRQ-010 | — | Không (leave) |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-034) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
