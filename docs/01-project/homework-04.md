# Homework 04 — Câu hỏi & câu trả lời (25/08/2026, 11h50–12h05)

Nguồn: phiên Minipower HRM cùng ngày. Không gồm khối Git/SSH sáng sớm (~8h20–8h55).

---

## Câu 1 — DOC-01: mục tiêu đã có chỉ số đo được chưa? Ai là người duyệt?

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-01-vision-business-case.md` — mục tiêu đã có chỉ số đo được chưa? Ai là người duyệt?

**Trả lời:**

G-001…G-007 **có KPI**, mức đo **không đều**. Timeline chung: go-live **2027**.

| Goal | Đo được? | Ghi chú |
|------|----------|---------|
| G-003 | Có | 1000 dòng &lt;5s; 100% bắt lỗi |
| G-004 | Có | Sai số **0 đồng**; 85% TV |
| G-005 | Có | 0 sót, 0 trễ (T-15/T-7, SN, lễ) |
| G-007 | Có (UAT) | NV/Manager không xem lương người khác |
| G-001 | Một phần | Có format MNV + checklist; chưa %/số hồ sơ |
| G-002 | Một phần | Có rule trừ quỹ / 3 NLĐ; chưa % đơn đúng hạn |
| G-006 | Yếu | “MVP” — chưa số màn/tỷ lệ dùng |

§6 không phải KPI mục tiêu: số lần sự cố lương TBD; % giờ 3 HR chưa; OPEX TBD. As-is §3 chưa đo.

**Người duyệt (§10)**

| Vai trò | Người | Trạng thái |
|---------|--------|------------|
| A — chốt tầm nhìn | Mr. Dư Hùng, PGD | Đã **Chốt** v0.2 (DEC-DIS-011), 2026-08-24 |
| Business Owner | Ban Nhân sự | ☐ chưa ký (không chặn cổng vision) |
| R soạn | Trịnh Yên (BA) | Soạn, không phải A |

A ký BRD trên giấy vẫn PGD (C-005); tài liệu = **DOC-03**, không phải DOC-01.

---

## Câu 2 — DOC-02: ai là A trong RACI?

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-02-stakeholder-analysis.md` — ai là A trong RACI?

**Trả lời:**

A **không** phải một người cho mọi dòng — RACI §4:

| Deliverable | A (Accountable) |
|-------------|-----------------|
| Approve BRD | SH-001 — Mr. Dư Hùng, PGD |
| Công thức lương / 85% | SH-001 — PGD |
| Mẫu Excel công | SH-002 — Ban Nhân sự / C&B |
| N+3 khóa Git/CRM | SH-001 — PGD |
| UAT lương 0 đồng | SH-002 — Ban NS / C&B |

A ký BRD / A stakeholder (cổng DOC-02): PGD Dư Hùng (A-SH-01).  
R soạn: SH-009 Trịnh Yên — không phải A (DEC-DIS-012).

Thiếu cột SH-003/008 nên Ban QT và BGĐ không có A trên bảng này.

---

## Câu 3 — DOC-03: cái gì trong phạm vi, cái gì ngoài?

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-03-brd.md` — cái gì trong phạm vi, cái gì ngoài?

**Trả lời:**

**Trong phạm vi** — HRM nội bộ, web + mobile MVP:

| Module | Việc |
|--------|------|
| EMP | Hồ sơ, unique CCCD/email Cty/MST, org, HĐ, thâm niên, đổi LM |
| LEV | Phép 2 cấp, quỹ khi HR duyệt, ốm/BHXH, bàn giao, 3 ngày công chuẩn, đột xuất chỉ HR |
| TIM | Một mẫu Excel Cty (nội dung động), preview, chốt tháng, OT, phép đã duyệt |
| PAY | N_tính đã sửa, PC HĐ + tháng, BH/TNCN tạm, phiếu PDF/email/mobile mình |
| PRB | Cảnh báo TV T-15/T-7; Đạt / gia hạn / không đạt |
| EVT | SN tuần 08:00; lễ T-7 |
| LIF | Checklist on/off; khóa Git/CRM N+3 |
| RPT | Turnover, cơ cấu, quỹ lương/OT (Should, vẫn in) |
| IAM | Bảng III; Manager không xem lương |

**Ngoài phạm vi:** hardware máy CC (chỉ import Excel); ATS đầy đủ; sổ cái KT / nộp BH nhà nước tự động; chữ ký số HĐLĐ với CQNN (trừ khi bổ sung); thông báo phép sang CRM bán hàng.

**Biên giới:** máy CC xuất 1 mẫu; cấp/khóa email–Git–CRM sản phẩm–chat; thông báo Email/App/HRM. Khóa Git/CRM = in; bắn tin sang CRM bán hàng = out.

---

## Câu 4 — Dự án đang ở phase nào? Liệt kê DOC đã có và trạng thái

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project` — dự án đang ở phase nào? Liệt kê DOC đã có và trạng thái.

**Trả lời:**

**Phase đang làm: requirements** (`profile.json`). Discovery 01–03 đã chốt. Slice leave xong; payroll DOC-04 đã chốt; chưa UC/SRS payroll.

### docs/01-project/

| DOC | File | Trạng thái |
|-----|------|------------|
| DOC-01 | DOC-01-vision-business-case.md | **Chốt** v0.2 · DEC-DIS-011 |
| DOC-02 | DOC-02-stakeholder-analysis.md | **Chốt** v0.2 · DEC-DIS-006+012 |
| DOC-03 | DOC-03-brd.md | **Chốt** v0.7 · DEC-DIS-015/016 |
| — | homework-04.md | Bài tập, không cổng Minipower |

Ban NS ☐ trên 01–03. Chưa snapshot baseline repo.

### Module đã có DOC

| DOC | Module | Trạng thái |
|-----|--------|------------|
| DOC-04 | leave | **Chốt** v0.3 |
| DOC-05 | leave | **Chốt** v0.2 |
| DOC-06 | leave | **Chốt** v0.2 |
| DOC-07 | leave | **Chốt** v0.1 |
| DOC-19 | leave | **Chốt khung** (HTML nợ) |
| DOC-04 | payroll | **Chốt** v0.1 · 05/19 chưa |

### Skeleton / chưa

DOC-15, DOC-18: template Draft. DOC-08–14, 13, 16, 17: chưa (trừ ADR README). EMP, TIM, PRB, EVT, LIF, RPT, IAM: chưa folder DOC. CRM PARKED.

---

## Câu 5 — BRD đã đủ chưa? Thiếu mục nào?

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-03-brd.md` — BRD đã đủ chưa? Thiếu mục nào?

**Trả lời:**

**Đủ cổng BRD** — DOC-03 v0.7 đã chốt (PGD, DEC-DIS-015 + Phụ lục A). Mục template 1–13 đều có; thêm module index + Phụ lục A.

Không thiếu khung. Còn nợ chữ ký / đóng băng:

- Ban NS Owner ☐
- Snapshot `02-baseline/` ☐
- §11 cột Impact (template có, file không)

Mỏng (đã chấp nhận DEC): TO-BE 1 câu; Rationale BRQ gộp DEC-014; AS-IS không inventory tool.

Lỗ nội dung (không chặn cổng): BO-005 / BO-007 không có BRQ nghiệp vụ riêng.

UC/FR/AC không thuộc BRD. Sửa in/out sau chốt = CR.

---

## Câu 6 — Mỗi BRQ ở mục 7 phục vụ BO nào ở mục 3?

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-03-brd.md` — với mỗi BRQ ở mục 7, cho biết nó phục vụ mục tiêu BO nào ở mục 3. Trình bày thành bảng.

**Trả lời:**

Mapping đã nằm Phụ lục A DOC-03:

| BRQ | Phục vụ BO | Mục tiêu (tóm) |
|-----|------------|----------------|
| BRQ-001 | BO-001…007 | Ô dù 8 phân hệ + IAM. BO-007 chỉ đi qua đây |
| BRQ-002 | BO-004 | N_tính → lương đúng quy chế |
| BRQ-003 | BO-004 | 85% thử việc |
| BRQ-004 | BO-003 | Một mẫu Excel công |
| BRQ-005 | BO-001 | Vòng đời / N+3 |
| BRQ-006 | BO-006 | Mobile MVP |
| BRQ-007 | BO-004 | Phụ cấp/thưởng trên phiếu |
| BRQ-008 | BO-002 | 3 ngày công chuẩn + hạn nộp |
| BRQ-009 | BO-003, 004, 005, 006 | UAT: &lt;5s; 0đ; cảnh báo; cô lập phiếu |
| BRQ-010 | BO-002 | Đột xuất chỉ HR |

Lỗ: BO-005 không có BRQ nghiệp vụ riêng; BO-007 (Should) không có BRQ Must riêng.

---

## Câu 7 — Có BO nào chưa có BRQ nào phục vụ không? Liệt kê ra

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-03-brd.md` — có mục tiêu BO nào ở mục 3 mà chưa có BRQ nào phục vụ không? Liệt kê ra.

**Trả lời:**

Không có BO nào = 0 BRQ nếu tính BRQ-001 (phủ BO-001…007).

Chỗ mỏng (không trống, thiếu BRQ *riêng*):

| BO | Mục tiêu | BRQ ngoài 001 |
|----|----------|----------------|
| BO-005 | Cảnh báo TV / SN / lễ | Chỉ BRQ-009 (UAT) — không có BRQ nghiệp vụ T-15/T-7, SN, lễ |
| BO-007 | Analytics HR (Should) | Không — chỉ BRQ-001 |

BO-001…004 và BO-006 đều có BRQ chuyên biệt.

---

## Câu 8 — Có BRQ nào không phục vụ BO nào không? Liệt kê ra

**Câu hỏi (gốc):** `/minipower` Phase: discovery · `@docs/01-project/DOC-03-brd.md` — có BRQ nào ở mục 7 không phục vụ mục tiêu BO nào không? Liệt kê ra.

**Trả lời:**

Không. Mục 7 không có BRQ “mồ côi” — cả 10 dòng phục vụ ≥1 BO.

**Danh sách lỗ = rỗng.** Không có ID nào để liệt kê.

---

## Câu 9 — Mỗi BRQ trong BRD đã có FR nào ở DOC-06 giải quyết chưa?

**Câu hỏi (gốc):** `/minipower` · `HRM/docs/03-modules/leave/DOC-06-srs.md` — mỗi BRQ trong BRD đã có FR nào ở DOC-06 giải quyết chưa?

**Trả lời:**

DOC-06 leave (đã chốt) chỉ cover phép. Bảng §6.1:

| BRQ | FR trên DOC-06 | Đã giải? |
|-----|----------------|----------|
| BRQ-001 | LEV-FR-001…018 (phép + role C1/C2) | Một phần — 7 phân hệ kia + IAM đầy đủ ở SRS khác |
| BRQ-002 | — | Chưa (payroll · PAY-BR-001) |
| BRQ-003 | — | Chưa (payroll · PAY-BR-003) |
| BRQ-004 | — | Chưa (timekeeping) |
| BRQ-005 | — | Chưa (lifecycle) |
| BRQ-006 | LEV-FR-002, 001, 015 | Có kênh phép; hồ sơ/phiếu lương → module khác |
| BRQ-007 | — | Chưa (payroll · PAY-BR-005) |
| BRQ-008 | LEV-FR-006, 007 | Có |
| BRQ-009 | — | Chưa trên leave (UAT lương/công/cảnh báo/phiếu) |
| BRQ-010 | LEV-FR-007, 010, 011, 012, 017, 018 | Có |

Đã xong trên leave: 006 (một phần), 008, 010, phần phép của 001.

---

## Câu 10 — Tiếp theo tôi cần làm gì?

**Câu hỏi (gốc):** `/minipower` — tiếp theo tôi cần làm gì? Trả lời ngắn, theo ưu tiên.

**Trả lời:**

1. Payroll: DOC-05 (UC) rồi DOC-19 khung → DOC-06/07. DOC-04 PAY đã chốt.
2. Hoặc DOC-04 timekeeping (BRQ-004) nếu ưu tiên công trước lương.
3. Không architecture / baseline / CRM.
4. Tuỳ chọn: OQ-010; Ban NS; DOC-16 leave.

---

## Câu 11 — Dự án nhỏ, một người làm. Bước nào rút gọn được? Bỏ thì mất gì?

**Câu hỏi (gốc):** `/minipower` — dự án nhỏ, một người làm. Bước nào rút gọn được? Bỏ thì mất gì?

**Trả lời:**

Một người rút ceremony, không rút 8 module + lương + PII. Rubric: Small = 1 module; HRM sát Large. Discovery 01–03 và leave 04–07+19 khung đã xong — đừng làm lại.

**Rút được**

| Bước | Rút | Bỏ thì mất |
|------|-----|------------|
| `02-baseline/` | Hoãn | CR/lịch sử ký khó tách |
| Fan-out 8 module | 1 module (đang payroll) | Module kia không có BR/FR |
| DOC-19 HTML | Khung chữ (như leave) | Wireframe bấm được |
| DOC-13 full | 1 trang bảo mật lương | QC thiếu NFR |
| DOC-14/15 | Mốc 2026/2027 | Ước lượng không giấy |
| DOC-08–12 | SAD mỏng + vài ADR lúc code | Đoán Git/CRM/N+3 |
| DOC-16 mọi module | Chỉ leave + payroll khi UAT | Test không bám AC |
| Review mọi slice | Chỉ khi đổi BR lương/phép | Lỗ công thức |

**Không rút:** payroll DOC-05→07 (04 đã chốt); DOC-04 timekeeping trước khi code công; cổng chốt BR / khung 19 / SRS từng module; cô lập phiếu (IAM).

Đã rút sẵn: Ban NS nợ; ROI %; HTML MCP; không CRM.
