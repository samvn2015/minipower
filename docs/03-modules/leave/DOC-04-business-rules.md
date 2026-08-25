# DOC-04 — Quy tắc nghiệp vụ — Leave (LEV)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.3 | 2026-08-24 | Dư Hùng (BA) | **Chốt** (cổng BR · leave) |

**Module:** leave · **MOD:** LEV · **Phạm vi:** URD-02 + DEC-DIS-003/004.  
**Không** gồm: import công (TIM), trừ lương (PAY — chỉ nhận ngày phép đã duyệt).

**Cổng:** DEC-REQ-005 **đã chốt** 2026-08-24. Nợ: OQ-REQ-010 (chỉ NV hủy hộ). **Không** = baseline repo (`02-baseline/`). Mở: DOC-05 · khung DOC-19. **Chưa** DOC-06.

---

## 1. Mục đích & phạm vi

Chuẩn hóa: loại phép, nộp đơn (web + mobile), hạn 3 NLĐ, nghỉ đột xuất, duyệt C1 (LM) → C2 (HR), trừ quỹ, file ốm/BHXH, bàn giao, thông báo. Kênh thông báo = **Email / App / HRM** — không CRM bán hàng (DEC-DIS-001).

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| LEV-BR-001 | Catalog loại phép | 6 loại URD-02 | Validation | Must | BRQ-001 | SH-002 |
| LEV-BR-002 | Đơn vị thời gian | Cả ngày / Sáng / Chiều; 0,5 ngày | Calculation | Must | URD-02 | SH-002 |
| LEV-BR-003 | File ốm/BHXH | File **đúng mẫu Cty**; không file/sai mẫu → không submit | Validation | Must | URD-02 | SH-002 |
| LEV-BR-004 | Người bàn giao | Bắt buộc chọn NV khác, đang active | Validation | Must | URD-02 | SH-005 |
| LEV-BR-005 | Chặn vượt quỹ năm | Vượt quỹ năm → chặn, trừ KHL | Validation | Must | URD-02 | SH-002 |
| LEV-BR-006 | Trừ quỹ khi HR C2 | Chỉ trừ quỹ năm khi HR duyệt chính thức | Calculation | Must | BR-004 | SH-002 |
| LEV-BR-007 | Hạn 3 NLĐ | ≥3 **ngày công chuẩn liền** → nộp ≥3 NLĐ trước | Validation | Must | BR-007, BRQ-008 | SH-002 |
| LEV-BR-008 | Nghỉ đột xuất | Không chặn submit; chỉ HR chốt ngoại lệ | Authorization | Must | BR-008, BRQ-010 | SH-002 |
| LEV-BR-009 | C1 LM | LM chỉ đơn cấp dưới; Phê duyệt / Từ chối + lý do | Authorization | Must | URD-02, IAM | SH-005 |
| LEV-BR-010 | C2 HR | HR duyệt chính thức / từ chối; cấu hình luồng = IT/HR admin | Authorization | Must | URD-02, IAM | SH-002 |
| LEV-BR-011 | Thông báo | Email/App/HRM tới NV, LM, HR theo bước | Inference | Must | DEC-DIS-001 | SH-002 |
| LEV-BR-012 | Kênh self-service | Web + mobile cùng rule | Authorization | Must | BRQ-006 | SH-004 |
| LEV-BR-013 | Overlap | Cấm trùng khi Open/Đã duyệt; **Đã hủy** không chiếm ngày | Validation | Must | DEC-REQ-004 | SH-002 |
| LEV-BR-014 | Quyền tạo/duyệt | NV tạo đơn mình; Manager C1; HR C2 | Authorization | Must | IAM III | SH-002 |
| LEV-BR-015 | Không hủy sau C2 | MVP: không thu hồi / hoàn quỹ sau HR duyệt | Authorization | Must | OQ-REQ-005 | SH-002 |
| LEV-BR-016 | Hủy trước C2 | NV hủy đơn chờ duyệt → được nộp đơn khác cùng ngày | Authorization | Must | DEC-REQ-004 | SH-004 |

**Loại:** Validation · Calculation · Authorization · Inference.

## 3. Chi tiết quy tắc

### LEV-BR-001 — Catalog loại phép

| Mục | Nội dung |
|-----|----------|
| **Statement** | Đơn chỉ được tạo với đúng một loại: Phép năm · Phép không hưởng lương · Phép ốm/BHXH · Phép kết hôn · Phép tang chế · Nghỉ chế độ Nam/Nữ. |
| **Condition** | IF NV chọn loại ngoài catalog |
| **Action** | THEN hệ thống từ chối lưu |
| **Exception** | Bổ sung loại = CR + cập nhật catalog (không hardcode UI-only) |
| **Source** | URD-02 · DEC-REQ-004 |
| **Effective date** | MVP |
| **Trace** | BRQ-001 · UC TBD |

Trần ngày theo loại (kết hôn, tang, chế độ, …) = **tùy chọn**. Go-live: **không trần** (để trống = không chặn theo trần loại). HR cấu hình số khi cần; chỉ khi đã có số mới chặn đơn vượt trần. Không hardcode luật.

### LEV-BR-002 — Đơn vị thời gian

| Mục | Nội dung |
|-----|----------|
| **Statement** | Khoảng nghỉ = Từ ngày–Đến ngày; mỗi ngày trong khoảng mang nhãn Cả ngày (1,0) hoặc Sáng (0,5) hoặc Chiều (0,5). Tổng ngày đơn = tổng nhãn. |
| **Condition** | IF Sáng và Chiều cùng một ngày lịch |
| **Action** | THEN = 1,0 ngày (tương đương Cả ngày) |
| **Exception** | — |
| **Source** | URD-02 |
| **Trace** | LEV-BR-005 (quỹ năm), không dùng để kích hoạt hạn 3 NLĐ |

### LEV-BR-003 — File ốm / BHXH

| Mục | Nội dung |
|-----|----------|
| **Statement** | Loại Phép ốm/BHXH: bắt buộc ≥1 file **đúng mẫu công ty quy định** (giấy xác nhận y tế trên form Cty) trước submit và trước HR duyệt chính thức. |
| **Condition** | IF loại = ốm/BHXH AND (không có file OR file không đúng mẫu Cty đang hiệu lực) |
| **Action** | THEN không cho submit; nếu file bị gỡ / sai mẫu sau C1 → HR không được duyệt chính thức |
| **Exception** | HR không “duyệt giấy tay” thay mẫu — chỉ kiểm có file đúng mẫu (không OCR bắt buộc MVP) |
| **Source** | URD-02 · DEC-REQ-004 |
| **Trace** | BR-008 |

### LEV-BR-004 — Người bàn giao

| Mục | Nội dung |
|-----|----------|
| **Statement** | Đơn phải chọn đúng 1 người bàn giao tạm: MNV khác người xin, trạng thái đang làm việc. |
| **Condition** | IF thiếu / trùng người xin / NV đã nghỉ |
| **Action** | THEN không submit |
| **Exception** | Không miễn đơn 0,5 ngày (OQ-REQ-006) |
| **Source** | URD-02 |
| **Trace** | IAM “Xác nhận bàn giao” (LIF — ngoài LEV trừ khi UC xác nhận) |

### LEV-BR-005 — Chặn vượt quỹ phép năm

| Mục | Nội dung |
|-----|----------|
| **Statement** | Nếu loại **trừ quỹ phép năm** và số ngày đơn > quỹ còn lại (tại thời điểm submit **và** tại thời điểm HR duyệt) → không gửi / không duyệt. |
| **Condition** | IF loại ∈ {Phép năm} AND ngày_đơn > quỹ_còn |
| **Action** | THEN chặn + thông báo số quỹ còn |
| **Exception** | **Phép không hưởng lương:** không chặn theo quỹ năm. **Ốm/BHXH, kết hôn, tang, chế độ:** không trừ quỹ năm. Trần loại: LEV-BR-001 (go-live không trần). |
| **Source** | URD-02 |
| **Trace** | BR-004 |

### LEV-BR-006 — Thời điểm trừ quỹ

| Mục | Nội dung |
|-----|----------|
| **Statement** | Hệ thống trừ quỹ phép năm **chỉ** khi HR bấm Duyệt chính thức (C2) trên đơn loại trừ quỹ năm. C1 không trừ quỹ. Từ chối / hủy trước C2: không trừ. |
| **Condition** | IF HR C2 = duyệt AND loại trừ quỹ năm |
| **Action** | THEN quỹ_còn := quỹ_còn − ngày_đơn (atomic với đổi trạng thái Đã duyệt) |
| **Exception** | Nghỉ đột xuất: vẫn chỉ trừ khi HR đã duyệt ngoại lệ **và** duyệt chính thức (có thể cùng một thao tác C2 — LEV-BR-008) |
| **Source** | URD-02 · BR-004 · DEC-DIS-004 |
| **Trace** | BR-004, BR-008 |

### LEV-BR-007 — Hạn nộp 3 ngày làm việc

| Mục | Nội dung |
|-----|----------|
| **Statement** | Rule hạn nộp áp khi khoảng nghỉ chứa **≥ 3 ngày công chuẩn liên tiếp** trên lịch công ty (lễ/CN không phải ngày công thì **không** đếm, **không** phá chuỗi ngày công). Ngày công chỉ nghỉ Sáng hoặc Chiều vẫn đếm **1 ngày công**. Thời điểm submit phải **trước** ngày bắt đầu nghỉ **≥ 3 ngày làm việc** (A-004, D-004). |
| **Condition** | IF số ngày công chuẩn liên tiếp trong đơn ≥ 3 AND (ngày_bắt_đầu − ngày_submit) tính theo NLĐ &lt; 3 AND đơn **không** mang cờ Nghỉ đột xuất |
| **Action** | THEN từ chối submit đường bình thường; gợi ý đánh dấu Nghỉ đột xuất |
| **Exception** | LEV-BR-008 |
| **Source** | DEC-DIS-003 · BR-007 · DEC-REQ-004 (chọn C) |
| **Trace** | BRQ-008 |

*Ví dụ:* Nghỉ Thứ Sáu–Thứ Hai, T7+CN không phải ngày công → chuỗi ngày công = Thứ Sáu + Thứ Hai = **2** → **không** kích hoạt hạn 3 NLĐ. Nghỉ Thứ Tư–Thứ Sáu (3 ngày công liền) → **có** kích hoạt. Tổng 3,0 ngày phép rải không liền ngày công → **không** kích hoạt.

### LEV-BR-008 — Nghỉ đột xuất — chỉ HR chốt

| Mục | Nội dung |
|-----|----------|
| **Statement** | Đơn đánh dấu Nghỉ đột xuất (NV hoặc HR) **được submit** dù trễ 3 NLĐ. Hệ thống không hard-block. **Chỉ** vai trò Chuyên viên HR/C&B được phê duyệt **ngoại lệ + chính thức**. LM **không** đủ để đóng ngoại lệ hay trừ quỹ. |
| **Condition** | IF cờ đột xuất = true |
| **Action** | THEN: (1) nhận đơn; (2) LM vẫn nhận C1 (thông tin / duyệt hoặc từ chối lịch phòng); (3) quỹ và trạng thái “Đã duyệt chính thức” chỉ sau HR C2 trên đơn đột xuất |
| **Exception** | Ốm/BHXH vẫn LEV-BR-003. HR **không** bypass C1 (OQ-REQ-003). LM từ chối → dừng; muốn nghỉ phải đơn mới. |
| **Source** | DEC-DIS-004 · BR-008 |
| **Trace** | BRQ-010 |

### LEV-BR-009 — Cấp 1 Line Manager

| Mục | Nội dung |
|-----|----------|
| **Statement** | C1 chỉ Manager của người xin (IAM: duyệt đơn cấp dưới). Hành động: Phê duyệt **hoặc** Từ chối kèm lý do bắt buộc khi từ chối. |
| **Condition** | IF actor không phải LM của NV AND không phải HR acting |
| **Action** | THEN 403 / ẩn nút C1 |
| **Exception** | Matrix Manager **không** C1 (OQ-REQ-007). |
| **Source** | URD-02 · bảng III |
| **Trace** | LEV-BR-014 |

### LEV-BR-010 — Cấp 2 HR

| Mục | Nội dung |
|-----|----------|
| **Statement** | Sau C1 duyệt, đơn vào hàng HR. HR kiểm quỹ (LEV-BR-005) rồi Duyệt chính thức hoặc Từ chối + lý do. IT Admin cấu hình luồng, không duyệt nghiệp vụ trừ khi được gán role HR. |
| **Condition** | IF C1 chưa duyệt |
| **Action** | THEN HR không C2 (kể cả đơn đột xuất) |
| **Exception** | LEV-BR-008: HR là người duy nhất đóng ngoại lệ |
| **Source** | URD-02 · IAM “Duyệt đơn cấp 2” |
| **Trace** | BR-004 |

### LEV-BR-011 — Thông báo

| Mục | Nội dung |
|-----|----------|
| **Statement** | Mỗi đổi trạng thái (nộp, C1, C2, từ chối) gửi thông báo Email **và/hoặc** App/HRM tới NV, LM, HR theo bước. **Không** gọi CRM bán hàng. |
| **Condition** | IF URD ghi “Email/App/CRM” |
| **Action** | THEN map CRM = kênh HRM nội bộ, không phải sản phẩm CRM khách |
| **Exception** | — |
| **Source** | DEC-DIS-001 |
| **Trace** | DOC-03 4.2 |

### LEV-BR-012 — Web và mobile

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV tạo/xem đơn, quỹ, trạng thái trên web **và** mobile MVP; cùng LEV-BR-*. |
| **Condition** | IF kênh = mobile |
| **Action** | THEN không nới rule (cùng validation) |
| **Exception** | — |
| **Source** | BRQ-006 |
| **Trace** | IAM NV |

### LEV-BR-013 — Không chồng lấn

| Mục | Nội dung |
|-----|----------|
| **Statement** | Hai đơn cùng NV không chồng khoảng ngày (theo nhãn Cả ngày/Sáng/Chiều) nếu trạng thái ∈ {Chờ C1, Chờ C2, Đã duyệt}. |
| **Condition** | IF overlap trên ngày/buổi còn chiếm chỗ |
| **Action** | THEN chặn submit đơn mới |
| **Exception** | Đơn Từ chối / **Đã hủy** không chiếm chỗ → được nộp đơn khác **cùng ngày** (DEC-REQ-004) |
| **Source** | DEC-REQ-004 (câu 2) |
| **Trace** | LEV-BR-016 · TIM |

### LEV-BR-014 — Phân quyền LEV

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV: tạo đơn của mình. Manager: C1 cấp dưới, không C2. HR/C&B: C2 + cấu hình catalog/luồng (cùng IT nếu IAM tách). IT: cấu hình luồng, không C2 nghiệp vụ. |
| **Condition** | IF Manager bấm C2 |
| **Action** | THEN từ chối |
| **Exception** | User kiêm NV + Manager: C1 chỉ đơn cấp dưới, không tự C1 đơn mình |
| **Source** | URD III |
| **Trace** | IAM |

### LEV-BR-015 — Không hủy / thu hồi sau C2 (MVP)

| Mục | Nội dung |
|-----|----------|
| **Statement** | Sau HR duyệt chính thức: NV và LM **không** hủy đơn; hệ thống **không** hoàn quỹ tự động. Thu hồi + hoàn quỹ = ngoài MVP (CR). |
| **Condition** | IF trạng thái = Đã duyệt chính thức AND actor yêu cầu hủy |
| **Action** | THEN từ chối |
| **Exception** | Từ chối C1/C2 hoặc **hủy trước C2** (LEV-BR-016): không trừ quỹ |
| **Source** | OQ-REQ-005 |
| **Trace** | LEV-BR-006 · LEV-BR-016 |

### LEV-BR-016 — Hủy trước C2 rồi nộp lại cùng ngày

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV được hủy **đơn của mình** khi trạng thái ∈ {Chờ C1, Chờ C2}. Hủy xong được nộp đơn khác chồng ngày/buổi đã giải phóng. |
| **Condition** | IF NV hủy AND trạng thái chờ C1 hoặc C2 |
| **Action** | THEN trạng thái Đã hủy; không trừ quỹ; LEV-BR-013 không còn chiếm chỗ |
| **Exception** | Sau C2: LEV-BR-015. **Giả định (OQ-REQ-010):** LM/HR không hủy hộ MVP — chỉ NV hủy đơn mình |
| **Source** | DEC-REQ-004 |
| **Trace** | LEV-BR-013, 015 |

## 4. Bảng quyết định — hạn nộp vs đột xuất

| ≥ 3 ngày công chuẩn liền | Submit trước ≥ 3 NLĐ | Cờ đột xuất | Kết quả submit | Ai đóng duyệt chính thức / trừ quỹ |
|---------------------|----------------------|-------------|----------------|-------------------------------------|
| Không | — | — | Cho phép (luồng chuẩn) | HR C2 sau LM C1 |
| Có | Có | — | Cho phép (luồng chuẩn) | HR C2 sau LM C1 |
| Có | Không | Không | **Chặn** + gợi ý đột xuất | — |
| Có | Không | Có | Cho phép | **Chỉ HR** (LM không trừ quỹ) |

## 5. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | LEV-BR-001…014 | Distill URD-02 + DEC-DIS-003/004 | — |
| 0.2 | 001, 004–010, 015 | Đóng OQ-REQ-001…009 (mặc định + diễn giải 002/005) | — |
| 0.3 | 001, 003, 007, 013, 016 | QA: 1C ngày công liền; hủy rồi nộp lại cùng ngày; mẫu Cty; không trần mặc định | — |
| 0.3 | — | **Chốt cổng BR** (DEC-REQ-005); nợ OQ-010 | — |
