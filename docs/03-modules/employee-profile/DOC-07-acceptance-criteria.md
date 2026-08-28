# DOC-07 — Tiêu chí chấp nhận — Employee profile (EMP)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (AC employee-profile · DEC-REQ-036) |

**Gherkin** · Trace DOC-06 v0.1 **Chốt** (DEC-REQ-033, gồm EMP-FR-017). **Cổng AC đã chốt** (PGD · DEC-REQ-036). Nợ: DOC-13; HTML MCP; catalog bậc học; Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự DOC-16.

---

## 1. Mục đích

AC Must cho QC/Dev slice **hồ sơ**. Không cover PAY phiếu, LEV, LIF N+3.

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| EMP-AC-001 | EMP-FR-001 | EMP-UC-001 | HR tạo NV + org + HĐ | Must |
| EMP-AC-002 | EMP-FR-002 | EMP-UC-001 | Unique MNV/CCCD | Must |
| EMP-AC-003 | EMP-FR-003 | EMP-UC-001 | Unique email/MST khi có | Must |
| EMP-AC-004 | EMP-FR-004 | EMP-UC-001 | Org hiệu lực | Must |
| EMP-AC-005 | EMP-FR-005 | EMP-UC-001 | HĐ; cảnh báo thiếu HĐ | Must |
| EMP-AC-006 | EMP-FR-006 | EMP-UC-002 | HR sửa; cấm đổi LM trên SCR-002 | Must |
| EMP-AC-007 | EMP-FR-007 | EMP-UC-003 | Self-service web=mobile | Must |
| EMP-AC-008 | EMP-FR-008 | EMP-UC-004 | Đổi LM một bậc | Must |
| EMP-AC-009 | EMP-FR-009 | EMP-UC-004 | Không mở phiếu lương | Must |
| EMP-AC-010 | EMP-FR-010 | EMP-UC-005 | Thâm niên master | Must |
| EMP-AC-011 | EMP-FR-011 | EMP-UC-001 | 403 hồ sơ người khác | Must |
| EMP-AC-012 | EMP-FR-012 | EMP-UC-003 | Ẩn màn HR với NV | Must |
| EMP-AC-013 | EMP-FR-013 | — | Đủ EMP-SCR-001…006 | Must |
| EMP-AC-014 | EMP-FR-014 | EMP-UC-001 | Field/HĐ từ master | Must |
| EMP-AC-015 | EMP-FR-015 | EMP-UC-004 | LM 403 phiếu lương | Must |
| EMP-AC-016 | EMP-FR-016 | EMP-UC-004 | LM mới org không hiệu lực | Must |
| EMP-AC-017 | EMP-FR-017 | EMP-UC-001 | Trình độ học vấn | Must |

## 3. Kịch bản Gherkin

### EMP-AC-001 — Tạo NV (EMP-FR-001)

```gherkin
  Scenario: Happy — HR tạo trên EMP-SCR-002
    Given role HR/C&B
    When HR lưu định danh + org + HĐ
    Then hồ sơ tồn tại; không tính lương
  Scenario: Negative — NV tạo người khác
    Then 403
```

### EMP-AC-002 — Unique MNV/CCCD (EMP-FR-002)

```gherkin
  Scenario: Negative — trùng MNV
    When HR lưu MNV đã có
    Then chặn
  Scenario: Negative — trùng CCCD
    Then chặn
```

### EMP-AC-003 — Email/MST (EMP-FR-003)

```gherkin
  Scenario: Happy — email trống (chưa cấp)
    When HR lưu không email Cty
    Then OK
  Scenario: Negative — trùng email khi đã nhập
    Then chặn
  Scenario: Happy — MST trống
    Then OK
```

### EMP-AC-004 — Org (EMP-FR-004)

```gherkin
  Scenario: Negative — đơn vị ngừng
    When HR gắn org không hiệu lực
    Then chặn
```

### EMP-AC-005 — HĐ (EMP-FR-005)

```gherkin
  Scenario: Happy — có HĐ TV/chính thức
    Then PAY đọc được fact HĐ
  Scenario: Negative — không HĐ hiệu lực
    Then cảnh báo; không im lặng 85%
```

### EMP-AC-006 — Sửa HR (EMP-FR-006)

```gherkin
  Scenario: Happy — HR sửa định danh trên SCR-002
    Then lưu nếu unique/org OK
  Scenario: Negative — đổi LM trên SCR-002
    Then chặn; phải SCR-005/006
```

### EMP-AC-007 — Self-service (EMP-FR-007)

```gherkin
  Scenario: Happy — NV sửa field được phép web
    Then lưu; mobile cùng rule
  Scenario: Negative — NV sửa MNV/CCCD
    Then 403 hoặc read-only
```

### EMP-AC-008 — Đổi LM (EMP-FR-008)

```gherkin
  Scenario: Happy — duyệt một bậc
    Given HR gửi EMP-SCR-005
    When người duyệt duyệt EMP-SCR-006
    Then LM mới ghi
  Scenario: Negative — ghi LM không duyệt
    Then chặn
```

### EMP-AC-009 — Không lương (EMP-FR-009)

```gherkin
  Scenario: Negative — đổi LM mở phiếu cấp dưới
    Then cấm; PAY-BR-007 thắng
```

### EMP-AC-010 — Thâm niên (EMP-FR-010)

```gherkin
  Scenario: Happy — hiển thị theo master
    When mở hồ sơ
    Then thâm niên = công thức master
  Scenario: Negative — hardcode năm luật
    Then fail AC
```

### EMP-AC-011 — 403 (EMP-FR-011)

```gherkin
  Scenario: Negative — NV sửa hồ sơ khác
    Then 403
  Scenario: Negative — NV tự đổi LM MVP
    Then 403
```

### EMP-AC-012 — Ẩn màn (EMP-FR-012)

```gherkin
  Scenario: Negative — NV mở EMP-SCR-001
    Then 403 hoặc không menu
```

### EMP-AC-013 — Màn DOC-19 (EMP-FR-013)

```gherkin
  Scenario: Happy — đủ 6 màn
    Then EMP-SCR-001…006; pixel HTML không Must
```

### EMP-AC-014 — Master field (EMP-FR-014)

```gherkin
  Scenario: Negative — list field cứng URD
    Then fail AC
```

### EMP-AC-015 — LM phiếu (EMP-FR-015)

```gherkin
  Scenario: Negative — LM mở phiếu cấp dưới từ EMP
    Then 403
```

### EMP-AC-016 — Org LM mới (EMP-FR-016)

```gherkin
  Scenario: Negative — duyệt khi org LM mới ngừng
    Then từ chối ghi LM
```

### EMP-AC-017 — Học vấn (EMP-FR-017)

```gherkin
  Scenario: Happy — HR chọn bậc từ master trên SCR-002
    Then lưu trên hồ sơ; SCR-003 hiển thị
  Scenario: Negative — mã không thuộc master
    Then chặn
  Scenario: Negative — hardcode THPT/ĐH trên code
    Then fail AC
```

## 4. Checklist NFR / manual

| AC ID | Criteria | Pass / Fail | Tester | Date |
|-------|----------|-------------|--------|------|
| EMP-AC-NFR-001 | 403 hồ sơ người khác — DOC-13 | | | |
| EMP-AC-NFR-002 | Log tạo/sửa hồ sơ; duyệt đổi LM — DOC-13 | | | |

## 5. DoD slice hồ sơ

- [ ] 100% EMP-AC Must (001–017) pass
- [ ] Sign-off PGD (cổng AC EMP)

## 6. Truy vết

| AC ID | FR | UC | Test Case (DOC-16) |
|-------|----|----|---------------------|
| EMP-AC-001 | EMP-FR-001 | EMP-UC-001 | |
| EMP-AC-002 | EMP-FR-002 | EMP-UC-001 | |
| EMP-AC-003 | EMP-FR-003 | EMP-UC-001 | |
| EMP-AC-004 | EMP-FR-004 | EMP-UC-001 | |
| EMP-AC-005 | EMP-FR-005 | EMP-UC-001 | |
| EMP-AC-006 | EMP-FR-006 | EMP-UC-002 | |
| EMP-AC-007 | EMP-FR-007 | EMP-UC-003 | |
| EMP-AC-008 | EMP-FR-008 | EMP-UC-004 | |
| EMP-AC-009 | EMP-FR-009 | EMP-UC-004 | |
| EMP-AC-010 | EMP-FR-010 | EMP-UC-005 | |
| EMP-AC-011 | EMP-FR-011 | EMP-UC-001 | |
| EMP-AC-012 | EMP-FR-012 | EMP-UC-003 | |
| EMP-AC-013 | EMP-FR-013 | — | |
| EMP-AC-014 | EMP-FR-014 | EMP-UC-001 | |
| EMP-AC-015 | EMP-FR-015 | EMP-UC-004 | |
| EMP-AC-016 | EMP-FR-016 | EMP-UC-004 | |
| EMP-AC-017 | EMP-FR-017 | EMP-UC-001 | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-036) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
