# DOC-03 — Tài liệu Yêu cầu Nghiệp vụ (BRD)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.7 | 2026-08-24 | Trịnh Yên (BA) | **Chốt** (BRD · DEC-DIS-015) + Phụ lục A (DEC-DIS-016) |

**Phạm vi:** DEC-DIS-001–**016**.  
**Cổng:** DOC-03 **đã chốt** (PGD). **Chưa** snapshot `02-baseline/`. Ban NS chưa ký Owner (nợ, không chặn A). Sửa BRD sau hôm nay = CR / DEC mới.

---

## 1. Kiểm soát tài liệu

| Phiên bản | Ngày | Tác giả | Thay đổi |
|-----------|------|---------|----------|
| 0.1 | 2026-08-24 | Dư Hùng | Distill URD + câu trả lời 2026-08-24 |
| 0.2 | 2026-08-24 | Dư Hùng | X = **3 ngày làm việc** trước kỳ nghỉ ≥3 ngày liên tiếp |
| 0.3 | 2026-08-24 | Dư Hùng | Ngoại lệ 3 NLĐ: **HR phê duyệt nghỉ đột xuất** |
| 0.4 | 2026-08-24 | Trịnh Yên | BO-002: X = **3 NLĐ** (DEC-DIS-003), bỏ placeholder |
| 0.5 | 2026-08-24 | Trịnh Yên | ≥3 ngày = **ngày công chuẩn liền**; header DEC-001–012; CN-004/005 |
| 0.6 | 2026-08-24 | Trịnh Yên | AS-IS / mẫu CC / lịch / PC / rationale = **khai báo động theo quy chế Cty** (DEC-DIS-014) · **chốt BRD** (DEC-DIS-015) |
| 0.7 | 2026-08-24 | Trịnh Yên | Phụ lục A: BRQ → BO (không đổi in/out) |

## 2. Tóm tắt điều hành

HRM nội bộ mInvoice: 8 phân hệ URD + IAM + **mobile MVP**. Payroll: N_thực **gồm** phép hưởng lương → không cộng lại. Thử việc **85% quy chế**. Công: **1 mẫu Excel Cty**. Off: khóa Git/CRM **N+3**. Phép: nghỉ ≥**3 ngày công chuẩn liền** → nộp trước **≥ 3 ngày công chuẩn**; trễ → **nghỉ đột xuất do HR duyệt**. **Xây 2026 (~1 tỷ); dùng từ 2027**. A: Mr. Dư Hùng, PGD. BA: **Trịnh Yên**.

## 3. Mục tiêu nghiệp vụ

| ID | Objective | Success Metric | Priority |
|----|-----------|----------------|----------|
| BO-001 | Hồ sơ + vòng đời | URD-01, 07 | Must |
| BO-002 | Phép 2 cấp | URD-02; nghỉ ≥**3 ngày công chuẩn liền** → nộp trước **≥ 3 ngày công chuẩn** (DEC-DIS-003, **013**) | Must |
| BO-003 | Công 1 template | Import UAT | Must |
| BO-004 | Lương đúng quy chế | 0 đồng; 85% TV | Must |
| BO-005 | Cảnh báo TV / SN / lễ | 0 sót 0 trễ | Must |
| BO-006 | Self-service web + **mobile** | MVP | Must |
| BO-007 | Analytics HR | URD-08 | Should |

## 4. Phạm vi

### 4.1 Trong phạm vi

- EMP: hồ sơ, unique CCCD/email Cty/MST, org, HĐ, thâm niên, đổi LM → luồng duyệt.
- LEV: loại phép URD; LM → HR; trừ quỹ khi HR duyệt; file ốm/BHXH; bàn giao; nghỉ ≥**3 ngày công chuẩn liền** nộp **≥ 3 ngày công chuẩn** trước; trễ: **Nghỉ đột xuất**, **chỉ HR** duyệt ngoại lệ (LM không chốt một mình).
- TIM: **một** template toàn Cty tại một thời điểm (nội dung **động theo quy chế**); preview lỗi; chốt tháng; OT; phép đã duyệt vào bảng công.
- PAY: công thức **đã sửa** (mục 8); phụ cấp/thưởng HĐ **và** nhập tháng; BH + TNCN tạm; phiếu lương PDF/email; **mobile** xem phiếu mình.
- PRB: T-15 / T-7; Đạt / gia hạn / không đạt.
- EVT: SN tuần + 08:00; lễ T-7.
- LIF: on/off checklist; IT khóa Git/CRM **N+3** (N = ngày LV cuối).
- RPT: turnover, cơ cấu, quỹ lương/OT.
- IAM: bảng III URD; Manager không xem lương.
- Kênh NV: **web + mobile** (hồ sơ, đơn phép, quỹ, phiếu lương, thông báo).

### 4.2 Ngoài phạm vi

- Hardware máy chấm công (chỉ import file).
- ATS tuyển dụng đầy đủ.
- Sổ cái kế toán / nộp BH nhà nước tự động (HR chốt sổ — quy trình ngoài).
- Chữ ký số HĐLĐ với cơ quan nhà nước (trừ khi requirements bổ sung).
- Thông báo phép sang **CRM bán hàng** (không thuộc URD theo anh).

### 4.3 Biên giới

| Hệ thống | Ghi chú |
|----------|---------|
| Máy CC | Xuất Excel theo **1 mẫu Cty** (phiên bản HR / quy chế — động) |
| Email @minvoice.vn, Git, CRM (sản phẩm), chat | Cấp lúc on; khóa Git/CRM **N+3** |
| Email/App/HRM | Thông báo duyệt phép, cảnh báo |

## 5. AS-IS

**Nguyên tắc (DEC-DIS-014):** tool, file, lịch, danh mục đang dùng **không đóng băng trên BRD** — HRM đọc **chính sách/quy chế Cty tại thời điểm áp dụng**.

| Area | Mô tả | Pain |
|------|-------|------|
| Công | Máy CC → Excel; **mẫu = phiên bản HR công bố theo quy chế** | Thủ công, lệch mẫu |
| Lương / hồ sơ / phép | Quy trình + tool **theo chính sách Cty** (không inventory tool trên BRD) | Phân tán, chưa SoT |
| Off | Checklist khóa TK **theo quy chế** tại thời điểm off | Trước đây chưa N+3 |

## 6. TO-BE

Một HRM: công chốt → lương; phép 2 cấp; LIF N+3; NV web+mobile.

## 7. Yêu cầu nghiệp vụ

**Rationale (cột template):** không viết từng dòng. Giá trị vận hành (mẫu Excel, lịch công/lễ, danh mục PC/thưởng, tỷ lệ BH/TNCN, tool as-is) = **khai báo động theo quy chế Cty**. BRD chỉ chốt **nguyên tắc**. Trace BRQ→BO: **Phụ lục A**.

| ID | Requirement | Stakeholder | Priority |
|----|-------------|-------------|----------|
| BRQ-001 | 8 phân hệ + IAM theo URD (đã chỉnh lương / N+3 / mobile) | SH-002 | Must |
| BRQ-002 | N_tính **không** cộng lại phép hưởng (N_thực đã gồm) | SH-002 | Must |
| BRQ-003 | 85% lương thử việc = quy chế | SH-001, 002 | Must |
| BRQ-004 | **Một** mẫu Excel công toàn Cty tại một thời điểm; **cột/file = master HR theo quy chế** (động) | SH-002, 006 | Must |
| BRQ-005 | Khóa Git/CRM N+3 | SH-006 | Must |
| BRQ-006 | Mobile self-service in MVP | SH-004 | Must |
| BRQ-007 | Phụ cấp/thưởng: kênh HĐ cố định **và** nhập tháng; **danh mục = master theo quy chế** (động) | SH-002 | Must |
| BRQ-008 | Nghỉ ≥**3 ngày công chuẩn liền**: nộp **≥ 3 ngày công chuẩn** trước; trễ = đột xuất **HR duyệt** | SH-002 | Must |
| BRQ-009 | UAT: 0đ lương; 1000 dòng &lt;5s; cảnh báo đúng; cô lập phiếu lương | SH-002 | Must |
| BRQ-010 | Nghỉ đột xuất: HR phê duyệt; LM không tự chốt ngoại lệ 3 NLĐ | SH-002, 005 | Must |

## 8. Quy tắc nghiệp vụ (tóm tắt)

| ID | Rule |
|----|------|
| BR-001 | **N_tính = N_thực − N_phép_không_lương**; không + N_phép_hưởng. Trần = ngày công chuẩn tháng. |
| BR-002 | Thử việc: lương thời gian × **85%** (quy chế). |
| BR-003 | OT: 1.5 / 2.0 / 3.0 theo URD; giờ từ bảng công đã chốt. |
| BR-004 | Trừ quỹ phép năm khi **HR duyệt cấp 2**. |
| BR-005 | Unique: MNV, CCCD, email Cty, MST cá nhân. |
| BR-006 | Off: khóa Git & CRM (sản phẩm) = **N+3**. |
| BR-007 | Nghỉ ≥ **3 ngày công chuẩn liền** (không đếm T7/CN/lễ vào chuỗi): nộp **≥ 3 ngày công chuẩn** trước ngày bắt đầu nghỉ. Submit đúng hạn: luồng LM → HR như URD-02. |
| BR-008 | **Nghỉ đột xuất** (nộp trễ hơn 3 NLĐ, hoặc NV/HR đánh dấu đột xuất): hệ thống **không** chặn submit; **chỉ HR** (C&B / chuyên viên HR) được phê duyệt chính thức. LM có thể nhận thông tin / C1 nhưng **không** đủ để trừ quỹ nếu chưa có duyệt HR ngoại lệ. Ốm/BHXH vẫn bắt buộc file theo URD. |

→ Chi tiết hành vi DOC-04. **Danh mục / mẫu / lịch / tỷ lệ** không đóng trên BRD hay DOC-04 tĩnh — cấu hình theo quy chế (DEC-DIS-014).

## 9. Ràng buộc

| ID | Loại | Mô tả |
|----|------|-------|
| CN-001 | Legal | BHXH/BHYT/BHTN, TNCN — tỷ lệ **theo luật / quy chế** tại kỳ lương, không hardcode URD |
| CN-002 | Security | PII; lương cô lập; mobile cùng IAM |
| CN-003 | Tech | **Một** template Excel tại một thời điểm; nội dung mẫu **động theo quy chế HR** |
| CN-004 | Budget | CAPEX HRM **~1 tỷ đồng trong 2026** (DEC-DIS-008, 010) |
| CN-005 | Timeline | **Go-live / bắt đầu sử dụng: 2027**; 2026 chỉ xây (DEC-DIS-010) |
| CN-006 | Policy | Mẫu công, lịch ngày công/lễ, danh mục PC/thưởng, tỷ lệ BH/TNCN, tool as-is: **khai báo động theo quy chế Cty**; BRD không đóng bộ giá trị |

## 10. Giả định

| ID | Assumption | Impact if wrong |
|----|------------|-----------------|
| A-001 | N_KHL nằm trong N_thực nên trừ; nếu KHL không nằm trong N_thực → N_tính = N_thực | Sai lương |
| A-002 | Danh mục phụ cấp/thưởng = **master theo quy chế Cty** (động), không đóng trên BRD | Master lệch quy chế → sai dòng lương |
| A-003 | N = ngày LV cuối (không phải ngày ký nghỉ) | Sai N+3 |
| A-004 | “Ngày công chuẩn” = ngày trên **lịch Cty theo quy chế** (lễ/CN **không** tính). Dùng **cả** (a) chuỗi nghỉ ≥3 ngày **và** (b) hạn nộp 3 ngày trước. | Sai ngưỡng 3 ngày / sai hạn nộp |
| A-005 | AS-IS tools không cần liệt kê trên BRD; vận hành hiện tại **theo chính sách Cty** đến lúc go-live HRM | Thiếu SoT nếu coi BRD là inventory tool |

## 11. Phụ thuộc

| ID | Dependency | Owner |
|----|------------|-------|
| D-001 | Mẫu Excel CC (1 Cty, **phiên bản HR theo quy chế** — động) | HR + IT |
| D-002 | Tỷ lệ BH, TNCN (**theo luật / quy chế tại kỳ lương** — động) | C&B / luật |
| D-003 | Danh mục phụ cấp/thưởng (**master quy chế** — động) | C&B |
| D-004 | Lịch ngày làm việc / ngày lễ (**lịch Cty theo quy chế** — động; tính 3 NLĐ) | HR |

## 12. Thuật ngữ

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| N_thực | Ngày công thực tế từ CC — **đã gồm** ngày phép hưởng lương |
| N_tính | Ngày công tính lương — BR-001 |
| N | Ngày làm việc cuối (offboarding) |
| N+3 | Khóa Git/CRM sau N ba ngày |
| X / 3 NLĐ | Nghỉ ≥**3 ngày công chuẩn liền** → nộp trước **≥ 3 ngày công chuẩn** (DEC-DIS-003, **013**) |
| Ngày công chuẩn liền | Chuỗi ngày công công ty liền nhau; **không** phải 3 ngày lịch (T7/CN/lễ không đếm) |
| Nghỉ đột xuất | Nộp trễ hơn 3 NLĐ (hoặc NV/HR đánh dấu); **chỉ HR** duyệt ngoại lệ (DEC-DIS-004) |

## 13. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-24 | **Chốt** v0.6 (DEC-DIS-015) · v0.7 phụ lục A (DEC-DIS-016) · ☐ repo `02-baseline/` |
| Business Owner | Ban Nhân sự | | ☐ Nợ — không chặn cổng A |
| BA (R) | Trịnh Yên | 2026-08-24 | Soạn → PGD chốt |

## Module index

Folder module tạo khi **requirements**.

| Module ID | MOD | Folder | Priority | In scope | Ghi chú |
|-----------|-----|--------|----------|----------|---------|
| employee-profile | EMP | `03-modules/employee-profile/` | Must | ☑ | URD-01 |
| leave | LEV | `03-modules/leave/` | Must | ☑ | URD-02; 3 ngày công chuẩn + đột xuất HR |
| timekeeping | TIM | `03-modules/timekeeping/` | Must | ☑ | 1 mẫu Excel |
| payroll | PAY | `03-modules/payroll/` | Must | ☑ | BR-001, 85% |
| probation | PRB | `03-modules/probation/` | Must | ☑ | URD-05 |
| events | EVT | `03-modules/events/` | Must | ☑ | URD-06 |
| lifecycle | LIF | `03-modules/lifecycle/` | Must | ☑ | N+3 |
| hr-analytics | RPT | `03-modules/hr-analytics/` | Should | ☑ | URD-08 |
| identity | IAM | `03-modules/identity/` | Must | ☑ | URD III + mobile |

## Phụ lục A — Trace BRQ (§7) → BO (§3)

Không đổi phạm vi. Một BRQ có thể phục vụ nhiều BO. **BO chính** = sát nhất.

| BRQ | Phục vụ BO | Ghi chú |
|-----|------------|---------|
| BRQ-001 | BO-001…007 | Ô dù 8 phân hệ + IAM. **BO-007** chỉ đi qua BRQ này (không có BRQ riêng). |
| BRQ-002 | BO-004 | N_tính → lương đúng quy chế |
| BRQ-003 | BO-004 | 85% thử việc |
| BRQ-004 | BO-003 | Một mẫu Excel công |
| BRQ-005 | BO-001 | Vòng đời / off (N+3) |
| BRQ-006 | BO-006 | Mobile MVP |
| BRQ-007 | BO-004 | Phụ cấp/thưởng trên phiếu lương |
| BRQ-008 | BO-002 | Ngưỡng 3 ngày công chuẩn + hạn nộp |
| BRQ-009 | BO-003, BO-004, BO-005, BO-006 | UAT: import &lt;5s; 0đ; cảnh báo; cô lập phiếu |
| BRQ-010 | BO-002 | Đột xuất chỉ HR |

**Lỗ:** BO-005 không có BRQ nghiệp vụ riêng (chỉ UAT + BRQ-001). BO-007 Should không có BRQ Must riêng.
