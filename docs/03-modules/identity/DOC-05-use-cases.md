# DOC-05 — Kịch bản sử dụng — Identity (IAM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (UC identity · DEC-REQ-043) |

**Module:** identity · **MOD:** IAM · **Tiên quyết:** DOC-04 **Chốt** v0.2 (DEC-REQ-041).  
**Phạm vi UC:** IAM-BR-001…017 · NFR-002…006 · BRQ-006.  
**Không:** SSO/MFA (DOC-08); PRB/EVT/RPT; đóng list permission; **khóa Git/CRM** (LIF-UC-003). Ma trận §4 đầy đủ → DOC-06 / NFR-004 (không nhét “mọi API” vào một UC).  
**Cổng:** DOC-05 **đã chốt** (PGD · DEC-REQ-043). Nợ: DOC-19; DOC-06/07; Ban HR ☐. **Chưa** `02-baseline/`. FR = sau DOC-06. **Không** tự khung 19 / SRS.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| IAM-ACT-001 | Nhân viên (IAM-ROLE-NV) | Đăng nhập; dữ liệu mình | Primary |
| IAM-ACT-002 | Line Manager | C1 phép cấp dưới; **không** lương cấp dưới | Primary |
| IAM-ACT-003 | HR / C&B | SoT nghiệp vụ; gán role (cùng IT) | Primary |
| IAM-ACT-004 | IT Admin | Gán role; **disable login** HRM | Primary |
| IAM-ACT-005 | PGD (app) | Đăng nhập; **không** mặc định lương Cty | Secondary |
| IAM-ACT-006 | Hệ thống | 401/403; hợp quyền; map MNV; **không** bắn CRM sales | System |
| IAM-ACT-007 | LIF | Có thể **gọi** disable login; khóa Git/CRM **không** thuộc UC này | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| IAM-UC-001 | Đăng nhập / hết phiên (web+mobile) | IAM-ACT-001…005 | Must | IAM-BR-001, 002, 017 |
| IAM-UC-002 | Gán / gỡ role | IAM-ACT-003 / 004 | Must | IAM-BR-003, 013, 014 |
| IAM-UC-003 | Negative: 403 lương & màn HR | IAM-ACT-002 / 001 | Must | IAM-BR-004…008, 011, 015, 016 |
| IAM-UC-004 | Vô hiệu **login** HRM | IAM-ACT-004 | Must | IAM-BR-010, 017 |
| IAM-UC-005 | Cấm identity sang CRM bán hàng | IAM-ACT-006 | Must | IAM-BR-012 |

## 3. Sơ đồ use case

```text
[User] ──► (IAM-UC-001 Login) web = mobile ──x hết phiên / vô hiệu → 401
[HR/IT] ──► (IAM-UC-002 Gán role) ──x NV/LM tự nâng
[LM/NV] ──x (IAM-UC-003) phiếu người khác / màn HR
[IT]    ──► (IAM-UC-004 Disable login) ──x khóa Git trên IAM
[LIF]   ──► (LIF-UC-003) khóa Git/CRM   (ngoài DOC-05 IAM)
[IAM]   ──x (IAM-UC-005) CRM sales
```

## 4. Đặc tả use case (Fully Dressed)

### IAM-UC-001 — Đăng nhập / hết phiên (web + mobile)

| Mục | Nội dung |
|-----|----------|
| **ID** | IAM-UC-001 |
| **Actor chính** | User đã có TK map MNV |
| **Mục tiêu** | Phiên hợp lệ hai kênh; hết phiên → 401 rồi login lại |
| **Preconditions** | TK hiệu lực; gắn 1 MNV EMP |
| **Postconditions (success)** | Phiên; **cùng** role mobile |
| **Trigger** | Mở HRM hoặc phiên hết |
| **Frequency** | Mỗi phiên |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | User | Đăng nhập (cơ chế = DOC-08) |
| 2 | Hệ thống | Kiểm TK hiệu lực + map MNV |
| 3 | Hệ thống | Cấp phiên; cùng role hai kênh |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Sai mật khẩu / chưa login | 401 | IAM-BR-001 |
| EF-2 | **Phiên hết hạn** | 401; user lặp step 1 | IAM-BR-001 |
| EF-3 | TK vô hiệu | 401 | IAM-BR-010 |
| EF-4 | Mobile khác role web | Cấm | IAM-BR-002 |
| EF-5 | HRM public không login | Cấm MVP | IAM-BR-001 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| IAM-BR-001, 002, 017 | 1–3, EF |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### IAM-UC-002 — Gán / gỡ role

| Mục | Nội dung |
|-----|----------|
| **ID** | IAM-UC-002 |
| **Actor chính** | IAM-ACT-003 hoặc 004 |
| **Mục tiêu** | Gán 5 role MVP; hợp quyền không lách lương |
| **Preconditions** | Caller = HR hoặc IT |
| **Postconditions (success)** | Role mới; LM không nhận permission lương |
| **Trigger** | HR/IT quản trị IAM |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR hoặc IT | Chọn NV + role |
| 2 | Hệ thống | Lưu; union permission |
| 3 | Hệ thống | Thiếu permission lương → 403 phiếu người khác |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | NV/LM gán role | 403 | IAM-BR-013 |
| EF-2 | Gán LM + quyền lương im lặng | Cấm | IAM-BR-004, 009, 014 |
| EF-3 | Role ngoài 5 không thuộc master | Chặn | IAM-BR-003 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| IAM-BR-003, 013, 014 | 1–3, EF |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### IAM-UC-003 — Negative: 403 lương và màn HR

| Mục | Nội dung |
|-----|----------|
| **ID** | IAM-UC-003 |
| **Actor chính** | IAM-ACT-002 (LM) / IAM-ACT-001 (NV) |
| **Mục tiêu** | Vài negative **người dùng** — không thay ma trận mọi API (DOC-06) |
| **Preconditions** | Đã UC-001 |
| **Trigger** | User mở đúng màn cấm |
| **Frequency** | UAT |

#### Luồng chính (đối chứng — được phép)

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | LM | Mở đơn phép **C1** cấp dưới |
| 2 | Hệ thống | 200; **không** mở phiếu lương |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | LM mở phiếu lương cấp dưới | 403 + audit nếu cố xem lương | IAM-BR-004, 011 |
| EF-2 | NV mở hồ sơ / phép / phiếu người khác | 403 | IAM-BR-005 |
| EF-3 | NV hoặc LM vào TIM chốt / PAY kỳ / EMP DS Cty | 403 | IAM-BR-008 |
| EF-4 | LM duyệt C2 / đột xuất | 403 | IAM-BR-016 |
| EF-5 | IT (không role HR) mở PAY | 403 | IAM-BR-007 |
| EF-6 | PGD mở phiếu Cty không HR | 403 | DOC-04 §1 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| IAM-BR-004…008, 011, 015, 016 | chính + EF |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### IAM-UC-004 — Vô hiệu login HRM

| Mục | Nội dung |
|-----|----------|
| **ID** | IAM-UC-004 |
| **Actor chính** | IAM-ACT-004 |
| **Actor phụ** | IAM-ACT-007 (LIF **gọi** disable login — tùy) |
| **Mục tiêu** | Chỉ **vô hiệu đăng nhập HRM**. Khóa Git/CRM = **LIF-UC-003**, không làm trên màn IAM |
| **Preconditions** | TK tồn tại; hồ sơ EMP không xóa |
| **Postconditions (success)** | Login UC-001 EF-3; Git/CRM **không** đổi trong UC này |
| **Trigger** | IT disable; hoặc LIF gọi API disable login |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | IT (hoặc LIF gọi) | Disable login |
| 2 | Hệ thống | TK vô hiệu |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | HR/IT bấm **khóa Git** trên IAM | 403 — làm LIF | IAM-BR-007 |
| EF-2 | NV tự disable | 403 | IAM-BR-013 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| IAM-BR-010, 017 | 1–2 |
| IAM-BR-007 | EF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### IAM-UC-005 — Cấm identity sang CRM bán hàng

| Mục | Nội dung |
|-----|----------|
| **ID** | IAM-UC-005 |
| **Actor chính** | IAM-ACT-006 |
| **Mục tiêu** | Không cấp token / federation sang CRM **sales** |
| **Trigger** | Tích hợp / webhook cố gắn IAM HRM → CRM sales |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | Hệ thống | Từ chối cấp identity sang CRM bán hàng |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Webhook phép/LIF kèm token sales | Cấm | IAM-BR-012 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| IAM-BR-012 | 1, EF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

## 5. Tóm tắt

| UC ID | Actor | Mô tả 1 câu |
|-------|-------|-------------|
| IAM-UC-001 | User | Login 2 kênh; hết phiên / vô hiệu → 401. |
| IAM-UC-002 | HR/IT | Gán role; không lách lương. |
| IAM-UC-003 | LM/NV | Negative 403 (lương, màn HR); C1 phép OK. |
| IAM-UC-004 | IT | Chỉ disable login; Git = LIF. |
| IAM-UC-005 | Hệ thống | Không CRM sales. |

IAM-BR-001…017 có UC (009 đổi LM nằm UC-002 EF-2). PRB/EVT/RPT không UC.

## 6. Nhật ký

| Phiên bản | Thay đổi |
|-----------|----------|
| 0.1 | Draft DEC-REQ-042 |
| 0.2 | Siết UC-003/004/005 + hết phiên vào UC-001; **chốt** DEC-REQ-043 |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.2 (DEC-REQ-043) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-26 | Điều chỉnh → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
