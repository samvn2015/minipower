# DOC-06 — Đặc tả Yêu cầu Phần mềm — Payroll (PAY)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (SRS payroll · DEC-REQ-014) |

**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 **Chốt** (DEC-REQ-011) · DOC-05 **Chốt** (DEC-REQ-012) · DOC-19 **Chốt khung** (DEC-REQ-013).  
**Cổng:** SRS payroll **đã chốt** (PGD · DEC-REQ-014). Nợ: DOC-07; DOC-13; HTML MCP; làm tròn; Ban HR ☐. **Chưa** `02-baseline/`.

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **payroll** cho BA/Dev/QC. Không gồm import Excel công (TIM), quỹ phép / C1–C2 (LEV), N+3 (LIF).

### 1.2 Phạm vi

HRM mInvoice — phân hệ lương (BRQ-002, 003, 007, 009 phần lương). Web + mobile MVP (phiếu mình). Tính từ công **đã chốt** + HĐ + master quy chế.

### 1.3 Định nghĩa, viết tắt

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| N_thực | Ngày công CC đã chốt; **đã gồm** phép hưởng lương (A-001) |
| N_KHL | Ngày phép không lương **Đã duyệt** (LEV) nằm trong kỳ |
| N_tính | N_thực − N_KHL; không cộng thêm phép hưởng |
| Kỳ Draft / Chốt | Bản tính chưa phát hành / đã khóa phiếu |
| Master kỳ | Catalog PC + tỷ lệ BH/TNCN **hiệu lực tại kỳ lương** |

### 1.4 Tài liệu tham chiếu

| ID | Tài liệu |
|----|----------|
| DOC-03 | BRD HRM **Chốt** (v0.7) |
| DOC-04 | PAY-BR-001…012 **Chốt** |
| DOC-05 | PAY-UC-001…005 **Chốt** |
| DOC-19 | PAY-SCR-001…007 **Chốt khung** |

### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI (DOC-19) · §5 NFR → DOC-13 · §6 trace + BRQ BRD.

## 2. Mô tả tổng quan

### 2.1 Bối cảnh sản phẩm

Module trong HRM. Phụ thuộc: IAM; TIM tháng **đã chốt**; LEV ngày **Đã duyệt**; HĐ + master C&B (động, DEC-DIS-014). Đầu ra: phiếu NV (web/mobile/PDF/email).

### 2.2 Chức năng sản phẩm

Tính kỳ · chốt kỳ · nhập PC tháng · xem phiếu mình · xuất hàng loạt. Không sửa công trên PAY.

### 2.3 Phân loại người dùng

| User class | Actor | |
|------------|-------|-|
| NV | PAY-ACT-001 | Phiếu mình |
| LM | PAY-ACT-002 | Không màn lương |
| HR/C&B | PAY-ACT-003 | Tính / chốt / PC tháng / xuất |
| IT | PAY-ACT-004 | Không xem số lương trừ IAM HR |

### 2.4 Môi trường vận hành

| Mục | Yêu cầu |
|-----|---------|
| Client | Browser + app mobile MVP; cùng rule phiếu |
| Server | TBD architecture |
| Network | Nội bộ mInvoice |

### 2.5 Ràng buộc thiết kế & triển khai

| ID | Constraint |
|----|------------|
| PAY-CN-001 | Không hardcode % BH/TNCN từ URD; lấy master kỳ |
| PAY-CN-002 | Không list mã PC cứng trên SRS; dropdown master |
| PAY-CN-003 | HTML wireframe không Must pixel-perfect (MCP nợ) |
| PAY-CN-004 | Không form sửa N_thực / OT / phép trong PAY |

### 2.6 Giả định & phụ thuộc

| ID | Mô tả |
|----|-------|
| PAY-A-001 | TIM đã chốt: N_thực gồm phép hưởng (DOC-03 A-001) |
| PAY-A-002 | C&B duy trì master PC/BH trước tính kỳ |
| PAY-A-003 | Hủy chốt kỳ = ngoài MVP (CR) |
| PAY-A-004 | Làm tròn / UAT 0 đồng chi tiết → DOC-07 |

## 3. Yêu cầu chức năng

| FR ID | Mô tả (shall) | Priority | Source | Verify |
|-------|---------------|----------|--------|--------|
| PAY-FR-001 | Hệ thống shall cho HR/C&B tính kỳ trên PAY-SCR-002 **chỉ khi** TIM tháng đã chốt; đọc N_thực, OT (1.5/2.0/3.0), N_KHL từ TIM+LEV Đã duyệt, HĐ, master kỳ | Must | UC-001 · BR-004,009,010 | Test |
| PAY-FR-002 | Hệ thống shall tính N_tính = N_thực − N_KHL và **cấm** cộng thêm ngày phép hưởng lương | Must | UC-001 · BR-001,011 | Test |
| PAY-FR-003 | Hệ thống shall nhân hệ số **0,85** phần lương thời gian khi trạng thái HĐ = thử việc **tại kỳ**; hết TV = 100% (đổi tỷ lệ = master, không sửa code URD) | Must | UC-001 · BR-003 | Test |
| PAY-FR-004 | Hệ thống shall tính OT chỉ từ loại giờ trên bảng công đã chốt; PAY không nhập OT tay | Must | UC-001 · BR-004 | Test |
| PAY-FR-005 | Hệ thống shall cộng PC/thưởng kênh HĐ cố định **và** dòng nhập tháng; mã tháng phải ∈ master kỳ | Must | UC-001,003 · BR-005 | Test |
| PAY-FR-006 | Hệ thống shall tạm tính BH + TNCN theo tỷ lệ **hiệu lực kỳ**; cấm hardcode % URD | Must | UC-001 · BR-006 | Test |
| PAY-FR-007 | Hệ thống shall **chặn chốt** (PAY-SCR-003) nếu N_tính > ngày công chuẩn tháng (lịch Cty); preview được hiện cảnh báo | Must | UC-002 · BR-002 | Test |
| PAY-FR-008 | Hệ thống shall **cấm** sửa ngày công / OT / phép trên PAY-SCR-002 và mọi màn PAY | Must | UC-001 EF-5 · BR-009 | Test |
| PAY-FR-009 | Hệ thống shall 403 nếu NV hoặc LM tính kỳ, chốt kỳ, nhập PC tháng, hoặc xuất hàng loạt | Must | UC-001…003,005 · BR-010 | Test |
| PAY-FR-010 | Hệ thống shall chỉ trả phiếu kỳ **đã chốt** cho chủ phiếu; NV xem người khác → 403; LM xem lương cấp dưới → 403; HR theo IAM | Must | UC-004 · BR-007 | Test |
| PAY-FR-011 | Hệ thống shall áp **cùng** PAY-FR-010 trên PAY-SCR-006 (mobile); không nới quyền | Must | UC-004 AF-1 · BR-012 | Test |
| PAY-FR-012 | Hệ thống shall xuất PDF/email hàng loạt (PAY-SCR-007) mỗi phiếu đúng chủ / địa chỉ; **cấm** CC LM hoặc gửi nhầm người | Must | UC-005 · BR-007 | Test |
| PAY-FR-013 | Nếu TIM tách phép hưởng khỏi N_thực, hệ thống shall **không** im lặng cộng lại; cảnh báo A-001 / CR; không tự sửa công | Must | UC-001 EF-4 · BR-001,011 | Test |
| PAY-FR-014 | Hệ thống shall hiển thị preview PAY-SCR-002 đủ cột: N_thực, N_KHL, N_tính, hệ số TV, OT, PC HĐ, PC tháng, BH, TNCN tạm, thực lĩnh | Must | UC-001 · DOC-19 | Test |
| PAY-FR-015 | Hệ thống shall cho HR lưu PC tháng trên PAY-SCR-004 (kỳ, NV, mã master, số tiền); mã không thuộc master → chặn | Must | UC-003 · BR-005 | Test |
| PAY-FR-016 | Hệ thống shall cho tính lại **ghi đè Draft** cùng kỳ chưa chốt; **không** đè kỳ đã chốt; **không** nút hủy chốt MVP | Must | UC-001 AF-1 · UC-002 · PAY-A-003 | Test |
| PAY-FR-017 | Hệ thống shall ẩn PAY-SCR-001…004,007 với NV/LM; NV chỉ vào phiếu mình | Must | DOC-19 · BR-007,010 | Test |
| PAY-FR-018 | Hệ thống shall lưu từng dòng tính để UAT kỳ mẫu so bảng tay = **0 đồng** sau làm tròn quy chế (chi tiết DOC-07) | Must | UC-002 AF-1 · BR-008 | Test |

### PAY-FR-002 — N_tính không cộng kép (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | N_tính = N_thực − N_KHL. Phép hưởng đã nằm trong N_thực. |
| **Inputs** | N_thực (TIM chốt), N_KHL (LEV Đã duyệt), lịch chuẩn tháng |
| **Processing** | IF cộng N_phép_hưởng vào N_tính THEN cấm. IF N_KHL không nằm trong N_thực THEN cảnh báo A-001, không im lặng. |
| **Outputs** | N_tính trên preview + phiếu |
| **Error handling** | UC-001 EF-3, EF-4 |

### PAY-FR-010 — Cô lập phiếu (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Đọc phiếu = chủ sở hữu hoặc HR/C&B được IAM. LM không phải HR. |
| **Preconditions** | Kỳ Chốt; session IAM |
| **Postconditions** | 200 phiếu mình hoặc 403 |
| **Error handling** | UC-004 EF-1, EF-2, EF-3 (kỳ chưa chốt: không phiếu NV) |

## 4. Yêu cầu giao diện bên ngoài

### 4.1 UI

Bắt buộc đủ field/luồng DOC-19 PAY-SCR-001…007. Pixel HTML MCP **không** Must.

### 4.2 Phần cứng

Không (máy CC = TIM, ngoài PAY).

### 4.3 Phần mềm

IAM · TIM chốt tháng · LEV Đã duyệt · master C&B · HĐ. API → DOC-12. Nộp BH nhà nước / sổ cái = **ngoài scope**.

### 4.4 Truyền thông

Email phiếu đúng người; không webhook CRM bán hàng.

## 5. NFR tóm tắt

→ **DOC-13** (chưa slice).

| NFR ID | Category | Tóm tắt |
|--------|----------|---------|
| *(DOC-13)* | Security | Cô lập phiếu; 403 LM/NV sai vai |
| *(DOC-13)* | Audit | Log tính / chốt / xuất / xem phiếu |

## 6. Ma trận truy vết (tóm tắt)

| FR ID | UC | BR | AC | Test |
|-------|----|----|----|------|
| PAY-FR-001 | 001 | 004,009,010 | *(DOC-07)* | |
| PAY-FR-002 | 001 | 001,011 | | |
| PAY-FR-003 | 001 | 003 | | |
| PAY-FR-004 | 001 | 004 | | |
| PAY-FR-005 | 001,003 | 005 | | |
| PAY-FR-006 | 001 | 006 | | |
| PAY-FR-007 | 002 | 002 | | |
| PAY-FR-008 | 001 | 009 | | |
| PAY-FR-009 | 001…005 | 010 | | |
| PAY-FR-010 | 004 | 007 | | |
| PAY-FR-011 | 004 | 012 | | |
| PAY-FR-012 | 005 | 007 | | |
| PAY-FR-013 | 001 | 001,011 | | |
| PAY-FR-014 | 001 | — | | |
| PAY-FR-015 | 003 | 005 | | |
| PAY-FR-016 | 001,002 | — | | |
| PAY-FR-017 | — | 007,010 | | |
| PAY-FR-018 | 002 | 008 | | |

### 6.1 BRQ (DOC-03) → FR payroll

SRS **chỉ** module lương. BRQ phép/công/LIF **không** có FR đủ ở đây.

| BRQ | PAY-FR | Kết luận |
|-----|--------|----------|
| BRQ-001 | 001…018 (phần lương + IAM phiếu) | Một phần. Phép/công/LIF/EMP → SRS module khác. |
| BRQ-002 | 001, 002, 007, 013, 014 | Có (N_tính) |
| BRQ-003 | 003 | Có (85%) |
| BRQ-004 | — | Không (timekeeping) |
| BRQ-005 | — | Không (lifecycle / N+3) |
| BRQ-006 | 010, 011 | Có kênh phiếu (web+mobile). Hồ sơ/phép → module khác. |
| BRQ-007 | 005, 015 | Có (PC hai kênh + master) |
| BRQ-008 | — | Không (leave) |
| BRQ-009 | 010, 012, 017, 018 | Có phần lương/phiếu/UAT 0đ. Công/cảnh báo khác → TIM/PRB. |
| BRQ-010 | — | Không (leave 3 NLĐ) |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-014) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
