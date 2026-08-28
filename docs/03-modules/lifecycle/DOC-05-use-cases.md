# DOC-05 — Kịch bản sử dụng — Lifecycle (LIF)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (UC lifecycle · DEC-REQ-027) |

**Module:** lifecycle · **MOD:** LIF · **Tiên quyết:** DOC-04 **Chốt** (DEC-REQ-024).  
**Phạm vi UC:** BRQ-005 · BR-006 · A-003 · LIF-BR-001…012.  
**Không:** ATS; notify CRM **bán hàng**; khóa lương/PAY; e-sign HĐLĐ; list tick cứng.  
**Cổng:** DOC-05 **đã chốt** (PGD · DEC-REQ-027). Nợ: tick list master; **N+3 = 3 ngày lịch** (nháp); khung DOC-19; DOC-06/07; Ban HR ☐. **Chưa** `02-baseline/`. FR ID = sau DOC-06. CRM SP khóa in scope; notify sales ngoài. **Không** tự khung 19 / SRS.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| LIF-ACT-001 | Nhân viên | Đối tượng on/off; **không** tự xác nhận N đủ để chạy N+3 | Secondary |
| LIF-ACT-002 | HR / C&B | Checklist on/off; xác nhận ngày LV cuối (N) | Primary |
| LIF-ACT-003 | IT / IAM | Cấp tài khoản lúc on; job/thực thi khóa Git & CRM SP | Primary |
| LIF-ACT-004 | Hệ thống | Job N+3; chặn khóa sớm; không bắn CRM sales | System |
| LIF-ACT-005 | Git / CRM sản phẩm / chat | Hệ thống đích cấp/khóa | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| LIF-UC-001 | Onboarding: checklist + cấp tài khoản | LIF-ACT-002 / 003 | Must | LIF-BR-005, 007, 012 |
| LIF-UC-002 | Xác nhận ngày LV cuối (N) | LIF-ACT-002 | Must | LIF-BR-001, 009 |
| LIF-UC-003 | Khóa Git + CRM SP tại N+3 | LIF-ACT-003 / 004 | Must | LIF-BR-002, 003, 004, 008 |
| LIF-UC-004 | Offboarding checklist | LIF-ACT-002 | Must | LIF-BR-006, 012 |
| LIF-UC-005 | Khóa chat / ngoại lệ an ninh | LIF-ACT-003 | Should / Must khi có CR | LIF-BR-011, 004, 010 |

## 3. Sơ đồ use case

```text
[HR+IT] ──► (LIF-UC-001 On) checklist + cấp email/Git/CRM SP/chat
[HR]    ──► (LIF-UC-002 Xác nhận N) N = ngày LV cuối ──x ngày ký đơn
[Job]   ──► (LIF-UC-003 N+3) khóa Git + CRM SP ──x trước N+3 ──x HR SSH
[HR]    ──► (LIF-UC-004 Off checklist) mục = master
[IT]    ──► (LIF-UC-005 Chat / CR an ninh)
[LIF]   ──x notify CRM bán hàng
```

## 4. Đặc tả use case (Fully Dressed)

### LIF-UC-001 — Onboarding: checklist + cấp tài khoản

| Mục | Nội dung |
|-----|----------|
| **ID** | LIF-UC-001 |
| **Actor chính** | LIF-ACT-002 |
| **Actor phụ** | LIF-ACT-003, LIF-ACT-005 |
| **Mục tiêu** | Hoàn tất on: checklist động; **cấp** email @Cty, Git, CRM SP, chat — **không** đợi N+3 |
| **Preconditions** | Hồ sơ EMP đã tạo (hoặc song song); role HR + IT theo IAM |
| **Postconditions (success)** | Checklist on đủ theo master; tài khoản đã cấp; N+3 **không** chạy |
| **Trigger** | NV mới onboard |
| **Frequency** | Mỗi onboard |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Mở hồ sơ on; tick checklist (mục = master, không list cứng) |
| 2 | IT / IAM | Cấp email, Git, CRM **sản phẩm**, chat |
| 3 | Hệ thống | Ghi trạng thái đã cấp; email unique khi có giá trị (EMP-BR-003) |
| 4 | HR | Đóng on **chỉ khi** checklist bắt buộc (master) đủ |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Đóng on khi thiếu tick Must | Chặn | LIF-BR-005 |
| EF-2 | Trì hoãn cấp Git đến N+3 | Cấm | LIF-BR-007 |
| EF-3 | NV tự cấp Git | 403 | LIF-BR-008 |
| EF-4 | List tick cứng trên UC | Cấm | LIF-BR-012 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LIF-BR-005 | 1, 4, EF-1 |
| LIF-BR-007 | 2, EF-2 |
| LIF-BR-012 | 1, EF-4 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### LIF-UC-002 — Xác nhận ngày LV cuối (N)

| Mục | Nội dung |
|-----|----------|
| **ID** | LIF-UC-002 |
| **Actor chính** | LIF-ACT-002 |
| **Mục tiêu** | HR xác nhận **N = ngày làm việc cuối** — không dùng ngày ký đơn nghỉ |
| **Preconditions** | Hồ sơ off đã mở |
| **Postconditions (success)** | N đã xác nhận → đủ điều kiện đếm N+3; NV tự nhập **không** kích job |
| **Trigger** | Quyết định nghỉ việc / offboarding |
| **Frequency** | Mỗi off |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Nhập/xác nhận ngày LV cuối |
| 2 | Hệ thống | Lưu N; **không** lấy ngày ký đơn làm N |
| 3 | Hệ thống | Lên lịch job khóa tại N+3 (xem UC-003) |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Lấy ngày ký = N | Cấm | LIF-BR-001 |
| EF-2 | NV tự set N | Không kích N+3 | LIF-BR-009 |
| EF-3 | Chưa xác nhận N | Job UC-003 không chạy | LIF-BR-002 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LIF-BR-001 | 1–2, EF-1 |
| LIF-BR-009 | 1, EF-2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### LIF-UC-003 — Khóa Git + CRM sản phẩm tại N+3

| Mục | Nội dung |
|-----|----------|
| **ID** | LIF-UC-003 |
| **Actor chính** | LIF-ACT-004 (job) / LIF-ACT-003 |
| **Mục tiêu** | Tại **N+3** (nháp: **3 ngày lịch** sau N): khóa Git và CRM **sản phẩm**. Cùng mốc. Không khóa trước. HR không SSH |
| **Preconditions** | UC-002 đã xác nhận N |
| **Postconditions (success)** | Git + CRM SP khóa; không webhook sales |
| **Trigger** | Ngày hệ thống ≥ N+3 |
| **Frequency** | Tự động mỗi off đã có N |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | Hệ thống | Kiểm ngày ≥ N + **3 ngày lịch** (nợ: nếu phải là ngày công chuẩn → CR LIF-BR-002) |
| 2 | IT / job IAM | Khóa Git (repo Cty) |
| 3 | IT / job IAM | Khóa CRM sản phẩm **cùng mốc** |
| 4 | Hệ thống | Audit; **không** gửi sự kiện sang CRM bán hàng |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Khóa trước N+3 không CR/an ninh | Cấm / audit vi phạm | LIF-BR-004 |
| EF-2 | HR bấm khóa Git trực tiếp (không IT/IAM) | 403 hoặc chỉ ticket IT | LIF-BR-008 |
| EF-3 | Chỉ khóa Git, quên CRM SP | Cấm lệch mốc | LIF-BR-003 |
| EF-4 | Bắn notify pipeline sales / phép sang CRM sales | Cấm | LIF-BR-010 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LIF-BR-002 | 1–2 |
| LIF-BR-003 | 3, EF-3 |
| LIF-BR-004 | EF-1 |
| LIF-BR-008 | 2–3, EF-2 |
| LIF-BR-010 | 4, EF-4 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### LIF-UC-004 — Offboarding checklist

| Mục | Nội dung |
|-----|----------|
| **ID** | LIF-UC-004 |
| **Actor chính** | LIF-ACT-002 |
| **Mục tiêu** | Checklist off động; không đóng trạng thái off khi thiếu tick Must (master) |
| **Preconditions** | Hồ sơ off |
| **Postconditions (success)** | Off hoàn tất checklist; **không** thay N+3 (khóa Git/CRM = UC-003) |
| **Trigger** | Bắt đầu off |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR (và IT theo mục) | Tick checklist off (master) |
| 2 | Hệ thống | Cho đóng trạng thái off khi hết tick Must |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Đóng off thiếu tick Must | Chặn | LIF-BR-006 |
| EF-2 | List tick cứng | Cấm | LIF-BR-012 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LIF-BR-006 | 1–2, EF-1 |
| LIF-BR-012 | EF-2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### LIF-UC-005 — Khóa chat / ngoại lệ an ninh

| Mục | Nội dung |
|-----|----------|
| **ID** | LIF-UC-005 |
| **Actor chính** | LIF-ACT-003 |
| **Mục tiêu** | Chat: cấp lúc on (UC-001); **mốc khóa chat = master/checklist** — có thể ≠ N+3. Khóa Git/CRM **trước** N+3 chỉ khi CR/an ninh |
| **Preconditions** | Quy chế chat trên master; hoặc CR bảo mật |
| **Postconditions (success)** | Chat khóa đúng mốc master; sự cố an ninh ghi CR |
| **Trigger** | Tick checklist / CR |
| **Priority** | Should trên catalog; Must nếu quy chế yêu cầu chat trên checklist |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | IT | Khóa chat theo mốc master (không mặc định = Git nếu quy chế khác) |
| 2 | IT | (Ngoại lệ) Khóa Git/CRM trước N+3 **chỉ** khi CR/an ninh — ghi audit |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Coi khóa chat = N+3 Git khi master khác | Cấm im lặng | LIF-BR-011 |
| EF-2 | Khóa sớm không CR | Vi phạm | LIF-BR-004 |
| EF-3 | Notify CRM sales | Cấm | LIF-BR-010 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LIF-BR-011 | 1, EF-1 |
| LIF-BR-004 | 2, EF-2 |
| LIF-BR-010 | EF-3 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

## 5. Tóm tắt

| UC ID | Actor | Mô tả 1 câu |
|-------|-------|-------------|
| LIF-UC-001 | HR+IT | Checklist on + cấp email/Git/CRM SP/chat ngay. |
| LIF-UC-002 | HR | N = ngày LV cuối; NV không kích N+3. |
| LIF-UC-003 | Job/IT | N+3 (ngày lịch nháp) khóa Git + CRM SP; HR không SSH. |
| LIF-UC-004 | HR | Checklist off động. |
| LIF-UC-005 | IT | Chat theo master; khóa sớm chỉ CR/an ninh. |

UC-001…005 đủ dressed. LIF-BR-001…012 có UC. Không UC notify sales.

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-027) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
