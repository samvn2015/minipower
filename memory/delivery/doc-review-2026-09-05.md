# Doc-review gate — DOC-11 + DOC-12 · [2026-09-05]

**Slice:** `04-platform` — DOC-11 v0.1 (Chốt, DEC-ARC-008) · DOC-12 v0.1 (Chốt, DEC-ARC-010)
**Đối chiếu:** code `../hrm/` — build 0 error · 144/144 unit test pass · 56 migration · 41 entity · 87 endpoint / 13 controller
**Reviewer:** QC (một pass đối kháng, không subagent) · **Verdict: ⛔ BLOCK** — 3 Blocker

---

## Finding

### 🔴 Blocker

**[Blocker] Mâu thuẫn chéo — DOC-11 §1.2/§2/§3.5 · ADR-001 · ADR-002**
Bằng chứng: DOC-11 §2 `[IdentityAccount] 1───1 [Employee] (IAM DB vs EMP DB — copy ID, không 1 DB)`; §1.2 "FK xuyên service | chỉ **ID**, không join SQL hai DB"; §2 "**Cấm:** bảng lương trong IAM/LEV/TIM DB"; §3.5 "PAY DB tách PII". Code: **một** `Persistence/AppDbContext.cs` với 41 DbSet, một `ConnectionStrings`, một DB `hrm` (OQ-DLV-003).
Vì sao lỗi: NFR-002 (cô lập lương) hiện chỉ được bảo vệ ở tầng ứng dụng, không có ranh giới DB như thiết kế. Baseline DOC-11 = đóng băng kiến trúc chưa từng được xây; DR/replicate "cả N DB" (§6) không áp dụng được.
Đề xuất: SA + PGD quyết — sửa DOC-11 + ADR-001 về single-DB kèm `DEC-ARC-*` mới, **hoặc** ghi nợ kiến trúc có deadline. Không baseline trước khi quyết.

**[Blocker] Đầy đủ + Mâu thuẫn — DOC-11 §3, §8**
Bằng chứng: 15 entity trong DOC-11 không tồn tại trong code (`Payslip`, `PayFormulaSnapshot`, `PayrollPeriod`, `ProbationCase`, `ProbationTask`, `LmProposal`, `PrbDecision`, `LifecycleCase`, `AccessLock`, `ChecklistItem`, `AuthAudit`, `Contract`, `Education`, `TimesheetTemplate`, `ImportBatch`); 20 entity trong code không có trong DOC-11 (`PayAllowanceCatalog`, `PayContractAllowance`, `PayMonthlyAllowance`, `PayWorkdayCalendar`, `PayRegulation`, `PayExportOutbox`, `SeniorityRule`, `LineManagerChangeRequest`, `LeaveNotification`, `EmployeeCode`, `LifAccessLockOutbox`, `LifOn/OffChecklistItem`, `LifOn/OffChecklistTick`, `TimesheetTemplateVersion/Column`, `TimesheetImportBatch/Row`, `ProbationCriterion/Evaluation/Outcome/Reminder/ExtendDuration`).
Vì sao lỗi: `Payslip` là entity trung tâm PAY — §5 đánh dấu Sensitive **Y**, §8 trace `NFR-002 | Payslip chỉ DB-PAY`. Entity không tồn tại → dòng truy vết NFR-002 trỏ vào hư không, QC không viết được test từ DOC-11, dev đọc DOC-11 sẽ dựng sai schema.
Đề xuất: SA reverse-engineer §3 từ schema thật (56 migration + `AppDbContextModelSnapshot`), giữ nguyên §4 master data.

**[Blocker] Đầy đủ — DOC-12 §4 · `openapi.yaml`**
Bằng chứng: DOC-12 §4 liệt kê **17** endpoint; `openapi.yaml` có **16** path. Code có **87** endpoint trên 13 controller. 17 endpoint tài liệu **đều đã implement đúng** (đã kiểm `/iam/me`, `PATCH /emp/employees/{id:guid}`, `c1`, `c2`, `balances`, `imports`, `close`, `payslips`, `run`, `propose`, `decide`, `locks`), nhưng 70 endpoint còn lại không có ở đâu trong DOC-12. 4 controller nằm hoàn toàn ngoài spec: `DevAuth`, `EmpCatalog`, `LineManagerChangeRequests`, `TimekeepingDevice`.
Vì sao lỗi: baseline DOC-12 = ký một hợp đồng API mô tả 20% hệ thống. Đội tích hợp và QC đọc spec sẽ thiếu 4/5 bề mặt.
Đề xuất: SA sinh `openapi.yaml` từ Swagger runtime rồi rà tay, thay vì viết tiếp thủ công.

### 🟡 Major

**[Major] Mâu thuẫn — DOC-12 §2 vs `DevAuthController`**
Bằng chứng: DOC-12 §2 "**Cấm:** `POST /auth/login` với password"; code có `[Route("dev")] [HttpGet("token")]`. DEC-DLV-011 (2026-09-04) cho phép bypass JWKS ở DEV/UAT, nhưng DOC-12 Chốt 2026-08-26 chưa ghi ngoại lệ, và endpoint không được đánh dấu DEV-only trong spec.
Đề xuất: SA thêm ghi chú §5 "DEV/UAT only" + guard theo môi trường; hoặc mở CR.

**[Major] Testable — DOC-11 §1.2 · DOC-12 §4: kiểu PK vẫn "TBD"**
Bằng chứng: DOC-11 "PK | `{entity}Id` — kiểu **TBD** (UUID hoặc long)"; DOC-12 "`{id}` = string (kiểu PK TBD DOC-11)". Code đã chốt **Guid** (`[HttpPatch("{id:guid}")]`), 56 migration đã áp.
Đề xuất: đóng TBD bằng `DEC-ARC-*` ghi nhận Guid.

**[Major] Mâu thuẫn — DOC-11 §3.4 TIM template**
Bằng chứng: "TimesheetTemplate | version, master **một** tại một thời điểm" (1 entity). Code tách `TimesheetTemplateVersion` + `TimesheetTemplateColumn`, cột động hiện thực qua `TimesheetImportBatch`/`TimesheetImportRow`.
Đề xuất: SA cập nhật §3.4 theo mô hình thật.

**[Major] Traceability — DOC-11 §8 thiếu LEV, TIM**
Bằng chứng: bảng §8 chỉ trace ADR-002, INT-001, INT-003, INT-004/005, EMP unique, PAY, PRB, NFR-002. Không dòng nào nối `LeaveBalance`/`LeaveRequest`/`LeaveAudit` → LEV-FR, hay `TimesheetPeriod`/`TimesheetLine` → TIM-FR.
Đề xuất: BA owner LEV/TIM bổ sung dòng trace.

**[Major] Testable — ngưỡng NFR trống**
Bằng chứng: DOC-11 §6 retention toàn "**TBD** quy chế/luật"; DOC-12 §7 Rate "**TBD**", Timeout GW "**TBD**".
Vì sao lỗi: QC không viết được test hiệu năng/lưu trữ. Trùng nợ BLK-002 (RTO/RPO).

### ⚪ Minor

- **[Minor] DOC-11 §3.1** — `AuthAudit` riêng IAM vs code dùng `EmpAuditLog` chung cho 7 module (DEC-DLV-022/024).
- **[Minor] DOC-11 §3.2** — `Contract`/`Education` vs code `EmployeeContract`/`EducationLevel`; quy ước tên §1.2 không được giữ.
- **[Minor] DOC-11 §3.8** — "Notification (DB-NOTIF, **nếu tách**)" chưa đóng nhánh; code có `LeaveNotification` + `PayExportOutbox` trong DB chung.
- **[Minor] DOC-11 §9** — ô Business Owner "☐ Nợ", nhưng BLK-005 / DEC-DLV-008 ghi Ban HR đã ký 2026-08-26.

---

## Trace gaps

| Chuỗi | Trạng thái |
|---|---|
| `NFR-002 → Payslip → DB-PAY` | **Đứt** — entity `Payslip` không tồn tại trong code |
| `LEV-FR → LeaveBalance/LeaveRequest` | **Đứt** — DOC-11 §8 không có dòng |
| `TIM-FR → TimesheetPeriod/Line` | **Đứt** — DOC-11 §8 không có dòng |
| `PRB-FR-009 → /prb/.../decide` | ✓ đủ (DOC-12 §8) |
| `INT-004/005 → AccessLock` | **Đứt** — code dùng `LifAccessLockOutbox` |
| `INT-003 → ImportBatch` | **Đứt** — code dùng `TimesheetImportBatch` |
| 70/87 endpoint → FR | **Đứt** — không có trong DOC-12 |

---

## Verdict

⛔ **BLOCK baseline** — 3 Blocker, 5 Major, 4 Minor.

Không mở `docs/02-baseline/` cho DOC-11 và DOC-12 cho tới khi 3 Blocker được owner (SA) xử lý và PGD chốt hướng cho Blocker kiến trúc dữ liệu.

DOC-11 và DOC-12 đang mang trạng thái **Chốt** — trạng thái này nay không phản ánh thực tế; cần hạ về Draft hoặc mở CR trước khi sửa.

**Không tự sửa DOC** — mọi finding chuyển owner SA (Trịnh Yên soạn, PGD chốt).
