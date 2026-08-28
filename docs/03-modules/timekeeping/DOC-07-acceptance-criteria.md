# DOC-07 — Tiêu chí chấp nhận — Timekeeping (TIM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (AC timekeeping · DEC-REQ-021) |

**Gherkin** · Trace DOC-06 v0.1 **Chốt** (DEC-REQ-020). **Cổng AC đã chốt** (PGD · DEC-REQ-021). Nợ: cơ chế mã version trên file; DOC-13; HTML MCP; Ban HR ☐. **Chưa** `02-baseline/`.

---

## 1. Mục đích

AC Must cho QC/Dev slice **công**. Không cover PAY tính lương, LEV C1–C2, máy CC.

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| TIM-AC-001 | TIM-FR-001 | TIM-UC-001 | Một version mẫu hiệu lực | Must |
| TIM-AC-002 | TIM-FR-002 | TIM-UC-001 | Cột từ master, không hardcode | Must |
| TIM-AC-003 | TIM-FR-003 | TIM-UC-002 | Import khớp version | Must |
| TIM-AC-004 | TIM-FR-004 | TIM-UC-002 | Preview; cấm commit khi còn lỗi | Must |
| TIM-AC-005 | TIM-FR-005 | TIM-UC-003 | Commit hết lỗi Must | Must |
| TIM-AC-006 | TIM-FR-006 | TIM-UC-004 | Chốt tháng; PAY đọc | Must |
| TIM-AC-007 | TIM-FR-007 | TIM-UC-004 | OT có loại trước chốt | Must |
| TIM-AC-008 | TIM-FR-008 | TIM-UC-004 | Chỉ phép Đã duyệt vào công | Must |
| TIM-AC-009 | TIM-FR-009 | TIM-UC-004 | N_thực gồm phép hưởng | Must |
| TIM-AC-010 | TIM-FR-010 | TIM-UC-002 | Chỉ Excel mẫu; không máy CC | Must |
| TIM-AC-011 | TIM-FR-011 | TIM-UC-001 | NV/LM 403 | Must |
| TIM-AC-012 | TIM-FR-012 | TIM-UC-005 | Bỏ chốt; cấm nếu PAY đã chốt | Must |
| TIM-AC-013 | TIM-FR-013 | TIM-UC-002 | Ẩn màn HR với NV/LM | Must |
| TIM-AC-014 | TIM-FR-014 | TIM-UC-002 | Đủ TIM-SCR-001…006 | Must |
| TIM-AC-015 | TIM-FR-015 | TIM-UC-001 | Cấm hai mẫu Active | Must |
| TIM-AC-016 | TIM-FR-016 | TIM-UC-001 | Preview không tự commit khi đổi mẫu | Must |

## 3. Kịch bản Gherkin

### TIM-AC-001 — Một mẫu (TIM-FR-001)

```gherkin
  Scenario: Happy — công bố version mới
    Given HR trên TIM-SCR-002
    When HR công bố version V2
    Then V2 hiệu lực; import file V1 bị từ chối
  Scenario: Negative — NV công bố mẫu
    When NV công bố
    Then 403
```

### TIM-AC-002 — Master cột (TIM-FR-002)

```gherkin
  Scenario: Happy — cột theo master kỳ
    Given master HR đổi tên cột
    When import file theo master mới (đúng version)
    Then preview dùng mapping master
  Scenario: Negative — cột hardcode URD
    Then fail AC
```

### TIM-AC-003 — Khớp version (TIM-FR-003)

```gherkin
  Scenario: Negative — sai version
    Given mẫu hiệu lực = V2
    When HR upload file V1
    Then từ chối import; không ghi sổ
```

### TIM-AC-004 — Preview (TIM-FR-004)

```gherkin
  Scenario: Happy — hiện lỗi dòng
    Given file đúng version có dòng thiếu NV
    When preview TIM-SCR-003
    Then dòng lỗi Must hiện; chưa commit
  Scenario: Negative — ghi khi còn lỗi Must
    When HR commit
    Then chặn
```

### TIM-AC-005 — Commit (TIM-FR-005)

```gherkin
  Scenario: Happy — hết lỗi Must
    Given preview sạch
    When HR ghi TIM-SCR-004
    Then bảng công Draft; chưa chốt
```

### TIM-AC-006 — Chốt tháng (TIM-FR-006)

```gherkin
  Scenario: Happy — chốt sau commit
    Given đã commit; hết lỗi
    When HR chốt TIM-SCR-005
    Then tháng Chốt; PAY được đọc
  Scenario: Negative — PAY tính khi TIM chưa chốt
    Then PAY chặn (PAY-FR-001)
```

### TIM-AC-007 — OT (TIM-FR-007)

```gherkin
  Scenario: Negative — có giờ OT không loại
    When HR chốt
    Then chặn
  Scenario: Happy — loại 1.5/2.0/3.0
    Then được chốt nếu hết lỗi khác
```

### TIM-AC-008 — Phép Đã duyệt (TIM-FR-008)

```gherkin
  Scenario: Happy — Đã duyệt vào công
    Given đơn phép Đã duyệt trong tháng
    When chốt
    Then ngày phép trên bảng công
  Scenario: Negative — đơn Chờ C1
    Then không vào công
```

### TIM-AC-009 — N_thực (TIM-FR-009)

```gherkin
  Scenario: Happy — gồm phép hưởng
    When xuất N_thực cho PAY
    Then đã gồm ngày phép hưởng; không tách để PAY cộng
  Scenario: Negative — N_thực sạch phép
    Then fail AC
```

### TIM-AC-010 — Không máy CC (TIM-FR-010)

```gherkin
  Scenario: Happy — Excel đúng mẫu
    When HR import Excel version hiệu lực
    Then vào preview
  Scenario: Negative — API máy CC
    Then ngoài scope / cấm
```

### TIM-AC-011 — 403 (TIM-FR-011)

```gherkin
  Scenario: Negative — LM import
    When LM upload
    Then 403
  Scenario: Negative — NV chốt tháng
    Then 403
```

### TIM-AC-012 — Bỏ chốt (TIM-FR-012)

```gherkin
  Scenario: Happy — PAY chưa chốt
    Given tháng TIM Chốt; kỳ PAY Draft
    When HR bỏ chốt TIM-SCR-006
    Then tháng Draft; được import lại
  Scenario: Negative — kỳ PAY đã chốt
    Then cấm bỏ chốt TIM
  Scenario: Negative — sửa ô trên PAY
    Then cấm
```

### TIM-AC-013 — Ẩn màn (TIM-FR-013)

```gherkin
  Scenario: Negative — NV mở DS tháng
    When NV vào TIM-SCR-001
    Then 403 hoặc không menu
```

### TIM-AC-014 — Màn DOC-19 (TIM-FR-014)

```gherkin
  Scenario: Happy — đủ 6 màn HR
    Then có TIM-SCR-001…006; luồng mẫu → import → commit → chốt → bỏ chốt
```

### TIM-AC-015 — Hai mẫu (TIM-FR-015)

```gherkin
  Scenario: Negative — hai version Active
    When HR để V1 và V2 cùng hiệu lực
    Then cấm
```

### TIM-AC-016 — Preview không tự ghi (TIM-FR-016)

```gherkin
  Scenario: Negative — đổi mẫu khi đang preview
    Given preview chưa commit
    When HR công bố mẫu mới
    Then preview cũ không tự commit
```

## 4. Checklist NFR / manual

| AC ID | Criteria | Pass / Fail | Tester | Date |
|-------|----------|-------------|--------|------|
| TIM-AC-NFR-001 | 403 NV/LM — DOC-13 | | | |
| TIM-AC-NFR-002 | Log công bố / import / chốt / bỏ chốt — DOC-13 | | | |

## 5. DoD slice công

- [ ] 100% TIM-AC Must (001–016) pass
- [ ] Sign-off PGD (cổng AC timekeeping)

## 6. Truy vết

| AC ID | FR | UC | Test Case (DOC-16) |
|-------|----|----|---------------------|
| TIM-AC-001 | TIM-FR-001 | TIM-UC-001 | |
| TIM-AC-002 | TIM-FR-002 | TIM-UC-001 | |
| TIM-AC-003 | TIM-FR-003 | TIM-UC-002 | |
| TIM-AC-004 | TIM-FR-004 | TIM-UC-002 | |
| TIM-AC-005 | TIM-FR-005 | TIM-UC-003 | |
| TIM-AC-006 | TIM-FR-006 | TIM-UC-004 | |
| TIM-AC-007 | TIM-FR-007 | TIM-UC-004 | |
| TIM-AC-008 | TIM-FR-008 | TIM-UC-004 | |
| TIM-AC-009 | TIM-FR-009 | TIM-UC-004 | |
| TIM-AC-010 | TIM-FR-010 | TIM-UC-002 | |
| TIM-AC-011 | TIM-FR-011 | TIM-UC-001 | |
| TIM-AC-012 | TIM-FR-012 | TIM-UC-005 | |
| TIM-AC-013 | TIM-FR-013 | TIM-UC-002 | |
| TIM-AC-014 | TIM-FR-014 | TIM-UC-002 | |
| TIM-AC-015 | TIM-FR-015 | TIM-UC-001 | |
| TIM-AC-016 | TIM-FR-016 | TIM-UC-001 | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-021) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
