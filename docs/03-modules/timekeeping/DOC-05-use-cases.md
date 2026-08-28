# DOC-05 — Kịch bản sử dụng — Timekeeping (TIM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (UC timekeeping · DEC-REQ-018) |

**Module:** timekeeping · **MOD:** TIM · **Tiên quyết:** DOC-04 **Chốt** (DEC-REQ-017).  
**Phạm vi UC:** BRQ-004 · TIM-BR-001…012.  
**Không:** máy CC; sửa công trên PAY; C1–C2 phép; UC xem công NV (không Must); list cột Excel cứng.  
**Cổng:** DOC-05 **đã chốt** (PGD · DEC-REQ-018). Nợ: DOC-19 khung; DOC-06/07; Ban HR ☐. **Chưa** `02-baseline/`. FR ID = sau DOC-06.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| TIM-ACT-001 | Nhân viên | Không import/chốt — **không UC riêng** (TIM-BR-012) | Secondary |
| TIM-ACT-002 | Line Manager | Không import/chốt — **không UC riêng** | Secondary |
| TIM-ACT-003 | HR / C&B | Công bố mẫu; import; preview; chốt / bỏ chốt tháng | Primary |
| TIM-ACT-004 | IT Admin | Hỗ trợ kỹ thuật mẫu; không chốt công trừ IAM HR — **không UC riêng** (thuộc UC-001) | Secondary |
| TIM-ACT-005 | Hệ thống | Kiểm version, preview lỗi, merge phép Đã duyệt, N_thực | System |
| TIM-ACT-006 | LEV | Cung cấp ngày phép **Đã duyệt** | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| TIM-UC-001 | Công bố version mẫu Excel | TIM-ACT-003 | Must | TIM-BR-001, 002 · BRQ-004 |
| TIM-UC-002 | Import + preview | TIM-ACT-003 | Must | TIM-BR-003, 004, 009, 010, 012 |
| TIM-UC-003 | Ghi bảng công (commit) | TIM-ACT-003 | Must | TIM-BR-004 |
| TIM-UC-004 | Chốt tháng công | TIM-ACT-003 | Must | TIM-BR-005, 006, 007, 008 |
| TIM-UC-005 | Bỏ chốt và import lại | TIM-ACT-003 | Must | TIM-BR-011 |

## 3. Sơ đồ use case

```text
[HR] ──► (TIM-UC-001 Công bố mẫu)     1 version / thời điểm
[HR] ──► (TIM-UC-002 Import + preview) ──x sai version / còn lỗi Must
[HR] ──► (TIM-UC-003 Commit)           hết lỗi Must
[HR] ──► (TIM-UC-004 Chốt tháng) ──► PAY được đọc
[HR] ──► (TIM-UC-005 Bỏ chốt) ──► import lại
[NV/LM] ──x import / chốt (403)
```

## 4. Đặc tả use case (Fully Dressed)

### TIM-UC-001 — Công bố version mẫu Excel

| Mục | Nội dung |
|-----|----------|
| **ID** | TIM-UC-001 |
| **Actor chính** | TIM-ACT-003 |
| **Actor phụ** | TIM-ACT-004 (kỹ thuật file) |
| **Mục tiêu** | Đúng **một** template hiệu lực toàn Cty; cột theo master quy chế |
| **Preconditions** | Role HR/C&B |
| **Postconditions (success)** | Version mới = mẫu đang dùng; version cũ từ chối import |
| **Trigger** | HR/IT cập nhật mẫu theo quy chế |
| **Frequency** | Khi đổi quy chế / cột |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Tải / khai báo mẫu + mã version (cột = master, không list cứng trên UC) |
| 2 | Hệ thống | Đặt version này = hiệu lực; vô hiệu version cũ với import mới |
| 3 | Hệ thống | File đang preview chưa commit: không tự commit theo mẫu mới |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Hai mẫu song song cùng hiệu lực | Cấm | TIM-BR-001 |
| EF-2 | NV/LM công bố mẫu | 403 | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| TIM-BR-001, 002 | 1–2 |
| TIM-BR-010 | EF-2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### TIM-UC-002 — Import + preview

| Mục | Nội dung |
|-----|----------|
| **ID** | TIM-UC-002 |
| **Actor chính** | TIM-ACT-003 |
| **Mục tiêu** | Upload Excel đúng mẫu; xem dòng OK / lỗi; chưa ghi sổ |
| **Preconditions** | Có version hiệu lực; tháng chưa bắt buộc chốt |
| **Postconditions (success)** | Preview trên màn; chưa commit |
| **Postconditions (failure)** | Không preview như đã ghi công |
| **Trigger** | HR chọn file |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Chọn tháng + file Excel |
| 2 | Hệ thống | Kiểm khớp version mẫu đang hiệu lực |
| 3 | Hệ thống | Preview từng dòng (thiếu NV, sai OT, lệch cột) |
| 4 | HR | Xem danh sách lỗi Must |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Sai version | Chặn step 2 | Không preview commit |
| EF-2 | File không phải Excel mẫu (máy CC raw / API) | Cấm | TIM-BR-009 |
| EF-3 | NV/LM upload | 403 | TIM-BR-012 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| TIM-BR-003 | 2 |
| TIM-BR-004 | 3–4 |
| TIM-BR-009 | EF-2 |
| TIM-BR-010, 012 | EF-3 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### TIM-UC-003 — Ghi bảng công (commit)

| Mục | Nội dung |
|-----|----------|
| **ID** | TIM-UC-003 |
| **Actor chính** | TIM-ACT-003 |
| **Mục tiêu** | Lưu bảng công Draft tháng sau preview sạch lỗi Must |
| **Preconditions** | Preview UC-002 còn trên session; hết lỗi Must |
| **Postconditions (success)** | Bảng công ghi; **chưa** chốt |
| **Trigger** | HR bấm Ghi |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Xác nhận ghi |
| 2 | Hệ thống | Commit nếu hết lỗi Must |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Còn lỗi Must | Chặn | TIM-BR-004 |
| EF-2 | NV/LM ghi | 403 | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| TIM-BR-004 | 2, EF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### TIM-UC-004 — Chốt tháng công

| Mục | Nội dung |
|-----|----------|
| **ID** | TIM-UC-004 |
| **Actor chính** | TIM-ACT-003 |
| **Actor phụ** | TIM-ACT-006 |
| **Mục tiêu** | Khóa tháng: OT + phép Đã duyệt + N_thực gồm phép hưởng → PAY được đọc |
| **Preconditions** | Đã commit UC-003; không còn lỗi Must |
| **Postconditions (success)** | Tháng Chốt; PAY-FR-001 được phép tính |
| **Trigger** | HR bấm Chốt tháng |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | Hệ thống | Merge ngày LEV **Đã duyệt** (không đơn chờ) |
| 2 | Hệ thống | N_thực gồm phép hưởng; OT có loại 1.5/2.0/3.0 |
| 3 | HR | Xác nhận chốt |
| 4 | Hệ thống | Đánh dấu chốt |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Dòng OT thiếu loại hệ số | Chặn chốt | TIM-BR-006 |
| EF-2 | Tách phép hưởng khỏi N_thực | Cấm im lặng | TIM-BR-008 |
| EF-3 | NV/LM chốt | 403 | |
| EF-4 | Chưa commit | Chặn | |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| TIM-BR-007 | 1 |
| TIM-BR-008 | 2, EF-2 |
| TIM-BR-006 | 2, EF-1 |
| TIM-BR-005 | 4 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### TIM-UC-005 — Bỏ chốt và import lại

| Mục | Nội dung |
|-----|----------|
| **ID** | TIM-UC-005 |
| **Actor chính** | TIM-ACT-003 |
| **Mục tiêu** | Sửa công: bỏ chốt TIM → import/preview/commit → chốt lại; không sửa trên PAY |
| **Preconditions** | Tháng đang chốt TIM; kỳ PAY **chưa** chốt |
| **Postconditions (success)** | Tháng Draft; PAY phải tính lại Draft |
| **Trigger** | HR bỏ chốt |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Bỏ chốt tháng |
| 2 | HR | Lặp UC-002 → 003 → 004 |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Kỳ lương PAY đã chốt | Cấm bỏ chốt TIM tự động mở lương | TIM-BR-011 |
| EF-2 | Sửa ô công trên màn PAY | Cấm | PAY |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| TIM-BR-011 | 1, EF-1 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

## 5. Tóm tắt

| UC ID | Actor | Mô tả 1 câu |
|-------|-------|-------------|
| TIM-UC-001 | HR | Một mẫu hiệu lực; cột master. |
| TIM-UC-002 | HR | Import đúng version; preview lỗi. |
| TIM-UC-003 | HR | Commit khi hết lỗi Must. |
| TIM-UC-004 | HR | Chốt: OT + phép Đã duyệt + N_thực gồm phép hưởng. |
| TIM-UC-005 | HR | Bỏ chốt rồi import lại; không đụng kỳ PAY đã chốt. |

UC-001…005 đủ dressed. Không UC xem công NV.

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-018) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
