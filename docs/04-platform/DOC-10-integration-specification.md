# DOC-10 — Đặc tả Tích hợp

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (INT · DEC-ARC-006) |

**Hohpe EIP** · Adjunct DOC-08 **Chốt** · ADR-001 / 002 / 003 / **007** **Accepted**.  
**Cổng:** PGD chốt v0.1 (DEC-ARC-006). Nợ: **Lark issuer URL** (IT); Git/CRM API vendor; RTO/RPO phút; Ban HR ☐. **Chưa** `02-baseline/`. **Không** tự DOC-17. MVP INT-001 = **OIDC Lark** (ADR-007 v0.2).

**Cấm:** INT sang CRM **bán hàng** (NFR-007).

---

## 1. Tổng quan

### 1.1 Bản đồ tích hợp

```text
 [Lark IdP] ←OIDC→ [LBS] → [API Gateway] → [IAM|EMP|LEV|TIM|PAY|PRB|LIF|Job|Notif]
                                                          │
                    [NV/HR Web · Mobile] ─────────────────┘  (chỉ qua LBS+GW+SSO)

 Adapter (chỉ từ service Active):
   TIM  ← file Excel CC (1 mẫu master)
   Notif → SMTP @minvoice.vn
   LIF  → Git (khóa TK)     secret IT, không HR
   LIF  → CRM sản phẩm (khóa TK)
   ✗    → CRM bán hàng (không INT)
```

Replicate Prod→DR: **nội bộ DB**, không phải INT hệ ngoài (ADR-003).

### 1.2 Nguyên tắc tích hợp

| Principle | Mô tả |
|-----------|-------|
| Cổng duy nhất | Client/hệ ngoài **không** gọi thẳng microservice; trừ IdP (browser redirect) và SMTP/Git/CRM từ adapter LIF/Notif |
| SSO bắt buộc | Mọi phiên người dùng qua IdP; GW từ chối request không token (ADR-001) |
| Idempotency | Khóa Git/CRM N+3: gọi lại cùng NV+N không tạo khóa kép lỗi; import Excel theo file-id |
| Retry | Outbound Git/CRM/SMTP: retry hữu hạn + DLQ/alert; **không** retry tạo event CRM sales |
| Job một nơi | Adapter/job **chỉ** node Active (ADR-003) |
| Không PII lương qua INT | PAY không đẩy phiếu sang hệ ngoài trừ email **chính chủ** (INT-002) |
| Split-brain | Cấm dual-Active hai DC khi gọi Git/CRM |

## 2. Danh mục tích hợp

| INT ID | Hệ thống ngoài | Mục đích | Direction | Pattern | Protocol | Frequency | Owner |
|--------|----------------|----------|-----------|---------|----------|-----------|-------|
| INT-001 | **Lark** SSO IdP Cty | Xác thực user web+mobile (Google · Apple · @lhqglobal.vn) | Inbound (token) + redirect | API Gateway | **OIDC** (ADR-007) | Real-time | SH-006 |
| INT-002 | SMTP / mail Cty | Cảnh báo, phép, phiếu, T-15/T-7 | Outbound | Point-to-point | SMTP/TLS | Event | Notif |
| INT-003 | File Excel CC | Import công 1 mẫu master | Inbound | Upload / batch | HTTPS file | Theo kỳ / ad-hoc | TIM |
| INT-004 | Git | Khóa tài khoản N+3 | Outbound | Point-to-point | REST/API Git **TBD** | Job N+3 | LIF + IT |
| INT-005 | CRM **sản phẩm** | Khóa TK N+3 | Outbound | Point-to-point | API CRM **TBD** | Job N+3 | LIF + IT |
| INT-006 | CRM **bán hàng** | — | **Cấm** | — | — | — | — |

**Không** INT: máy CC hardware, ATS, sổ cái/nộp BH NN, chữ ký số CQNN.

## 3. Chi tiết tích hợp

### INT-001 — SSO IdP

| Mục | Nội dung |
|-----|----------|
| **Source** | **Lark** (Feishu) — directory `@lhqglobal.vn` (DEC-DLV-010). Login: Google · Apple · mail công ty qua cổng Lark |
| **Target** | API Gateway → IAM map `sub` → role HRM (SoT PostgreSQL) |
| **Trigger** | Login / refresh token |
| **Data scope** | `sub`, email (provision/linking). **Không** dùng role claim IdP làm SoT (ADR-002) |
| **Volume** | Mọi NV/LM/HR active — số user TBD |
| **SLA** | Phụ thuộc IdP; HRM 24/7 (ADR-003) nếu IdP sập → không login |
| **Auth** | **OIDC** (ADR-007); cùng IdP web = mobile |
| **Error** | 401 hết hạn; không fallback user/pass local trên ADR-001 |
| **Mapping** | Lark `sub` → `iam_identity_account.IdpSubject` → RBAC HRM; token **ADR-002 Accepted** |
| **HA** | IdP A/S hoặc IdP Cty; DR cùng ADR-003 |

```text
Client → LBS → GW → (401/redirect) IdP → code/token → GW validate → IAM roles → MS
```

### INT-002 — SMTP

| Mục | Nội dung |
|-----|----------|
| **Source** | Notification service (Active) |
| **Target** | Mail Cty `@minvoice.vn` |
| **Trigger** | Event phép, T-15/T-7, phiếu (kỳ chốt), N+3 nhắc IT |
| **Data scope** | To = chính chủ / HR theo FR; không BCC CRM sales |
| **Volume** | TBD |
| **SLA** | Best-effort + retry; kênh in-app HRM vẫn Must (PRB-FR-011) |
| **Auth** | SMTP credential IT/vault — không HR cầm |
| **Error** | DLQ + alert Ops; không block chốt phép nếu mail fail (trừ FR bắt buộc mail — theo từng AC) |
| **Mapping** | Template động quy chế — không hardcode list field |

### INT-003 — Excel chấm công

| Mục | Nội dung |
|-----|----------|
| **Source** | File xuất máy CC / HR upload — **1 mẫu** master tại một thời điểm |
| **Target** | TIM service |
| **Trigger** | Upload HR |
| **Data scope** | Cột = catalog quy chế (động); preview lỗi trước chốt |
| **Volume** | UAT 1000 dòng &lt;5s (NFR-001) đo sau LBS+GW |
| **SLA** | Sync request import |
| **Auth** | SSO + role HR (403 NV/LM) |
| **Error** | Preview danh sách lỗi; không ghi công khi fail AC |
| **Mapping** | DOC-11 TimesheetTemplate / ImportBatch **Chốt** khung |

### INT-004 — Git khóa TK

| Mục | Nội dung |
|-----|----------|
| **Source** | LIF job N+3 (chỉ Active) |
| **Target** | Git hosting Cty |
| **Trigger** | N = ngày LV cuối + 3 |
| **Data scope** | Định danh TK Git (map EMP) — **không** password Git trên HRM |
| **Volume** | Offboarding |
| **SLA** | Idempotent lock; alert IT nếu API fail |
| **Auth** | Service account IT (NFR-006) |
| **Error** | Retry + DLQ; không để HR paste token |
| **Mapping** | LIF-FR khóa Git |

### INT-005 — CRM sản phẩm khóa TK

| Mục | Nội dung |
|-----|----------|
| **Source** | LIF job N+3 (chỉ Active) |
| **Target** | CRM **sản phẩm** (không phải CRM bán hàng) |
| **Trigger** | Cùng N+3 |
| **Data scope** | Khóa/disable user sản phẩm |
| **Auth** | Service account IT |
| **Error** | Như INT-004 |
| **Mapping** | LIF-FR N+3 |

### INT-006 — Cấm CRM bán hàng

Không endpoint, không event bus, không email-to-CRM-sales. Test: có gọi → **fail AC** (NFR-007, PRB-AC-010).

## 4. Hợp đồng Message / Payload

Chi tiết field → **DOC-12** khi mở. DOC-10 chỉ khóa **hướng, hệ, cấm**.

| INT | Payload mức SAD |
|-----|-----------------|
| INT-001 | Token IdP + claims tối thiểu (sub, email) |
| INT-002 | to, template-id, ids nghiệp vụ (không full phiếu lương trên bus) |
| INT-003 | file + version mẫu master |
| INT-004/005 | employee-id, lock=true, N, idempotency-key |

## 5. Bảo mật

| Mục | Yêu cầu |
|-----|---------|
| Transport | TLS 1.2+ mọi INT |
| Secrets | Vault/IT; Git/CRM/SMTP **không** trên UI HR |
| PII | Lương không qua INT-001/004/005 |
| SSO | Không session local thay IdP (ADR-001) |
| MFA | Chưa bắt (ADR-006 / OQ-ARC-007) |

## 6. Giám sát & Hỗ trợ

| Metric | Threshold | Alert |
|--------|-----------|-------|
| INT-001 IdP down | GW 5xx/401 spike | Ops + IAM |
| INT-004/005 lock fail | 1 fail sau retry | IT |
| INT-006 probe | Mọi call CRM sales | **P1** vi phạm NFR-007 |
| Job trên Standby/DR | Count &gt; 0 | P1 ADR-003 |

Số ngưỡng % **TBD** (không bịa).

## 7. Truy vết

| FR / NFR / ADR | INT |
|----------------|-----|
| ADR-001 SSO, GW, LBS | INT-001 |
| NFR-003 web=mobile | INT-001 |
| NFR-006 Git secret | INT-004 |
| LIF N+3 | INT-004, INT-005 |
| TIM 1 mẫu | INT-003 |
| PRB-FR-011 / phép mail | INT-002 |
| NFR-007 | INT-006 |
| ADR-003 job Active | mọi outbound |

## 8. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-ARC-006) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
| Business Owner | Ban HR · IT | | ☐ Nợ IdP/Git/CRM API |
