# DOC-10 — Đặc tả Tích hợp

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (INT · DEC-ARC-006) |
| 0.2 | 2026-09-18 | soạn nháp SA (trợ lý) | **Chốt** (DEC-ARC-035 · PGD) — **INT-001 (Lark IdP) bỏ** theo ADR-012; bỏ API Gateway, DR, "service" theo ADR-010/013; **adapter ra ghi rõ chưa code** (doc-review pass 3 B2/M13) |
| 0.2.1 | 2026-09-18 | soạn nháp SA/QC (trợ lý) | **Chốt** *(sửa lỗi)* — pass 4: mã 422 → **400**; định tính trạng thái test; cột `GitLockedAtUtc` thuộc case |

**Hohpe EIP** · Adjunct DOC-08 **v0.4** · **ADR-013** (một host, không GW) · **ADR-012** (không IdP) · **ADR-010** (một DC) · ADR-005.  
**Cổng:** PGD chốt v0.1 (DEC-ARC-006) · v0.2 (DEC-ARC-035). Nợ: Git/CRM API vendor; SMTP host; RTO phút; Ban HR ☐; **toàn bộ adapter ra chưa code** (R-015). **Chưa** `02-baseline/`. **Không** tự DOC-17. **Không còn INT xác thực** — đăng nhập là nội bộ HRM (ADR-012), không phải tích hợp.

**Cấm:** INT sang CRM **bán hàng** (NFR-007).

---

## 1. Tổng quan

### 1.1 Bản đồ tích hợp

```text
 [NV/HR Web · Mobile] → [LBS] → [Hrm.Host — một process: IAM|EMP|LEV|TIM|PAY|PRB|LIF|Job|Notif]
                                  (Bearer JWT HRM ký — không IdP ngoài, không Gateway)

 Adapter ra (chỉ từ node Active) — ⚠️ CHƯA CODE, outbox mới ghi:
   TIM  ← file Excel CC (1 mẫu master)
   Notif → SMTP @minvoice.vn
   LIF  → Git (khóa TK)     secret IT, không HR
   LIF  → CRM sản phẩm (khóa TK)
   ✗    → CRM bán hàng (không INT)
```

Không có DR site (ADR-010). Backup/restore là việc nội bộ DB, không phải INT.

### 1.2 Nguyên tắc tích hợp

| Principle | Mô tả |
|-----------|-------|
| Cổng duy nhất | Client/hệ ngoài đi qua **LBS → `Hrm.Host`**; không có đường vào DB hay module riêng lẻ. Chiều ra chỉ từ adapter LIF/Notif/Job |
| Xác thực bắt buộc | Mọi request mang **Bearer JWT HRM ký**; host từ chối token thiếu/giả/không `sub` tại biên (ADR-012 §4, `RequireIdpSubject`). **Không** phải INT |
| Idempotency | Khóa Git/CRM N+3: gọi lại cùng NV+N không tạo khóa kép lỗi; import Excel theo file-id |
| Retry | Outbound Git/CRM/SMTP: retry hữu hạn + DLQ/alert; **không** retry tạo event CRM sales |
| Job một nơi | Adapter/job **chỉ** node Active (ADR-010 §5, `Hrm:HostRole`) |
| Không PII lương qua INT | PAY không đẩy phiếu sang hệ ngoài trừ email **chính chủ** (INT-002) |
| Split-brain | Cấm hai node cùng Active khi gọi Git/CRM — LBS chỉ bơm một node; `HostRole=Standby` từ chối job |

## 2. Danh mục tích hợp

| INT ID | Hệ thống ngoài | Mục đích | Direction | Pattern | Protocol | Frequency | Owner |
|--------|----------------|----------|-----------|---------|----------|-----------|-------|
| ~~INT-001~~ | ~~Lark SSO IdP~~ | **Bỏ** — ADR-012 (2026-09-15): HRM tự quản username/password, không IdP ngoài. Giữ số để trace lịch sử | — | — | — | — | — |
| INT-002 | SMTP / mail Cty | Cảnh báo, phép, phiếu, T-15/T-7 | Outbound | Outbox → adapter *(adapter **chưa code**)* | SMTP/TLS | Event | Notif |
| INT-003 | File Excel CC | Import công 1 mẫu master | Inbound | Upload / batch | HTTPS file | Theo kỳ / ad-hoc | TIM |
| INT-004 | Git | Khóa tài khoản N+3 | Outbound | `LifAccessLockOutbox` → adapter *(adapter **chưa code**)* | REST/API Git **TBD** | Job N+3 | LIF + IT |
| INT-005 | CRM **sản phẩm** | Khóa TK N+3 | Outbound | `LifAccessLockOutbox` → adapter *(adapter **chưa code**)* | API CRM **TBD** | Job N+3 | LIF + IT |
| INT-006 | CRM **bán hàng** | — | **Cấm** | — | — | — | — |

**Không** INT: máy CC hardware, ATS, sổ cái/nộp BH NN, chữ ký số CQNN.

## 3. Chi tiết tích hợp

### ~~INT-001 — SSO IdP~~ — **bỏ (ADR-012)**

Toàn bộ mục v0.1 (Lark, OIDC, JWKS, redirect qua Gateway) **không còn hiệu lực**. Xác thực nay là nội bộ: `POST /v1/iam/auth/login` → JWT HRM ký (DOC-12 §2, **chưa code** — CR-002). Không có hệ ngoài nào tham gia → không phải INT. Cột `iam_identity_account.IdpSubject` giữ tên, ngữ nghĩa = `sub` của JWT HRM (DOC-11 §3.1). Luồng JIT-provision *"first login: map IdP sub → EMP qua email"* (IAM-FR-017) **phải bỏ** — tài khoản do HR/IT tạo (DOC-08 R-012).

### INT-002 — SMTP

| Mục | Nội dung |
|-----|----------|
| **Source** | Notification trong `Hrm.Host` (node Active) — bảng `LeaveNotification` / outbox; **chưa có adapter SMTP** |
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
| **Target** | TIM (`/v1/tim/imports`) |
| **Trigger** | Upload HR |
| **Data scope** | Cột = catalog quy chế (động); preview lỗi trước chốt |
| **Volume** | UAT 1000 dòng &lt;5s (NFR-001) đo sau LBS |
| **SLA** | Sync request import |
| **Auth** | JWT HRM + role HR (403 NV/LM) |
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
| INT-002 | to, template-id, ids nghiệp vụ (không full phiếu lương trên bus) |
| INT-003 | file + version mẫu master |
| INT-004/005 | `EmployeeId`, `EmployeeCode`, `AsOfDate` (N), `Channel`, `TargetSystems` — theo `LifAccessLockOutbox` (DOC-11 §3.7); idempotency = bỏ qua case đã `LifOffboardingCase.GitLockedAtUtc && CrmSpLockedAtUtc` (hai cột trên **case**, không phải outbox), **không** có cột idempotency-key |

## 5. Bảo mật

| Mục | Yêu cầu |
|-----|---------|
| Transport | TLS 1.2+ mọi INT |
| Secrets | Vault/IT; Git/CRM/SMTP **không** trên UI HR |
| PII | Lương không qua INT-004/005; chỉ email chính chủ (INT-002) |
| Xác thực | JWT HRM ký; mật khẩu theo DOC-13 NFR-S07…S11 (ADR-012) — nội bộ, không INT |
| MFA | Chưa bắt (OQ-ARC-007, phát biểu lại sau ADR-012) |
| Bộ lập lịch gọi job | **Chưa có cơ chế** xác thực cho máy — OQ-ARC-019 |

## 6. Giám sát & Hỗ trợ

| Metric | Threshold | Alert |
|--------|-----------|-------|
| Login fail spike | 401/429 `POST /v1/iam/auth/login` bất thường (credential stuffing) — *endpoint và 429 chưa code (CR-002)* | Ops + IAM |
| INT-004/005 lock fail | 1 fail sau retry | IT |
| INT-006 probe | Mọi call CRM sales | **P1** vi phạm NFR-007 |
| Job trên Standby | **400** `Host Standby` (`BadRequestException`) hoặc job chạy từ node Standby | P1 ADR-010 |

Số ngưỡng % **TBD** (không bịa).

## 7. Truy vết

| FR / NFR / ADR | INT |
|----------------|-----|
| ADR-012 đăng nhập nội bộ | *(không INT)* — DOC-12 §2 |
| ADR-013 một host, LBS | mọi INT đi qua `Hrm.Host` |
| NFR-003 web=mobile | cùng JWT — không INT |
| NFR-006 Git secret | INT-004 |
| LIF N+3 | INT-004, INT-005 |
| TIM 1 mẫu | INT-003 |
| PRB-FR-011 / phép mail | INT-002 |
| NFR-007 | INT-006 |
| ADR-010 §5 job Active | mọi outbound |
| R-015 adapter chưa code | INT-002/004/005 |

## 8. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-ARC-006) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-09-18 | **Chốt v0.2** (DEC-ARC-035) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
| Business Owner | Ban HR · IT | | ☐ Nợ Git/CRM API, SMTP host |
