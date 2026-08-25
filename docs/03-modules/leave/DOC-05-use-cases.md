# DOC-05 — Kịch bản sử dụng — Leave (LEV)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-24 | Trịnh Yên (BA) | **Chốt** (UC leave · DEC-REQ-009) |

**Tiên quyết:** DOC-04 **Chốt** · DOC-06/07 **Chốt**.  
**Cổng:** DOC-05 **đã chốt** (PGD). Nợ: OQ-REQ-010; ACT-004 không UC riêng (FR-017 ∈ UC-003); HTML MCP. **Chưa** `02-baseline/`.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| LEV-ACT-001 | Nhân viên | Tạo/xem/hủy đơn mình; xem quỹ; web + mobile | Primary |
| LEV-ACT-002 | Line Manager | C1 đơn cấp dưới | Primary |
| LEV-ACT-003 | HR / C&B | C2 chính thức; ngoại lệ đột xuất; catalog | Primary |
| LEV-ACT-004 | IT Admin | Cấu hình luồng; không C2 — **không UC riêng** (LEV-FR-017 / UC-003 EF-2) | Secondary |
| LEV-ACT-005 | Hệ thống thông báo | Email / App / HRM | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| LEV-UC-001 | Nộp đơn nghỉ phép | LEV-ACT-001 | Must | BRQ-008 · LEV-BR-001…008, 012, 013 |
| LEV-UC-002 | Duyệt / từ chối cấp 1 | LEV-ACT-002 | Must | LEV-BR-009, 014 |
| LEV-UC-003 | Duyệt / từ chối cấp 2 | LEV-ACT-003 | Must | LEV-BR-005, 006, 008, 010 |
| LEV-UC-004 | Hủy đơn trước C2 | LEV-ACT-001 | Must | LEV-BR-016, 013, 015 |
| LEV-UC-005 | Xem quỹ phép | LEV-ACT-001 | Must | LEV-BR-005, 012 |
| LEV-UC-006 | Cấu hình catalog / trần loại | LEV-ACT-003 | Should | LEV-BR-001 |

## 3. Sơ đồ use case

```text
[NV] ──► (LEV-UC-001 Nộp đơn)
              │ extend: đột xuất, ốm/mẫu Cty, chặn 3 NLĐ
              ▼ include
         (LEV-UC-005 Xem quỹ)
[NV] ──► (LEV-UC-004 Hủy trước C2)
[LM] ──► (LEV-UC-002 C1)
[HR] ──► (LEV-UC-003 C2) ──► trừ quỹ năm
[HR] ──► (LEV-UC-006 Catalog)
[Hệ thống] ◄── thông báo (LEV-BR-011)
```

## 4. Đặc tả use case (Fully Dressed)

### LEV-UC-001 — Nộp đơn nghỉ phép

| Mục | Nội dung |
|-----|----------|
| **ID** | LEV-UC-001 |
| **Actor chính** | LEV-ACT-001 |
| **Actor phụ** | LEV-ACT-005; LEV-ACT-002 (nhận C1) |
| **Mục tiêu** | Đăng ký nghỉ đúng loại, thời gian, bàn giao |
| **Preconditions** | NV đăng nhập; IAM cho phép tạo đơn mình |
| **Postconditions (success)** | Đơn Chờ C1; thông báo LM |
| **Postconditions (failure)** | Không lưu / không submit; quỹ không đổi |
| **Trigger** | NV mở form đơn (web hoặc mobile) |
| **Frequency** | Theo nhu cầu |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | NV | Chọn loại phép (6 loại) |
| 2 | NV | Chọn Từ–Đến; mỗi ngày Cả ngày / Sáng / Chiều |
| 3 | NV | Nhập lý do; chọn 1 người bàn giao active ≠ mình |
| 4 | Hệ thống | Kiểm tra overlap, quỹ năm (nếu loại trừ quỹ năm), trần loại nếu HR đã cấu hình |
| 5 | Hệ thống | Nếu ≥ 3 ngày công chuẩn liền: kiểm tra nộp trước ≥ 3 NLĐ |
| 6 | NV | Submit |
| 7 | Hệ thống | Lưu Chờ C1; gửi thông báo Email/App/HRM (không CRM bán hàng) |

#### Luồng thay thế

| ID | Điều kiện | Steps |
|----|-----------|-------|
| AF-1 | Loại ốm/BHXH | Bắt buộc file **đúng mẫu Cty** trước step 6 |
| AF-2 | Trễ hạn 3 NLĐ | NV đánh dấu Nghỉ đột xuất → vẫn submit (LEV-BR-008); không AF-2 thì EF-2 |
| AF-3 | Kênh mobile | Cùng validation web |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Overlap Open/Đã duyệt | Chặn step 6 | Thông báo ngày/buổi trùng |
| EF-2 | ≥ 3 ngày công liền, trễ 3 NLĐ, không cờ đột xuất | Chặn; gợi ý AF-2 | Không tạo đơn |
| EF-3 | Phép năm vượt quỹ | Chặn | Hiện quỹ còn |
| EF-4 | Thiếu bàn giao / NV nghỉ việc | Chặn | |
| EF-5 | ốm: không file hoặc sai mẫu | Chặn | |
| EF-6 | Trần loại đã cấu hình và vượt | Chặn | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LEV-BR-001, 002, 004, 005, 007, 008, 012, 013 | 1–6 |
| LEV-BR-003 | AF-1, EF-5 |
| LEV-BR-011 | 7 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| LEV-FR-001…009 | Nộp đơn + thông báo — DOC-06 |

---

### LEV-UC-002 — Duyệt / từ chối cấp 1

| Mục | Nội dung |
|-----|----------|
| **ID** | LEV-UC-002 |
| **Actor chính** | LEV-ACT-002 |
| **Mục tiêu** | C1 lịch phòng |
| **Preconditions** | Đơn Chờ C1; actor = LM của NV (không Matrix) |
| **Postconditions (success)** | Chờ C2 (duyệt) hoặc Từ chối |
| **Postconditions (failure)** | 403 nếu không phải LM |
| **Trigger** | Thông báo / inbox duyệt |
| **Frequency** | Mỗi đơn |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | LM | Mở đơn cấp dưới |
| 2 | LM | Phê duyệt |
| 3 | Hệ thống | Chờ C2; thông báo HR + NV; **không** trừ quỹ |

#### Luồng thay thế

| ID | Điều kiện | Steps |
|----|-----------|-------|
| AF-1 | Từ chối | Lý do bắt buộc → dừng; không C2; không trừ quỹ |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Không phải LM / tự C1 đơn mình | Ẩn nút | 403 |
| EF-2 | Đơn đột xuất | Vẫn C1; không đóng ngoại lệ | Quỹ chỉ sau C2 HR |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LEV-BR-009, 014, 008 | 1–3, EF-2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| LEV-FR-010, 018 | C1 |

---

### LEV-UC-003 — Duyệt / từ chối cấp 2 (HR)

| Mục | Nội dung |
|-----|----------|
| **ID** | LEV-UC-003 |
| **Actor chính** | LEV-ACT-003 |
| **Mục tiêu** | Duyệt chính thức; trừ quỹ năm nếu thuộc loại trừ quỹ |
| **Preconditions** | C1 đã duyệt |
| **Postconditions (success)** | Đã duyệt; quỹ năm đã trừ (nếu áp dụng) |
| **Trigger** | Hàng chờ HR |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Mở đơn Chờ C2 |
| 2 | Hệ thống | Kiểm quỹ năm / file ốm còn đúng mẫu |
| 3 | HR | Duyệt chính thức (đơn đột xuất: cùng thao tác = ngoại lệ + C2) |
| 4 | Hệ thống | Atomic: Đã duyệt + trừ quỹ năm nếu loại phép năm; thông báo |

#### Luồng thay thế

| ID | Điều kiện | Steps |
|----|-----------|-------|
| AF-1 | Từ chối + lý do | Không trừ quỹ |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | C1 chưa duyệt | Không C2 | Kể cả đột xuất |
| EF-2 | Manager bấm C2 | Từ chối | LEV-BR-014 |
| EF-3 | File ốm bị gỡ / sai mẫu | Không duyệt | |
| EF-4 | Quỹ năm không đủ lúc C2 | Không duyệt | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LEV-BR-005, 006, 008, 010, 014, 015 | 1–4 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| LEV-FR-008, 011, 012, 017 | C2 + file lúc duyệt |

---

### LEV-UC-004 — Hủy đơn trước C2

| Mục | Nội dung |
|-----|----------|
| **ID** | LEV-UC-004 |
| **Actor chính** | LEV-ACT-001 |
| **Mục tiêu** | Giải phóng ngày để nộp đơn khác |
| **Preconditions** | Đơn của mình; Chờ C1 hoặc Chờ C2 |
| **Postconditions (success)** | Đã hủy; ngày trống; được LEV-UC-001 lại cùng ngày |
| **Trigger** | NV hủy |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | NV | Hủy đơn chờ |
| 2 | Hệ thống | Đã hủy; thông báo LM/HR nếu đã C1 |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Đã duyệt C2 | Chặn | LEV-BR-015 |
| EF-2 | LM/HR hủy hộ | Chặn MVP | OQ-REQ-010 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| LEV-BR-016, 013, 015 | 1–2, EF-* |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| LEV-FR-013, 014 | Hủy |

---

### LEV-UC-005 — Xem quỹ phép

| Mục | Nội dung |
|-----|----------|
| **ID** | LEV-UC-005 |
| **Actor chính** | LEV-ACT-001 |
| **Mục tiêu** | Xem quỹ phép năm của mình |
| **Preconditions** | NV đăng nhập |
| **Postconditions (success)** | Hiển thị quỹ năm còn lại (web/mobile) |
| **Trigger** | LEV-SCR-007 |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | NV | Mở quỹ mình |
| 2 | Hệ thống | Hiện quỹ phép năm |

#### Luồng ngoại lệ

| ID | Điều kiện | Kết quả |
|----|-----------|---------|
| EF-1 | Xem quỹ người khác (không phải LM cấp dưới / HR) | Từ chối |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| LEV-FR-015 | DOC-06 · LEV-AC-015 |

---

### LEV-UC-006 — Cấu hình trần loại

| Mục | Nội dung |
|-----|----------|
| **ID** | LEV-UC-006 |
| **Actor chính** | LEV-ACT-003 |
| **Mục tiêu** | Trần ngày theo loại; trống = không trần |
| **Preconditions** | Role HR |
| **Postconditions (success)** | Catalog lưu; chặn nộp chỉ khi đã có số |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Mở catalog loại phép |
| 2 | HR | Để trống hoặc nhập trần |
| 3 | Hệ thống | Lưu; trống → không chặn vì trần |

#### Luồng ngoại lệ

| ID | Điều kiện | Kết quả |
|----|-----------|---------|
| EF-1 | Đã có trần và đơn vượt | Chặn nộp (UC-001 EF-6) |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| LEV-FR-016 | Should · LEV-AC-016 |

## 5. Tóm tắt

UC-001…006 đủ dressed. Casual không còn là bản chính.

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-24 | **Chốt** v0.2 (DEC-REQ-009) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-24 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
