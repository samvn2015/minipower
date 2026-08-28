# DOC-05 — Kịch bản sử dụng — Probation (PRB)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (cổng UC · PRB · DEC-REQ-053) |

**Module:** probation · **MOD:** PRB · **Tiên quyết:** DOC-04 **Chốt** (DEC-REQ-051).  
**Phạm vi UC:** URD-05 · BO-005 (phần TV) · PRB-BR-001…012.  
**Không:** tính 85% lương (PAY); ATS; list tiêu chí phiếu cứng; SN/lễ (EVT); notify CRM **bán hàng**; e-sign HĐ.  
**Cổng UC đóng.** Sửa UC = CR. Nợ kèm chốt: T-15/T-7 = **ngày lịch**; không LM → task T-7 về HR; Ban HR ☐. **Chưa** `02-baseline/`. FR ID = sau DOC-06. **Không** tự DOC-19.

---

## 1. Danh mục tác nhân

| Actor ID | Tên | Mô tả | Loại |
|----------|-----|-------|------|
| PRB-ACT-001 | Nhân viên TV | Đối tượng theo dõi; **không** tự chốt kết quả TV | Secondary |
| PRB-ACT-002 | Line manager | Điền phiếu / **đề xuất** kết quả; **không** SoT chốt | Primary |
| PRB-ACT-003 | HR / C&B | Chốt Đạt / Gia hạn / Không đạt; xử lý thiếu mốc HĐ | Primary |
| PRB-ACT-004 | Hệ thống | Job T-15 / T-7; coverage 0 sót; kênh nhắc; không bắn CRM sales | System |
| PRB-ACT-005 | EMP / LIF / PAY | Hệ thống đích: HĐ, off, hệ số TV (đọc HĐ, không đọc PRB để tính lương) | System |

## 2. Danh sách use case

| UC ID | Tên | Actor chính | Priority | Trace |
|-------|-----|-------------|----------|-------|
| PRB-UC-001 | Cảnh báo T-15 | PRB-ACT-004 | Must | PRB-BR-001, 002, 008, 010, 011 |
| PRB-UC-002 | Task T-7 + phiếu đề xuất | PRB-ACT-002 / 004 | Must | PRB-BR-003, 008, 009, 011, 012 |
| PRB-UC-003 | HR chốt kết quả TV | PRB-ACT-003 | Must | PRB-BR-004…007, 009 |
| PRB-UC-004 | Thiếu mốc HĐ — không bịa | PRB-ACT-003 / 004 | Must | PRB-BR-001, 008 |

## 3. Sơ đồ use case

```text
[Job] ──► (PRB-UC-001 T-15) nhắc HRM + email/app ──x CRM sales
[Job] ──► (PRB-UC-002 T-7) giao task LM ──► phiếu động (master)
[LM]  ──► đề xuất ∈ {Đạt, Gia hạn, Không đạt} ──x tự chốt
[HR]  ──► (PRB-UC-003) chốt 3 mã
            ├─ Đạt     → EMP chính thức (PAY hết 85% theo HĐ)
            ├─ Gia hạn → kéo KT TV (master, không hardcode tháng)
            └─ Không đạt → mở off LIF (không xóa im lặng)
[HR/Job] ──► (PRB-UC-004) thiếu mốc → cảnh báo, không bịa ngày
```

## 4. Đặc tả use case (Fully Dressed)

### PRB-UC-001 — Cảnh báo T-15

| Mục | Nội dung |
|-----|----------|
| **ID** | PRB-UC-001 |
| **Actor chính** | PRB-ACT-004 |
| **Actor phụ** | PRB-ACT-003 (nhận nhắc); PRB-ACT-001 (đối tượng) |
| **Mục tiêu** | Nhắc **15 ngày lịch** trước ngày kết thúc TV — 0 sót NV TV đang hiệu lực |
| **Preconditions** | HĐ EMP có ngày KT TV; trạng thái đang TV |
| **Postconditions (success)** | Đã gửi nhắc HRM + email/app; **không** gửi CRM bán hàng |
| **Postconditions (failure)** | Thiếu mốc → UC-004; không im lặng coi như đã nhắc |
| **Trigger** | Ngày hệ thống = KT_TV − 15 ngày lịch |
| **Frequency** | Mỗi NV TV một lần / đợt (gia hạn → KT mới → lịch mới) |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | Hệ thống | Lấy KT_TV từ HĐ EMP (không tự bịa) |
| 2 | Hệ thống | Chọn NV đang TV, còn hiệu lực, đúng T-15 |
| 3 | Hệ thống | Gửi nhắc in-app HRM + email/app (không bịa vendor) |
| 4 | Hệ thống | Ghi đã nhắc T-15 (audit) |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Bỏ NV TV khỏi hàng | Fail BO-005 | PRB-BR-008 |
| EF-2 | Bắn CRM bán hàng | Cấm | PRB-BR-010 |
| EF-3 | Thiếu KT_TV | Không bịa; UC-004 | PRB-BR-001 |
| EF-4 | Đếm ngày công thay lịch | Ngoài phạm vi v0.1; CR | Nợ DEC-REQ-051 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PRB-BR-001 | 1, EF-3 |
| PRB-BR-002 | Trigger, 2 |
| PRB-BR-008 | 2, EF-1 |
| PRB-BR-010 | EF-2 |
| PRB-BR-011 | 3 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### PRB-UC-002 — Task T-7 + phiếu đề xuất

| Mục | Nội dung |
|-----|----------|
| **ID** | PRB-UC-002 |
| **Actor chính** | PRB-ACT-002 |
| **Actor phụ** | PRB-ACT-004 (giao task); PRB-ACT-001 |
| **Mục tiêu** | **7 ngày lịch** trước KT TV: có task đánh giá; LM điền phiếu **động** và đề xuất 1/3 mã |
| **Preconditions** | Có mốc KT_TV; NV đang TV |
| **Postconditions (success)** | Task đã giao; phiếu lưu đề xuất (chưa đổi HĐ) |
| **Postconditions (failure)** | LM không chốt được SoT; thiếu phiếu không chặn HR UC-003 |
| **Trigger** | Ngày hệ thống = KT_TV − 7 ngày lịch |
| **Frequency** | Mỗi đợt TV / mỗi lần gia hạn (KT mới) |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | Hệ thống | Tạo task đánh giá; gán **LM** (catalog IAM — không hardcode tên role ngoài NV/LM/HR) |
| 2 | Hệ thống | Nhắc kênh HRM + email/app; **không** CRM sales |
| 3 | LM | Mở phiếu; tiêu chí = **master** (không list cứng trên UC) |
| 4 | LM | Chọn đề xuất ∈ {Đạt, Gia hạn, Không đạt} và lưu |
| 5 | Hệ thống | Lưu đề xuất; **không** đổi HĐ / không mở LIF |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | LM bấm “chốt chính thức” | 403 | PRB-BR-009 |
| EF-2 | Lưu “đạt có điều kiện” không master | Chặn | PRB-BR-004 |
| EF-3 | List tiêu chí cứng trên màn | Cấm | PRB-BR-012 |
| EF-4 | Không có LM | Task về HR; HR vẫn UC-003 | IAM / nợ catalog |
| EF-5 | T-7 trễ / LM không điền | HR vẫn chốt được UC-003 | PRB-BR-009 |
| EF-6 | Bỏ NV khỏi hàng T-7 | Fail BO-005 | PRB-BR-008 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PRB-BR-003 | Trigger, 1 |
| PRB-BR-004 | 4, EF-2 |
| PRB-BR-008 | EF-6 |
| PRB-BR-009 | 4, 5, EF-1, EF-5 |
| PRB-BR-011 | 2 |
| PRB-BR-012 | 3, EF-3 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### PRB-UC-003 — HR chốt kết quả TV

| Mục | Nội dung |
|-----|----------|
| **ID** | PRB-UC-003 |
| **Actor chính** | PRB-ACT-003 |
| **Actor phụ** | PRB-ACT-005 (EMP / LIF / PAY đọc HĐ) |
| **Mục tiêu** | HR **chốt** đúng 1/3 kết quả; hệ thống cập nhật HĐ / mở off — PRB không tính lương |
| **Preconditions** | Role HR (IAM); có hồ sơ TV (đề xuất LM **không** bắt buộc) |
| **Postconditions (success)** | Kết quả đã chốt; Đạt → EMP chính thức; Gia hạn → KT mới theo master; Không đạt → mở hồ sơ off LIF |
| **Postconditions (failure)** | NV/LM không ghi được SoT; mã lạ bị chặn |
| **Trigger** | HR chốt trước/đúng hạn KT (quy chế; không bịa SLA trên UC) |
| **Frequency** | Mỗi đợt TV |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | HR | Mở hồ sơ TV; xem đề xuất LM nếu có |
| 2 | HR | Chọn kết quả ∈ {Đạt, Gia hạn, Không đạt} |
| 3 | HR | Nếu Gia hạn: chọn **thời lượng từ master** (không nhập tháng tự do trên UC) |
| 4 | Hệ thống | Ghi SoT; audit người chốt = HR |
| 5 | Hệ thống | **Đạt:** yêu cầu EMP chuyển HĐ chính thức (PAY hết 85% theo HĐ kỳ sau — không tính trên PRB) |
| 6 | Hệ thống | **Gia hạn:** cập nhật KT_TV; lịch T-15/T-7 theo KT mới |
| 7 | Hệ thống | **Không đạt:** mở luồng off LIF (checklist/N = LIF DOC-05); **không** xóa im lặng hồ sơ EMP |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | NV hoặc LM chốt SoT | 403 | PRB-BR-009 |
| EF-2 | Mã thứ 4 / “đạt có điều kiện” | Chặn | PRB-BR-004 |
| EF-3 | Gia hạn hardcode “+2 tháng” trên UC | Cấm | PRB-BR-006 |
| EF-4 | Không đạt nhưng không mở off | Cấm | PRB-BR-007 |
| EF-5 | PRB tự tính 85% | Cấm | PRB-BR-005 · PAY |
| EF-6 | Notify CRM sales khi chốt | Cấm | PRB-BR-010 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PRB-BR-004 | 2, EF-2 |
| PRB-BR-005 | 5, EF-5 |
| PRB-BR-006 | 3, 6, EF-3 |
| PRB-BR-007 | 7, EF-4 |
| PRB-BR-009 | 4, EF-1 |
| PRB-BR-010 | EF-6 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

### PRB-UC-004 — Thiếu mốc HĐ — không bịa

| Mục | Nội dung |
|-----|----------|
| **ID** | PRB-UC-004 |
| **Actor chính** | PRB-ACT-003 |
| **Actor phụ** | PRB-ACT-004 |
| **Mục tiêu** | Không bịa ngày BĐ/KT TV; cảnh báo HR để bổ sung trên EMP |
| **Preconditions** | Hồ sơ NV / HĐ thiếu mốc TV |
| **Postconditions (success)** | Mốc có trên EMP → NV vào hàng T-15/T-7 |
| **Postconditions (failure)** | Job không gán ngày giả |
| **Trigger** | Job T-15/T-7 gặp thiếu KT_TV; hoặc HR phát hiện |
| **Frequency** | Khi dữ liệu HĐ thiếu |

#### Luồng chính

| Step | Actor | Hành động |
|------|-------|-----------|
| 1 | Hệ thống | Phát hiện thiếu BĐ/KT TV; **không** default ngày |
| 2 | Hệ thống | Cảnh báo HR (HRM + email/app); không CRM sales |
| 3 | HR | Bổ sung mốc trên hồ sơ HĐ EMP (EMP, không nhập “ảo” trong PRB) |
| 4 | Hệ thống | Đưa NV vào coverage T-15/T-7 theo mốc mới |

#### Luồng ngoại lệ

| ID | Điều kiện | Steps | Kết quả |
|----|-----------|-------|---------|
| EF-1 | Job tự gán KT = BĐ + 60 ngày | Cấm | PRB-BR-001 |
| EF-2 | Bỏ NV vì thiếu mốc mà không cảnh báo | Fail coverage có điều kiện | PRB-BR-008 · 001 |

#### Quy tắc nghiệp vụ

| BR ID | Step |
|-------|------|
| PRB-BR-001 | 1, 3, EF-1 |
| PRB-BR-008 | 4, EF-2 |
| PRB-BR-010 | 2 |
| PRB-BR-011 | 2 |

#### Truy vết

| FR ID | Ghi chú |
|-------|---------|
| — | DOC-06 |

---

## 5. Tóm tắt (Casual)

| UC | Một câu |
|----|---------|
| 001 | Job nhắc T-15 theo ngày lịch, đủ coverage, không CRM sales. |
| 002 | Job T-7 + LM phiếu master, đề xuất không chốt. |
| 003 | HR chốt 3 mã → EMP / master gia hạn / LIF off. |
| 004 | Thiếu mốc → cảnh báo, sửa EMP, không bịa. |

## 6. Nhật ký thay đổi

| Phiên bản | UC ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | PRB-UC-001…004 | Distill DOC-04 Chốt; LM đề xuất / HR chốt; không LM → HR | — |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | ☑ Chốt v0.1 — DEC-REQ-053 |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn Draft |
| Business Owner | Ban HR | | ☐ Nợ |
