# DOC-01 — Tầm nhìn & Hồ sơ kinh doanh

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-24 | Dư Hùng (BA) | **Chốt** (tầm nhìn · DEC-DIS-011) |

**Nguồn:** URD HRM · DEC-DIS-001–**011**.  
**Cổng:** DOC-01 **đã chốt** (PGD, v0.2). **Chưa** snapshot `02-baseline/`. Nợ giữ: % thời gian 3 HR làm lương/công/phép; OPEX từ 2027; Ban NS chưa ký; A **BRD** = **DOC-03**.

---

## 1. Tóm tắt điều hành

mInvoice cần **HRM nội bộ**: một hồ sơ vòng đời NS (on → công/phép/lương → off), duyệt/cảnh báo tự động, chấm công **một mẫu Excel toàn công ty**, payroll theo **quy chế** (thử việc 85%), self-service **web + mobile MVP**. Khóa Git/CRM **N+3** sau ngày làm việc cuối. **Đầu tư xây ~1 tỷ trong 2026; bắt đầu sử dụng từ 2027** (DEC-DIS-010). A ký BRD: **Mr. Dư Hùng, PGD**.

## 2. Tuyên bố tầm nhìn

**Tầm nhìn:** NV, LM, HR/C&B, IT làm việc trên một hệ thống: hồ sơ–đơn từ–công–lương–checklist vào/ra; phiếu lương chỉ người đó thấy.

**Mission:** Số hóa vòng đời NS; tự động duyệt/cảnh báo; chuẩn hóa công–lương; trải nghiệm nhân viên (kể cả mobile).

## 3. Vấn đề nghiệp vụ

| Mục | Nội dung |
|-----|----------|
| **Hiện tại** | Hồ sơ/duyệt/công/lương phân tán; máy CC + Excel; mốc thử việc/lễ/SN dễ sót. As-is **chưa đo**. |
| **Root cause** | Chưa SoT NS + quỹ phép/công/lương. |
| **Impact** | Quy mô Cty: quỹ lương **~3 tỷ**/tháng; tổng chi **~4 tỷ**/tháng. **Mảng HR = 3 nhân sự** (PGD). Lợi ích HRM chỉ từ giờ/sai sót của nhóm này (+ intangible NV), không phải 4 tỷ. |

## 4. Mục tiêu & KPI

| Goal ID | Mục tiêu | KPI | Target | Timeline |
|---------|----------|-----|--------|----------|
| G-001 | SoT hồ sơ + vòng đời | MNV `mINV-YYYY-XXXX`; on/off checklist | URD-01, 07 | Go-live **2027** |
| G-002 | Phép 2 cấp + quỹ | Trừ quỹ khi HR duyệt; nghỉ ≥**3 ngày làm việc liền** nộp trước **≥ 3 NLĐ** | URD-02 · DEC-DIS-003 | Go-live **2027** |
| G-003 | Công | 1 template Cty; import 1000 dòng &lt;5s; 100% bắt lỗi | UAT URD | Go-live **2027** |
| G-004 | Lương | Sai số **0 đồng** vs kiểm tra tay; 85% thử việc (quy chế) | UAT | Go-live **2027** |
| G-005 | Cảnh báo | Thử việc T-15/T-7; SN/lễ đúng mốc, 0 sót 0 trễ | URD-05, 06 | Go-live **2027** |
| G-006 | EX | Self-service web **và mobile** (hồ sơ, phép, phiếu lương) | MVP | Go-live **2027** |
| G-007 | Bảo mật lương | NV/Manager không xem lương người khác | URD III, UAT | Go-live **2027** |

## 5. Giải pháp đề xuất (tổng quan)

Ứng dụng HRM (web + mobile NV) + import Excel công + payroll theo BR đã chốt + thông báo Email/App/HRM. Không thay máy CC. Tích hợp: cấp/khóa TK (email, HRM, Git, CRM bán hàng, …) theo checklist — khóa Git/CRM **N+3**.

## 6. Hồ sơ kinh doanh

### 6.1 Lợi ích

| Benefit | Loại | Ước lượng |
|---------|------|-----------|
| Đúng lương, đúng hạn thử việc | Tangible | Trần = thời gian **3 người HR** (không phải 150 LĐ) |
| Giảm sự cố sai lương | Tangible | TBD số lần |
| Self-service phép / phiếu / mobile | Intangible | Ngoài 3 HR |

**Đội HR:** **3 người**. Trần giờ (100% thời gian, 176h/người): **528 giờ/tháng** · **~6.336 giờ/năm**. Trần lương 3 FTE: **720 triệu đồng/năm**. Không giả định cắt headcount — chỉ một phần giờ lương/công/phép.  
Đơn giá giờ ~113.600 đ (20 triệu ÷ 176h). ~150 LĐ toàn Cty (3 tỷ ÷ 20 triệu).

### 6.2 Chi phí

**A. Vận hành công ty (bối cảnh — không phải CAPEX HRM)**

| Hạng mục | / tháng | / năm | Ghi chú |
|----------|---------|--------|---------|
| Quỹ lương | ~3 tỷ | ~36 tỷ | Anh chốt |
| Chi phí khác (cố định + biến đổi) | ~1 tỷ | ~12 tỷ | 4 tỷ tổng − 3 tỷ lương |
| **Tổng chi phí Cty** | **~4 tỷ** | **~48 tỷ** | Anh chốt |

**B. Dự án HRM**

| Hạng mục | 2026 (xây) | Từ 2027 (đưa vào dùng) | Ghi chú |
|----------|------------|------------------------|---------|
| CAPEX (xây + hạ tầng ban đầu) | **~1 tỷ đồng** | — | PGD · DEC-DIS-008/010 |
| Lợi ích vận hành HRM | **0** (chưa dùng) | Bắt đầu đếm | DEC-DIS-010 |
| OPEX (hosting, bảo trì) | TBD (có thể gộp trong 1 tỷ) | TBD | Chưa tách |

### 6.3 ROI / hoàn vốn

| Chỉ số | Giá trị |
|--------|---------|
| 2026 | Chi **~1 tỷ** CAPEX; **không** có lợi ích vận hành (chưa go-live) |
| Từ 2027 | Trần lợi ích 3 HR = **720 triệu đồng/năm** (100% giờ — **không thực tế**) |
| Payback (chỉ lương 3 HR, trần 100%) | ~1 tỷ ÷ 720tr ≈ **1,4 năm sau go-live** → sớm nhất **giữa 2028** nếu 100% giờ 3 người (không kỳ vọng) |
| Kết luận giấy | 2026 = đầu tư xây. Case vận hành (0đ lương, 0 sót TV, EX + một phần giờ 3 HR) **bắt đầu 2027**. Hòa vốn tiền mặt không kỳ vọng trong năm xây. |

## 7. Giả định & ràng buộc

| ID | Loại | Mô tả |
|----|------|-------|
| A-001 | Assumption | N_thực **đã gồm** ngày phép hưởng lương → không cộng lại N_phép_hưởng (DEC-DIS-002). |
| A-002 | Assumption | Phụ cấp/thưởng: **cả** cố định HĐ **và** nhập tháng; danh mục chi tiết → DOC-04. |
| A-003 | Assumption | Nghỉ ≥3 ngày liên tiếp: nộp **≥ 3 ngày làm việc** trước ngày bắt đầu nghỉ (DEC-DIS-003). |
| C-001 | Constraint | 85% lương thử việc = **quy chế** mInvoice. |
| C-002 | Constraint | **Một** mẫu Excel chấm công toàn công ty. |
| C-003 | Constraint | Khóa Git/CRM: **N+3** (N = ngày làm việc cuối). |
| C-004 | Constraint | Mobile self-service **in MVP**. |
| C-005 | Constraint | A ký BRD = Mr. Dư Hùng, PGD. |
| A-006 | Assumption | 176 giờ/tháng để ra đơn giá giờ từ 20 triệu; ~150 LĐ từ quỹ 3 tỷ. |
| A-007 | Assumption | CAPEX HRM 2026 ~1 tỷ gồm xây + hạ tầng; OPEX hosting **chưa tách** khỏi 1 tỷ khi tính hòa vốn. |
| A-008 | Assumption | Mảng HR **3 người**; trần 528h/tháng; không giả định cắt 3 headcount. |
| C-006 | Constraint | Go-live / bắt đầu **sử dụng** HRM: **2027** (DEC-DIS-010). 2026 chỉ xây. |
| A-009 | Assumption | Lợi ích giờ/sai sót HR **không** đếm trong 2026; đếm từ năm dương lịch go-live (2027). |

## 8. Rủi ro

| ID | Rủi ro | Mức | Mitigation |
|----|--------|-----|------------|
| R-001 | Công thức URD gốc cộng kép phép hưởng | H | BR N_tính đã sửa; HR UAT |
| R-002 | Lạm dụng “đột xuất” để bỏ 3 NLĐ | M | Chỉ HR duyệt ngoại lệ (BR-008) |
| R-003 | PGD vừa A vừa BA soạn | M | **Đóng:** BA = Trịnh Yên (DEC-DIS-012); PGD chỉ A |
| R-004 | N+3 vs quyết toán lương lệch | M | LIF checklist tách bước lương / khóa TK |
| R-005 | PII + lương trên mobile | H | IAM + UAT cô lập phiếu lương |

## 9. Khuyến nghị

**Proceed** — DOC-01 v0.2 **chốt** (DEC-DIS-011). Defer **baseline repo** đến khi PGD ký DOC-03. Nợ OPEX / % giờ HR / chữ ký Ban NS **không** mở lại cổng vision trừ CR.

## 10. Phê duyệt

| Vai trò | Họ tên | Chữ ký | Ngày |
|---------|--------|--------|------|
| Sponsor / A (tầm nhìn) | Mr. Dư Hùng, PGD | **Chốt** v0.2 (DEC-DIS-011) | 2026-08-24 |
| Business Owner HR | Ban Nhân sự mInvoice | | ☐ Nợ — không chặn cổng vision |
| BA (R soạn) | Trịnh Yên | R — DEC-DIS-012 | 2026-08-24 |
