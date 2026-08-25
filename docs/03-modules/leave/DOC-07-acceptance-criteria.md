# DOC-07 — Tiêu chí chấp nhận — Leave (LEV)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-24 | Trịnh Yên (BA) | **Chốt** (AC leave · DEC-REQ-008) |

**Gherkin** · Trace DOC-06 v0.2 **Chốt**. Nợ: OQ-REQ-010; DOC-13; HTML MCP; Ban HR ☐. **Chưa** `02-baseline/`.

---

## 1. Mục đích

AC Must (+ Should FR-016) cho QC/Dev slice **phép**. Không cover payroll / Excel công / N+3.

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| LEV-AC-001 | LEV-FR-001 | LEV-UC-001 | Form 6 loại + bàn giao | Must |
| LEV-AC-002 | LEV-FR-002 | LEV-UC-001 | Cùng rule trên mobile | Must |
| LEV-AC-003 | LEV-FR-003 | LEV-UC-001 | Chặn overlap Open | Must |
| LEV-AC-004 | LEV-FR-004 | LEV-UC-001 | Chặn quỹ năm thiếu | Must |
| LEV-AC-005 | LEV-FR-005 | LEV-UC-001 | Không quỹ năm cho loại không trừ | Must |
| LEV-AC-006 | LEV-FR-006 | LEV-UC-001 | Hạn 3 NLĐ ngày công liền | Must |
| LEV-AC-007 | LEV-FR-007 | LEV-UC-001 | Đột xuất submit; C1 không trừ quỹ | Must |
| LEV-AC-008 | LEV-FR-008 | LEV-UC-001 | File mẫu Cty ốm/BHXH | Must |
| LEV-AC-009 | LEV-FR-009 | LEV-UC-001 | Thông báo; không CRM bán hàng | Must |
| LEV-AC-010 | LEV-FR-010 | LEV-UC-002 | C1 chỉ LM; không Matrix; không tự C1 | Must |
| LEV-AC-011 | LEV-FR-011 | LEV-UC-003 | C2 sau C1 kể cả đột xuất | Must |
| LEV-AC-012 | LEV-FR-012 | LEV-UC-003 | C2 atomic + trừ quỹ năm | Must |
| LEV-AC-013 | LEV-FR-013 | LEV-UC-004 | NV hủy trước C2; nộp lại cùng ngày | Must |
| LEV-AC-014 | LEV-FR-014 | LEV-UC-004 | Không hủy/hoàn quỹ sau C2 | Must |
| LEV-AC-015 | LEV-FR-015 | LEV-UC-005 | Xem quỹ mình | Must |
| LEV-AC-016 | LEV-FR-016 | LEV-UC-006 | Trần catalog trống/có số | Should |
| LEV-AC-017 | LEV-FR-017 | LEV-UC-003 | Manager/IT không C2 | Must |
| LEV-AC-018 | LEV-FR-018 | LEV-UC-002 | C1 không trừ quỹ | Must |

## 3. Kịch bản Gherkin

### LEV-AC-001 — Tạo đơn (LEV-FR-001)

```gherkin
Feature: Nộp đơn phép
  Scenario: Happy — đủ field
    Given NV đăng nhập web LEV-SCR-002
    When NV chọn 1 trong 6 loại, Từ–Đến, nhãn buổi, lý do, 1 người bàn giao active khác mình
    And submit hợp lệ
    Then đơn trạng thái Chờ C1
  Scenario: Negative — bàn giao = chính mình
    When NV chọn bàn giao là chính mình
    Then hệ thống chặn submit
```

### LEV-AC-002 — Mobile cùng rule (LEV-FR-002)

```gherkin
  Scenario: Happy — mobile pass cùng case web
    Given cùng dữ liệu hợp lệ trên LEV-SCR-003
    When NV submit
    Then kết quả validation giống web
  Scenario: Negative — mobile fail cùng case web fail
    Given dữ liệu web bị chặn (overlap / quỹ / 3 NLĐ)
    When NV submit trên mobile
    Then bị chặn cùng lý do
```

### LEV-AC-003 — Overlap (LEV-FR-003)

```gherkin
  Scenario: Negative — trùng đơn Open
    Given NV đã có đơn Chờ C1 hoặc Chờ C2 hoặc Đã duyệt cùng ngày/buổi
    When NV submit đơn mới trùng
    Then chặn; quỹ không đổi
```

### LEV-AC-004 — Quỹ năm thiếu (LEV-FR-004)

```gherkin
  Scenario: Negative — phép năm vượt quỹ
    Given loại phép năm; số ngày đơn > quỹ còn
    When NV submit hoặc HR C2
    Then chặn; quỹ không trừ
```

### LEV-AC-005 — Loại không trừ năm (LEV-FR-005)

```gherkin
  Scenario: Happy — ốm/KHL/kết hôn/tang/chế độ
    Given loại không trừ quỹ năm; quỹ năm = X
    When NV submit và HR C2 hợp lệ
    Then quỹ năm vẫn X
```

### LEV-AC-006 — 3 NLĐ (LEV-FR-006)

```gherkin
  Scenario: Negative — ≥3 ngày công liền, nộp trễ, không cờ đột xuất
    Given chuỗi ≥ 3 ngày công chuẩn liền; NLĐ tới ngày bắt đầu < 3; không cờ đột xuất
    When NV submit
    Then chặn và gợi ý đánh dấu đột xuất
  Scenario: Happy — 2 ngày công liền không kích hoạt hạn
    Given nghỉ 2 ngày công liền
    When NV submit sát ngày
    Then không áp hạn 3 NLĐ
```

### LEV-AC-007 — Đột xuất (LEV-FR-007)

```gherkin
  Scenario: Happy — có cờ đột xuất được submit
    Given trễ 3 NLĐ và cờ Nghỉ đột xuất
    When NV submit
    Then đơn Chờ C1; quỹ chưa trừ
  Scenario: Negative — LM C1 không trừ quỹ
    Given đơn đột xuất Chờ C1
    When LM duyệt C1
    Then quỹ không đổi
```

### LEV-AC-008 — File ốm (LEV-FR-008)

```gherkin
  Scenario: Negative — ốm không file mẫu
    Given loại ốm/BHXH; thiếu file đúng mẫu Cty
    When NV submit hoặc HR C2
    Then chặn
```

### LEV-AC-009 — Thông báo (LEV-FR-009)

```gherkin
  Scenario: Happy — kênh nội bộ
    When xảy ra nộp / C1 / C2 / từ chối / hủy
    Then có thông báo Email và/hoặc App/HRM
    And không gọi CRM bán hàng
```

### LEV-AC-010 — C1 (LEV-FR-010)

```gherkin
  Scenario: Happy — LM cấp dưới duyệt
    Given LM đúng cây của NV
    When LM Phê duyệt C1
    Then đơn Chờ C2
  Scenario: Negative — không phải LM / Matrix / tự C1
    When Manager khác, Matrix, hoặc NV tự C1 đơn mình
    Then không C1 được; từ chối C1 bắt buộc lý do
```

### LEV-AC-011 — C2 cần C1 (LEV-FR-011)

```gherkin
  Scenario: Negative — đột xuất chưa C1
    Given đơn đột xuất Chờ C1
    When HR bấm C2
    Then hệ thống từ chối C2
```

### LEV-AC-012 — C2 atomic (LEV-FR-012)

```gherkin
  Scenario: Happy — phép năm
    Given C1 đã duyệt; quỹ đủ; loại phép năm
    When HR C2
    Then Đã duyệt và quỹ năm đã trừ cùng lúc
  Scenario: Happy — đột xuất
    Given C1 đã duyệt; cờ đột xuất
    When HR C2
    Then ngoại lệ + Đã duyệt một thao tác
```

### LEV-AC-013 — Hủy trước C2 (LEV-FR-013)

```gherkin
  Scenario: Happy — NV hủy Chờ C1/C2
    Given đơn của NV Chờ C1 hoặc Chờ C2
    When NV hủy
    Then Đã hủy; ngày không chiếm; NV nộp lại cùng ngày được
  Scenario: Negative — người khác hủy (OQ-010)
    When LM hoặc HR hủy hộ
    Then MVP từ chối (chỉ NV hủy đơn mình)
```

### LEV-AC-014 — Sau C2 (LEV-FR-014)

```gherkin
  Scenario: Negative — hủy/hoàn quỹ sau Đã duyệt
    Given đơn Đã duyệt
    When NV/LM/HR hủy hoặc hoàn quỹ
    Then hệ thống từ chối
```

### LEV-AC-015 — Quỹ mình (LEV-FR-015)

```gherkin
  Scenario: Happy — xem quỹ mình
    When NV mở LEV-SCR-007
    Then thấy quỹ phép năm của mình
  Scenario: Negative — quỹ người khác
    When NV mở quỹ đồng nghiệp
    Then bị từ chối (trừ LM cấp dưới / HR theo IAM)
```

### LEV-AC-016 — Trần catalog (LEV-FR-016)

```gherkin
  Scenario: Happy — trống = không trần
    Given HR để trống trần loại
    When NV nộp trong phạm vi khác
    Then không chặn vì trần
  Scenario: Negative — có số
    Given HR đặt trần 3 ngày
    When NV nộp 4 ngày loại đó
    Then chặn
```

### LEV-AC-017 — Ẩn C2 (LEV-FR-017)

```gherkin
  Scenario: Negative — Manager / IT không role HR
    When Manager hoặc IT (không gán HR) mở C2
    Then không có thao tác C2
```

### LEV-AC-018 — C1 không trừ quỹ (LEV-FR-018)

```gherkin
  Scenario: Happy — phép năm sau C1
    Given quỹ năm = X; đơn phép năm
    When LM duyệt C1
    Then quỹ vẫn X
```

## 4. Checklist NFR / manual

| AC ID | Criteria | Pass / Fail | Tester | Date |
|-------|----------|-------------|--------|------|
| LEV-AC-NFR-001 | NV không xem đơn/quỹ người khác (trừ LM/HR) — DOC-13 | | | |
| LEV-AC-NFR-002 | Log C1/C2/hủy — DOC-13 | | | |

## 5. DoD slice phép

- [ ] 100% LEV-AC Must (001–015, 017–018) pass
- [ ] LEV-AC-016 Should: pass hoặc nợ có DEC
- [ ] Sign-off PGD (cổng AC leave)

## 6. Truy vết

| AC ID | FR | UC | Test Case (DOC-16) |
|-------|----|----|---------------------|
| LEV-AC-001 | LEV-FR-001 | LEV-UC-001 | |
| LEV-AC-002 | LEV-FR-002 | LEV-UC-001 | |
| LEV-AC-003 | LEV-FR-003 | LEV-UC-001 | |
| LEV-AC-004 | LEV-FR-004 | LEV-UC-001 | |
| LEV-AC-005 | LEV-FR-005 | LEV-UC-001 | |
| LEV-AC-006 | LEV-FR-006 | LEV-UC-001 | |
| LEV-AC-007 | LEV-FR-007 | LEV-UC-001 | |
| LEV-AC-008 | LEV-FR-008 | LEV-UC-001 | |
| LEV-AC-009 | LEV-FR-009 | LEV-UC-001 | |
| LEV-AC-010 | LEV-FR-010 | LEV-UC-002 | |
| LEV-AC-011 | LEV-FR-011 | LEV-UC-003 | |
| LEV-AC-012 | LEV-FR-012 | LEV-UC-003 | |
| LEV-AC-013 | LEV-FR-013 | LEV-UC-004 | |
| LEV-AC-014 | LEV-FR-014 | LEV-UC-004 | |
| LEV-AC-015 | LEV-FR-015 | LEV-UC-005 | |
| LEV-AC-016 | LEV-FR-016 | LEV-UC-006 | |
| LEV-AC-017 | LEV-FR-017 | LEV-UC-003 | |
| LEV-AC-018 | LEV-FR-018 | LEV-UC-002 | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-24 | **Chốt** v0.1 (DEC-REQ-008) · ☐ repo `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-24 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
