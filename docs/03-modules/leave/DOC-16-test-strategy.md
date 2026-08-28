# DOC-16 — Chiến lược Kiểm thử & Test Cases (leave)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA) | **Chốt** (DEC-DLV-004) |

**Module:** leave · **MOD:** `LEV`  
**ISTQB** · Trace [DOC-07](DOC-07-acceptance-criteria.md) **Chốt** · [DOC-06](DOC-06-srs.md) **Chốt** · API [DOC-12](../../04-platform/DOC-12-api-spec/DOC-12-api-specification.md) `/lev/*`.  
**Cổng:** PGD chốt v0.1 (DEC-DLV-004). Nợ: OQ-REQ-010; HTML MCP; Ban HR ☐; chưa chạy TC. Sửa catalog đã chốt = CR. **Không** tự code. **Chưa** `02-baseline/`.

---

## 1. Mục đích

TC Must (+ Should AC-016) slice **phép**. Không cover PAY, Excel TIM, N+3.

Môi trường E2E: **sau LBS+GW+OIDC** (DOC-16 chương trình).  
**§3:** bước/precondition theo Gherkin DOC-07. ID catalog **không đổi**. Mô tả ngắn TC-006/011 lệch AC → QC chạy theo §3.

## 2. Danh mục test case

### 2.1 Nộp đơn — C1/C2 — quỹ — hủy

| TC ID | Mô tả | Kết quả mong muốn | Layer | Path | Priority | Trạng thái |
|-------|-------|-------------------|-------|------|----------|------------|
| LEV-TC-001 | Form 6 loại + bàn giao khác mình | Chờ C1 | E2E | Happy | Must | |
| LEV-TC-001n | Bàn giao = chính mình | Chặn | API | Unhappy | Must | |
| LEV-TC-002 | Cùng rule mobile | Hành vi = web | E2E | Happy | Must | |
| LEV-TC-003 | Overlap đơn Open | Chặn | API | Unhappy | Must | |
| LEV-TC-004 | Quỹ năm thiếu (loại trừ quỹ) | Chặn | API | Unhappy | Must | |
| LEV-TC-005 | Loại không trừ quỹ năm | Không trừ khi nộp | API | Happy | Must | |
| LEV-TC-006 | ≥3 ngày công chuẩn liền, nộp trễ hạn | Đột xuất; không chặn submit | E2E | Happy | Must | |
| LEV-TC-007 | Đột xuất: C1 không trừ quỹ | Quỹ nguyên đến C2 | API | Happy | Must | |
| LEV-TC-008 | Ốm/BHXH thiếu file mẫu Cty | Chặn | E2E | Unhappy | Must | |
| LEV-TC-009 | Notify in-app/mail; **0** CRM sales | Có kênh HRM; INT-006 fail nếu có call | E2E | Happy | Must | |
| LEV-TC-010 | C1 chỉ LM; không Matrix; NV không tự C1 | 403 sai role | API | Unhappy | Must | |
| LEV-TC-011 | C2 sau C1 kể cả đột xuất | HR C2 được | E2E | Happy | Must | |
| LEV-TC-012 | C2 atomic trừ quỹ năm | Trừ đúng 1 lần; fail → không trừ dở | API | Happy | Must | |
| LEV-TC-013 | NV hủy trước C2; nộp lại cùng ngày | Hủy OK; nộp lại được | E2E | Happy | Must | |
| LEV-TC-014 | Hủy/hoàn quỹ sau C2 | Chặn | API | Unhappy | Must | |
| LEV-TC-015 | NV xem quỹ mình | 200 số dư | E2E | Happy | Must | |
| LEV-TC-016 | Trần catalog trống vs có số | Should — theo catalog | API | Happy | Should | |
| LEV-TC-017 | Manager/IT không C2 | 403 | API | Unhappy | Must | |
| LEV-TC-018 | C1 không trừ quỹ (đơn thường) | Quỹ nguyên sau C1 | API | Happy | Must | |
| LEV-TC-019 | OQ-010: LM/HR hủy hộ | **Skip** MVP — giả định chỉ NV hủy mình | — | — | — | Skip |

## 3. Chi tiết test case

Quy ước: Bearer OIDC. Path khung DOC-12: `POST /lev/requests`, `POST /lev/requests/{id}/c1|c2`, `GET /lev/balances`. Hủy đơn: UI/API module LEV — path hủy **chưa** trên DOC-12 khung; không bịa URL. Không đo % SLA.

### LEV-TC-001 — Nộp đơn hợp lệ

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-001, LEV-AC-001, LEV-UC-001 |
| **Preconditions** | JWT NV; người bàn giao **active ≠** NV; đủ quỹ nếu loại trừ năm |
| **Steps** | 1. Mở form web LEV-SCR-002. 2. Chọn 1/6 loại; Từ–Đến; nhãn buổi; lý do; 1 người bàn giao. 3. `POST /lev/requests` (hoặc Submit UI). |
| **Expected** | 201; trạng thái **Chờ C1**; quỹ chưa trừ (xem TC-018). |
| **Layer / Path** | E2E · Happy |
| **Status** | |

### LEV-TC-001n — Bàn giao = chính mình

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-001, LEV-AC-001 (negative) |
| **Preconditions** | JWT NV |
| **Steps** | 1. Chọn bàn giao = chính NV. 2. Submit. |
| **Expected** | Chặn; không tạo đơn Open; quỹ không đổi. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### LEV-TC-002 — Cùng rule mobile

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-002, LEV-AC-002, LEV-UC-001 |
| **Preconditions** | Cùng payload đã pass/fail trên web |
| **Steps** | **H:** LEV-SCR-003 + dữ liệu TC-001 → Submit. **U:** payload web đã chặn (overlap / quỹ / 3 NLĐ) → Submit mobile. |
| **Expected** | **H:** cùng Chờ C1. **U:** chặn **cùng lý do** web. |
| **Layer / Path** | E2E · Happy (+ Unhappy cặp) |
| **Status** | |

### LEV-TC-003 — Overlap đơn Open

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-003, LEV-AC-003 |
| **Preconditions** | Đơn Open: Chờ C1 **hoặc** Chờ C2 **hoặc** Đã duyệt; trùng ngày/buổi |
| **Steps** | 1. Ghi quỹ = X. 2. `POST /lev/requests` trùng. |
| **Expected** | Chặn; quỹ vẫn X; đơn cũ không đổi. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### LEV-TC-004 — Quỹ năm thiếu

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-004, LEV-AC-004 |
| **Preconditions** | Loại **trừ quỹ năm**; số ngày đơn > quỹ còn |
| **Steps** | **A:** NV submit. **B:** đơn đã C1, quỹ bị kéo xuống thiếu trước C2; JWT HR `POST .../c2`. |
| **Expected** | Chặn cả A và B; quỹ không trừ (B: đơn không Đã duyệt). |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### LEV-TC-005 — Loại không trừ quỹ năm

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-005, LEV-AC-005 |
| **Preconditions** | Loại không trừ năm (ốm/KHL/kết hôn/tang/chế độ theo SRS); quỹ năm = X |
| **Steps** | 1. Submit hợp lệ. 2. LM C1. 3. HR C2 hợp lệ (file nếu ốm). |
| **Expected** | Sau submit **và** sau C2: quỹ năm = X. |
| **Layer / Path** | API · Happy |
| **Status** | |

### LEV-TC-006 — Hạn 3 ngày công liền (bám AC-006, không bám mô tả catalog “đột xuất”)

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-006, LEV-AC-006 |
| **Preconditions** | **N:** ≥3 ngày công chuẩn liền; NLĐ tới ngày bắt đầu < 3; **không** cờ đột xuất. **H:** nghỉ **2** ngày công liền, nộp sát ngày. |
| **Steps** | **N:** Submit không cờ. **H:** Submit 2 ngày. |
| **Expected** | **N:** chặn + gợi ý đánh dấu đột xuất. **H:** không áp hạn 3 NLĐ (hợp lệ khác vẫn pass). |
| **Layer / Path** | E2E · Unhappy + Happy |
| **Status** | |

### LEV-TC-007 — Đột xuất: submit được; C1 không trừ quỹ

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-007, LEV-AC-007 |
| **Preconditions** | Trễ hạn 3 NLĐ; **có** cờ Nghỉ đột xuất; quỹ = X |
| **Steps** | 1. Submit. 2. LM `POST .../c1`. |
| **Expected** | Sau 1: Chờ C1; quỹ = X. Sau 2: quỹ = X. |
| **Layer / Path** | API · Happy |
| **Status** | |

### LEV-TC-008 — Ốm/BHXH thiếu file mẫu Cty

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-008, LEV-AC-008 |
| **Preconditions** | Loại ốm/BHXH; thiếu file đúng mẫu Cty |
| **Steps** | **A:** NV submit. **B:** (nếu lọt) HR `POST .../c2`. |
| **Expected** | Chặn A; B cũng chặn nếu tới C2. |
| **Layer / Path** | E2E · Unhappy |
| **Status** | |

### LEV-TC-009 — Notify HRM; 0 CRM sales

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-009, LEV-AC-009, NFR-007, INT-006 |
| **Preconditions** | Probe/log outbound; **cấm** INT-006 |
| **Steps** | Lần lượt: nộp / C1 / C2 / từ chối C1 (có lý do) / hủy trước C2. Bắt traffic. |
| **Expected** | Mỗi sự kiện: Email **và/hoặc** in-app HRM. **0** call CRM sales. |
| **Layer / Path** | E2E · Happy |
| **Severity nếu fail** | Blocker go-live |
| **Status** | |

### LEV-TC-010 — C1 chỉ LM đúng cây

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-010, LEV-AC-010, LEV-UC-002 |
| **Preconditions** | Đơn Chờ C1; quỹ = X |
| **Steps** | **H:** JWT LM đúng cây → `POST .../c1` phê duyệt. **U1:** Manager khác / Matrix / NV tự C1 → `POST .../c1`. **U2:** Từ chối C1 **không** lý do. |
| **Expected** | **H:** Chờ C2; quỹ = X. **U1:** 403; quỹ = X. **U2:** không từ chối được (bắt buộc lý do). |
| **Layer / Path** | API · Happy + Unhappy |
| **Status** | |

### LEV-TC-011 — C2 bắt buộc đã C1 (bám AC-011; catalog “C2 sau C1” = nhánh H)

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-011, LEV-AC-011, LEV-UC-003 |
| **Preconditions** | **N:** đơn đột xuất **Chờ C1**. **H:** cùng loại đã **C1**. |
| **Steps** | **N:** JWT HR `POST .../c2`. **H:** JWT HR `POST .../c2`. |
| **Expected** | **N:** từ chối C2; quỹ/trạng thái không Đã duyệt. **H:** C2 được (chi tiết trừ quỹ → TC-012). |
| **Layer / Path** | E2E · Unhappy + Happy |
| **Status** | |

### LEV-TC-012 — C2 atomic trừ quỹ năm

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-012, LEV-AC-012, LEV-UC-003 |
| **Preconditions** | Đã C1; quỹ đủ; JWT HR. **H1:** loại trừ năm. **H2:** cờ đột xuất. |
| **Steps** | 1. `POST .../c2`. 2. (API) giả lập lỗi giữa trừ quỹ và commit trạng thái. |
| **Expected** | **H1:** Đã duyệt **và** quỹ − đúng **cùng lúc**. **H2:** ngoại lệ + Đã duyệt một thao tác. **Lỗi:** quỹ không đổi; đơn không C2 dở. |
| **Layer / Path** | API · Happy / Unhappy |
| **Severity nếu fail** | Blocker (trừ quỹ sai) |
| **Status** | |

### LEV-TC-013 — NV hủy trước C2; nộp lại cùng ngày

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-013, LEV-AC-013, LEV-UC-004 |
| **Preconditions** | Đơn của NV Chờ C1 **hoặc** Chờ C2 |
| **Steps** | 1. NV hủy đơn mình. 2. NV nộp lại cùng ngày/buổi (hợp lệ khác). |
| **Expected** | Đã hủy; ngày không chiếm Open; bước 2 tạo đơn mới được. |
| **Layer / Path** | E2E · Happy |
| **Status** | |

### LEV-TC-014 — Không hủy / hoàn quỹ sau C2

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-014, LEV-AC-014 |
| **Preconditions** | Đơn **Đã duyệt**; quỹ đã trừ (nếu loại trừ năm) = X' |
| **Steps** | NV, LM, HR lần lượt hủy hoặc hoàn quỹ. |
| **Expected** | Từ chối; trạng thái vẫn Đã duyệt; quỹ vẫn X'. |
| **Layer / Path** | API · Unhappy |
| **Status** | |

### LEV-TC-015 — NV xem quỹ mình

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-015, LEV-AC-015, LEV-UC-005 |
| **Preconditions** | JWT NV; có số dư phép năm |
| **Steps** | **H:** UI LEV-SCR-007 hoặc `GET /lev/balances` (mình). **U:** NV mở quỹ đồng nghiệp (không phải LM cấp dưới / không HR). |
| **Expected** | **H:** 200 số dư mình. **U:** từ chối (403). |
| **Layer / Path** | E2E · Happy + Unhappy |
| **Status** | |

### LEV-TC-016 — Trần catalog (Should)

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-016, LEV-AC-016, LEV-UC-006 |
| **Preconditions** | **H:** HR để trống trần loại. **N:** HR trần = 3 ngày loại đó. |
| **Steps** | **H:** NV nộp trong phạm vi rule khác. **N:** NV nộp 4 ngày loại đó. |
| **Expected** | **H:** không chặn vì trần. **N:** chặn. |
| **Layer / Path** | API · Happy + Unhappy |
| **Priority** | Should |
| **Status** | |

### LEV-TC-017 — Manager/IT không C2

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-017, LEV-AC-017 |
| **Preconditions** | Đơn Chờ C2; JWT Manager **hoặc** IT **không** gán HR |
| **Steps** | 1. Mở UI C2. 2. `POST .../c2`. |
| **Expected** | Không thao tác C2 trên UI; API 403; quỹ/trạng thái không đổi. |
| **Layer / Path** | API · Unhappy |
| **Severity nếu fail** | Blocker (C2 lọt) |
| **Status** | |

### LEV-TC-018 — C1 không trừ quỹ (đơn thường)

| Mục | Nội dung |
|-----|----------|
| **Trace** | LEV-FR-018, LEV-AC-018, LEV-UC-002 |
| **Preconditions** | Loại trừ năm; quỹ = X; đơn Chờ C1 (không đột xuất) |
| **Steps** | LM `POST .../c1` phê duyệt. |
| **Expected** | Chờ C2; quỹ = X. |
| **Layer / Path** | API · Happy |
| **Status** | |

### LEV-TC-019 — OQ-010 hủy hộ (Skip MVP)

| Mục | Nội dung |
|-----|----------|
| **Trace** | OQ-REQ-010; LEV-AC-013 negative |
| **Preconditions** | — |
| **Steps** | Không chạy MVP. |
| **Expected** | Skip: chỉ NV hủy đơn mình (TC-013). LM/HR hủy hộ **không** Must. |
| **Layer / Path** | — |
| **Status** | Skip |

**Manual NFR (DOC-07 §4, chưa ID catalog):** LEV-AC-NFR-001 đồng nghiệp / LEV-AC-NFR-002 log C1/C2/hủy — chạy cùng UAT; không bịa SLA.

## 4. Ma trận truy vết

| TC ID | FR | AC | UC | Coverage |
|-------|----|----|-----|----------|
| LEV-TC-001 / 001n | LEV-FR-001 | LEV-AC-001 | LEV-UC-001 | ✅ |
| LEV-TC-002 | LEV-FR-002 | LEV-AC-002 | LEV-UC-001 | ✅ |
| LEV-TC-003 | LEV-FR-003 | LEV-AC-003 | LEV-UC-001 | ✅ |
| LEV-TC-004 | LEV-FR-004 | LEV-AC-004 | LEV-UC-001 | ✅ |
| LEV-TC-005 | LEV-FR-005 | LEV-AC-005 | LEV-UC-001 | ✅ |
| LEV-TC-006 | LEV-FR-006 | LEV-AC-006 | LEV-UC-001 | ✅ |
| LEV-TC-007 | LEV-FR-007 | LEV-AC-007 | LEV-UC-001 | ✅ |
| LEV-TC-008 | LEV-FR-008 | LEV-AC-008 | LEV-UC-001 | ✅ |
| LEV-TC-009 | LEV-FR-009 | LEV-AC-009 | LEV-UC-001 | ✅ |
| LEV-TC-010 | LEV-FR-010 | LEV-AC-010 | LEV-UC-002 | ✅ |
| LEV-TC-011 | LEV-FR-011 | LEV-AC-011 | LEV-UC-003 | ✅ |
| LEV-TC-012 | LEV-FR-012 | LEV-AC-012 | LEV-UC-003 | ✅ |
| LEV-TC-013 | LEV-FR-013 | LEV-AC-013 | LEV-UC-004 | ✅ |
| LEV-TC-014 | LEV-FR-014 | LEV-AC-014 | LEV-UC-004 | ✅ |
| LEV-TC-015 | LEV-FR-015 | LEV-AC-015 | LEV-UC-005 | ✅ |
| LEV-TC-016 | LEV-FR-016 | LEV-AC-016 | LEV-UC-006 | ✅ Should |
| LEV-TC-017 | LEV-FR-017 | LEV-AC-017 | LEV-UC-003 | ✅ |
| LEV-TC-018 | LEV-FR-018 | LEV-AC-018 | LEV-UC-002 | ✅ |
| LEV-TC-019 | OQ-010 | — | — | ⚠️ Skip MVP |

## 5. Phương pháp

| Loại | Áp dụng leave |
|------|----------------|
| Unit | Trừ quỹ / overlap |
| API | `/lev/*` DOC-12 |
| E2E | Web+mobile cùng rule |
| Security | 403 C1/C2; INT-006 |

## 6. Môi trường

Dev / UAT (sau GW) / Prod smoke.

## 7. Nghiêm trọng

Fail LEV-TC-009 / trừ quỹ sai / 403 C2 → Blocker.

## 8. Nhật ký

| Phiên bản | Thay đổi | Tác giả |
|-----------|----------|---------|
| 0.1 | Chốt gói DOC-16 (DEC-DLV-004) | PGD Dư Hùng |
| 0.1 | Bổ sung §3 chi tiết leave; ID catalog không đổi (DEC-DLV-005) | Trịnh Yên |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| QC | | | Catalog Chốt; §3 chi tiết; chưa execute |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
