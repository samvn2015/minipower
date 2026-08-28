# DOC-07 — Tiêu chí chấp nhận — Probation (PRB)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (AC PRB · DEC-REQ-059) |

**Gherkin** · Trace DOC-06 v0.1 **Chốt** (DEC-REQ-057). **Cổng AC đã chốt** (PGD · DEC-REQ-059). Nợ: DOC-16; HTML MCP; Ban HR ☐. **Chưa** `02-baseline/`. DOC-13 đã chốt. **Không** tự DOC-16.

---

## 1. Mục đích

AC Must slice **PRB**. Không cover tính 85% (PAY); khóa Git (LIF); ATS.

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| PRB-AC-001 | PRB-FR-001 | PRB-UC-004 | Không bịa mốc | Must |
| PRB-AC-002 | PRB-FR-002 | PRB-UC-001 | T-15 ngày lịch | Must |
| PRB-AC-003 | PRB-FR-003 | PRB-UC-002 | Task T-7 | Must |
| PRB-AC-004 | PRB-FR-004 | PRB-UC-002,003 | Chỉ 3 mã | Must |
| PRB-AC-005 | PRB-FR-005 | PRB-UC-003 | Đạt → EMP; không 85% PRB | Must |
| PRB-AC-006 | PRB-FR-006 | PRB-UC-003 | Gia hạn = master | Must |
| PRB-AC-007 | PRB-FR-007 | PRB-UC-003 | Không đạt → LIF; không xóa im | Must |
| PRB-AC-008 | PRB-FR-008 | PRB-UC-001,002 | 0 sót coverage | Must |
| PRB-AC-009 | PRB-FR-009 | PRB-UC-002,003 | LM/NV 403 chốt | Must |
| PRB-AC-010 | PRB-FR-010 | PRB-UC-001 | Cấm CRM sales | Must |
| PRB-AC-011 | PRB-FR-011 | PRB-UC-001,002 | Kênh HRM + email/app | Must |
| PRB-AC-012 | PRB-FR-012 | PRB-UC-002 | Phiếu động | Must |
| PRB-AC-013 | PRB-FR-013 | — | Đủ PRB-SCR-001…004 | Must |
| PRB-AC-014 | PRB-FR-014 | PRB-UC-002 | Không LM → HR; thiếu đề xuất vẫn chốt | Must |
| PRB-AC-015 | PRB-FR-015 | PRB-UC-004 | Không date picker ảo | Must |
| PRB-AC-016 | PRB-FR-016 | PRB-UC-003 | Lịch T-15/T-7 theo KT mới | Must |
| PRB-AC-017 | PRB-FR-017 | PRB-UC-003 | Audit người chốt = HR | Must |

## 3. Kịch bản Gherkin

### PRB-AC-001 — Mốc EMP (PRB-FR-001)

```gherkin
  Scenario: Happy — KT_TV lấy từ HĐ EMP
    Given HĐ có ngày KT TV
    Then job/màn PRB dùng đúng ngày đó
  Scenario: Negative — job gán KT mặc định
    Then fail AC
```

### PRB-AC-002 — T-15 (PRB-FR-002)

```gherkin
  Scenario: Happy — ngày hệ thống = KT_TV − 15 ngày lịch
    Then có cảnh báo T-15
  Scenario: Negative — đếm ngày công thay lịch (không CR)
    Then fail AC v0.1
```

### PRB-AC-003 — T-7 (PRB-FR-003)

```gherkin
  Scenario: Happy — ngày hệ thống = KT_TV − 7 ngày lịch
    Then có task đánh giá
  Scenario: Negative — không tạo task khi đủ mốc
    Then fail AC
```

### PRB-AC-004 — 3 mã (PRB-FR-004)

```gherkin
  Scenario: Happy — lưu Đạt / Gia hạn / Không đạt
    Then OK
  Scenario: Negative — “đạt có điều kiện” không master
    Then chặn
```

### PRB-AC-005 — Đạt (PRB-FR-005)

```gherkin
  Scenario: Happy — HR chốt Đạt
    Then EMP yêu cầu chuyển HĐ chính thức
  Scenario: Negative — PRB xuất hệ số 85%
    Then fail AC
```

### PRB-AC-006 — Gia hạn (PRB-FR-006)

```gherkin
  Scenario: Happy — chọn thời lượng master
    Then KT_TV cập nhật
  Scenario: Negative — nhập số tháng tự do
    Then chặn
```

### PRB-AC-007 — Không đạt (PRB-FR-007)

```gherkin
  Scenario: Happy — HR chốt Không đạt
    Then mở luồng off LIF
  Scenario: Negative — xóa im lặng hồ sơ EMP
    Then fail AC
```

### PRB-AC-008 — Coverage (PRB-FR-008)

```gherkin
  Scenario: Happy — mọi NV TV đủ mốc vào hàng T-15/T-7
    Then không sót
  Scenario: Negative — bỏ một NV TV hiệu lực
    Then fail AC
```

### PRB-AC-009 — SoT HR (PRB-FR-009)

```gherkin
  Scenario: Happy — HR [Chốt] trên PRB-SCR-003
    Then 200; SoT lưu
  Scenario: Negative — LM [Chốt] / NV [Chốt]
    Then 403
  Scenario: Happy — LM [Lưu đề xuất]
    Then không đổi HĐ
```

### PRB-AC-010 — CRM sales (PRB-FR-010)

```gherkin
  Scenario: Negative — job/màn PRB notify CRM bán hàng
    Then fail AC
```

### PRB-AC-011 — Kênh nhắc (PRB-FR-011)

```gherkin
  Scenario: Happy — T-15/T-7 có in-app HRM và email/app
    Then đủ hai kênh
  Scenario: Negative — chỉ in-app, không email/app
    Then fail AC
```

### PRB-AC-012 — Phiếu động (PRB-FR-012)

```gherkin
  Scenario: Happy — tiêu chí từ master
    Then render đúng catalog
  Scenario: Negative — list field cứng trên UI
    Then fail AC
```

### PRB-AC-013 — Màn khung (PRB-FR-013)

```gherkin
  Scenario: Happy — có PRB-SCR-001…004
    Then đủ 4 màn
  Scenario: Negative — thiếu pixel HTML MCP
    Then không fail AC
```

### PRB-AC-014 — Không LM (PRB-FR-014)

```gherkin
  Scenario: Happy — NV không có LM
    Then task T-7 gán HR
  Scenario: Happy — LM chưa đề xuất
    Then HR vẫn chốt được
```

### PRB-AC-015 — Date ảo (PRB-FR-015)

```gherkin
  Scenario: Happy — PRB-SCR-004 cảnh báo + link EMP
    Then không có date picker KT trên PRB
  Scenario: Negative — nhập KT ảo trên PRB
    Then fail AC
```

### PRB-AC-016 — KT mới (PRB-FR-016)

```gherkin
  Scenario: Happy — sau Gia hạn
    Then T-15/T-7 tính theo KT mới
  Scenario: Negative — vẫn nhắc theo KT cũ
    Then fail AC
```

### PRB-AC-017 — Audit (PRB-FR-017)

```gherkin
  Scenario: Happy — HR chốt
    Then audit lưu user HR + thời điểm
  Scenario: Negative — SoT không audit
    Then fail AC
```

## 4. Checklist NFR

| AC ID | Criteria | Pass / Fail |
|-------|----------|-------------|
| → DOC-13 | NFR-002…006 đã chốt | Không lặp |

## 5. DoD (slice)

- [x] Mỗi FR Must có ≥1 AC happy + ≥1 negative (trừ 013 pixel nợ; 014 hai happy)
- [ ] DOC-16
- [x] Sign-off PGD (chốt tài liệu · DEC-REQ-059)

## 6. Truy vết

| AC ID | FR | UC | Test (DOC-16) |
|-------|----|----|----------------|
| PRB-AC-001 | PRB-FR-001 | PRB-UC-004 | |
| PRB-AC-002 | PRB-FR-002 | PRB-UC-001 | |
| PRB-AC-003 | PRB-FR-003 | PRB-UC-002 | |
| PRB-AC-004 | PRB-FR-004 | PRB-UC-002,003 | |
| PRB-AC-005 | PRB-FR-005 | PRB-UC-003 | |
| PRB-AC-006 | PRB-FR-006 | PRB-UC-003 | |
| PRB-AC-007 | PRB-FR-007 | PRB-UC-003 | |
| PRB-AC-008 | PRB-FR-008 | PRB-UC-001,002 | |
| PRB-AC-009 | PRB-FR-009 | PRB-UC-002,003 | |
| PRB-AC-010 | PRB-FR-010 | PRB-UC-001 | |
| PRB-AC-011 | PRB-FR-011 | PRB-UC-001,002 | |
| PRB-AC-012 | PRB-FR-012 | PRB-UC-002 | |
| PRB-AC-013 | PRB-FR-013 | — | |
| PRB-AC-014 | PRB-FR-014 | PRB-UC-002 | |
| PRB-AC-015 | PRB-FR-015 | PRB-UC-004 | |
| PRB-AC-016 | PRB-FR-016 | PRB-UC-003 | |
| PRB-AC-017 | PRB-FR-017 | PRB-UC-003 | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-REQ-059) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
