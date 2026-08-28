# DOC-06 — Đặc tả Yêu cầu Phần mềm — Employee profile (EMP)


| Phiên bản | Ngày       | Tác giả        | Trạng thái                                   |
| --------- | ---------- | -------------- | -------------------------------------------- |
| 0.1       | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (SRS employee-profile · DEC-REQ-033 · gồm EMP-FR-017) |


**IEEE 830** · **ISO/IEC/IEEE 29148**. Tiên quyết: DOC-04 **Chốt** (DEC-REQ-023) · DOC-05 **Chốt** (DEC-REQ-026) · DOC-19 **Chốt khung** (DEC-REQ-029).  
**Cổng:** SRS EMP **đã chốt** (PGD · DEC-REQ-033), gồm **EMP-FR-017** trình độ học vấn. Nợ: DOC-07; DOC-13; HTML MCP; catalog bậc học master; Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự AC.

---

## 1. Giới thiệu

### 1.1 Mục đích

SRS module **employee-profile** cho BA/Dev/QC. Không gồm ATS; phiếu lương (PAY); phép (LEV); N+3 Git/CRM (LIF).

### 1.2 Phạm vi

HRM mInvoice — hồ sơ NV (URD-01, BR-005, BRQ-001 phần hồ sơ, BRQ-006 hồ sơ web+mobile). Unique, org, HĐ, **trình độ học vấn**, thâm niên master, đổi LM có duyệt.

### 1.3 Định nghĩa, viết tắt


| Thuật ngữ       | Định nghĩa                                            |
| --------------- | ----------------------------------------------------- |
| SoT hồ sơ       | HR/C&B là nguồn sự thật định danh / org / HĐ          |
| Field được phép | Field NV được sửa theo IAM + master (không list cứng) |
| Đổi LM          | Thay Line Manager **chỉ** sau duyệt một bậc           |
| Thâm niên         | Giá trị theo công thức **master quy chế**                                      |
| Trình độ học vấn | Field hồ sơ Must; bậc/giá trị = **master** (không hardcode THPT/ĐH/ThS trên SRS) |




### 1.4 Tài liệu tham chiếu


| ID     | Tài liệu                       |
| ------ | ------------------------------ |
| DOC-03 | BRD HRM **Chốt** (v0.7)        |
| DOC-04 | EMP-BR-001…012 **Chốt**        |
| DOC-05 | EMP-UC-001…005 **Chốt**        |
| DOC-19 | EMP-SCR-001…006 **Chốt khung** |




### 1.5 Tổng quan

§2 bối cảnh · §3 FR · §4 UI (DOC-19) · §5 NFR → DOC-13 · §6 trace + BRQ.

## 2. Mô tả tổng quan



### 2.1 Bối cảnh sản phẩm

Module trong HRM. Phụ thuộc: IAM; catalog org + field/HĐ master. Đầu ra: hồ sơ + HĐ cho PAY đọc hệ số TV. **Không** cấp quyền phiếu lương khi đổi LM.

### 2.2 Chức năng sản phẩm

Tạo/sửa hồ sơ HR · self-service web+mobile · đổi LM có duyệt · hiển thị thâm niên.

### 2.3 Phân loại người dùng


| User class         | Actor       |                                  |
| ------------------ | ----------- | -------------------------------- |
| NV                 | EMP-ACT-001 | Hồ sơ mình                       |
| LM                 | EMP-ACT-002 | Không SoT Cty; không phiếu lương |
| HR/C&B             | EMP-ACT-003 | Tạo/sửa; đề xuất đổi LM          |
| Người duyệt đổi LM | EMP-ACT-004 | Một bậc (IAM)                    |




### 2.4 Môi trường vận hành


| Mục     | Yêu cầu                                   |
| ------- | ----------------------------------------- |
| Client  | Browser HR + web/app NV (cùng rule hồ sơ) |
| Server  | TBD architecture                          |
| Network | Nội bộ mInvoice                           |




### 2.5 Ràng buộc thiết kế & triển khai


| ID         | Constraint                                              |
| ---------- | ------------------------------------------------------- |
| EMP-CN-001 | Không hardcode list field hồ sơ/HĐ trên SRS/code URD    |
| EMP-CN-002 | Không hardcode công thức thâm niên (năm luật) trên code |
| EMP-CN-003 | HTML MCP không Must pixel-perfect                       |
| EMP-CN-004 | Không Matrix / C1–C2 đổi LM (MVP một bậc)               |




### 2.6 Giả định & phụ thuộc


| ID        | Mô tả                                           |
| --------- | ----------------------------------------------- |
| EMP-A-001 | Catalog field / tái tuyển CCCD = master quy chế |
| EMP-A-002 | PAY đọc HĐ; EMP không tính lương                |
| EMP-A-003 | LIF cấp email lúc on; unique khi có giá trị     |




## 3. Yêu cầu chức năng


| FR ID      | Mô tả (shall)                                                                                                              | Priority | Source                  | Verify |
| ---------- | -------------------------------------------------------------------------------------------------------------------------- | -------- | ----------------------- | ------ |
| EMP-FR-001 | Hệ thống shall cho HR tạo NV trên EMP-SCR-002: định danh + org hiệu lực + HĐ; không tính lương                             | Must     | UC-001 · BR-006,010     | Test   |
| EMP-FR-002 | Hệ thống shall chặn lưu nếu trùng MNV hoặc CCCD                                                                            | Must     | UC-001,002 · BR-001,002 | Test   |
| EMP-FR-003 | Hệ thống shall chặn trùng email Cty hoặc MST **khi field có giá trị**; trống được nếu chưa cấp / chưa có MST               | Must     | UC-001 · BR-003,004     | Test   |
| EMP-FR-004 | Hệ thống shall chặn gắn đơn vị ngừng / không thuộc catalog hiệu lực                                                        | Must     | UC-001,002 · BR-005     | Test   |
| EMP-FR-005 | Hệ thống shall lưu HĐ (loại, ngày, TV/chính thức) là fact PAY đọc; **cảnh báo** nếu không có HĐ hiệu lực — cấm im lặng 85% | Must     | UC-001 · BR-006         | Test   |
| EMP-FR-006 | Hệ thống shall cho HR sửa SoT trên EMP-SCR-002; **cấm** đổi LM trên màn này (phải EMP-SCR-005/006)                         | Must     | UC-002 · BR-008,010     | Test   |
| EMP-FR-007 | Hệ thống shall cho NV xem/sửa field được phép trên EMP-SCR-003 và 004; **cùng** validation; field HR-only read-only / 403  | Must     | UC-003 · BR-009         | Test   |
| EMP-FR-008 | Hệ thống shall đổi LM **chỉ sau** duyệt một bậc (EMP-SCR-005→006); cấm ghi im lặng                                         | Must     | UC-004 · BR-008         | Test   |
| EMP-FR-009 | Hệ thống shall **cấm** gán quyền xem phiếu lương cho LM mới khi đổi LM/org                                                 | Must     | UC-004 · BR-011         | Test   |
| EMP-FR-010 | Hệ thống shall tính/hiển thị thâm niên theo **master**; cấm hardcode năm luật                                              | Must     | UC-005 · BR-007         | Test   |
| EMP-FR-011 | Hệ thống shall 403 nếu NV tạo/sửa hồ sơ người khác hoặc tự đổi LM (MVP)                                                    | Must     | UC-001…004 · BR-010     | Test   |
| EMP-FR-012 | Hệ thống shall ẩn EMP-SCR-001/002/005 với NV thường (trừ IAM); NV vào 003/004                                              | Must     | DOC-19                  | Test   |
| EMP-FR-013 | Hệ thống shall đủ luồng EMP-SCR-001…006; pixel HTML **không** Must                                                         | Must     | DOC-19                  | Test   |
| EMP-FR-014 | Hệ thống shall lấy danh mục field/HĐ từ master; cấm list cứng trên FR                                                      | Must     | BR-012                  | Test   |
| EMP-FR-015 | Hệ thống shall 403 LM mở phiếu lương cấp dưới từ luồng EMP                                                                 | Must     | BR-011 · PAY-BR-007     | Test   |
| EMP-FR-016 | Hệ thống shall từ chối duyệt đổi LM nếu org LM mới không hiệu lực                                                          | Must     | UC-004 · BR-005         | Test   |
| EMP-FR-017 | Hệ thống shall có **trình độ học vấn** trên hồ sơ (EMP-SCR-002 bắt buộc có field; SCR-003/004 hiển thị). Giá trị chọn từ **master**. HR tạo/sửa. NV sửa chỉ khi IAM/master cho phép. Cấm hardcode danh sách bậc học trên code URD | Must     | UC-001…003 · BR-010,012 | Test   |




### EMP-FR-002 — Unique định danh (chi tiết)


| Mục                | Nội dung                                                                                       |
| ------------------ | ---------------------------------------------------------------------------------------------- |
| **Description**    | MNV và CCCD unique bắt buộc. Email Cty / MST unique khi có giá trị.                            |
| **Inputs**         | MNV, CCCD, email Cty, MST                                                                      |
| **Processing**     | IF trùng NV khác THEN reject. IF email/MST trống THEN OK. Tái tuyển CCCD = master (không bịa). |
| **Outputs**        | Lưu hoặc lỗi unique                                                                            |
| **Error handling** | UC-001 EF-1, EF-2                                                                              |




### EMP-FR-008 — Đổi LM có duyệt (chi tiết)


| Mục                | Nội dung                                                        |
| ------------------ | --------------------------------------------------------------- |
| **Description**    | LM mới chỉ sau EMP-SCR-006 duyệt. Không widget LM trên SCR-002. |
| **Preconditions**  | Role khởi tạo = HR/C&B (MVP)                                    |
| **Postconditions** | LM ghi; **không** quyền phiếu lương                             |
| **Error handling** | UC-004 EF-1…5                                                   |

### EMP-FR-017 — Trình độ học vấn (chi tiết)

| Mục | Nội dung |
|-----|----------|
| **Description** | Field học vấn là Must trên hồ sơ (có chỗ lưu + UI). **Không** đóng list bậc trên SRS. |
| **Inputs** | Mã/giá trị từ catalog master |
| **Processing** | IF giá trị không thuộc master đang hiệu lực THEN reject. IF thiếu field trên form HR THEN fail FR. Trống được trừ khi master đánh bắt buộc. |
| **Outputs** | Học vấn trên hồ sơ SoT |
| **Error handling** | Mã ngừng hiệu lực = chặn lưu |




## 4. Yêu cầu giao diện bên ngoài



### 4.1 UI

Bắt buộc EMP-SCR-001…006. Pixel MCP **không** Must.  
EMP-SCR-002: control **Trình độ học vấn** (dropdown master). EMP-SCR-003/004: cùng field (read-only hoặc sửa theo IAM).

### 4.2 Phần cứng

Không.

### 4.3 Phần mềm

IAM · master org/field · PAY đọc HĐ, không nhận quyền lương từ EMP. API → DOC-12.

### 4.4 Truyền thông

Không webhook CRM bán hàng.

## 5. NFR tóm tắt

→ **DOC-13** (chưa slice).


| NFR ID     | Category | Tóm tắt                                 |
| ---------- | -------- | --------------------------------------- |
| *(DOC-13)* | Security | 403 hồ sơ người khác; không mở lương    |
| *(DOC-13)* | Audit    | Log tạo/sửa hồ sơ; đề xuất/duyệt đổi LM |




## 6. Ma trận truy vết (tóm tắt)


| FR ID      | UC      | BR      | AC         | Test |
| ---------- | ------- | ------- | ---------- | ---- |
| EMP-FR-001 | 001     | 006,010 | *(DOC-07)* |      |
| EMP-FR-002 | 001,002 | 001,002 |            |      |
| EMP-FR-003 | 001     | 003,004 |            |      |
| EMP-FR-004 | 001,002 | 005     |            |      |
| EMP-FR-005 | 001     | 006     |            |      |
| EMP-FR-006 | 002     | 008,010 |            |      |
| EMP-FR-007 | 003     | 009     |            |      |
| EMP-FR-008 | 004     | 008     |            |      |
| EMP-FR-009 | 004     | 011     |            |      |
| EMP-FR-010 | 005     | 007     |            |      |
| EMP-FR-011 | 001…004 | 010     |            |      |
| EMP-FR-012 | —       | —       |            |      |
| EMP-FR-013 | —       | —       |            |      |
| EMP-FR-014 | —       | 012     |            |      |
| EMP-FR-015 | 004     | 011     |            |      |
| EMP-FR-016 | 004     | 005     |            |      |
| EMP-FR-017 | 001…003 | 010,012 |            |      |




### 6.1 BRQ (DOC-03) → FR employee-profile

SRS **chỉ** module hồ sơ.


| BRQ     | EMP-FR            | Kết luận                           |
| ------- | ----------------- | ---------------------------------- |
| BRQ-001 | 001…006, 011, 014, **017** | Một phần (hồ sơ, gồm học vấn)      |
| BRQ-002 | —                 | Không (payroll)                    |
| BRQ-003 | 005               | Một phần (HĐ cho 85%)              |
| BRQ-004 | —                 | Không (TIM)                        |
| BRQ-005 | —                 | Không (LIF)                        |
| BRQ-006 | 007, 012          | Có (hồ sơ web+mobile; không phiếu) |
| BRQ-007 | —                 | Không (PC)                         |
| BRQ-008 | —                 | Không (leave)                      |
| BRQ-009 | —                 | Không (UAT lương)                  |
| BRQ-010 | —                 | Không (leave)                      |




## 7. Phê duyệt


| Vai trò         | Họ tên           | Ngày       | Baseline    |
| --------------- | ---------------- | ---------- | ----------- |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-033) · ☐ `02-baseline/` |
| BA (R)          | Trịnh Yên        | 2026-08-25 | Soạn → PGD chốt  |
| Business Owner  | Ban HR           |            | ☐ Nợ        |


