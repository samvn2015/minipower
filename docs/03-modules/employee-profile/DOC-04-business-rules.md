# DOC-04 — Quy tắc nghiệp vụ — Employee profile (EMP)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (cổng BR · EMP · DEC-REQ-023) |

**Module:** employee-profile · **MOD:** EMP · **Phạm vi:** URD-01, BR-005, BRQ-001 (phần hồ sơ), BRQ-006 (hồ sơ web+mobile).  
**Không:** ATS tuyển dụng; phiếu lương (PAY); phép (LEV); N+3 khóa Git/CRM (LIF); list field hồ sơ cứng (master quy chế).

**Cổng:** DEC-REQ-023 **đã chốt** 2026-08-25. Nợ: catalog field; Ban HR ☐. **Chưa** `02-baseline/`. Mở: DOC-05 + khung 19. **Không** tự viết.

---

## 1. Mục đích & phạm vi

SoT hồ sơ NV: định danh unique, org, HĐ, thâm niên, đổi LM có duyệt; NV xem/sửa hồ sơ mình trên web+mobile theo IAM.

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| EMP-BR-001 | Unique MNV | Mã NV không trùng trong Cty | Validation | Must | BR-005 | SH-002 |
| EMP-BR-002 | Unique CCCD | CCCD không trùng | Validation | Must | BR-005 | SH-002 |
| EMP-BR-003 | Unique email Cty | Email @Cty không trùng | Validation | Must | BR-005 | SH-002 |
| EMP-BR-004 | Unique MST | MST cá nhân không trùng (khi có) | Validation | Must | BR-005 | SH-002 |
| EMP-BR-005 | Org | NV thuộc đơn vị / cây org đang hiệu lực | Validation | Must | URD-01 | SH-002 |
| EMP-BR-006 | Hợp đồng | HĐ (loại, ngày, TV/chính thức) là nguồn PAY đọc hệ số TV | Inference | Must | PAY-BR-003 | SH-002 |
| EMP-BR-007 | Thâm niên | Tính theo quy chế (master), không hardcode công thức URD | Calculation | Must | URD-01 · 014 | SH-002 |
| EMP-BR-008 | Đổi LM | Đổi Line Manager **bắt buộc** luồng duyệt; cấm ghi im lặng | Authorization | Must | URD-01 | SH-005 |
| EMP-BR-009 | Self-service hồ sơ | NV xem/sửa field được phép trên **web + mobile** cùng rule | Authorization | Must | BRQ-006 | SH-004 |
| EMP-BR-010 | HR SoT | HR/C&B tạo/sửa hồ sơ toàn Cty theo IAM | Authorization | Must | IAM | SH-002 |
| EMP-BR-011 | LM không xem lương | Đổi LM / org **không** mở quyền xem phiếu lương | Authorization | Must | PAY-BR-007 | SH-002 |
| EMP-BR-012 | Field động | Danh mục field/HĐ = master quy chế; không đóng list trên BR | Inference | Must | DEC-DIS-014 | SH-002 |

## 3. Chi tiết quy tắc

### EMP-BR-001 — Unique MNV

| Mục | Nội dung |
|-----|----------|
| **Statement** | Mỗi NV một MNV duy nhất trong tenant Cty. |
| **Condition** | IF trùng MNV |
| **Action** | THEN chặn lưu |
| **Source** | BR-005 |
| **Trace** | BRQ-001 |

### EMP-BR-002 — Unique CCCD

| Mục | Nội dung |
|-----|----------|
| **Statement** | CCCD/CMND unique. |
| **Condition** | IF trùng CCCD NV khác (kể cả nghỉ việc — policy: **chặn** trừ CR) |
| **Action** | THEN chặn |
| **Exception** | Tái tuyển cùng người = cùng CCCD, MNV mới hoặc cũ = quy chế master, không bịa trên BR |
| **Source** | BR-005 |
| **Trace** | URD-01 |

### EMP-BR-003 — Unique email Cty

| Mục | Nội dung |
|-----|----------|
| **Statement** | Email công ty unique khi đã cấp. |
| **Condition** | IF trùng email Cty |
| **Action** | THEN chặn |
| **Exception** | Chưa cấp email (onboarding) = trống được, unique khi có giá trị |
| **Source** | BR-005 · DOC-03 4.3 |
| **Trace** | LIF cấp lúc on |

### EMP-BR-004 — Unique MST

| Mục | Nội dung |
|-----|----------|
| **Statement** | MST cá nhân unique khi có. |
| **Condition** | IF trùng MST |
| **Action** | THEN chặn |
| **Exception** | Trống được nếu chưa có MST |
| **Source** | BR-005 |
| **Trace** | — |

### EMP-BR-005 — Org

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV phải gắn đơn vị thuộc catalog org hiệu lực. |
| **Condition** | IF đơn vị ngừng / không tồn tại |
| **Action** | THEN chặn lưu / chuyển |
| **Source** | URD-01 |
| **Trace** | — |

### EMP-BR-006 — Hợp đồng

| Mục | Nội dung |
|-----|----------|
| **Statement** | Trạng thái HĐ (thử việc / chính thức, ngày) là fact cho PAY 85%. EMP không tính lương. |
| **Condition** | IF không có HĐ hiệu lực tại kỳ |
| **Action** | THEN PAY/HR xử lý theo quy chế (cảnh báo) — không im lặng 85% |
| **Source** | PAY-BR-003 |
| **Trace** | PAY |

### EMP-BR-007 — Thâm niên

| Mục | Nội dung |
|-----|----------|
| **Statement** | Thâm niên theo mốc quy chế (master), không hardcode số năm luật trên BR. |
| **Condition** | IF Dev hardcode |
| **Action** | THEN cấm |
| **Source** | DEC-DIS-014 |
| **Trace** | — |

### EMP-BR-008 — Đổi LM

| Mục | Nội dung |
|-----|----------|
| **Statement** | Đổi Line Manager phải qua **luồng duyệt** (bậc cụ thể = DOC-05 / IAM). Không HR ghi đè im lặng trừ role được IAM. |
| **Condition** | IF lưu LM mới không qua bước duyệt bắt buộc |
| **Action** | THEN chặn |
| **Exception** | Chi tiết C1/C2 đổi LM = DOC-05; MVP không Matrix (đã chốt leave) |
| **Source** | URD-01 |
| **Trace** | LEV-BR Matrix không |

### EMP-BR-009 — Hồ sơ mình web+mobile

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV xem và sửa **field được phép** trên hồ sơ mình; cùng validation hai kênh. |
| **Condition** | IF NV sửa field HR-only (MNV, CCCD, …) |
| **Action** | THEN 403 / read-only |
| **Source** | BRQ-006 |
| **Trace** | DOC-19 sau |

### EMP-BR-010 — HR SoT

| Mục | Nội dung |
|-----|----------|
| **Statement** | Chỉ HR/C&B (IAM) tạo NV, sửa định danh, gán org hàng loạt. |
| **Condition** | IF NV tạo hồ sơ người khác |
| **Action** | THEN 403 |
| **Source** | IAM |
| **Trace** | — |

### EMP-BR-011 — Không mở lương khi đổi org/LM

| Mục | Nội dung |
|-----|----------|
| **Statement** | LM mới **không** được xem phiếu lương cấp dưới. |
| **Condition** | IF đổi LM kéo theo quyền lương |
| **Action** | THEN cấm (PAY-BR-007 thắng) |
| **Source** | URD III |
| **Trace** | PAY |

### EMP-BR-012 — Field động

| Mục | Nội dung |
|-----|----------|
| **Statement** | DOC-04 **không** đóng danh sách field hồ sơ/HĐ. |
| **Source** | DEC-DIS-014 |
| **Trace** | D-003 một phần trên PAY |

## 4. Bảng quyết định — Unique

| Field | Trống | Trùng NV khác |
|-------|-------|----------------|
| MNV | Cấm | Cấm |
| CCCD | Cấm (MVP) | Cấm |
| Email Cty | Được (chưa cấp) | Cấm nếu có giá trị |
| MST | Được | Cấm nếu có giá trị |

## 5. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | EMP-BR-001…012 | Distill URD-01 + BR-005 + BRQ-006 hồ sơ | — |
| 0.1 | — | **Chốt cổng BR** (DEC-REQ-023) | — |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-023) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
