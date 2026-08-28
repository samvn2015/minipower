# DOC-04 — Quy tắc nghiệp vụ — Identity (IAM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (cổng BR · IAM · DEC-REQ-041) |

**Module:** identity · **MOD:** IAM · **Phạm vi:** URD III (nguyên tắc), CN-002, BRQ-001/006, NFR-002…006, **ma trận quyền distill từ DOC-04/05/06/07 đã chốt** (LEV, PAY, TIM, EMP, LIF).  
**Không:** ATS; SSO/MFA (DOC-08); đóng từng nút UI; PRB/EVT/RPT (chưa SRS — cùng nguyên tắc 005/008 khi mở); CRM sales.

**Cổng:** DEC-REQ-041 **đã chốt** 2026-08-25 (v0.2). Nợ: bảng III file URD gốc; PRB/EVT/RPT; Ban HR ☐. **Chưa** `02-baseline/`. Mở: DOC-05 + khung 19. **Không** tự viết.

---

## 1. Mục đích & phạm vi

Một IAM web+mobile; cô lập lương; 403 màn HR; gán role có kiểm soát.

**Role MVP (mã catalog — tên hiển thị master):**

| Mã | Tên | SH |
|----|-----|-----|
| IAM-ROLE-NV | Nhân viên | SH-004 |
| IAM-ROLE-LM | Line Manager | SH-005 |
| IAM-ROLE-HR | HR / C&B | SH-002 |
| IAM-ROLE-IT | IT Admin | SH-006 |
| IAM-ROLE-PGD | PGD / BGĐ (app) | SH-001 / SH-008 |

PGD **không** mặc định xem lương Cty trừ khi catalog gán thêm IAM-ROLE-HR (hoặc permission lương). Báo cáo quỹ (RPT) = module sau.

## 2. Danh mục quy tắc nghiệp vụ

| ID | Tên | Mô tả rule | Loại | Priority | Trace | Owner |
|----|-----|------------|------|----------|-------|-------|
| IAM-BR-001 | Đăng nhập | API/màn Must cần phiên; MVP **không** HRM public | Authorization | Must | URD III | SH-006 |
| IAM-BR-002 | Cùng IAM 2 kênh | Web = mobile: user, role, 403 | Authorization | Must | BRQ-006 · CN-002 | SH-006 |
| IAM-BR-003 | Catalog + 5 role MVP | 5 mã §1 + permission master; thêm role = master, không sửa BR | Inference | Must | DEC-DIS-014 | SH-006 |
| IAM-BR-004 | LM không lương | LM cấm phiếu/số lương NV khác (kể cả cấp dưới) | Authorization | Must | G-007 · PAY-BR-007 | SH-002 |
| IAM-BR-005 | NV chỉ mình | Hồ sơ được phép, phép, quỹ mình, phiếu kỳ chốt mình | Authorization | Must | EMP/LEV/PAY | SH-004 |
| IAM-BR-006 | HR SoT | Hồ sơ Cty, TIM, PAY kỳ, LEV C2, LIF N — §4 | Authorization | Must | SH-002 | SH-002 |
| IAM-BR-007 | IT kỹ thuật | LIF cấp/khóa Git–CRM; **không** lương mặc định | Authorization | Must | LIF-BR-008 | SH-006 |
| IAM-BR-008 | 403 màn HR | NV/LM: TIM import/chốt, PAY kỳ, EMP DS Cty, LIF khóa Git | Authorization | Must | NFR-004 | SH-006 |
| IAM-BR-009 | Đổi LM không nới lương | Gán LM ≠ quyền phiếu | Authorization | Must | EMP-BR-011 | SH-002 |
| IAM-BR-010 | Khóa TK | Trạng thái hiệu lực/vô hiệu; LIF/IT gọi; HR không SSH Git | Authorization | Must | BRQ-005 | SH-006 |
| IAM-BR-011 | PII + audit lương | PII theo catalog; mọi xem phiếu lương ghi audit | Authorization | Must | CN-002 · NFR-005 | SH-006 |
| IAM-BR-012 | Không CRM sales | Không federation/token sang CRM bán hàng | Constraint | Must | DEC-DIS-001 | SH-002 |
| IAM-BR-013 | Gán role | Chỉ IAM-ROLE-IT hoặc IAM-ROLE-HR (catalog) được gán/gỡ role người khác | Authorization | Must | URD III | SH-006 |
| IAM-BR-014 | Hợp quyền | Nhiều role: hợp permission; **cấm lương** nếu không có permission lương (LM+HR mới xem lương HR) | Authorization | Must | IAM-BR-004 | SH-006 |
| IAM-BR-015 | LEV C1 | LM được xem/duyệt đơn phép **Open C1** của cấp dưới — **không** suy ra xem lương | Authorization | Must | LEV | SH-005 |
| IAM-BR-016 | LEV C2 / đột xuất | Chỉ HR trừ quỹ / duyệt đột xuất (LEV đã chốt) | Authorization | Must | LEV-BR | SH-002 |
| IAM-BR-017 | Tài khoản gắn NV | Login map 1–1 MNV đang hiệu lực (EMP); TK vô hiệu ≠ xóa hồ sơ | Inference | Must | EMP | SH-006 |

## 3. Chi tiết quy tắc

### IAM-BR-001 — Phiên

| Mục | Nội dung |
|-----|----------|
| **Statement** | Không phiên → 401. Cookie/JWT = DOC-08. |
| **Condition** | IF API Must không phiên |
| **Action** | THEN 401 |
| **Source** | URD III |

### IAM-BR-002 — Web = mobile

| Mục | Nội dung |
|-----|----------|
| **Statement** | Cùng user/role/403 hai kênh. |
| **Condition** | IF mobile nới quyền |
| **Action** | THEN fail CN-002 |
| **Source** | BRQ-006 |

### IAM-BR-003 — Catalog

| Mục | Nội dung |
|-----|----------|
| **Statement** | 5 role §1 = MVP Must. Permission lẻ = master. Không đóng list nút trên BR. |
| **Source** | DEC-DIS-014 |

### IAM-BR-004 — Manager không lương

| Mục | Nội dung |
|-----|----------|
| **Statement** | LM không phiếu / thực lĩnh / BH-TNCN NV khác. |
| **Condition** | IF LM mở phiếu người khác |
| **Action** | THEN 403 |
| **Source** | G-007 · PAY-BR-007 |

### IAM-BR-005 — NV mình

| Mục | Nội dung |
|-----|----------|
| **Statement** | Resource.owner ≠ user → 403 (trừ HR catalog). |
| **Source** | EMP-BR-009 · LEV · PAY phiếu mình |

### IAM-BR-006 / 007 / 008

Theo **§4**. HR = SoT nghiệp vụ Cty. IT = kỹ thuật LIF, không PAY. NV/LM 403 màn HR.

### IAM-BR-009

Đổi LM không copy permission lương.

### IAM-BR-010

Disable login HRM và/hoặc connector Git-CRM theo LIF; HR không credential Git.

### IAM-BR-011

Xem phiếu lương → audit. CCCD/MST: EMP + role.

### IAM-BR-012

Không identity sang CRM sales.

### IAM-BR-013 — Gán role

| Mục | Nội dung |
|-----|----------|
| **Statement** | NV không tự nâng role. LM không gán HR. |
| **Condition** | IF NV/LM PATCH role người khác |
| **Action** | THEN 403 |
| **Source** | URD III |

### IAM-BR-014 — Hợp quyền

| Mục | Nội dung |
|-----|----------|
| **Statement** | Union permission. Thiếu permission `payslip.read.any` / tương đương catalog → 403 lương người khác. Role LM **không** chứa permission đó. |
| **Source** | IAM-BR-004 |

### IAM-BR-015 / 016 — Phép

C1 = LM cấp dưới (LEV đã chốt, không Matrix). C2 + đột xuất = HR. Không suy ra lương.

### IAM-BR-017 — Map MNV

| Mục | Nội dung |
|-----|----------|
| **Statement** | Tài khoản gắn đúng một hồ sơ EMP hiệu lực. Off: disable TK (BR-010), hồ sơ vẫn SoT EMP. |
| **Source** | EMP · LIF |

## 4. Ma trận quyền MVP (distill module đã chốt)

**Chú thích:** C = được · K = cấm (403) · — = ngoài module / chưa SRS.

| Năng lực | NV | LM | HR | IT | PGD mặc định |
|----------|----|----|----|----|----------------|
| Đăng nhập 2 kênh | C | C | C | C | C |
| Hồ sơ mình (field IAM) | C | C | C | C | C |
| Hồ sơ người khác / DS Cty EMP | K | K | C | K | K |
| Đề xuất/duyệt đổi LM | K | K* | C | K | K* |
| Đơn phép mình + quỹ mình | C | C | C | K | C |
| C1 phép cấp dưới | K | C | C | K | K |
| C2 / đột xuất / trừ quỹ | K | K | C | K | K |
| TIM mẫu/import/chốt | K | K | C | K** | K |
| PAY tính/chốt/PC tháng/xuất lô | K | K | C | K | K |
| Phiếu lương mình (kỳ chốt) | C | C | C | K | C |
| Phiếu lương người khác | K | **K** | C | K | K |
| LIF xác nhận N / checklist HR | K | K | C | C*** | K |
| LIF khóa Git/CRM (thực thi) | K | K | K | C | K |
| Gán role IAM | K | K | C | C | K |
| CRM sales | K | K | K | K | K |
| PRB / EVT / RPT | — | — | — | — | — |

\* Đổi LM: MVP EMP-UC-004 = HR khởi tạo + một bậc duyệt (IAM-ROLE-HR hoặc PGD nếu catalog). LM **không** tự ghi LM.  
\*\* IT hỗ trợ file mẫu TIM **không** chốt công trừ khi catalog gán HR.  
\*\*\* IT tick mục kỹ thuật checklist; **không** xác nhận N thay HR (LIF-BR-009).

## 5. Bảng quyết định — Lương

| Role | Phiếu mình | Phiếu cấp dưới | Kỳ PAY HR |
|------|------------|----------------|-----------|
| NV | C (chốt) | K | K |
| LM | C (chốt, của mình) | **K** | K |
| HR | C | C | C |
| IT | K | K | K |
| PGD | C (của mình) | K | K |

## 6. Nhật ký thay đổi

| Phiên bản | BR ID | Thay đổi | CR Ref |
|-----------|-------|----------|--------|
| 0.1 | IAM-BR-001…012 | Distill nguyên tắc | — |
| 0.2 | IAM-BR-013…017 + §4 | Đủ ma trận; **chốt cổng BR** (DEC-REQ-041) | — |

## 7. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.2 (DEC-REQ-041) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
