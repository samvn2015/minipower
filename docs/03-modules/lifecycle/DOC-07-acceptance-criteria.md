# DOC-07 — Tiêu chí chấp nhận — Lifecycle (LIF)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (AC lifecycle · DEC-REQ-037) |

**Gherkin** · Trace DOC-06 v0.1 **Chốt** (DEC-REQ-034). **Cổng AC đã chốt** (PGD · DEC-REQ-037). Nợ: DOC-13; HTML MCP; tick master; **N+3 ngày lịch** (nháp); Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự DOC-16.  
Không AC notify CRM bán hàng (cấm).

---

## 1. Mục đích

AC cho QC/Dev slice **on/off**. Không cover PAY khóa lương; ATS; e-sign.

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| LIF-AC-001 | LIF-FR-001 | LIF-UC-001 | Checklist on; chặn thiếu Must | Must |
| LIF-AC-002 | LIF-FR-002 | LIF-UC-001 | Cấp TK lúc on | Must |
| LIF-AC-003 | LIF-FR-003 | LIF-UC-002 | N = ngày LV cuối | Must |
| LIF-AC-004 | LIF-FR-004 | LIF-UC-002 | NV không kích N+3 | Must |
| LIF-AC-005 | LIF-FR-005 | LIF-UC-003 | Khóa Git N+3 lịch | Must |
| LIF-AC-006 | LIF-FR-006 | LIF-UC-003 | CRM SP cùng mốc | Must |
| LIF-AC-007 | LIF-FR-007 | LIF-UC-003 | Cấm khóa trước N+3 | Must |
| LIF-AC-008 | LIF-FR-008 | LIF-UC-003 | HR 403 khóa Git | Must |
| LIF-AC-009 | LIF-FR-009 | LIF-UC-004 | Checklist off | Must |
| LIF-AC-010 | LIF-FR-010 | LIF-UC-003 | Không CRM sales | Must |
| LIF-AC-011 | LIF-FR-011 | LIF-UC-005 | Chat theo master | Should |
| LIF-AC-012 | LIF-FR-012 | — | Đủ LIF-SCR-001…006 | Must |
| LIF-AC-013 | LIF-FR-013 | LIF-UC-002 | Hiện N và N+3 | Must |
| LIF-AC-014 | LIF-FR-014 | LIF-UC-005 | Audit khóa sớm | Must |
| LIF-AC-015 | LIF-FR-015 | LIF-UC-002 | NV 403 ghi N job | Must |
| LIF-AC-016 | LIF-FR-016 | — | Không nút CRM sales | Must |

## 3. Kịch bản Gherkin

### LIF-AC-001 — Checklist on (LIF-FR-001)

```gherkin
  Scenario: Happy — tick Must đủ
    Given mục từ master
    When HR đóng on LIF-SCR-002
    Then cho đóng
  Scenario: Negative — thiếu tick Must
    Then chặn
```

### LIF-AC-002 — Cấp lúc on (LIF-FR-002)

```gherkin
  Scenario: Happy — cấp email/Git/CRM SP/chat lúc on
    Then trạng thái đã cấp trên SCR-002
  Scenario: Negative — hẹn cấp Git = N+3
    Then fail AC
```

### LIF-AC-003 — N (LIF-FR-003)

```gherkin
  Scenario: Happy — HR xác nhận ngày LV cuối SCR-003
    Then N lưu; không lấy ngày ký đơn
  Scenario: Negative — dùng ngày ký = N
    Then chặn
```

### LIF-AC-004 — NV không kích job (LIF-FR-004)

```gherkin
  Scenario: Negative — chỉ NV nhập N
    Then job N+3 không chạy
```

### LIF-AC-005 — Khóa Git (LIF-FR-005)

```gherkin
  Scenario: Happy — ngày ≥ N+3 lịch; N đã HR xác nhận
    Then Git khóa
  Scenario: Negative — chưa đến N+3
    Then chưa khóa (trừ CR)
```

### LIF-AC-006 — CRM SP (LIF-FR-006)

```gherkin
  Scenario: Happy — khóa CRM SP cùng lúc Git
    Then hai bên cùng khóa
  Scenario: Negative — chỉ khóa Git
    Then fail AC
```

### LIF-AC-007 — Không khóa sớm (LIF-FR-007)

```gherkin
  Scenario: Negative — job khóa trước N+3 không CR
    Then cấm
  Scenario: Happy — CR an ninh trên SCR-006
    Then được khóa sớm + audit
```

### LIF-AC-008 — HR không SSH (LIF-FR-008)

```gherkin
  Scenario: Negative — HR bấm khóa Git
    Then 403 hoặc chỉ ticket IT
```

### LIF-AC-009 — Checklist off (LIF-FR-009)

```gherkin
  Scenario: Negative — đóng off thiếu Must
    Then chặn
```

### LIF-AC-010 — Không sales (LIF-FR-010)

```gherkin
  Scenario: Negative — webhook CRM bán hàng khi off
    Then cấm / không gửi
```

### LIF-AC-011 — Chat (LIF-FR-011)

```gherkin
  Scenario: Happy — khóa chat theo master ≠ N+3 nếu quy chế khác
    Then mốc đúng master
  Scenario: Negative — mặc định im lặng = Git
    Then fail AC nếu master khác
```

### LIF-AC-012 — Màn DOC-19 (LIF-FR-012)

```gherkin
  Scenario: Happy — đủ LIF-SCR-001…006
    Then pixel HTML không Must
```

### LIF-AC-013 — Hiện N+3 (LIF-FR-013)

```gherkin
  Scenario: Happy — SCR-001/004 hiện N và N+3 dự kiến
    Given N đã xác nhận
    Then N+3 = N + 3 ngày lịch (nháp)
```

### LIF-AC-014 — Audit CR (LIF-FR-014)

```gherkin
  Scenario: Happy — khóa sớm có CR
    Then có log CR
  Scenario: Negative — khóa sớm không CR
    Then ghi vi phạm
```

### LIF-AC-015 — NV ghi N (LIF-FR-015)

```gherkin
  Scenario: Negative — NV lưu N kích job
    Then 403 hoặc không schedule
```

### LIF-AC-016 — Không nút sales (LIF-FR-016)

```gherkin
  Scenario: Negative — LIF-SCR-004 có nút gửi CRM sales
    Then fail AC
```

## 4. Checklist NFR / manual

| AC ID | Criteria | Pass / Fail | Tester | Date |
|-------|----------|-------------|--------|------|
| LIF-AC-NFR-001 | HR không credential Git — DOC-13 | | | |
| LIF-AC-NFR-002 | Log xác nhận N / khóa Git-CRM — DOC-13 | | | |

## 5. DoD slice LIF

- [ ] 100% LIF-AC Must (001–010, 012–016) pass; AC-011 Should
- [ ] Sign-off PGD (cổng AC LIF)

## 6. Truy vết

| AC ID | FR | UC | Test Case (DOC-16) |
|-------|----|----|---------------------|
| LIF-AC-001 | LIF-FR-001 | LIF-UC-001 | |
| LIF-AC-002 | LIF-FR-002 | LIF-UC-001 | |
| LIF-AC-003 | LIF-FR-003 | LIF-UC-002 | |
| LIF-AC-004 | LIF-FR-004 | LIF-UC-002 | |
| LIF-AC-005 | LIF-FR-005 | LIF-UC-003 | |
| LIF-AC-006 | LIF-FR-006 | LIF-UC-003 | |
| LIF-AC-007 | LIF-FR-007 | LIF-UC-003 | |
| LIF-AC-008 | LIF-FR-008 | LIF-UC-003 | |
| LIF-AC-009 | LIF-FR-009 | LIF-UC-004 | |
| LIF-AC-010 | LIF-FR-010 | LIF-UC-003 | |
| LIF-AC-011 | LIF-FR-011 | LIF-UC-005 | |
| LIF-AC-012 | LIF-FR-012 | — | |
| LIF-AC-013 | LIF-FR-013 | LIF-UC-002 | |
| LIF-AC-014 | LIF-FR-014 | LIF-UC-005 | |
| LIF-AC-015 | LIF-FR-015 | LIF-UC-002 | |
| LIF-AC-016 | LIF-FR-016 | — | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-037) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
