# DOC-05 — Kịch bản sử dụng — Employee profile (EMP)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (UC employee-profile · DEC-REQ-026) |

**Module:** employee-profile · **MOD:** EMP · **Tiên quyết:** DOC-04 **Chốt** (DEC-REQ-023).  
**Phạm vi UC:** URD-01 · BR-005 · BRQ-006 (hồ sơ) · EMP-BR-001…012.  
**Không:** ATS; phiếu lương (PAY); phép (LEV); N+3 Git/CRM (LIF); list field hồ sơ cứng.  
**Cổng:** DOC-05 **đã chốt** (PGD · DEC-REQ-026). Nợ: catalog field; bậc IAM đổi LM ngoài 1 bậc; khung DOC-19; DOC-06/07; Ban HR ☐. **Chưa** `02-baseline/`. FR ID = sau DOC-06. **Không** tự khung 19 / SRS.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| EMP-ACT-001 | Nhân viên | Xem/sửa field được phép trên hồ sơ mình (web + mobile) | Primary |
| EMP-ACT-002 | Line Manager | Không SoT hồ sơ Cty; không xem phiếu lương cấp dưới | Secondary |
| EMP-ACT-003 | HR / C&B | Tạo/sửa hồ sơ, org, HĐ, định danh; khởi tạo đổi LM | Primary |
| EMP-ACT-004 | Người duyệt đổi LM | Role IAM (MVP: HR C&B hoặc PGD — **một bậc**, không Matrix) | Primary |
| EMP-ACT-005 | Hệ thống | Unique, org hiệu lực, thâm niên theo master | System |
| EMP-ACT-006 | PAY | Đọc HĐ (hệ số TV); **không** nhận quyền lương từ đổi LM | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| EMP-UC-001 | Tạo hồ sơ NV + org + HĐ | EMP-ACT-003 | Must | EMP-BR-001…006, 010, 012 |
| EMP-UC-002 | HR sửa hồ sơ / định danh | EMP-ACT-003 | Must | EMP-BR-001…006, 010, 012 |
| EMP-UC-003 | Self-service hồ sơ mình (web+mobile) | EMP-ACT-001 | Must | EMP-BR-009, 012 |
| EMP-UC-004 | Đổi Line Manager (có duyệt) | EMP-ACT-003 | Must | EMP-BR-008, 011 |
| EMP-UC-005 | Xem thâm niên theo quy chế | EMP-ACT-001 / 003 | Must | EMP-BR-007 |

## 3. Sơ đồ use case

```text
[HR] ──► (EMP-UC-001 Tạo NV)     unique + org + HĐ
[HR] ──► (EMP-UC-002 Sửa hồ sơ) ──x trùng MNV/CCCD/email/MST
[NV] ──► (EMP-UC-003 Self-service) web = mobile ──x field HR-only
[HR] ──► (EMP-UC-004 Đổi LM) ──► [Duyệt 1 bậc] ──x ghi im lặng
                                 ──x mở quyền phiếu lương
[NV/HR] ──► (EMP-UC-005 Thâm niên) công thức = master
[NV] ──x tạo hồ sơ người khác (403)
```

## 4. Đặc tả use case (Fully Dressed)

### EMP-UC-001 — Tạo hồ sơ NV + org + HĐ

| Mục | Nội dung |
|-----|----------|
| **ID** | EMP-UC-001 |
| **Actor chính** | EMP-ACT-003 |
| **Actor phụ** | EMP-ACT-005, EMP-ACT-006 (đọc HĐ sau) |
| **Mục tiêu** | Tạo NV mới: MNV, CCCD, org hiệu lực, HĐ (loại/ngày/TV–chính thức); email/MST nếu đã có |
| **Preconditions** | Role HR/C&B (IAM); catalog org + field HĐ theo master |
| **Postconditions (success)** | Hồ sơ tồn tại; PAY đọc được HĐ khi kỳ lương; email trống được nếu chưa cấp (LIF on) |
| **Trigger** | HR tạo NV (onboard / nhập liệu) |
| **Frequency** | Mỗi NV mới |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Nhập định danh + org + HĐ; field khác = master (không list cứng trên UC) |
| 2 | Hệ thống | Kiểm unique MNV, CCCD; email/MST unique **khi có giá trị**; org đang hiệu lực |
| 3 | Hệ thống | Lưu hồ sơ + HĐ; không tính lương |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Trùng MNV / CCCD | Chặn step 2 | EMP-BR-001, 002 |
| EF-2 | Trùng email Cty hoặc MST khi đã nhập | Chặn | EMP-BR-003, 004 |
| EF-3 | Đơn vị ngừng / không tồn tại | Chặn | EMP-BR-005 |
| EF-4 | NV/LM tạo hồ sơ người khác | 403 | EMP-BR-010 |
| EF-5 | Không có HĐ hiệu lực | Cảnh báo (không im lặng 85%) | EMP-BR-006 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| EMP-BR-001…006 | 2–3, EF |
| EMP-BR-010 | EF-4 |
| EMP-BR-012 | 1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### EMP-UC-002 — HR sửa hồ sơ / định danh

| Mục | Nội dung |
|-----|----------|
| **ID** | EMP-UC-002 |
| **Actor chính** | EMP-ACT-003 |
| **Mục tiêu** | Sửa SoT hồ sơ Cty (định danh, org, HĐ) theo IAM |
| **Preconditions** | NV đã tồn tại; role HR/C&B |
| **Postconditions (success)** | Thay đổi ghi; unique/org vẫn đúng |
| **Trigger** | HR mở hồ sơ |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Sửa field được IAM (kể cả MNV/CCCD/HĐ) |
| 2 | Hệ thống | Lặp kiểm unique + org như UC-001 |
| 3 | Hệ thống | Lưu |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Trùng unique | Chặn | EMP-BR-001…004 |
| EF-2 | Tái tuyển cùng CCCD | MNV mới/cũ = master quy chế; không bịa trên UC | EMP-BR-002 |
| EF-3 | NV sửa field HR-only qua API | 403 | EMP-BR-009, 010 |
| EF-4 | Sửa LM trên màn này **không** qua UC-004 | Chặn | EMP-BR-008 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| EMP-BR-001…006, 010 | 2, EF |
| EMP-BR-008 | EF-4 |
| EMP-BR-012 | 1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### EMP-UC-003 — Self-service hồ sơ mình (web + mobile)

| Mục | Nội dung |
|-----|----------|
| **ID** | EMP-UC-003 |
| **Actor chính** | EMP-ACT-001 |
| **Mục tiêu** | NV xem và sửa **field được phép** trên hồ sơ mình; cùng rule hai kênh |
| **Preconditions** | Đã đăng nhập; IAM gán field được sửa (master, không list cứng) |
| **Postconditions (success)** | Field được phép cập nhật; field HR-only read-only |
| **Trigger** | NV mở Hồ sơ (web hoặc app) |
| **Frequency** | Theo nhu cầu |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | NV | Mở hồ sơ mình (web hoặc mobile) |
| 2 | Hệ thống | Hiển thị field; khóa field HR-only (MNV, CCCD, …) |
| 3 | NV | Sửa field được phép |
| 4 | Hệ thống | Validate giống hai kênh; lưu |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Sửa field HR-only | 403 / read-only | EMP-BR-009 |
| EF-2 | Mở hồ sơ NV khác | 403 | EMP-BR-010 |
| EF-3 | Rule web ≠ mobile | Cấm | EMP-BR-009 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| EMP-BR-009 | 1–4, EF |
| EMP-BR-012 | 2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### EMP-UC-004 — Đổi Line Manager (có duyệt)

| Mục | Nội dung |
|-----|----------|
| **ID** | EMP-UC-004 |
| **Actor chính** | EMP-ACT-003 |
| **Actor phụ** | EMP-ACT-004, EMP-ACT-002, EMP-ACT-006 |
| **Mục tiêu** | Đổi LM qua **luồng duyệt một bậc** (IAM). Cấm ghi im lặng. Đổi **không** mở quyền xem phiếu lương |
| **Preconditions** | NV có hồ sơ; role được khởi tạo đổi LM (MVP: HR/C&B) |
| **Postconditions (success)** | LM mới chỉ sau khi duyệt; PAY-BR-007 không bị phá |
| **Trigger** | HR (hoặc role IAM) đề xuất đổi LM |
| **Frequency** | Khi chuyển quản lý |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Chọn NV + LM mới (org vẫn hiệu lực) |
| 2 | Hệ thống | Tạo yêu cầu Đổi LM — **chưa** ghi LM |
| 3 | Người duyệt | Duyệt / từ chối (**một bậc**; không Matrix — cùng chốt leave) |
| 4 | Hệ thống | Nếu duyệt: ghi LM mới; **không** gán quyền phiếu lương cho LM mới |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Lưu LM không qua bước 2–3 | Chặn | EMP-BR-008 |
| EF-2 | Từ chối | Không đổi LM | — |
| EF-3 | Đổi LM kéo quyền xem lương cấp dưới | Cấm | EMP-BR-011 |
| EF-4 | NV tự đổi LM | 403 trừ IAM cho phép (MVP: không) | EMP-BR-008 |
| EF-5 | C1/C2 nhiều bậc | Ngoài MVP; CR | EMP-BR-008 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| EMP-BR-008 | 1–4, EF-1, EF-4 |
| EMP-BR-011 | 4, EF-3 |
| EMP-BR-005 | 1 (org) |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### EMP-UC-005 — Xem thâm niên theo quy chế

| Mục | Nội dung |
|-----|----------|
| **ID** | EMP-UC-005 |
| **Actor chính** | EMP-ACT-001 / EMP-ACT-003 |
| **Mục tiêu** | Hiển thị thâm niên theo **master quy chế**, không hardcode số năm trên UC |
| **Preconditions** | Có ngày mốc trên hồ sơ/HĐ theo master |
| **Postconditions (success)** | Giá trị thâm niên = công thức master |
| **Trigger** | Mở hồ sơ (kèm UC-002/003) |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | NV hoặc HR | Mở hồ sơ |
| 2 | Hệ thống | Tính thâm niên theo master |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Hardcode năm luật trên code | Cấm | EMP-BR-007 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| EMP-BR-007 | 2, EF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

## 5. Tóm tắt

| UC ID | Actor | Mô tả 1 câu |
|-------|-------|-------------|
| EMP-UC-001 | HR | Tạo NV: unique + org + HĐ; email/MST trống được khi chưa có. |
| EMP-UC-002 | HR | Sửa SoT; đổi LM không đi đường tắt. |
| EMP-UC-003 | NV | Web = mobile; field HR-only khóa. |
| EMP-UC-004 | HR + duyệt 1 bậc | Đổi LM có duyệt; không mở phiếu lương. |
| EMP-UC-005 | NV/HR | Thâm niên = master. |

UC-001…005 đủ dressed. EMP-BR-001…012 có UC.

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-026) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
