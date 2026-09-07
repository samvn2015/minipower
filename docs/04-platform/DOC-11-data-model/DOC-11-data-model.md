# DOC-11 — Mô hình Dữ liệu (khung)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (khung ER · DEC-ARC-008) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — §2 và §3 sinh lại từ schema thật, đóng doc-review **Blocker B2** |

**UML / ERD khái niệm** · DOC-08 **Chốt** · ADR-001 DB-per-service **Accepted** · ADR-002 **Accepted**.  
**Cổng:** PGD chốt v0.1 (DEC-ARC-008). Engine: **PostgreSQL** (ADR-009). Nợ: version/host/connection; UUID vs bigint; list field master; EVT/RPT. **Chưa** `02-baseline/`.

Không hardcode tỷ lệ BH/TNCN, số ngày phép luật, cột Excel — catalog.

---

## 1. Giới thiệu

### 1.1 Mục đích & Phạm vi

Khung **thực thể + ranh giới service** cho 7 module Must. Không physical DDL. Tham chiếu DOC-06 (FR), DOC-10 (INT).

### 1.2 Quy ước

| Quy ước | Giá trị |
|---------|---------|
| Tên entity | PascalCase |
| PK | `Id` kiểu **Guid** — chốt trong code, 56 migration đã áp *(v0.1 ghi TBD)* |
| FK xuyên service | chỉ **ID**, không join SQL hai DB |
| Audit nghiệp vụ | ai/khi nào trên thực thể chốt (NFR-005) |

## 2. Mô hình khái niệm

> **v0.2 — trạng thái hiện tại.** §2 của v0.1 mô tả **N database tách theo service**. Code hiện là **một** `AppDbContext` / một database `hrm` (doc-review Blocker B1). Đích đến vẫn là DB-per-service theo ADR-001, lộ trình ở **[ADR-011](../DOC-09-adr/ADR-011-lo-trinh-tach-service.md)** wave 1. Sơ đồ dưới mô tả **quan hệ nghiệp vụ**, không phải ranh giới vật lý.

```text
[IdentityAccount] ──(IdpSubject)── [Employee]     (cùng DB hiện tại; tách schema ở W1)
[Employee] 1───* [EmployeeContract] ── KtTv
[Employee] *───1 [OrgUnit];  *───1 LM (LineManagerChangeRequest duyệt đổi)
[Employee] 1───* [LeaveBalance] *───1 [LeaveType]
[LeaveRequest] *───1 Employee; C1/C2 approve|reject; [LeaveNotification] outbox
[TimesheetTemplateVersion] 1───* [TimesheetTemplateColumn]
[TimesheetImportBatch] 1───* [TimesheetImportRow] → [TimesheetPeriod] 1───* [TimesheetLine]
[PayPeriod] 1───* [PayLine];  [PayContractSalary] [Pay*Allowance*] [PayRegulation] [PayWorkdayCalendar]
[ProbationEvaluation] *───1 Employee; [ProbationCriterion] [ProbationOutcome] [ProbationExtendDuration] [ProbationReminder]
[LifOnboardingCase] / [LifOffboardingCase] 1───* [Lif*ChecklistItem] / [Lif*ChecklistTick]
[LifAccessLockOutbox] → INT-004/005
[EmpAuditLog] — audit **dùng chung 7 module** (DEC-DLV-022/024), không phải AuthAudit riêng IAM
```

**Ràng buộc giữ nguyên từ v0.1:** cấm bảng lương lộ cho LM (NFR-002); cấm event CRM sales. **Khác v0.1:** hàng rào NFR-002 hiện **chỉ ở tầng ứng dụng** — chưa có GRANT/schema riêng (ADR-011 W1 mới đóng được).

## 3. Mô hình logic — **sinh từ schema thật**

> Bảng dưới trích từ `AppDbContextModelSnapshot` (56 migration đã áp), ngày 2026-09-07. **41 entity.** v0.1 liệt kê ~30 entity với tên khác hẳn — đó là nguồn của Blocker B2. Cột *Thuộc tính chính* rút gọn 7 cột đầu, không phải full DDL.

### 3.1 IAM — 3 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **AccountRole** | `iam_account_role` | `AccountId`, `RoleCode` | — |
| **IdentityAccount** | `iam_identity_account` | `DisplayName`, `EmailCty`, `EmployeeCode`, `IdpSubject`, `Status` | `IdpSubject` |
| **Role** | `iam_role` | `RoleCode`, `Name` | — |

### 3.2 EMP — 7 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **EducationLevel** | `emp_education_level` | `Code`, `Name`, `Status` | — |
| **EmpAuditLog** | `emp_audit_log` | `Action`, `ActorIdpSubject`, `Detail`, `EmployeeId`, `OccurredAtUtc`, `RelatedId` | — |
| **Employee** | `emp_employee` | `Cccd`, `EducationLevelCode`, `EmailCty`, `EmployeeCode`, `FullName`, `LineManagerEmployeeId`, `OrgUnitCode` … *(+3)* | `Cccd`, `EmailCty`, `EmployeeCode`, `TaxId` |
| **EmployeeContract** | `emp_contract` | `ContractType`, `EmployeeId`, `EndDate`, `IsProbation`, `StartDate` | `EmployeeId` |
| **LineManagerChangeRequest** | `emp_line_manager_change` | `EmployeeId`, `ProposedLineManagerEmployeeId`, `RequestedAtUtc`, `RequestedByIdpSubject`, `ReviewNote`, `ReviewedAtUtc`, `ReviewedByIdpSubject` … *(+1)* | — |
| **OrgUnit** | `emp_org_unit` | `Code`, `Name`, `Status` | — |
| **SeniorityRule** | `emp_seniority_rule` | `Code`, `BasisType`, `Status` | — |

### 3.3 LEV — 4 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **LeaveBalance** | `lev_leave_balance` | `EmployeeId`, `EntitledDays`, `UsedDays`, `Year` | — |
| **LeaveNotification** | `lev_notification_outbox` | `Channel`, `CreatedAtUtc`, `EmployeeId`, `EventType`, `LeaveRequestId`, `Message` | — |
| **LeaveRequest** | `lev_leave_request` | `AttachmentFileName`, `AttachmentMatchesCompanyTemplate`, `C1ReviewNote`, `C1ReviewedAtUtc`, `C1ReviewedByIdpSubject`, `C2ReviewNote`, `C2ReviewedAtUtc` … *(+12)* | — |
| **LeaveType** | `lev_leave_type` | `Code`, `DeductsAnnualBalance`, `Name`, `RequiresCompanyTemplateFile`, `Status` | — |

### 3.4 TIM — 6 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **TimesheetImportBatch** | `tim_import_batch` | `ErrorRows`, `FileName`, `HasMustErrors`, `PeriodYm`, `Status`, `TemplateVersionCode`, `TemplateVersionId` … *(+3)* | — |
| **TimesheetImportRow** | `tim_import_row` | `BatchId`, `EmployeeCode`, `EmployeeId`, `ErrorCode`, `ErrorMessage`, `IsOk`, `Ot15` … *(+5)* | — |
| **TimesheetLine** | `tim_timesheet_line` | `EmployeeCode`, `EmployeeId`, `LeaveDaysOther`, `LeaveDaysPaid`, `LeaveDaysUnpaid`, `Ot15`, `Ot20` … *(+4)* | — |
| **TimesheetPeriod** | `tim_period` | `ClosedAtUtc`, `ClosedByIdpSubject`, `CommittedAtUtc`, `CommittedByIdpSubject`, `PeriodYm`, `SourceImportBatchId`, `Status` | `PeriodYm` |
| **TimesheetTemplateColumn** | `tim_template_column` | `ColumnKey`, `DisplayName`, `IsRequired`, `MapsTo`, `SortOrder`, `TemplateVersionId` | — |
| **TimesheetTemplateVersion** | `tim_template_version` | `Name`, `PublishedAtUtc`, `PublishedByIdpSubject`, `Status`, `VersionCode` | `VersionCode` |

### 3.5 PAY — 9 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **PayAllowanceCatalog** | `pay_allowance_catalog` | `Code`, `IsActive`, `Name` | `Code` |
| **PayContractAllowance** | `pay_contract_allowance` | `Amount`, `Code`, `EmployeeCode`, `EmployeeId` | — |
| **PayContractSalary** | `pay_contract_salary` | `Amount`, `DependentCount`, `EmployeeCode`, `EmployeeId` | `EmployeeId` |
| **PayExportOutbox** | `pay_export_outbox` | `CcAddress`, `Channel`, `CreatedAtUtc`, `CreatedByIdpSubject`, `EmployeeCode`, `PdfFileName`, `PeriodYm` … *(+2)* | — |
| **PayLine** | `pay_line` | `BhAmount`, `BhRate`, `ContractAllowance`, `EmployeeCode`, `EmployeeId`, `LeaveDaysPaid`, `LeaveDaysUnpaid` … *(+11)* | — |
| **PayMonthlyAllowance** | `pay_monthly_allowance` | `Amount`, `Code`, `EmployeeCode`, `EmployeeId`, `PeriodYm` | — |
| **PayPeriod** | `pay_period` | `ClosedAtUtc`, `ClosedByIdpSubject`, `PeriodYm`, `RanAtUtc`, `RanByIdpSubject`, `Status` | `PeriodYm` |
| **PayRegulation** | `pay_regulation` | `Code`, `DecimalValue`, `Name` | `Code` |
| **PayWorkdayCalendar** | `pay_workday_calendar` | `PeriodYm`, `StandardWorkDays` | `PeriodYm` |

### 3.6 PRB — 5 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **ProbationCriterion** | `prb_criterion` | `Code`, `IsActive`, `Name`, `SortOrder` | `Code` |
| **ProbationEvaluation** | `prb_evaluation` | `CriteriaPayloadJson`, `DecidedAtUtc`, `DecidedByIdpSubject`, `DecidedOutcomeCode`, `DecisionNote`, `EmployeeCode`, `EmployeeId` … *(+7)* | — |
| **ProbationExtendDuration** | `prb_extend_duration` | `Code`, `IsActive`, `Months`, `Name`, `SortOrder` | `Code` |
| **ProbationOutcome** | `prb_outcome` | `Code`, `IsActive`, `Name`, `SortOrder` | `Code` |
| **ProbationReminder** | `prb_reminder` | `AsOfDate`, `AssigneeEmployeeCode`, `AssigneeEmployeeId`, `Channel`, `CreatedAtUtc`, `CreatedByIdpSubject`, `DueDate` … *(+6)* | — |

### 3.7 LIF — 7 entity

| Entity | Bảng | Thuộc tính chính | Unique |
|--------|------|------------------|--------|
| **LifAccessLockOutbox** | `lif_access_lock_outbox` | `AsOfDate`, `CaseId`, `Channel`, `CrReason`, `CreatedAtUtc`, `CreatedByIdpSubject`, `EmployeeCode` … *(+3)* | — |
| **LifOffChecklistItem** | `lif_off_checklist_item` | `Code`, `IsActive`, `IsMust`, `Name`, `SortOrder` | `Code` |
| **LifOffChecklistTick** | `lif_off_checklist_tick` | `CheckedAtUtc`, `CheckedByIdpSubject`, `IsChecked`, `ItemCode`, `OffboardingCaseId` | — |
| **LifOffboardingCase** | `lif_offboarding_case` | `ConfirmedAtUtc`, `ConfirmedByIdpSubject`, `CreatedAtUtc`, `CreatedByIdpSubject`, `CrmSpLockedAtUtc`, `EarlyCrReason`, `EmployeeCode` … *(+10)* | — |
| **LifOnChecklistItem** | `lif_on_checklist_item` | `Code`, `IsActive`, `IsMust`, `Name`, `SortOrder` | `Code` |
| **LifOnChecklistTick** | `lif_on_checklist_tick` | `CheckedAtUtc`, `CheckedByIdpSubject`, `IsChecked`, `ItemCode`, `OnboardingCaseId` | — |
| **LifOnboardingCase** | `lif_onboarding_case` | `ChatProvisioned`, `ChatProvisionedAtUtc`, `ClosedAtUtc`, `ClosedByIdpSubject`, `CreatedAtUtc`, `CreatedByIdpSubject`, `CrmSpProvisioned` … *(+9)* | — |

## 4. Master data

| Domain | Golden record | SoT | Sync |
|--------|---------------|-----|------|
| Employee / HĐ / KT_TV | EMP | EMP | PRB đọc, không ghi ảo |
| Org / LM | EMP | EMP | IAM chỉ map account |
| Loại phép, trần ngày | Catalog HR | LEV | động quy chế |
| Mẫu Excel CC | TIM template | TIM | 1 version hiệu lực |
| PC / BH / TNCN tỷ lệ | Catalog kỳ | PAY | không hardcode luật |
| Role HRM | IAM | IAM | không copy IdP group làm SoT |
| Git/CRM user id | IT + LIF map | LIF lock | INT-004/005 |

## 5. Từ điển (nhạy cảm)

| Attribute | Định nghĩa | Sensitive |
|-----------|------------|-----------|
| `PayLine` (+ `PayPeriod`) | Dòng lương theo kỳ. **Không có entity `Payslip`** — phiếu lương là *query* trên `PayLine`, sửa từ v0.2 | **Y** — chỉ PAY + chính chủ |
| `Cccd`, `Mst` trên `Employee` | Định danh | **Y** |
| idpSubject | Khóa SSO | Y (không public log) |
| `KtTv` trên `EmployeeContract` | Mốc TV từ HĐ | N (EMP) |
| `ProbationEvaluation` + `ProbationOutcome` | 3 mã kết quả (thay `PrbDecision` của v0.1) | N + audit HR |

## 6. Lưu trữ & vòng đời

| Entity | Retention | Ghi chú |
|--------|-----------|---------|
| `PayLine`/`PayPeriod`, `TimesheetPeriod` chốt, `LeaveRequest` C2, `ProbationEvaluation`, `LifAccessLockOutbox`, `EmpAuditLog` | **TBD quy chế / luật** — không bịa 7 năm | NFR-005 không xóa tay NV |
| `TimesheetImportBatch` file | TBD | |

Replicate Prod→DR theo ADR-003 (cả N DB).

## 7. Di chuyển dữ liệu

| Source | Target | Rules |
|--------|--------|-------|
| Tool/file as-is | EMP/LEV/TIM/PAY | **Động theo quy chế** (DEC-DIS-014); không inventory đóng băng |
| Volume / cleanse | TBD | Cutover DOC-17 |

## 8. Truy vết

| FR / INT / ADR | Entity |
|----------------|--------|
| ADR-002 | `IdentityAccount.IdpSubject` |
| INT-001 | `IdentityAccount` |
| LEV | `LeaveType`, `LeaveBalance`, `LeaveRequest`, `LeaveNotification` *(bổ sung v0.2)* |
| TIM | `TimesheetPeriod`, `TimesheetLine`, `TimesheetTemplateVersion/Column` *(bổ sung v0.2)* |
| INT-003 | `TimesheetImportBatch`, `TimesheetImportRow`, `TimesheetLine` |
| INT-004/005 | `LifAccessLockOutbox` |
| EMP unique | `Employee` — xem cột Unique §3.2 |
| PAY 85%, N_tính | `PayRegulation`, `PayWorkdayCalendar` *(không có `PayFormulaSnapshot`)* |
| PRB 3 mã, KT EMP | `ProbationOutcome`, `EmployeeContract.KtTv` |
| NFR-002 | `PayLine`/`PayPeriod` — **hiện chung DB**, hàng rào chỉ ở tầng ứng dụng (ADR-011 W1) |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** khung v0.1 (DEC-ARC-008) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
| Business Owner | Ban HR | | ☐ Nợ |
