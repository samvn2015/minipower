# DOC-08 — Tài liệu Kiến trúc Giải pháp (SAD)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (SAD khung · DEC-ARC-005) |

**SEI** Views and Beyond · **Kruchten 4+1**.  
**Tiền đề:** DOC-03 / 7× DOC-06 / DOC-13 **Chốt** (chưa `02-baseline/`). EVT + RPT **chưa SRS**.  
**Cổng SAD đã chốt** (PGD · DEC-ARC-005). DOC-10/11/12 **Chốt** khung. Nợ: RTO/RPO phút; **Lark issuer URL** (IT); LBS; MFA; Ban HR ☐. ADR-001 · 002 · 003 · **007** **Accepted** (v0.2 Lark). **Không** tự DOC-17.

---

## 1. Giới thiệu

### 1.1 Mục đích

Mô tả kiến trúc mục tiêu HRM nội bộ mInvoice đủ để chốt ADR và mở DOC-10/11/12. Không thay SRS.

### 1.2 Phạm vi

| In | Out |
|----|-----|
| 7 module đã SRS: EMP, LEV, TIM, PAY, PRB, LIF, IAM | ATS; máy CC hardware; sổ cái / nộp BH NN tự động |
| Biên: Excel CC, email @minvoice.vn, Git, CRM **sản phẩm** (khóa TK) | Notify sang CRM **bán hàng** (NFR-007 / PRB-FR-010) |
| Kênh NV: web + mobile cùng IAM (NFR-003, BRQ-006) | HTML pixel DOC-19; DOC-16 |
| EVT, RPT: chỗ đứng trên diagram, **không** chi tiết API | Chốt % uptime / số user (TBD ADR) |

### 1.3 Định nghĩa & Tài liệu tham chiếu

| Ref | Tài liệu |
|-----|----------|
| DOC-03 | BRD Chốt — scope, CN-001…006 |
| DOC-06 | SRS 7 module Chốt |
| DOC-13 | NFR-001…012 Chốt; NFR-012 TBD SAD |
| DOC-10 | **Chốt** v0.1 — DEC-ARC-006 |
| DOC-11 | **Chốt** khung v0.1 — DEC-ARC-008 |
| DOC-12 | **Chốt** khung v0.1 — DEC-ARC-010 |

### 1.4 Tổng quan kiến trúc

**Style (ADR-001 Accepted gói F):** microservices + **API Gateway** + **LBS** + **SSO**. Client không gọi thẳng DB/service domain. Adapter **ra** (email, Git/CRM lock) từ LIF/Notification — **cấm** CRM sales.

Xây **2026** / dùng **2027** (NFR-011). CAPEX ~1 tỷ (CN-004).

## 2. Mục tiêu & Ràng buộc kiến trúc

| ID | Goal / Constraint | NFR trace |
|----|-------------------|-----------|
| AG-001 | UAT 1000 dòng hoàn tất &lt; 5s (lương hoặc import công — kịch bản UAT) | NFR-001 |
| AG-002 | Cô lập PII + phiếu lương; LM không xem lương cấp dưới | NFR-002 |
| AG-003 | Cùng rule IAM web = mobile | NFR-003 |
| AG-004 | 403 màn HR (TIM/PAY/EMP/LIF) | NFR-004 |
| AG-005 | Audit bất biến nghiệp vụ (chốt công/lương, C1/C2, LM, N, khóa Git/CRM, chốt PRB) | NFR-005 |
| AG-006 | HR không cầm credential Git | NFR-006 |
| AG-007 | Không event phép/LIF/PRB sang CRM bán hàng | NFR-007 |
| AG-008 | Self-service NV web + mobile MVP | NFR-008 |
| AG-009 | Master động theo quy chế (mẫu CC, lịch, PC, BH/TNCN) — không hardcode | NFR-010, CN-006 |
| AG-010 | Vận hành **24/7**; HA **Active/Standby**; **DR/DC** — số RTO/RPO TBD | NFR-012 · ADR-003 |
| AG-011 | Go-live 2027; p95 API NV **TBD**; số user **TBD** | NFR-011, NFR-P02, NFR-SC01 |
| AG-012 | Mọi truy cập người dùng qua **SSO** (web = mobile) | NFR-003 · ADR-001 |
| AG-013 | Cân bằng tải **LBS** trên nhánh **Active** (Standby không nhận user) | ADR-001 · ADR-003 · DOC-17 |
| AG-014 | Job T-15/T-7/N+3 **chỉ** trên site/node Active | ADR-003 |

## 3. Stakeholder & Mối quan tâm

| Stakeholder | Concern | View addressing |
|-------------|---------|-----------------|
| PGD (A) | Scope, 2027, cô lập lương | Logic + Scenario |
| Ban HR / C&B | SoT chốt phép/công/lương/TV | Logic + Process |
| NV / LM | Self-service 2 kênh | Logic + Process |
| Dev / Tester | Module boundary, test NFR | Development |
| IT / IAM | Git/CRM N+3, không credential HR | Process + Physical |
| Ops | 24/7, A/S, DR/DC | Physical · ADR-003 |

## 4. Các góc nhìn kiến trúc

### 4.0 Các lớp (CSDL → microservice → middleware → LBS → client)

```text
[Client]     Web HR/NV/LM     Mobile NV
                  │                │
                  └────────┬───────┘
                           │ HTTPS
[LBS]            Cân bằng tải (sản phẩm TBD DOC-17)
                           │
[Middleware]     API Gateway
                 · TLS · định tuyến · SSO bắt buộc · correlation-id
                 · KHÔNG rule quỹ / N_tính / chốt TV
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
[Microservices]  IAM   EMP  LEV  TIM  PAY  PRB  LIF   (+ Notif, Job)
     .NET 9      │      │    │    │    │    │    │
                 ▼      ▼    ▼    ▼    ▼    ▼    ▼
[CSDL]        DB-IAM DB-EMP …  (database-per-service; PostgreSQL · ADR-009)

[SSO IdP]  ◄── Gateway + client (OIDC/SAML TBD)
           IAM map user IdP → role HRM (403, cô lập lương)

[Adapter]  từ LIF / Job / Notif → SMTP, Excel, Git, CRM sản phẩm
           KHÔNG → CRM bán hàng
```

| Lớp | Trách nhiệm | Không làm |
|-----|-------------|-----------|
| CSDL | SoT dữ liệu từng service; PAY tách PII lương | Một DB dùng chung mọi service (MVP F) |
| Microservice | Use-case + domain DOC-06 | Terminate TLS hộ toàn hệ |
| Middleware (GW) | Cổng, SSO, route, 401 nếu không phiên | Tính lương / trừ quỹ |
| LBS | Phân tải, health check | Ủy quyền nghiệp vụ |
| SSO IdP | Xác thực người | Role màn HR (thuộc IAM) |

### 4.1 Góc nhìn logic (Component)

| Component | Trách nhiệm | Technology |
|-----------|-------------|------------|
| LBS | Cân bằng tải | TBD DOC-17 |
| API Gateway | Middleware SSO + route | TBD · .NET/YARP/khác không khóa |
| SSO IdP | Login Cty | TBD DOC-10 |
| IAM service | Role, 403, map SSO | .NET 9 |
| EMP…LIF services | SoT module | .NET 9, DB riêng |
| Notification | In-app + email/app | Service riêng |
| Job | T-15, T-7, N+3 | Service riêng; broker ADR-005 |
| Adapters | Excel / mail / Git / CRM lock | Trên LIF/Job — DOC-10 |

### 4.2 Góc nhìn tiến trình (Runtime)

| Luồng | Đường đi | Ghi chú |
|-------|----------------|---------|
| Login | Client → LBS → GW → **SSO IdP** → IAM map role | Không cookie local thay SSO |
| LEV C1→C2 trừ quỹ | Client → LBS → GW → LEV; notify async | OQ-010 ngoài SAD |
| TIM import → chốt | GW → TIM; NFR-001 đo sau LBS+GW | 1 mẫu master |
| PAY sau công chốt | TIM → (saga/outbox) → PAY | Phân tán — ADR-005 |
| PRB T-15/T-7 | Job service → PRB/EMP | Không LM → HR |
| PRB HR chốt | GW → PRB → EMP hoặc LIF | 403 LM/NV chốt |
| LIF N+3 | Job → LIF adapter Git/CRM | Secret IT |
| Event CRM sales | **Cấm** tại GW và broker | Fail AC |

Hàng đợi giữa service: **TBD ADR-005**.

### 4.3 Góc nhìn phát triển (Module / Package)

| Repo / package (nháp) | Bound |
|-----------------------|--------|
| `identity` | IAM |
| `employee-profile` | EMP |
| `leave` | LEV |
| `timekeeping` | TIM |
| `payroll` | PAY |
| `probation` | PRB |
| `lifecycle` | LIF |
| `events` / `hr-analytics` | Chưa SRS — không code trước FR |
| `gateway` | Middleware (không SoT nghiệp vụ) |
| `docs/03-modules/{id}/` | Artifact req |

Cấu trúc: **một repo hoặc nhiều repo / một service-deploy** *(ADR-001 Accepted gói F)*.

### 4.4 Góc nhìn vật lý / Triển khai

**Hosting + HA (ADR-001 Accepted · ADR-003 Accepted):** private mInvoice; **24/7**; **Active/Standby** trong DC; **DC-Prod + DC-DR**. IdP/LBS TBD. DB = **PostgreSQL** (ADR-009; version/host TBD). **Không** Active/Active hai DC.

```text
                    ┌─ DC-Prod (ACTIVE) ─────────────────────────┐
 Client ──► [LBS-A] ─► [GW-A] ─► [MS-A…] ─► [DB-A]  (jobs ON)
                    │         Standby cùng DC: LBS-S / GW-S / MS-S / DB replica
                    └────────────────────────────────────────────┘
                                      │ replicate DB (sync/async TBD)
                    ┌─ DC-DR (STANDBY) ──────────────────────────┐
                    │  LBS-DR / GW-DR / MS-DR / DB-DR   jobs OFF
                    │  Promote khi failover (RTO TBD) · SSO IdP cùng mô hình
                    └────────────────────────────────────────────┘
 Outbound chỉ từ Active: SMTP, Git, CRM sản phẩm
```

| Environment | Nodes | Scaling |
|-------------|-------|---------|
| Prod | Cặp A/S + LBS trên Active | Ngang trong DC qua LBS sau failover nội bộ |
| DR | Standby đủ công suất promote | Không nhận user đến khi cắt DC-Prod |
| Dev / UAT | Không bắt buộc đủ DR | — |

### 4.5 Kịch bản (+1)

| Scenario | Views | Validates |
|----------|-------|-----------|
| Login SSO web = mobile | LBS + GW + IdP + IAM | NFR-003, AG-012 |
| LEV C2 trừ quỹ | Logic + Process | NFR-005 |
| TIM/PAY 1000 dòng &lt;5s | Process + Physical | NFR-001 |
| NV xem phiếu; LM 403 lương | Logic | NFR-002, 004 |
| PRB T-15/T-7 + HR chốt | Process + Job | NFR-009 (PRB) |
| LIF N+3 khóa Git/CRM | Process + Adapter | NFR-006 |
| Failover A/S hoặc cắt sang DR | Physical + Process | NFR-012, AG-010, 014 |
| Không notify CRM sales | Process | NFR-007 |

## 5. Mối quan tâm xuyên suốt

| Concern | Approach | ADR ref |
|---------|----------|---------|
| Security / RBAC | Sau SSO; IAM map role; 403 màn HR | ADR-001 · ADR-002 |
| SSO | JWT OIDC tại GW; IAM SoT role; SAML→JWT nội bộ | ADR-002 **Accepted**; IdP **Lark** (ADR-007 v0.2) |
| MFA | Chưa bắt | ADR-006 còn lại |
| Audit | Log bất biến nghiệp vụ | NFR-005; stack TBD |
| Mã hóa at-rest / TLS | **Không** đóng AES trên NFR | ADR TBD |
| Error / 403 | Theo AC từng module | — |
| Master động | Catalog quy chế, không hardcode luật | CN-006 |
| HA / DR | 24/7; Active/Standby; DR/DC; job chỉ Active | ADR-003 **Accepted** |

## 6. Tóm tắt quyết định kiến trúc

→ Chi tiết file ADR. Bảng: Accepted vs Proposed (nợ).

| ADR ID | Decision | Status |
|--------|----------|--------|
| ADR-001 | Microservices + Gateway + LBS + SSO + .NET 9 + private | **Accepted** — [file](DOC-09-adr/ADR-001-stack-style-hosting.md) |
| ADR-002 | JWT OIDC tại GW + IAM SoT role; SAML→JWT nội bộ | **Accepted** — [file](DOC-09-adr/ADR-002-sso-token.md) |
| ADR-003 | 24/7 + Active/Standby + DR/DC (RTO/RPO số TBD) | **Accepted** — [file](DOC-09-adr/ADR-003-ha-dr-active-standby.md) |
| ADR-004 | Mã hóa at-rest | Proposed |
| ADR-005 | Broker job (in-process vs queue) | Proposed |
| ADR-006 | MFA | Proposed |
| ADR-007 | **Lark** IdP Cty; OIDC-only MVP; Google/Apple/@lhqglobal.vn; không host IdP trong HRM | **Accepted** v0.2 — [file](DOC-09-adr/ADR-007-idp-oidc.md) |

## 7. Rủi ro & Nợ kỹ thuật

| ID | Rủi ro | Mitigation |
|----|--------|------------|
| R-001 | EVT/RPT chưa SRS → SAD thiếu luồng cảnh báo/báo cáo | Giữ chỗ; không API bịa |
| R-002 | RTO/RPO **phút** chưa chốt | OQ-ARC-002; pattern 24/7+A/S+DR đã có ADR-003 |
| R-003 | LBS/Git-CRM API vendor / Lark tenant-region chưa IT cung cấp | DOC-10 **Chốt** kèm nợ; LBS → DOC-17 |
| R-007 | Saga TIM→PAY trên microservices | ADR-005; NFR-001 đo qua LBS+GW |
| R-004 | Chưa `02-baseline/` req | SAD **Chốt** tài liệu; chưa baseline repo |
| R-005 | Ban HR chưa ký | Nợ cổng nghiệp vụ |
| R-006 | HTML MCP / DOC-16 trống | Không chặn SAD khung |

## 8. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-ARC-005) · ☐ `02-baseline/` |
| Solution Architect | | 2026-08-26 | Khung SAD theo ADR-001/003 |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn → PGD chốt |
| Business Owner | Ban HR | | ☐ Nợ |
