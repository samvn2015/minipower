# DOC-11 — Mô hình Dữ liệu (khung)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (khung ER · DEC-ARC-008) |

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
| PK | `{entity}Id` — kiểu **TBD** (UUID hoặc long) |
| FK xuyên service | chỉ **ID**, không join SQL hai DB |
| Audit nghiệp vụ | ai/khi nào trên thực thể chốt (NFR-005) |

## 2. Mô hình khái niệm (theo service)

```text
[IdentityAccount] 1───1 [Employee]          (IAM DB vs EMP DB — copy ID, không 1 DB)
[Employee] 1───* [Contract] ── KT_TV
[Employee] *───1 [OrgUnit]; *───1 LM (EmployeeId)
[Employee] 1───* [LeaveBalance] *───1 [LeaveType]
[LeaveRequest] *───1 Employee; C1/C2
[TimesheetPeriod] 1───* [TimesheetLine] ← [ImportBatch] (INT-003)
[PayrollPeriod] 1───* [Payslip] *───* [PayLine]   (PAY DB tách PII)
[ProbationCase] 1───1 Employee; *─── [Eval] [PrbDecision]
[LifecycleCase] 1───1 Employee; *─── [AccessLock] → INT-004/005
```

**Cấm:** bảng lương trong IAM/LEV/TIM DB; event CRM sales.

## 3. Mô hình logic (khung thuộc tính)

Kiểu cột chi tiết → khi chọn engine. Dưới đây = **bắt buộc có** để khớp FR, không phải full schema.

### 3.1 IAM (DB-IAM) — ADR-002

| Entity | Thuộc tính khung | BR / NFR |
|--------|------------------|----------|
| IdentityAccount | accountId PK; idpSubject **unique**; emailCty; status | map INT-001 |
| Role | roleCode (HR, LM, NV, …) | IAM DOC-06 |
| AccountRole | accountId, roleCode | 403 màn HR |
| AuthAudit | accountId, at, action | NFR-005 |

**Không** lưu password hash (SSO). **Không** lưu Git token.

### 3.2 EMP (DB-EMP)

| Entity | Thuộc tính khung |
|--------|------------------|
| Employee | employeeId, mnv **unique**, cccd **unique**, emailCty **unique**, mst **unique**, lmEmployeeId nullable |
| OrgUnit | orgUnitId, parentId |
| Contract | contractId, employeeId, type, start, **ktTv** (mốc PRB — SoT EMP) |
| Education | employeeId, … (EMP-FR-017) — catalog không hardcode |

### 3.3 LEV (DB-LEV)

| Entity | Thuộc tính khung |
|--------|------------------|
| LeaveType | code, paid?, maxDays **catalog HR** |
| LeaveBalance | employeeId, year, type, remaining |
| LeaveRequest | requestId, employeeId, type, from, to, status, c1, c2; file ốm |
| LeaveAudit | chốt C2 / trừ quỹ |

Trừ quỹ khi **HR C2** (LEV-BR). OQ-010 hủy hộ — ngoài schema bắt buộc.

### 3.4 TIM (DB-TIM)

| Entity | Thuộc tính khung |
|--------|------------------|
| TimesheetTemplate | version, master **một** tại một thời điểm |
| ImportBatch | batchId, templateVersion, fileRef INT-003 |
| TimesheetPeriod | yearMonth, status chốt |
| TimesheetLine | employeeId, periodId, giờ/công/OT — cột động theo template |

### 3.5 PAY (DB-PAY) — cô lập

| Entity | Thuộc tính khung |
|--------|------------------|
| PayrollPeriod | yearMonth, status |
| Payslip | payslipId, employeeId, period; **không** share DB với LM |
| PayLine | componentCode catalog, amount |
| PayFormulaSnapshot | N_tính, 85% quy chế — giá trị tại kỳ |

LM **không** có GRANT đọc Payslip (NFR-002).

### 3.6 PRB (DB-PRB)

| Entity | Thuộc tính khung |
|--------|------------------|
| ProbationCase | employeeId, ktTv **copy từ EMP** (không date picker ảo) |
| ProbationTask | T-15 / T-7, assignee (LM hoặc HR) |
| LmProposal | không SoT HĐ |
| PrbDecision | Đạt / Gia hạn / Không đạt; decidedBy **HR**; audit |

Gia hạn → cập nhật Contract.ktTv phía EMP (API), rồi job theo KT mới.

### 3.7 LIF (DB-LIF)

| Entity | Thuộc tính khung |
|--------|------------------|
| LifecycleCase | on/off, lastWorkingDate N |
| AccessLock | system=Git\|CrmProduct, lockedAt, nPlus3, idempotencyKey INT-004/005 |
| ChecklistItem | catalog on/off |

### 3.8 Notification (DB-NOTIF, nếu tách)

Delivery log in-app + mail (INT-002). Không SoT phép/lương.

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
| Payslip / PayLine | Phiếu + dòng lương | **Y** — chỉ PAY + chính chủ |
| cccd, mst | Định danh | **Y** |
| idpSubject | Khóa SSO | Y (không public log) |
| ktTv | Mốc TV từ HĐ | N (EMP) |
| PrbDecision | 3 mã | N + audit HR |

## 6. Lưu trữ & vòng đời

| Entity | Retention | Ghi chú |
|--------|-----------|---------|
| Payslip, chốt công, C2, PRB decision, AccessLock | **TBD quy chế / luật** — không bịa 7 năm | NFR-005 không xóa tay NV |
| ImportBatch file | TBD | |

Replicate Prod→DR theo ADR-003 (cả N DB).

## 7. Di chuyển dữ liệu

| Source | Target | Rules |
|--------|--------|-------|
| Tool/file as-is | EMP/LEV/TIM/PAY | **Động theo quy chế** (DEC-DIS-014); không inventory đóng băng |
| Volume / cleanse | TBD | Cutover DOC-17 |

## 8. Truy vết

| FR / INT / ADR | Entity |
|----------------|--------|
| ADR-002 | IdentityAccount.idpSubject |
| INT-001 | IdentityAccount |
| INT-003 | ImportBatch, TimesheetLine |
| INT-004/005 | AccessLock |
| EMP unique | Employee mnv/cccd/email/mst |
| PAY 85%, N_tính | PayFormulaSnapshot |
| PRB 3 mã, KT EMP | PrbDecision, Contract.ktTv |
| NFR-002 | Payslip chỉ DB-PAY |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** khung v0.1 (DEC-ARC-008) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
| Business Owner | Ban HR | | ☐ Nợ |
