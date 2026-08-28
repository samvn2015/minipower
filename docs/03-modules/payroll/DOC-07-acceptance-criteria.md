# DOC-07 — Tiêu chí chấp nhận — Payroll (PAY)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (AC payroll · DEC-REQ-015) |

**Gherkin** · Trace DOC-06 v0.1 **Chốt** (DEC-REQ-014). **Cổng AC đã chốt** (PGD · DEC-REQ-015). Nợ: làm tròn chi tiết trên master kỳ UAT; DOC-13; HTML MCP; Ban HR ☐. **Chưa** `02-baseline/`.

---

## 1. Mục đích

AC Must cho QC/Dev slice **lương**. Không cover TIM import, LEV C1–C2, N+3.

## 2. Danh mục

| AC ID | FR ID | UC ID | Mô tả ngắn | Priority |
|-------|-------|-------|------------|----------|
| PAY-AC-001 | PAY-FR-001 | PAY-UC-001 | Tính kỳ khi TIM đã chốt | Must |
| PAY-AC-002 | PAY-FR-002 | PAY-UC-001 | N_tính = N_thực − N_KHL | Must |
| PAY-AC-003 | PAY-FR-003 | PAY-UC-001 | Thử việc 85% | Must |
| PAY-AC-004 | PAY-FR-004 | PAY-UC-001 | OT từ công chốt | Must |
| PAY-AC-005 | PAY-FR-005 | PAY-UC-001 | PC HĐ + tháng; mã master | Must |
| PAY-AC-006 | PAY-FR-006 | PAY-UC-001 | BH/TNCN tỷ lệ kỳ | Must |
| PAY-AC-007 | PAY-FR-007 | PAY-UC-002 | Chặn chốt khi N_tính > chuẩn | Must |
| PAY-AC-008 | PAY-FR-008 | PAY-UC-001 | Cấm sửa công trên PAY | Must |
| PAY-AC-009 | PAY-FR-009 | PAY-UC-001 | NV/LM 403 tính/chốt/xuất | Must |
| PAY-AC-010 | PAY-FR-010 | PAY-UC-004 | Cô lập phiếu | Must |
| PAY-AC-011 | PAY-FR-011 | PAY-UC-004 | Mobile cùng IAM phiếu | Must |
| PAY-AC-012 | PAY-FR-012 | PAY-UC-005 | Xuất đúng người; không CC LM | Must |
| PAY-AC-013 | PAY-FR-013 | PAY-UC-001 | A-001 không cộng im lặng | Must |
| PAY-AC-014 | PAY-FR-014 | PAY-UC-001 | Cột preview DOC-19 | Must |
| PAY-AC-015 | PAY-FR-015 | PAY-UC-003 | Nhập PC tháng | Must |
| PAY-AC-016 | PAY-FR-016 | PAY-UC-001 | Tính lại Draft; không hủy chốt | Must |
| PAY-AC-017 | PAY-FR-017 | PAY-UC-004 | Ẩn màn HR với NV/LM | Must |
| PAY-AC-018 | PAY-FR-018 | PAY-UC-002 | UAT 0 đồng sau làm tròn | Must |

## 3. Kịch bản Gherkin

### PAY-AC-001 — Tính kỳ (PAY-FR-001)

```gherkin
  Scenario: Happy — TIM đã chốt
    Given HR/C&B; tháng TIM đã chốt; master kỳ có
    When HR tính kỳ trên PAY-SCR-002
    Then hệ thống đọc N_thực, OT, N_KHL, HĐ, master; bản Draft
  Scenario: Negative — TIM chưa chốt
    Given tháng TIM chưa chốt
    When HR tính kỳ
    Then chặn; không Draft
```

### PAY-AC-002 — N_tính (PAY-FR-002)

```gherkin
  Scenario: Happy — trừ KHL, không cộng phép hưởng
    Given N_thực gồm phép hưởng; N_KHL = K
    When hệ thống tính N_tính
    Then N_tính = N_thực − K; không + ngày phép hưởng
  Scenario: Negative — công thức cộng phép hưởng
    When bản tính cộng N_phép_hưởng vào N_tính
    Then fail AC; không chốt kỳ
```

### PAY-AC-003 — 85% (PAY-FR-003)

```gherkin
  Scenario: Happy — HĐ thử việc tại kỳ
    Given trạng thái HĐ = thử việc tại kỳ
    When tính lương thời gian
    Then hệ số 0,85
  Scenario: Happy — đã chính thức
    Given hết TV tại kỳ
    Then hệ số 100%
```

### PAY-AC-004 — OT (PAY-FR-004)

```gherkin
  Scenario: Happy — OT từ bảng công chốt
    Given loại OT 1.5/2.0/3.0 trên TIM đã chốt
    When tính kỳ
    Then OT đúng loại; PAY không có ô nhập OT
  Scenario: Negative — nhập OT tay trên PAY
    When HR nhập OT trên PAY
    Then cấm
```

### PAY-AC-005 — PC hai kênh (PAY-FR-005)

```gherkin
  Scenario: Happy — HĐ + dòng tháng mã hợp lệ
    Given PC HĐ và mã tháng ∈ master kỳ
    When tính kỳ
    Then cả hai kênh vào preview
  Scenario: Negative — mã không trên master
    When HR gắn mã lạ
    Then chặn ghi nhận / chặn chốt
```

### PAY-AC-006 — BH/TNCN (PAY-FR-006)

```gherkin
  Scenario: Happy — tỷ lệ master kỳ
    Given % BH/TNCN trên master kỳ ≠ số URD cũ
    When tính kỳ
    Then dùng % kỳ
  Scenario: Negative — hardcode % URD
    Then fail AC
```

### PAY-AC-007 — Trần ngày công (PAY-FR-007)

```gherkin
  Scenario: Negative — N_tính > chuẩn tháng
    Given N_tính > ngày công chuẩn lịch Cty
    When HR chốt PAY-SCR-003
    Then chặn chốt; preview vẫn xem cảnh báo
  Scenario: Happy — N_tính ≤ chuẩn
    When HR chốt
    Then kỳ Chốt nếu hết lỗi khác
```

### PAY-AC-008 — Không sửa công (PAY-FR-008)

```gherkin
  Scenario: Negative — sửa N_thực/OT/phép trên PAY
    When HR sửa ngày công trên PAY-SCR-002
    Then cấm; gợi ý sửa TIM rồi chốt lại
```

### PAY-AC-009 — Quyền chạy lương (PAY-FR-009)

```gherkin
  Scenario: Negative — NV tính kỳ
    When NV gọi tính/chốt/PC tháng/xuất hàng loạt
    Then 403
  Scenario: Negative — LM chốt kỳ
    When LM chốt
    Then 403
```

### PAY-AC-010 — Cô lập phiếu (PAY-FR-010)

```gherkin
  Scenario: Happy — NV xem phiếu mình kỳ Chốt
    Given kỳ đã chốt; NV chủ phiếu
    When mở PAY-SCR-005
    Then 200 phiếu mình
  Scenario: Negative — NV xem người khác
    Then 403
  Scenario: Negative — LM xem lương cấp dưới
    Then 403
  Scenario: Negative — kỳ Draft
    Then NV không thấy phiếu
```

### PAY-AC-011 — Mobile (PAY-FR-011)

```gherkin
  Scenario: Happy — cùng phiếu web
    Given cùng user kỳ Chốt trên PAY-SCR-006
    Then dữ liệu = web; không nới quyền
  Scenario: Negative — mobile xem phiếu người khác
    Then 403 cùng web
```

### PAY-AC-012 — Xuất hàng loạt (PAY-FR-012)

```gherkin
  Scenario: Happy — PDF/email đúng chủ
    Given kỳ Chốt; HR xuất PAY-SCR-007
    Then mỗi file/email gắn đúng NV
  Scenario: Negative — CC LM hoặc gửi nhầm
    Then cấm
```

### PAY-AC-013 — A-001 (PAY-FR-013)

```gherkin
  Scenario: Negative — TIM tách phép hưởng khỏi N_thực
    Given N_thực không gồm phép hưởng
    When tính kỳ
    Then cảnh báo CR; không im lặng cộng lại; không tự sửa công
```

### PAY-AC-014 — Preview cột (PAY-FR-014)

```gherkin
  Scenario: Happy — đủ cột DOC-19
    When HR mở PAY-SCR-002 sau tính thành công
    Then có N_thực, N_KHL, N_tính, hệ số TV, OT, PC HĐ, PC tháng, BH, TNCN tạm, thực lĩnh
```

### PAY-AC-015 — PC tháng (PAY-FR-015)

```gherkin
  Scenario: Happy — lưu mã master
    Given HR trên PAY-SCR-004; mã ∈ master kỳ
    When lưu số tiền
    Then dòng tháng có trên tính kỳ
  Scenario: Negative — mã không master
    Then chặn lưu
```

### PAY-AC-016 — Draft / không hủy chốt (PAY-FR-016)

```gherkin
  Scenario: Happy — tính lại kỳ Draft
    Given kỳ chưa chốt
    When HR tính lại
    Then ghi đè Draft
  Scenario: Negative — đè kỳ đã chốt
    Given kỳ Chốt
    When tính lại im lặng
    Then cấm
  Scenario: Negative — nút hủy chốt MVP
    Then không có trên PAY-SCR-003
```

### PAY-AC-017 — Ẩn màn HR (PAY-FR-017)

```gherkin
  Scenario: Negative — NV mở DS kỳ
    When NV vào PAY-SCR-001
    Then 403 hoặc không menu
  Scenario: Happy — NV chỉ phiếu mình
    Then chỉ PAY-SCR-005/006
```

### PAY-AC-018 — UAT 0 đồng (PAY-FR-018)

```gherkin
  Scenario: Happy — kỳ mẫu
    Given cùng master làm tròn quy chế; bảng tay C&B
    When so từng dòng + tổng vs hệ thống
    Then lệch = 0 đồng
  Scenario: Negative — lệch ≠ 0
    Then fail UAT; không go-live kỳ đó
```

> Quy tắc làm tròn cụ thể = master quy chế kỳ UAT (PAY-A-004) — không hardcode số chữ số trên AC này.

## 4. Checklist NFR / manual

| AC ID | Criteria | Pass / Fail | Tester | Date |
|-------|----------|-------------|--------|------|
| PAY-AC-NFR-001 | Cô lập phiếu + 403 — DOC-13 | | | |
| PAY-AC-NFR-002 | Log tính / chốt / xuất / xem phiếu — DOC-13 | | | |

## 5. DoD slice lương

- [ ] 100% PAY-AC Must (001–018) pass
- [ ] Sign-off PGD (cổng AC payroll)

## 6. Truy vết

| AC ID | FR | UC | Test Case (DOC-16) |
|-------|----|----|---------------------|
| PAY-AC-001 | PAY-FR-001 | PAY-UC-001 | |
| PAY-AC-002 | PAY-FR-002 | PAY-UC-001 | |
| PAY-AC-003 | PAY-FR-003 | PAY-UC-001 | |
| PAY-AC-004 | PAY-FR-004 | PAY-UC-001 | |
| PAY-AC-005 | PAY-FR-005 | PAY-UC-001 | |
| PAY-AC-006 | PAY-FR-006 | PAY-UC-001 | |
| PAY-AC-007 | PAY-FR-007 | PAY-UC-002 | |
| PAY-AC-008 | PAY-FR-008 | PAY-UC-001 | |
| PAY-AC-009 | PAY-FR-009 | PAY-UC-001 | |
| PAY-AC-010 | PAY-FR-010 | PAY-UC-004 | |
| PAY-AC-011 | PAY-FR-011 | PAY-UC-004 | |
| PAY-AC-012 | PAY-FR-012 | PAY-UC-005 | |
| PAY-AC-013 | PAY-FR-013 | PAY-UC-001 | |
| PAY-AC-014 | PAY-FR-014 | PAY-UC-001 | |
| PAY-AC-015 | PAY-FR-015 | PAY-UC-003 | |
| PAY-AC-016 | PAY-FR-016 | PAY-UC-001 | |
| PAY-AC-017 | PAY-FR-017 | PAY-UC-004 | |
| PAY-AC-018 | PAY-FR-018 | PAY-UC-002 | |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-015) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
