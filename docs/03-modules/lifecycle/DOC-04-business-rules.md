# DOC-04 — Quy tắc nghiệp vụ — Lifecycle (LIF)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (cổng BR · LIF · DEC-REQ-024) |

**Module:** lifecycle · **MOD:** LIF · **Phạm vi:** BRQ-005, BR-006, A-003, DOC-03 4.1 LIF / 4.3 Git-CRM.  
**Không:** ATS; thông báo phép sang CRM **bán hàng**; khóa lương/PAY; e-sign HĐLĐ NN; checklist cứng từng tick (động theo quy chế).

**Cổng:** DEC-REQ-024 **đã chốt** 2026-08-25. Nợ: tick list master; N+3 **ngày lịch** (nháp, chưa chốt vs ngày công); Ban HR ☐. **Chưa** `02-baseline/`. Mở: DOC-05 + khung 19. **Không** tự viết. CRM SP khóa N+3 in scope; notify sales vẫn ngoài.

---

## 1. Mục đích & phạm vi

On/off checklist; cấp tài khoản lúc on; **khóa Git và CRM sản phẩm tại N+3** (N = ngày LV cuối).

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| LIF-BR-001 | N = ngày LV cuối | N không phải ngày ký nghỉ việc | Inference | Must | A-003 | SH-002 |
| LIF-BR-002 | N+3 khóa Git | Khóa Git **sau N ba ngày** (lịch — TBD DOC-05: lịch hay ngày công) | Authorization | Must | BR-006 · BRQ-005 | SH-006 |
| LIF-BR-003 | N+3 khóa CRM SP | Khóa CRM **sản phẩm** cùng mốc N+3 | Authorization | Must | BR-006 | SH-006 |
| LIF-BR-004 | Không khóa sớm | Cấm khóa Git/CRM SP **trước** N+3 trừ CR/an ninh | Validation | Must | BRQ-005 | SH-006 |
| LIF-BR-005 | Onboarding checklist | Có checklist on; mục = master quy chế (động) | Validation | Must | DOC-03 4.1 | SH-002 |
| LIF-BR-006 | Offboarding checklist | Có checklist off; mục động | Validation | Must | DOC-03 4.1 | SH-002 |
| LIF-BR-007 | Cấp lúc on | Email @Cty, Git, CRM SP, chat: **cấp khi on** (không đợi N+3) | Inference | Must | DOC-03 4.3 | SH-006 |
| LIF-BR-008 | IT khóa | Chỉ IT/IAM được job khóa Git/CRM; HR không tự SSH | Authorization | Must | BRQ-005 SH-006 | SH-006 |
| LIF-BR-009 | HR SoT ngày N | HR nhập/xác nhận ngày LV cuối trên hồ sơ off | Authorization | Must | A-003 | SH-002 |
| LIF-BR-010 | Không notify CRM bán hàng | Off **không** bắn ticket phép sang CRM sales (DEC-DIS-001) | Constraint | Must | LEV | SH-002 |
| LIF-BR-011 | Chat | Cấp lúc on; khóa chat theo quy chế off (cùng checklist, mốc chi tiết DOC-05) | Inference | Should | 4.3 | SH-006 |
| LIF-BR-012 | Checklist động | Không đóng list tick trên BR | Inference | Must | DEC-DIS-014 | SH-002 |

## 3. Chi tiết quy tắc

### LIF-BR-001 — N

| Mục | Nội dung |
|-----|----------|
| **Statement** | N = **ngày làm việc cuối** đã xác nhận. Không dùng ngày ký đơn nghỉ làm N. |
| **Condition** | IF lấy ngày ký = N |
| **Action** | THEN sai N+3; cấm |
| **Source** | A-003 |
| **Trace** | BRQ-005 |

### LIF-BR-002 — Khóa Git N+3

| Mục | Nội dung |
|-----|----------|
| **Statement** | Quyền Git (repo Cty) bị khóa tại **N+3**. |
| **Condition** | IF ngày hệ thống ≥ N+3 AND off đã xác nhận N |
| **Action** | THEN khóa Git |
| **Exception** | N+3 = **3 ngày lịch** hay **3 ngày công chuẩn** = **TBD DOC-05** (BRD viết “ba ngày”; không bịa). Nháp: **ngày lịch** trừ khi anh chốt khác. |
| **Source** | BR-006 |
| **Trace** | SH-006 |

### LIF-BR-003 — Khóa CRM sản phẩm N+3

| Mục | Nội dung |
|-----|----------|
| **Statement** | Tài khoản CRM **sản phẩm** khóa cùng mốc Git (N+3). |
| **Condition** | IF N+3 |
| **Action** | THEN khóa CRM SP |
| **Exception** | Không đồng nghĩa webhook bán hàng / pipeline sales |
| **Source** | BR-006 · 4.3 |
| **Trace** | CRM PARKED chỉ **notify phép**; khóa quyền SP **in scope** |

### LIF-BR-004 — Không khóa trước N+3

| Mục | Nội dung |
|-----|----------|
| **Statement** | Job khóa Git/CRM SP không chạy trước N+3. |
| **Condition** | IF IT khóa tay trước N+3 không có CR/an ninh |
| **Action** | THEN vi phạm BR (audit) |
| **Exception** | Sự cố bảo mật = quy trình ngoài, ghi CR |
| **Source** | BRQ-005 |
| **Trace** | — |

### LIF-BR-005 / LIF-BR-006 — Checklist on / off

| Mục | Nội dung |
|-----|----------|
| **Statement** | On và off đều có checklist. Tên mục = master HR/IT, không list cứng. |
| **Condition** | IF đóng on/off không checklist |
| **Action** | THEN chặn hoàn tất trạng thái (chi tiết DOC-05) |
| **Source** | DOC-03 4.1 LIF |
| **Trace** | 014 |

### LIF-BR-007 — Cấp lúc on

| Mục | Nội dung |
|-----|----------|
| **Statement** | Email Cty, Git, CRM SP, chat được **cấp khi onboarding**, không chờ N+3. |
| **Condition** | IF trì hoãn cấp Git đến N+3 |
| **Action** | THEN sai (N+3 chỉ **khóa**) |
| **Source** | 4.3 |
| **Trace** | EMP email unique khi có |

### LIF-BR-008 — Ai khóa

| Mục | Nội dung |
|-----|----------|
| **Statement** | Thực thi khóa Git/CRM = IT (hoặc job IAM do IT). HR xác nhận N, không cầm credential Git. |
| **Condition** | IF HR bấm khóa Git trên LIF không qua IT/IAM |
| **Action** | THEN 403 hoặc chỉ tạo ticket IT |
| **Source** | BRQ-005 SH-006 |
| **Trace** | IAM |

### LIF-BR-009 — HR xác nhận N

| Mục | Nội dung |
|-----|----------|
| **Statement** | Ngày LV cuối do HR (C&B) xác nhận trên hồ sơ off. |
| **Condition** | IF NV tự set N |
| **Action** | THEN không đủ để kích N+3 |
| **Source** | A-003 |
| **Trace** | EMP |

### LIF-BR-010 — Không CRM bán hàng

| Mục | Nội dung |
|-----|----------|
| **Statement** | Luồng LIF không gửi sự kiện phép/off sang CRM **sales**. |
| **Source** | DEC-DIS-001 |
| **Trace** | LEV-BR thông báo |

### LIF-BR-011 — Chat

| Mục | Nội dung |
|-----|----------|
| **Statement** | Cấp chat lúc on. Mốc khóa chat = checklist/master (có thể ≠ N+3). Không bịa = Git nếu quy chế khác. |
| **Priority** | Should trên catalog; Must có mặt trên checklist nếu quy chế yêu cầu |
| **Source** | 4.3 |
| **Trace** | DOC-05 |

### LIF-BR-012 — Động

| Mục | Nội dung |
|-----|----------|
| **Statement** | Tool as-is / tick list không đóng trên BRD. |
| **Source** | CN-006 · A-005 |
| **Trace** | — |

## 4. Bảng quyết định — N+3

| Sự kiện | Git | CRM sản phẩm |
|---------|-----|----------------|
| On | Cấp | Cấp |
| Trước N+3 | Mở (trừ CR) | Mở |
| N+3 | Khóa | Khóa |

## 5. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | LIF-BR-001…012 | Distill BRQ-005 · BR-006 · A-003; TBD N+3 lịch vs ngày công | — |
| 0.1 | — | **Chốt cổng BR** (DEC-REQ-024) · nợ N+3 ngày lịch nháp | — |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-024) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
