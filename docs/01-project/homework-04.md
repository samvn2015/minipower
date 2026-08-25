# Homework 04 — Câu hỏi & câu trả lời (15–28)

Nguồn: phiên Minipower HRM · 2026-08-24.  
Đánh số theo danh sách “liệt kê các câu hỏi tôi đã hỏi”.

---

## Câu 15 — Có BO nào ở DOC-03 §3 chưa có BRQ nào phục vụ không?

**Câu hỏi (gốc):** `@DOC-03-brd.md` / HRM DOC-03 — có mục tiêu BO nào ở mục 3 mà chưa có BRQ nào phục vụ không?

**Trả lời:**

Không. Không BO nào bị **0 BRQ** nếu tính **BRQ-001** (ô dù 8 phân hệ + IAM): **BO-001…007** đều được phủ.

Nếu **không** coi BRQ-001 là đủ:

| BO | BRQ riêng (ngoài 001) | Kết luận |
|----|------------------------|----------|
| BO-001 | BRQ-005 | Có |
| BO-002 | BRQ-008, 010 | Có |
| BO-003 | BRQ-004, 009 | Có |
| BO-004 | BRQ-002, 003, 007, 009 | Có |
| BO-005 | chỉ BRQ-009 (UAT “cảnh báo đúng”) | Thiếu BRQ nghiệp vụ (T-15/T-7, SN, lễ) |
| BO-006 | BRQ-006, 009 | Có |
| BO-007 | không | Chỉ sống nhờ BRQ-001; Should, không có BRQ Must riêng |

Hai chỗ mỏng: **BO-005** và **BO-007** — đúng lỗ Phụ lục A DOC-03.

---

## Câu 16 — Có BRQ nào không phục vụ mục tiêu BO nào không?

**Câu hỏi (gốc):** Có BRQ nào ở các mục không phục vụ mục tiêu BO nào không?

**Trả lời:**

Không. Không BRQ nào ở §7 **không** phục vụ BO. Mỗi dòng gắn ≥1 mục tiêu (Phụ lục A):

| BRQ | BO |
|-----|-----|
| 001 | 001…007 |
| 002, 003, 007 | 004 |
| 004 | 003 |
| 005 | 001 |
| 006 | 006 |
| 008, 010 | 002 |
| 009 | 003, 004, 005, 006 |

Không có BRQ “mồ côi”. Lỗ đang là **BO thiếu BRQ riêng** (005, 007), không phải BRQ thừa.

---

## Câu 17 — BRD đã đủ chưa? Thiếu mục nào?

**Câu hỏi (gốc):** `@docs/01-project/DOC-03-brd.md` — BRD đã đủ chưa? Thiếu mục nào?

**Trả lời:**

**Đủ để chốt cổng BRD** — DOC-03 **v0.7 đã chốt** (PGD, DEC-DIS-015). In/out + module index đạt exit discovery bước 2. **Không** thiếu mục template (1–13 đều có).

**Thiếu chữ ký / đóng băng (không phải thiếu mục)**
- Ban NS Owner ☐
- Cột Baseline / `02-baseline/` ☐
- §11 thiếu cột **Impact** (template có; đã có Owner)

**Mỏng so với template (đã chấp nhận DEC)**
- §6 TO-BE: 1 câu, không bảng Process / Lợi ích
- §7 không cột **Rationale** từng dòng (gộp DEC-DIS-014)
- AS-IS không inventory tool (cố ý)

**Lỗ nội dung (không chặn cổng)**
- **BO-005 / BO-007** không có BRQ nghiệp vụ riêng
- BRQ-001 ô dù; cảnh báo TV/SN/lễ và analytics chờ DOC-04 module

**Không thuộc BRD:** UC/FR/AC (DOC-05–07). Việc tiếp theo là requirements, không viết thêm mục BRD trừ khi CR.

---

## Câu 18 — Dự án nhỏ, một người làm. Bước nào rút gọn được? Bỏ thì mất gì?

**Câu hỏi (gốc):** `@docs/01-project` — dự án nhỏ, một người làm. Bước nào rút gọn được? Bỏ thì mất gì?

**Trả lời:**

**01–03 đã chốt** — discovery không cần làm lại. “Một người” **rút được ceremony**, không rút **phạm vi HRM** (8 module + lương + PII). Rubric Minipower: Small = 0–5 điểm / 1 module; HRM sát **Large**.

### Đã rút / có thể rút tiếp

| Bước | Rút | Bỏ thì mất |
|------|-----|------------|
| Snapshot `02-baseline/` | Hoãn đến khi chủ đích đóng băng | Lịch sử ký, CR sau này khó tách |
| Fan-out 8 module cùng lúc | **1 module** (leave rồi payroll) | Module kia không có BR/FR — chấp nhận nợ |
| DOC-19 HTML | Khung chữ/mermaid | Wireframe bấm được; lệch UI lúc code |
| DOC-02 mermaid / RACI đủ cột | Giữ sơ bộ | Tranh A/R khi có người thứ 2 |
| DOC-13 NFR full | 1 trang bảo mật lương + audit | QC không có tiêu chí phi chức năng |
| DOC-14/15 WBS đầy | Milestone 2026 xây / 2027 dùng | Lịch & ước lượng không giấy |
| DOC-08–12 đầy đủ | 1 SAD mỏng + vài ADR | Dev đoán tích hợp Git/CRM/N+3 |
| Deliberation / doc-review mọi slice | Chỉ lúc đổi BR lương/phép | Lỗ công thức, mâu thuẫn chéo |

### Không rút (lương + phép)

| Bước | Vì sao |
|------|--------|
| DOC-04 leave + DOC-06/07 leave | Không SRS/AC → code phép đoán |
| DOC-04+06 payroll khi đụng lương | BRQ-002/003 không nằm leave |
| Cổng chốt BR / Prototype khung / SRS từng module | Một người vẫn cần “đóng” kẻo tự sửa vòng |
| IAM / cô lập phiếu | BRQ-009, CN-002 |

---

## Câu 19 — Tiếp theo tôi cần làm gì?

**Câu hỏi (gốc):** Tiếp theo tôi cần làm gì? Trả lời ngắn, theo ưu tiên. *(hỏi lại nhiều lần; dưới đây là các lần trả lời chính)*

**Trả lời (sau khi DOC-03 chốt, leave chưa xong):**

1. Chốt DOC-06 leave (DEC-REQ-007) rồi DOC-07.
2. Không snapshot `02-baseline/` cho đến khi chủ đích baseline.
3. Fan-out DOC-04 module Must còn lại — từng module, không CRM.
4. Tuỳ chọn: Ban NS ký; BRQ riêng BO-005 / BO-007.

**Sau khi chốt DOC-06:** viết + chốt DOC-07; không baseline / CRM; rồi DOC-04 payroll hoặc timekeeping.

**Sau khi chốt DOC-07:** chốt DOC-05 (còn Draft) hoặc chấp nhận nợ; rồi payroll/công; không architecture.

**Sau khi chốt DOC-05:** DOC-04 payroll hoặc timekeeping; không architecture / baseline / CRM; tuỳ chọn OQ-010, Ban NS, DOC-16.

**Sau khi soi DOC-04 leave (đã chốt, stale):** tạo DOC-04 payroll hoặc timekeeping; tuỳ chọn dọn stale leave DOC-04.

---

## Câu 20 — DOC-06 (DEC-REQ-007) nằm ở thư mục nào?

**Câu hỏi (gốc):** DOC-06 (DEC-REQ-007) nằm ở thư mục nào?

**Trả lời:**

- **DOC-06** (SRS leave): `HRM/docs/03-modules/leave/DOC-06-srs.md`
- **DEC-REQ-007:** `HRM/memory/requirements/decision-log.md`

---

## Câu 21 — Mỗi BRQ trong BRD đã có FR nào ở DOC-06 giải quyết chưa?

**Câu hỏi (gốc):** `@docs/03-modules/leave/DOC-06-srs.md` — mỗi BRQ trong BRD đã có FR nào ở DOC-06 giải quyết chưa?

**Trả lời:**

DOC-06 leave **chỉ** giải phần phép. BRQ lương/công/LIF **đúng là chưa có FR** ở file này.

| BRQ BRD | FR trên DOC-06 leave | Kết luận |
|---------|----------------------|----------|
| BRQ-001 | LEV-FR-001…018 (phần phép + IAM role C1/C2) | Một phần. 7 phân hệ kia + IAM đầy đủ → DOC-06 module khác. |
| BRQ-002 | — | Không. N_tính = payroll. |
| BRQ-003 | — | Không. 85% TV = payroll. |
| BRQ-004 | — | Không. Excel công = timekeeping. |
| BRQ-005 | — | Không. N+3 = lifecycle. |
| BRQ-006 | LEV-FR-002; FR-001/015 | Có kênh phép. Hồ sơ/phiếu lương mobile → module khác. |
| BRQ-007 | — | Không. Phụ cấp = payroll. |
| BRQ-008 | LEV-FR-006, 007 | Có. |
| BRQ-009 | — | Không. UAT 0đ / import / cảnh báo / phiếu ≠ leave. |
| BRQ-010 | LEV-FR-007, 010, 011, 012, 017, 018 | Có. |

Đã cover trên leave: BRQ-006 (một phần), **008**, **010**, phần phép của **001**.  
Chưa (đúng chỗ khác): 002, 003, 004, 005, 007, 009.

---

## Câu 22 — Cập nhật mapping BRQ→FR vào DOC-06

**Câu hỏi (gốc):** Cập nhật vào DOC-06.

**Trả lời / việc đã làm:**

Bảng BRQ→FR ghi vào **DOC-06 §6.1**, phiên bản **0.2** Draft (lúc đó). Tham chiếu DOC-03 = **Chốt**. BA = Trịnh Yên. Cổng DEC-REQ-007 lúc đó vẫn proposed.

File: `HRM/docs/03-modules/leave/DOC-06-srs.md`.

---

## Câu 23 — Chốt DOC-06

**Câu hỏi (gốc):** Chốt DOC-06.

**Trả lời / việc đã làm:**

DOC-06 leave **đã chốt** — **v0.2**, **DEC-REQ-007**, PGD 2026-08-24.

Nợ: OQ-REQ-010 (chỉ NV hủy); DOC-13; Ban HR ☐; HTML MCP; chưa `02-baseline/`. Sửa FR sau = CR.

Cổng SRS leave mở **DOC-07**.

---

## Câu 24 — Viết + chốt DOC-07 (AC) cùng slice phép

**Câu hỏi (gốc):** “Viết + chốt DOC-07 (AC) cùng slice phép — thực hiện luôn.”

**Trả lời / việc đã làm:**

File: `HRM/docs/03-modules/leave/DOC-07-acceptance-criteria.md`  
**DEC-REQ-008** · **LEV-AC-001…018** (Gherkin happy + negative) · PGD 2026-08-24.

Nợ: OQ-010; DOC-13; Ban HR; DOC-16 trống; DOC-05 lúc đó còn Draft. Không snapshot baseline.

Slice phép (04 + 19 khung + 06 + 07) đóng cổng AC.

---

## Câu 25 — DOC-05 còn tồn tại gì? (rồi chốt)

**Câu hỏi (gốc):** DOC-05 còn tồn tại gì? → rồi: chốt.

**Trả lời (tồn tại trước chốt):**

DOC-05 **Draft 0.1**. UC-001…004 đã khớp 04/06/07; tồn tại chủ yếu cổng + lệch formal.

- Status Draft; BA header còn Dư Hùng.
- UC-005, UC-006 chỉ Casual — thiếu Fully Dressed.
- IT (ACT-004) không có UC (FR-017 nằm UC-003 EF-2).
- Chưa phê duyệt.
- Trace lệch: UC-001 thiếu FR-009; UC-003 thiếu FR-008; UC-005/006 chưa FR-015/016.
- Nợ OQ-REQ-010.

**Sau khi chốt:** DOC-05 **v0.2**, **DEC-REQ-009**. Bổ sung dressed UC-005/006, gắn FR-008/009/015/016, BA Trịnh Yên. Nợ: OQ-010; IT không UC riêng; Ban HR ☐. Slice leave **04–07 + 19 khung** đủ cổng.

---

## Câu 26 — DOC-04 còn tồn tại gì?

**Câu hỏi (gốc):** DOC-04 tồn tại gì *(DOC-04 leave, file duy nhất lúc hỏi)*.

**Trả lời:**

DOC-04 **leave đã chốt** (v0.3, DEC-REQ-005). Còn lệch giấy / nợ, không thiếu rule.

**Stale:** header BA Dư Hùng; cổng ghi chưa DOC-06 trong khi 05–07 đã chốt; LEV-BR-001 Trace UC TBD; chưa bảng phê duyệt.

**Nợ lúc chốt:** OQ-REQ-010; Ban HR / `02-baseline/` ☐.

**Dễ hiểu nhầm:** §2 LEV-BR-005 “chặn, trừ KHL”; LEV-BR-011 không nêu thông báo hủy; LEV-BR-009 chữ *HR acting* vs không bypass C1.

Không có DOC-04 module khác (payroll/công chưa tạo) — đó là bước tiếp, không phải lỗ trên file leave. Sửa BR đã chốt = CR.

---

## Câu 27 — Tạo file DOC-04 payroll

**Câu hỏi (gốc):** DOC-04 payroll — tạo file.

**Trả lời / việc đã làm:**

Tạo **Draft**, **DEC-REQ-010**.

Path: `HRM/docs/03-modules/payroll/DOC-04-business-rules.md`  
**PAY-BR-001…012:** N_tính không cộng kép; trần ngày công; TV 85%; OT từ công chốt; PC hai kênh (master động); BH/TNCN động; cô lập phiếu; UAT 0đ; không sửa công trong PAY.

Chưa chốt (tại thời điểm tạo). Chưa UC/Prototype/SRS. Leave không đụng. Danh mục PC không đóng trên file (DEC-DIS-014).

---

## Câu 28 — Nếu không cần bổ sung gì thêm thì chốt (DOC-04 payroll)

**Câu hỏi (gốc):** DOC-04 payroll — nếu không cần bổ sung gì thêm thì chốt.

**Trả lời / việc đã làm:**

Không bổ sung rule mới — BRQ lương trên BRD đã đủ PAY-BR-001…012.

DOC-04 payroll **đã chốt** (v0.1, **DEC-REQ-011**).

Nợ cố ý: làm tròn → DOC-07; PC/BH master động; Ban HR ☐. Tạm ứng/kế toán không nằm BRD nên không nhồi.

Cổng BR payroll mở DOC-05 / DOC-19 (chưa tự viết). Sửa BR sau = CR.
