# DOC-07 — Tiêu chí chấp nhận — Identity (IAM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (BA) | **Chốt** (AC identity · DEC-REQ-049) |

**Gherkin** · Trace DOC-06 v0.1 **Chốt** (DEC-REQ-047). **Cổng AC đã chốt** (PGD · DEC-REQ-049). Nợ: DOC-16; SSO/MFA DOC-08; Ban HR ☐. **Chưa** `02-baseline/`. DOC-13 đã chốt. **Không** tự DOC-16.

---

## 1. Mục đích

AC Must slice **IAM**. Không cover khóa Git (LIF); tính lương (PAY).

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| IAM-AC-001 | IAM-FR-001 | IAM-UC-001 | 401 không phiên / hết hạn | Must |
| IAM-AC-002 | IAM-FR-002 | IAM-UC-001 | Web = mobile | Must |
| IAM-AC-003 | IAM-FR-003 | IAM-UC-002 | 5 role MVP | Must |
| IAM-AC-004 | IAM-FR-004 | IAM-UC-003 | LM 403 phiếu cấp dưới | Must |
| IAM-AC-005 | IAM-FR-005 | IAM-UC-003 | NV 403 dữ liệu người khác | Must |
| IAM-AC-006 | IAM-FR-006 | — | HR SoT màn Cty | Must |
| IAM-AC-007 | IAM-FR-007 | IAM-UC-003 | IT 403 PAY | Must |
| IAM-AC-008 | IAM-FR-008 | IAM-UC-003 | NV/LM 403 màn HR | Must |
| IAM-AC-009 | IAM-FR-009 | IAM-UC-002 | Đổi LM không nới lương | Must |
| IAM-AC-010 | IAM-FR-010 | IAM-UC-004 | Disable login; không nút Git | Must |
| IAM-AC-011 | IAM-FR-011 | IAM-UC-003 | Audit xem phiếu | Must |
| IAM-AC-012 | IAM-FR-012 | IAM-UC-005 | Cấm CRM sales | Must |
| IAM-AC-013 | IAM-FR-013 | IAM-UC-002 | NV/LM không gán role | Must |
| IAM-AC-014 | IAM-FR-014 | IAM-UC-002 | Hợp quyền; thiếu perm lương | Must |
| IAM-AC-015 | IAM-FR-015 | IAM-UC-003 | LM C1 phép OK | Must |
| IAM-AC-016 | IAM-FR-016 | IAM-UC-003 | LM 403 C2/đột xuất | Must |
| IAM-AC-017 | IAM-FR-017 | IAM-UC-001 | Map 1 MNV; disable ≠ xóa hồ sơ | Must |
| IAM-AC-018 | IAM-FR-018 | — | Đủ IAM-SCR-001…004 | Must |
| IAM-AC-019 | IAM-FR-019 | IAM-UC-003 | PGD 403 phiếu Cty | Must |

## 3. Kịch bản Gherkin

### IAM-AC-001 — Phiên (IAM-FR-001)

```gherkin
  Scenario: Happy — login IAM-SCR-001
    When user đăng nhập TK hiệu lực
    Then có phiên
  Scenario: Negative — hết phiên
    When gọi API Must
    Then 401
  Scenario: Negative — HRM public
    Then fail AC
```

### IAM-AC-002 — Hai kênh (IAM-FR-002)

```gherkin
  Scenario: Happy — cùng role web và mobile
    Then 403 giống nhau
  Scenario: Negative — mobile nới quyền
    Then fail AC
```

### IAM-AC-003 — 5 role (IAM-FR-003)

```gherkin
  Scenario: Happy — gán IAM-ROLE-LM
    Then role lưu
  Scenario: Negative — role lạ không master
    Then chặn
```

### IAM-AC-004 — LM lương (IAM-FR-004)

```gherkin
  Scenario: Negative — LM mở phiếu cấp dưới
    Then 403
```

### IAM-AC-005 — NV người khác (IAM-FR-005)

```gherkin
  Scenario: Negative — NV mở hồ sơ người khác
    Then 403
```

### IAM-AC-006 — HR SoT (IAM-FR-006)

```gherkin
  Scenario: Happy — HR vào EMP DS / TIM / PAY kỳ
    Then 200 (IAM)
```

### IAM-AC-007 — IT PAY (IAM-FR-007)

```gherkin
  Scenario: Negative — IT không role HR mở PAY
    Then 403
```

### IAM-AC-008 — Màn HR (IAM-FR-008)

```gherkin
  Scenario: Negative — NV vào TIM-SCR-001
    Then 403 hoặc không menu
```

### IAM-AC-009 — Đổi LM (IAM-FR-009)

```gherkin
  Scenario: Negative — gán LM kèm quyền lương
    Then cấm
```

### IAM-AC-010 — Disable (IAM-FR-010)

```gherkin
  Scenario: Happy — IT vô hiệu trên IAM-SCR-004
    Then login sau → 401
  Scenario: Negative — nút Khóa Git trên SCR-004
    Then fail AC
```

### IAM-AC-011 — Audit (IAM-FR-011)

```gherkin
  Scenario: Happy — HR xem phiếu NV
    Then có log audit
```

### IAM-AC-012 — CRM sales (IAM-FR-012)

```gherkin
  Scenario: Negative — cấp token CRM bán hàng
    Then cấm
```

### IAM-AC-013 — Gán role (IAM-FR-013)

```gherkin
  Scenario: Negative — NV gán role
    Then 403
```

### IAM-AC-014 — Hợp quyền (IAM-FR-014)

```gherkin
  Scenario: Happy — user LM+HR
    Then được quyền HR gồm lương catalog
  Scenario: Negative — chỉ LM
    Then 403 phiếu người khác
```

### IAM-AC-015 — C1 (IAM-FR-015)

```gherkin
  Scenario: Happy — LM C1 đơn cấp dưới
    Then 200; không mở phiếu
```

### IAM-AC-016 — C2 (IAM-FR-016)

```gherkin
  Scenario: Negative — LM C2
    Then 403
```

### IAM-AC-017 — Map MNV (IAM-FR-017)

```gherkin
  Scenario: Happy — 1 TK = 1 MNV
    Then login đúng hồ sơ
  Scenario: Negative — disable xóa hồ sơ EMP
    Then fail AC
```

### IAM-AC-018 — Màn (IAM-FR-018)

```gherkin
  Scenario: Happy — đủ IAM-SCR-001…004
    Then pixel HTML không Must
```

### IAM-AC-019 — PGD (IAM-FR-019)

```gherkin
  Scenario: Negative — PGD không HR mở phiếu Cty
    Then 403
```

## 4. Checklist NFR / manual

| AC ID | Criteria | Pass / Fail | Tester | Date |
|-------|----------|-------------|--------|------|
| IAM-AC-NFR-001 | NFR-002…004 403 / cô lập — DOC-13 | | | |
| IAM-AC-NFR-002 | NFR-005 audit lương — DOC-13 | | | |

## 5. DoD slice IAM

- [ ] 100% IAM-AC Must (001–019) pass
- [ ] Sign-off PGD

## 6. Truy vết

| AC ID | FR | UC | Test (DOC-16) |
|-------|----|----|----------------|
| IAM-AC-001 | IAM-FR-001 | IAM-UC-001 | |
| IAM-AC-002 | IAM-FR-002 | IAM-UC-001 | |
| IAM-AC-003 | IAM-FR-003 | IAM-UC-002 | |
| IAM-AC-004 | IAM-FR-004 | IAM-UC-003 | |
| IAM-AC-005 | IAM-FR-005 | IAM-UC-003 | |
| IAM-AC-006 | IAM-FR-006 | — | |
| IAM-AC-007 | IAM-FR-007 | IAM-UC-003 | |
| IAM-AC-008 | IAM-FR-008 | IAM-UC-003 | |
| IAM-AC-009 | IAM-FR-009 | IAM-UC-002 | |
| IAM-AC-010 | IAM-FR-010 | IAM-UC-004 | |
| IAM-AC-011 | IAM-FR-011 | IAM-UC-003 | |
| IAM-AC-012 | IAM-FR-012 | IAM-UC-005 | |
| IAM-AC-013 | IAM-FR-013 | IAM-UC-002 | |
| IAM-AC-014 | IAM-FR-014 | IAM-UC-002 | |
| IAM-AC-015 | IAM-FR-015 | IAM-UC-003 | |
| IAM-AC-016 | IAM-FR-016 | IAM-UC-003 | |
| IAM-AC-017 | IAM-FR-017 | IAM-UC-001 | |
| IAM-AC-018 | IAM-FR-018 | — | |
| IAM-AC-019 | IAM-FR-019 | IAM-UC-003 | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-REQ-049) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn → PGD chốt |
| Business Owner | Ban HR | 2026-08-26 | ☑ Ký (PGD xác nhận · DEC-DLV-008) |
