# DOC-08 — Tài liệu Kiến trúc Giải pháp (SAD)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (SAD khung · DEC-ARC-005) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — §4.4 bỏ DC-DR theo **ADR-010** (DEC-ARC-017/018) |
| 0.3 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — bỏ "saga TIM→PAY" và broker theo **ADR-005** (DEC-ARC-026) |
| 0.4 | 2026-09-16 | soạn nháp SA (trợ lý) | **Chốt** (DEC-ARC-030 · PGD Dư Hùng) — viết lại theo **ADR-013** (modular monolith, không Gateway) và **ADR-012** (bỏ SSO). Kiến trúc đích = kiến trúc đang chạy |

**SEI** Views and Beyond · **Kruchten 4+1**.  
**Tiền đề:** DOC-03 / 7× DOC-06 / DOC-13 **Chốt** (chưa `02-baseline/`). EVT + RPT **chưa SRS**. Mobile **chưa có code**.  
**Cổng SAD đã chốt** (PGD · v0.1 DEC-ARC-005 · v0.4 DEC-ARC-030). DOC-10/11/12 **Chốt** — DOC-11/12 phải sửa theo ADR-012/013 (xem §7). Nợ: RTO phút; sản phẩm LBS; chính sách mật khẩu + MFA (DOC-13); Ban HR ☐; **văn bản khách chưa vào `assets/`** (RK-08). ADR còn hiệu lực: **013** · **012** · **010** · **009** · **005** · 001 *(chỉ §3, §5 .NET 9, §6)* · 011 *(W1/W2)*. **Không** tự DOC-17.

> **Mô tả một kiến trúc, không phải hai.** Từ v0.4, SAD không còn phân biệt "hiện tại" và "đích đến" (ADR-011 hệ quả tiêu cực) — hai cái trùng nhau. Mọi sơ đồ dưới đây soi từ `hrm/` ngày 2026-09-16.

---

## 1. Giới thiệu

### 1.1 Mục đích

Mô tả kiến trúc HRM nội bộ mInvoice đủ để chốt ADR và mở DOC-10/11/12/17. Không thay SRS.

### 1.2 Phạm vi

| In | Out |
|----|-----|
| 7 module đã SRS: EMP, LEV, TIM, PAY, PRB, LIF, IAM | ATS; máy CC hardware; sổ cái / nộp BH NN tự động |
| Biên: Excel CC, email @minvoice.vn, Git, CRM **sản phẩm** (khóa TK) | Notify sang CRM **bán hàng** (NFR-007 / PRB-FR-010) |
| Kênh NV: web (có code) + mobile (**chưa có code** — cùng API, cùng IAM; NFR-003, BRQ-006) | HTML pixel DOC-19; DOC-16 |
| EVT, RPT: chỗ đứng trên diagram, **không** chi tiết API | Chốt % uptime / số user (TBD ADR) |

### 1.3 Định nghĩa & Tài liệu tham chiếu

| Ref | Tài liệu |
|-----|----------|
| DOC-03 | BRD Chốt — scope, CN-001…006 |
| DOC-06 | SRS 7 module Chốt |
| DOC-13 | NFR-001…012 Chốt v0.2; NFR mật khẩu **trống** (ADR-012) |
| DOC-10 | **Chốt** v0.1 — INT-001 (IdP) **không còn** (ADR-012) |
| DOC-11 | **Chốt** v0.2 (DEC-ARC-021) — §1.2 *"FK xuyên service"*, §3.1 *"không lưu password hash"* phải sửa |
| DOC-12 | **Chốt** v0.3 — §1 *"API per service + gateway"*, §2 *"cấm password"* phải sửa |
| ADR-013 | Modular monolith có hàng rào là đích; không Gateway; giữ LBS |
| ADR-012 | Bỏ SSO; HRM tự quản username/password |
| ADR-011 | W1 (schema + role) · W2 (test kiến trúc) — **đã xong**, giữ; W3/W4+ bỏ |

### 1.4 Tổng quan kiến trúc

**Style (ADR-013 Accepted):** **modular monolith có hàng rào cưỡng chế**. Một process `Hrm.Host` (.NET 9, Jarvis), một instance PostgreSQL với **7 schema + 7 DB role** theo bounded context và schema `shared` (audit). Ranh giới module chặn bằng **GRANT** ở DB và **test kiến trúc** trong CI — không bằng quy ước thư mục. **Không API Gateway.** **LBS** trước host chỉ để bơm traffic vào node Active (ADR-010). **Đăng nhập username/password do HRM tự quản** (ADR-012), JWT do HRM ký. Adapter **ra** (email, Git/CRM lock) từ LIF/Job — **cấm** CRM sales.

**Vì sao không microservices:** chủ đầu tư trả lời bằng văn bản **không cần** (DEC-ARC-029); bằng chứng kỹ thuật đã nghiêng về monolith từ deliberation B1 — 7 module Must chạy trên monolith, đội ops chưa có, và hàng rào NFR-002 đạt được bằng schema + role mà không cần tách service (OQ-ARC-011 xác minh).

Xây **2026** / dùng **2027** (NFR-011). CAPEX ~1 tỷ (CN-004).

## 2. Mục tiêu & Ràng buộc kiến trúc

| ID | Goal / Constraint | NFR trace |
|----|-------------------|-----------|
| AG-001 | UAT 1000 dòng hoàn tất &lt; 5s (lương hoặc import công — kịch bản UAT), đo **sau LBS** | NFR-001 |
| AG-002 | Cô lập PII + phiếu lương; LM không xem lương cấp dưới — hàng rào **dưới tầng ứng dụng** (role DB) | NFR-002 · ADR-011 W1 |
| AG-003 | Cùng rule IAM web = mobile | NFR-003 |
| AG-004 | 403 màn HR (TIM/PAY/EMP/LIF) | NFR-004 |
| AG-005 | Audit bất biến nghiệp vụ (chốt công/lương, C1/C2, LM, N, khóa Git/CRM, chốt PRB) + **đăng nhập/khoá/đổi mật khẩu** | NFR-005 · ADR-012 |
| AG-006 | HR không cầm credential Git | NFR-006 |
| AG-007 | Không event phép/LIF/PRB sang CRM bán hàng | NFR-007 |
| AG-008 | Self-service NV web + mobile MVP | NFR-008 |
| AG-009 | Master động theo quy chế (mẫu CC, lịch, PC, BH/TNCN) — không hardcode | NFR-010, CN-006 |
| AG-010 | Vận hành **24/7**; HA **Active/Standby trong một DC**; **không DR/DC** — RTO-failover / RTO-restore TBD | NFR-012a–d · **ADR-010** |
| AG-011 | Go-live 2027; p95 API NV **TBD**; số user **TBD** | NFR-011, NFR-P02, NFR-SC01 |
| AG-012 | Mọi truy cập người dùng qua **đăng nhập HRM** (username/password) → **Bearer JWT HRM ký**; web = mobile. **Không** IdP ngoài | NFR-003 · **ADR-012** |
| AG-013 | **LBS** trước host, chỉ bơm vào **Active** (Standby không nhận user) | ADR-001 §3 · **ADR-010** · DOC-17 |
| AG-014 | Job T-15/T-7/N+3 **chỉ** trên node Active; bộ lập lịch **ngoài** hệ thống | ADR-005 · ADR-010 §5 |
| AG-015 | Ranh giới bounded context cưỡng chế bằng **cơ chế** (schema + role + GRANT + test kiến trúc), không bằng quy ước | **ADR-013** · ADR-011 W1/W2 |
| AG-016 | Một deploy unit; **không** Gateway, **không** database-per-service, **không** broker | **ADR-013** · ADR-005 |

## 3. Stakeholder & Mối quan tâm

| Stakeholder | Concern | View addressing |
|-------------|---------|-----------------|
| PGD (A) | Scope, 2027, cô lập lương | Logic + Scenario |
| Chủ đầu tư | Không cần microservices / DR-DC / SSO — chi phí | §1.4 · ADR-010/012/013 |
| Ban HR / C&B | SoT chốt phép/công/lương/TV | Logic + Process |
| NV / LM | Self-service 2 kênh | Logic + Process |
| Dev / Tester | Module boundary **đo được**, test NFR | Development |
| IT / IAM | Git/CRM N+3, không credential HR; **reset mật khẩu** (OQ-DLV-009) | Process + Physical |
| Ops | 24/7, A/S một DC, backup/restore, **một** process để vận hành | Physical · **ADR-010** · ADR-013 |

## 4. Các góc nhìn kiến trúc

### 4.0 Các lớp (client → LBS → host → CSDL)

```text
[Client]     Web HR/NV/LM (React + Vite)      Mobile NV (chưa có code)
                  │                                 │
                  └───────────────┬─────────────────┘
                                  │ HTTPS · Authorization: Bearer <JWT HRM ký>
[LBS]            Cân bằng tải / TLS (sản phẩm TBD DOC-17) — chỉ bơm vào Active
                                  │
[Host]           Hrm.Host (.NET 9 · Jarvis) — MỘT process
                 pipeline: HttpsRedirection → CORS → OpenTelemetry (trace-id)
                           → AuthN JWT (fail-closed, RequireIdpSubject)
                           → AuthZ → ApiResponseWrapper → BusinessRefusalLogging
                           → Controllers /v1/{iam|emp|lev|tim|pay|prb|lif}/…
                           → /health (liveness · readiness 7 DbContext)
                 KHÔNG Gateway · KHÔNG rule nghiệp vụ ngoài Application/Domain
                                  │  7 connection string, 7 role
              ┌──────┬──────┬─────┼─────┬──────┬──────┐
              ▼      ▼      ▼     ▼     ▼      ▼      ▼
[CSDL]      iam    emp    lev   tim   pay    prb    lif      + shared (emp_audit_log)
            hrm_app_iam … hrm_app_lif — mỗi role CHỈ thấy schema của mình
            (ngoại lệ có văn bản: hrm_app_lev SELECT 4 cột emp.emp_employee)
            hrm_migrator: DDL toàn bộ, chỉ dùng cho migration (AppDbContext)
            MỘT instance PostgreSQL 16 (ADR-009)

[Job]      bộ lập lịch NGOÀI hệ thống → POST /v1/…/jobs/… (chỉ Active — ADR-010 §5)
[Adapter]  từ LIF / Job / Notif → SMTP, Excel, Git, CRM sản phẩm · KHÔNG → CRM bán hàng
```

| Lớp | Trách nhiệm | Không làm |
|-----|-------------|-----------|
| CSDL | SoT dữ liệu từng context trong **schema riêng**; quyền theo **role riêng**; PII/lương chỉ role `pay`/`emp` đọc được | Một role chung đọc mọi schema; join chéo schema ngoài ngoại lệ đã GRANT |
| Host | Use-case + domain DOC-06; xác thực JWT; 401/403; audit; readiness theo từng context | Tự đứng làm TLS/LB hộ toàn hệ; chứa credential Git |
| LBS | TLS, phân tải, health check, chỉ Active | Ủy quyền nghiệp vụ; route theo path (không cần — một host) |
| Bộ lập lịch | Gọi endpoint job đúng lịch | Chứa logic nghiệp vụ |

### 4.1 Góc nhìn logic (Component)

| Component | Trách nhiệm | Technology |
|-----------|-------------|------------|
| LBS | TLS, cân bằng tải, health → chỉ Active | TBD DOC-17 (OQ-DLV-002) |
| `Hrm.Host` | Composition root; pipeline §4.0; 13 controller `/v1/{ctx}/…`; `/dev/*` **chỉ Development** | .NET 9 · Jarvis Mvc/Auth/Health/OTEL |
| IAM | Tài khoản, role, 403; **đăng nhập username/password, hash, khoá, reset** (ADR-012 — **chưa code**, CR-002) | `IamDbContext` · role `hrm_app_iam` |
| EMP · LEV · TIM · PAY · PRB · LIF | SoT module theo DOC-06 | mỗi context một `DbContext` + role riêng |
| Audit | `shared.emp_audit_log` — mọi context INSERT, không UPDATE/DELETE | DEC-ARC-024 (phương án A) |
| Notification | In-app + email — **trong process** (bảng `LeaveNotification`, outbox PAY export) | Không service riêng |
| Job | T-15, T-7, N+3 — endpoint idempotent theo ngày | Bộ lập lịch ngoài (OQ-ARC-017) · **không** broker |
| Adapters | Excel / mail / Git / CRM lock | Trên LIF/Job — DOC-10 |
| Web client | SPA React + Vite; gọi API qua LBS | Hosting **TBD** DOC-17 |

### 4.2 Góc nhìn tiến trình (Runtime)

| Luồng | Đường đi | Ghi chú |
|-------|----------------|---------|
| Login | Client → LBS → `POST /v1/iam/auth/login` → IAM đối chiếu hash → **JWT HRM ký** | ADR-012 — **chưa code** (CR-002, chờ DOC-06/07 IAM + DOC-13). DEV: `POST /dev/login` plaintext config, **404 ngoài Development** (RK-09) |
| Mọi request sau đó | `Authorization: Bearer` → AuthN fail-closed (`ValidateIssuerSigningKey/Issuer/Audience`) → `RequireIdpSubject()` → AuthZ role | Token không có `sub` = 401 tại biên |
| LEV C1→C2 trừ quỹ | Client → LBS → host LEV; notify in-process | LEV đọc `emp` **4 cột** qua GRANT (OQ-ARC-015) |
| TIM import → chốt | LBS → host TIM; NFR-001 đo sau LBS | 1 mẫu master |
| PAY sau công chốt | PAY **đọc guard** trạng thái kỳ TIM **trong process**; TIM đọc ngược PAY | **Không phải saga** — ADR-005; fail-closed. Hai endpoint `GET /v1/{tim,pay}/periods/{ym}` giữ cho client, **không còn là hợp đồng liên service** (DOC-12 §4.3 sửa) |
| PRB T-15/T-7 | Bộ lập lịch → `POST /v1/prb/jobs/reminders/run` → PRB/EMP | Không LM → HR · RK-07 idempotency chưa kiểm |
| PRB HR chốt | LBS → host PRB → EMP hoặc LIF | 403 LM/NV chốt |
| LIF N+3 | Job → LIF adapter Git/CRM | Secret IT |
| Từ chối nghiệp vụ | `BusinessException` → `BusinessRefusalLoggingMiddleware` ghi log warning (actor, code, lý do) → wrapper trả lỗi | NFR-005 kênh log |
| Event CRM sales | **Cấm** — không có adapter, test AC | Fail AC |

Hàng đợi giữa context: **không dùng broker ở MVP** (ADR-005). Cross-context = gọi trong process qua interface Application, hoặc đọc chéo có GRANT rõ ràng. **Không có hop mạng** giữa các context → RK-05/OQ-ARC-018 (timeout/retry) không phát sinh.

### 4.3 Góc nhìn phát triển (Module / Package)

Cấu trúc thật `hrm/` (không phải nháp):

| Path | Bound |
|------|-------|
| `backend/src/Hrm.Domain.Shared` · `Hrm.Domain` · `Hrm.Application` · `Hrm.Infrastructure` · `Hrm.Host` | 5 layer Jarvis (ProjectReference `../jarvis/`, không NuGet) |
| namespace / thư mục `Identity`, `Employees`, `Leave`, `Timekeeping`, `Payroll`, `Probation`, `Lifecycle` trong từng layer | 7 bounded context |
| `Hrm.Infrastructure/Persistence/{Iam,Emp,Lev,Tim,Pay,Prb,Lif}DbContext.cs` | 7 context — mỗi cái một connection string, một role |
| `Hrm.Infrastructure/Persistence/AppDbContext.cs` | **chỉ** migration owner (`hrm_migrator`); không repository nào dùng |
| `backend/scripts/w1-roles.sql` | **SoT** của schema, role, GRANT — sửa GRANT phải qua review (RK-12) |
| `unittest/Hrm.Architecture.Tests` | Test kiến trúc: chặn reference chéo context; allowlist closure `emp` cho LEV |
| `unittest/Hrm.Domain.Tests` · `Hrm.Application.Tests` | 163 test (2026-09-16) |
| `frontend/` | React + Vite; `pages/` theo module |
| `autotest/` | Playwright API + UI |
| `docs/03-modules/{id}/` | Artifact req |
| `events` / `hr-analytics` | Chưa SRS — **không code trước FR** |

Quy tắc ranh giới (AG-015): context A **không** reference entity/repository của context B; đọc chéo phải (1) qua interface trong Application **và** (2) có GRANT tương ứng trong `w1-roles.sql`. Thiếu một trong hai → test kiến trúc hoặc PostgreSQL `permission denied` — **ồn, không im lặng**.

### 4.4 Góc nhìn vật lý / Triển khai

**Hosting + HA (ADR-001 §6 · ADR-010 · ADR-013):** private mInvoice; **24/7**; **Active/Standby trong một DC**. **Không** DC-DR — khách yêu cầu bỏ (DEC-ARC-018); thảm họa mất DC xử lý bằng **backup/restore**. LBS TBD. DB = **PostgreSQL 16** (ADR-009; host prod TBD). **Không** Active/Active. **Một** deploy unit (`Hrm.Host`) + SPA tĩnh.

```text
                    ┌─ DC-Prod (chỉ MỘT DC) ────────────────────────────────┐
 Client ──► [LBS-A] ─► [Hrm.Host-A] ─► [PostgreSQL-A primary: 8 schema]  (jobs ON)
                    │       Standby cùng DC: LBS-S · Hrm.Host-S · PostgreSQL-S replica
                    │       Standby nóng, không nhận user đến khi failover
                    └──────────────────────────────────────────────────────┘
                                      │ backup định kỳ (chu kỳ TBD)
                                      ▼
                              [ Backup store ] — hiện đặt trên máy Standby CÙNG DC
                                ⚠️ mất DC = mất backup (RK-01 · OQ-ARC-012 · NFR-012c)
 Outbound chỉ từ Active: SMTP, Git, CRM sản phẩm
```

| Environment | Nodes | Scaling |
|-------------|-------|---------|
| Prod | Cặp A/S + LBS trên Active, **một DC** | Ngang = **nhân bản `Hrm.Host`** sau LBS (JWT stateless, không session affinity); **không** scale từng module |
| Dev / UAT | Node đơn, không bắt buộc A/S | — |

Cấu hình Prod bắt buộc (DOC-17): **8** connection string (7 context + migrator) — thiếu một cái thì host **không được** rơi về chuỗi chung (bẫy `AddContextConnection` fallback); `IssuerSigningKeys` là secret thật; `ASPNETCORE_ENVIRONMENT != Development` (đóng `/dev/*`).

### 4.5 Kịch bản (+1)

| Scenario | Views | Validates |
|----------|-------|-----------|
| Login username/password web = mobile → JWT | LBS + Host IAM | NFR-003, AG-012 · **chưa code** |
| Token giả / không `sub` → 401 tại biên | Host pipeline | AG-012 · đã kiểm (forged token 401) |
| LEV C2 trừ quỹ | Logic + Process | NFR-005 |
| TIM/PAY 1000 dòng &lt;5s sau LBS | Process + Physical | NFR-001 |
| NV xem phiếu; LM 403 lương | Logic | NFR-002, 004 |
| **`psql` bằng role `hrm_app_lev` không đọc được `pay.*`** | Physical (DB) | NFR-002, AG-002, AG-015 · đã kiểm 8/8 |
| PRB T-15/T-7 + HR chốt | Process + Job | NFR-009 (PRB) |
| LIF N+3 khóa Git/CRM | Process + Adapter | NFR-006 |
| Failover A/S trong DC · restore từ backup khi mất DC | Physical + Process | NFR-012a–d, AG-010, 014 |
| Một `DbContext` mất DB → readiness 503, LBS rút node | Physical | AG-010 · đã kiểm |
| Không notify CRM sales | Process | NFR-007 |

## 5. Mối quan tâm xuyên suốt

| Concern | Approach | ADR ref |
|---------|----------|---------|
| Xác thực | **Username/password HRM tự quản** → JWT HRM ký; fail-closed; không IdP ngoài; `/dev/*` chỉ Development | **ADR-012** |
| Chính sách mật khẩu / khoá / reset | **TBD DOC-13** (OQ-DLV-010) · quy trình reset HR/IT (OQ-DLV-009) — **chặn code login** | ADR-012 §6 · CR-002 |
| MFA | Chưa bắt — *"MFA sau password?"* TOTP tự làm hay không có | OQ-ARC-007 *(phát biểu lại)* |
| RBAC | IAM DB = SoT role; 403 màn HR; cô lập lương | ADR-013 §7(a) |
| Ranh giới dữ liệu | Schema + role + GRANT theo cột; `AppDbContext` chỉ migrate | **ADR-011 W1** · ADR-013 |
| Ranh giới code | Test kiến trúc, allowlist rõ ràng | **ADR-011 W2** |
| Audit | `shared.emp_audit_log` bất biến; log từ chối nghiệp vụ | NFR-005 · DEC-ARC-024 |
| Observability | Jarvis OpenTelemetry (trace-id) → collector **TBD** DOC-17 | — |
| Mã hóa at-rest / TLS | TLS tại LBS; at-rest **không** đóng AES trên NFR | ADR-004 Proposed |
| Master động | Catalog quy chế, không hardcode luật | CN-006 |
| HA | 24/7; A/S **một DC**; backup/restore khi mất DC; job chỉ Active | **ADR-010** |

## 6. Tóm tắt quyết định kiến trúc

→ Chi tiết [register](DOC-09-adr/README.md).

| ADR ID | Decision | Status |
|--------|----------|--------|
| **ADR-013** | **Không cần microservices** — modular monolith có hàng rào là đích; không Gateway; giữ LBS *(chủ đầu tư)* | **Accepted** — [file](DOC-09-adr/ADR-013-modular-monolith-kien-truc-dich.md) |
| **ADR-012** | **Bỏ SSO** — HRM tự quản username/password *(khách yêu cầu)* | **Accepted** — [file](DOC-09-adr/ADR-012-bo-sso-password-tu-quan.md) |
| ADR-011 | W1 schema+role · W2 test kiến trúc — **đã xong, giữ**; W3/W4+ superseded | **Accepted** *(một phần)* — [file](DOC-09-adr/ADR-011-lo-trinh-tach-service.md) |
| ADR-010 | 24/7 + Active/Standby **một DC**; bỏ DR/DC *(khách yêu cầu)* | **Accepted** — [file](DOC-09-adr/ADR-010-ha-single-dc-active-standby.md) |
| ADR-009 | PostgreSQL là SoT | **Accepted** — [file](DOC-09-adr/ADR-009-postgresql.md) |
| ADR-005 | Job qua bộ lập lịch ngoài; TIM↔PAY là guard đọc **trong process**, **không** saga, **không** broker | **Accepted** — [file](DOC-09-adr/ADR-005-broker-va-coupling-tim-pay.md) |
| ADR-001 | ~~Microservices + Gateway~~ + LBS + ~~SSO~~ + .NET 9 + private | **Accepted** — chỉ còn §3, §5 .NET 9, §6 — [file](DOC-09-adr/ADR-001-stack-style-hosting.md) |
| ADR-003 | 24/7 + Active/Standby + ~~DR/DC~~ | **Accepted** — §3–5 superseded bởi ADR-010 |
| ADR-002 | JWT OIDC tại GW | **Superseded** bởi ADR-013 |
| ADR-007 | Lark IdP OIDC | **Superseded** bởi ADR-012 |
| ADR-004 | Mã hóa at-rest | Proposed |
| ADR-006 | MFA | Proposed — phát biểu lại sau ADR-012 |

## 7. Rủi ro & Nợ kỹ thuật

| ID | Rủi ro | Mitigation |
|----|--------|------------|
| R-001 | EVT/RPT chưa SRS → SAD thiếu luồng cảnh báo/báo cáo | Giữ chỗ; không API bịa |
| R-002 | RTO-failover / RTO-restore **phút** chưa chốt | OQ-DLV-004; pattern A/S một DC đã có ADR-010 |
| R-003 | Sản phẩm LBS, Git/CRM API vendor chưa IT cung cấp | DOC-10 **Chốt** kèm nợ; LBS → DOC-17 (OQ-DLV-002) |
| R-004 | Chưa `02-baseline/` req | SAD v0.4 Chốt; DOC-11/12 lệch ADR-012/013 — **sửa trước baseline** (RK-13) |
| R-005 | Ban HR chưa ký | Nợ cổng nghiệp vụ |
| R-006 | HTML MCP / DOC-16 trống | Không chặn SAD |
| R-007 | ~~Saga TIM→PAY~~ — **đóng hẳn**: guard trong process, không hop mạng (ADR-005 · ADR-013) | — |
| **R-008** | **Backup đặt cùng DC** → mất DC = mất dữ liệu; NFR-012c không đạt | **RK-01 · OQ-ARC-012** — chờ PGD (a) chấp nhận + xác nhận khách, (b) bản sao lạnh ngoài DC |
| **R-009** | **Văn bản khách chưa vào `assets/`** — ADR-010/012/013 đứng trên lời PGD; thư đang qua bưu điện | **RK-08** — khi nhận: đối chiếu *"không cần"* cả ba mục + RK-01 |
| **R-010** | Blast radius một process: module rò bộ nhớ kéo cả host | **RK-11** — A/S failover + readiness theo context; không thêm gì ở MVP |
| **R-011** | GRANT "tiện tay" hoặc chạy app bằng `hrm_migrator` → hàng rào NFR-002 biến mất **im lặng** | **RK-12** — `w1-roles.sql` là SoT; review PR soi GRANT; test kiến trúc vào CI (**CI chưa có**) |
| **R-012** | Login Prod **chưa code**; chính sách mật khẩu, reset, MFA trống | CR-002 gated: IAM DOC-06/07 + DOC-13 trước (OQ-DLV-009/010, OQ-ARC-007) |
| **R-013** | Mobile chưa có code; SAD chỉ giữ chỗ | Cùng API/JWT — không thiết kế riêng cho tới khi có FR |
| **R-014** | Prod chưa chạy được: 8 connection string, secret, CORS, OTEL collector, hosting SPA, bộ lập lịch — toàn TBD | DOC-17 v0.4 slice riêng; guard khởi động chặn placeholder |

## 8. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-09-16 | **Chốt** v0.1 (DEC-ARC-005) · **Chốt v0.4** (DEC-ARC-030) · ☐ `02-baseline/` |
| Solution Architect | | 2026-09-16 | Soạn v0.4 theo ADR-012/013 → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn v0.1 |
| Business Owner | Ban HR | | ☐ Nợ |
