# Decision Log — Delivery

> **ID:** `DEC-DLV-NNN`

### DEC-DLV-001 — Mở DOC-16 chương trình + DOC-17 Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-DLV-004 DOC-16 + DEC-DLV-007 DOC-17)*
- Context: Anh: tiếp DOC-16 và DOC-17. Template DOC-16 là per-module.
- Options: A Fan-out 7 file TC ngay · B Kèm code · C **Draft 1 DOC-16 chiến lược + 1 DOC-17; TC module sau; không code**
- Decision: chọn C
- Why (loại A vì một slice; loại B vì readiness code cần DOC-16 Chốt + AC)
- Consequences: Coverage ⚠️ đến khi có file module. Deploy A/S không khóa K8s. **Không** tự chốt / code.
- Affects: QC · DevOps · 7 module
- Trace: `docs/04-platform/DOC-16-test-strategy.md` · `docs/04-platform/DOC-17-deployment-guide.md`
- Confidence: cao *(phạm vi)* · vừa *(lệnh TBD)*

### DEC-DLV-002 — Mở leave DOC-16 TC Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-DLV-004)*
- Context: Anh chọn module **1 leave**.
- Options: A Kèm 6 module · B Kèm code · C **Chỉ leave DOC-16 Draft; TC-019 Skip OQ-010**
- Decision: chọn C
- Why (loại A vì một slice; loại B vì chưa chốt TC)
- Consequences: LEV-TC-001…018 map AC; 019 Skip. **Không** tự code / chốt. **Không** fan-out module khác.
- Affects: leave · DOC-16 chương trình
- Trace: `docs/03-modules/leave/DOC-16-test-strategy.md`
- Confidence: cao

### DEC-DLV-003 — Fan-out DOC-16 Draft 6 module còn lại · [2026-08-26]
- Status: accepted *(superseded by DEC-DLV-004)*
- Context: Anh: làm nốt các module DOC-16 còn lại (sau leave).
- Options: A Kèm code · B Kèm EVT/RPT · C **Draft PAY TIM EMP LIF IAM PRB; catalog map AC; không code**
- Decision: chọn C
- Why (loại A vì chưa chốt TC; loại B vì chưa SRS)
- Consequences: 6 file Draft. **Không** tự chốt / code.
- Affects: payroll · timekeeping · employee-profile · lifecycle · identity · probation
- Trace: `docs/03-modules/*/DOC-16-test-strategy.md`
- Confidence: cao

### DEC-DLV-004 — Chốt gói DOC-16 · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt gói DOC-16 (chương trình + 7 module Must).
- Options: A Giữ Draft · B **Chốt v0.1 catalog + chiến lược; chưa execute TC; không chốt DOC-17; không code; không baseline**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Sửa chiến lược/catalog đã chốt = CR. UAT Ban HR + chạy TC vẫn nợ. EVT/RPT không TC. DOC-17 Draft. **Không** tự code / baseline.
- Affects: QC · 7 module Must · DOC-16 chương trình
- Trace: `docs/04-platform/DOC-16-test-strategy.md` · `docs/03-modules/*/DOC-16-test-strategy.md`
- Confidence: cao *(khung)* · vừa *(coverage khi chưa chạy)*

### DEC-DLV-005 — Chi tiết TC leave DOC-16 §3 · [2026-08-26]
- Status: accepted *(bổ sung bước; ID catalog Chốt không đổi)*
- Context: Anh chọn module **leave** để viết chi tiết TC.
- Options: A Đổi/thêm ID catalog · B Kèm code · C **§3 đủ TC-001…019 bám DOC-07; không đổi ID; không code**
- Decision: chọn C
- Why (loại A vì catalog Chốt = CR; loại B vì chưa execute/readiness code)
- Consequences: QC chạy theo §3 khi mô tả §2.1 lệch AC-006/011. Path hủy chưa DOC-12. **Không** tự DOC-17 / code / baseline.
- Affects: leave DOC-16
- Trace: `docs/03-modules/leave/DOC-16-test-strategy.md` §3
- Confidence: cao

### DEC-DLV-006 — Chi tiết TC 6 module còn lại · [2026-08-26]
- Status: accepted *(bổ sung §3; ID catalog Chốt không đổi)*
- Context: Anh: làm tiếp chi tiết các module khác (sau leave).
- Options: A Đổi ID catalog · B Kèm code · C **§3 PAY TIM EMP LIF IAM PRB bám DOC-07 + path DOC-12; không code**
- Decision: chọn C
- Why (loại A vì catalog Chốt = CR; loại B vì chưa execute)
- Consequences: 7/7 module Must có §3. QC bám Gherkin khi catalog ngắn. **Không** tự DOC-17 / code / baseline.
- Affects: payroll · timekeeping · employee-profile · lifecycle · identity · probation
- Trace: `docs/03-modules/{payroll,timekeeping,employee-profile,lifecycle,identity,probation}/DOC-16-test-strategy.md` §3
- Confidence: cao

### DEC-DLV-007 — Chốt DOC-17 · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-17.
- Options: A Giữ Draft · B Bịa URL/kubectl/RTO phút · C **Chốt v0.1 runbook A/S; nợ URL LBS issuer DB RTO CI; không code; không baseline; không go-live**
- Decision: chọn C
- Why (loại A vì anh chốt; loại B vì ADR/DOC-13 không khóa số phút/sản phẩm)
- Consequences: Sửa bước deploy/rollback đã chốt = CR. Cổng go-live vẫn: AC Pass + dry-run + Ban HR. **Không** tự code / baseline.
- Affects: DevOps · QC · DOC-17
- Trace: `docs/04-platform/DOC-17-deployment-guide.md`
- Confidence: cao *(khung A/S)* · thấp *(lệnh/RTO)*

### DEC-DLV-008 — Ban HR ký + mở code slice IAM nháp · [2026-08-26]
- Status: accepted *(PGD: Ban HR đã ký; đủ tạm để thực thi)*
- Context: Anh: Ban HR đã ký, mở code. Readiness-gate: DOC-06/07/08/11/12/19 Chốt; thiếu JWKS, DB engine, JarvisRoot, SDK .NET trên máy soạn.
- Options: A Chặn đến đủ tuyệt đối · B Fan-out 7 MS + Jarvis giả · C **Đủ tạm; ghi nợ; 1 slice IAM Host; 401 /iam/me; không password; không Postgres/kubectl**
- Decision: chọn C
- Why (loại A vì anh mở code; loại B vì một slice + không bịa engine/vendor)
- Consequences: Code **nháp** đến OQ-DLV-001/003/005/006. 6 module khác chưa code. **Không** baseline / go-live.
- Affects: identity · `src/iam`
- Trace: `src/iam/Hrm.Iam.Host` · `memory/delivery/open-questions.md`
- Confidence: vừa *(nháp)* · thấp *(chưa build)*

### DEC-DLV-009 — Gắn Jarvis ProjectReference vào IAM Host · [2026-08-26]
- Status: accepted
- Context: Anh: gắn Jarvis vào slice IAM. JarvisRoot = `Học AI/jarvis`.
- Options: A Full 5-layer + EF · B **Mvc + Domain + Auth JWT + Swagger; chưa EF/OIDC Authority**
- Decision: chọn B
- Why (loại A vì chưa connection PostgreSQL / issuer IT)
- Consequences: `dotnet build` OK; Host listen :5080; `GET /v1/iam/me` không Bearer → 401. Authority trống + ValidateIssuerSigningKey=false (local). OQ-DLV-001/003 còn.
- Affects: `src/iam/Hrm.Iam.Host`
- Trace: JarvisRoot `/Users/Hung/Documents/Học AI/jarvis`
- Confidence: cao

### DEC-DLV-010 — IdP Lark + login Google / Apple / @lhqglobal.vn · [2026-08-28]
- Status: accepted *(PGD Dư Hùng)*
- Context: Anh chốt phạm vi đăng nhập OIDC HRM: email Google, Apple, mail công ty domain **@lhqglobal.vn** (chạy trên **Lark**). Khớp ADR-007 (HRM không host IdP).
- Options: A Tự tích hợp Google/Apple SDK trong HRM · B **Lark IdP Cty + OIDC RP; Google/Apple/@lhqglobal.vn qua cổng Lark/IT** · C Để mở vendor
- Decision: chọn **B**
- Why (loại A vì trái ADR-007; loại C vì anh chốt Lark)
- Consequences:
  - **IdP thương mại:** Lark (Feishu) — mail `@lhqglobal.vn` quản lý directory Lark.
  - **Google / Apple:** cấu hình federation hoặc phương thức đăng nhập trên **Lark / GW** (IT); HRM chỉ validate JWT từ **một issuer** Lark (khuyến nghị).
  - **HRM:** map `sub` OIDC → `iam_identity_account.IdpSubject`; roles SoT PostgreSQL (ADR-002). **Không** bịa issuer/JWKS URL.
  - OQ-DLV-001 còn: Lark **issuer + JWKS** cụ thể (tenant/region). OQ-DLV-007: brand **đã chốt Lark**; MFA/policy Apple hide-email còn IT.
- Affects: identity · Gateway · DOC-17 runbook · `hrm-backend` `Authentication:Jwt:Bearer:Authority`
- Trace: ADR-007 · OQ-DLV-001 · OQ-DLV-007 · `memory/delivery/open-questions.md`
- Confidence: cao *(vendor + domain)* · vừa *(issuer URL / federation Google-Apple)*

### DEC-DLV-011 — Tạm bỏ qua Lark JWKS (OQ-DLV-001) cho DEV/UAT · [2026-09-04]
- Status: accepted *(PGD Dư Hùng)*
- Context: Must 7 module đã land `#35`; IT chưa trả issuer/JWKS. Anh: tạm bỏ qua, không chờ JWKS để tiếp delivery/UAT.
- Options: A Dừng mọi việc đến JWKS · B **Bypass DEV/UAT bằng dev JWT** · C Bịa issuer/JWKS
- Decision: chọn **B**
- Why (loại A vì chậm Must/UAT; loại C vì cấm bịa OQ)
- Consequences:
  - **DEV/UAT:** `GET /dev/token` + `appsettings.Development.json` symmetric key; `ValidateIssuerSigningKey` theo env hiện có. Không bật `/dev/token` trên Production.
  - **Không** đổi DEC-DLV-010 (IdP vẫn Lark). **Không** điền Authority/JWKS giả.
  - OQ-DLV-001 **còn mở** — chỉ thôi chặn DEV/UAT; **Prod / go-live vẫn chặn** đến IT.
- Affects: identity auth local · TC IAM Partial · `memory/delivery/open-questions.md`
- Trace: OQ-DLV-001 · DEC-DLV-010 · PR #35
- Confidence: cao

### DEC-DLV-012 — Execute St identity DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: Sau Must #35 + TC-run 2026-09-04 + DEC-DLV-011; catalog IAM nhiều ô St trống.
- Options: A Để trống · B **Cập nhật St theo evidence e2e** (không đổi AC) · C Đánh Pass OIDC Lark
- Decision: chọn **B**
- Why (loại A vì che nợ QC; loại C vì JWKS chưa có)
- Consequences:
  - `docs/03-modules/identity/DOC-16` v0.2: Pass/Partial theo `e2e-api-iam-rbac` + PAY-J + LEV-C/D + tc-run.
  - Partial còn: TC-001/002/010/018/NFR-001 (OIDC Lark / disable-login e2e / SCR-001).
  - **Không** `02-baseline/`. Module DOC-16 khác chưa rollup trong DEC này.
- Affects: identity DOC-16 · memory/delivery
- Trace: DEC-DLV-011 · tc-run-2026-09-04
- Confidence: cao *(API e2e)* · vừa *(Partial OIDC)*

### DEC-DLV-013 — Execute St probation DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: Sau identity DOC-16 v0.2; PRB A–E e2e đã land.
- Options: A Để trống · B **Cập nhật St theo e2e A–E** · C Pass HA Standby khi chưa code
- Decision: chọn **B**
- Why (loại C vì job PRB chưa `IHostRoleGate`)
- Consequences:
  - `docs/03-modules/probation/DOC-16` v0.2: Pass hầu hết Must; **Partial** TC-013 (SCR chưa đủ 4 màn); **Open** TC-HA-001.
  - Không baseline. Module DOC-16 khác chưa trong DEC này.
- Affects: probation DOC-16
- Trace: e2e-api-prb-slice-a…e · DEC-DLV-012
- Confidence: cao

### DEC-DLV-014 — Execute St payroll DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: PAY slices A–K e2e đã land; tiếp rollup sau PRB DOC-16.
- Options: A Để trống · B **St theo e2e A–K** · C Pass đủ NFR-002 audit mọi thao tác
- Decision: chọn **B**
- Why (loại C vì mới chắc `PayslipViewed`)
- Consequences:
  - `docs/03-modules/payroll/DOC-16` v0.2: Pass TC-001…018 + NFR-001; **Partial** NFR-002.
  - Cùng PR/branch với DEC-DLV-013 (probation) nếu chưa merge.
- Affects: payroll DOC-16
- Trace: e2e-api-pay-slice-a…k · DEC-DLV-013
- Confidence: cao

### DEC-DLV-015 — Execute St timekeeping DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: TIM slices A–F e2e; tiếp sau PAY DOC-16.
- Options: A Để trống · B **St theo e2e A–F** · C Pass đủ SCR-014 / audit NFR
- Decision: chọn **B**
- Why (loại C vì UI/SCR và audit còn Partial)
- Consequences:
  - `docs/03-modules/timekeeping/DOC-16` v0.2: Pass hầu hết; **Partial** TC-008/014/NFR-002.
- Affects: timekeeping DOC-16
- Trace: e2e-api-tim-slice-a…f · DEC-DLV-014
- Confidence: cao

### DEC-DLV-016 — Execute St leave DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: LEV slices B–F e2e; tiếp sau TIM DOC-16.
- Options: A Để trống · B **St theo e2e B–F** · C Pass đủ mobile TC-002 / Should-016
- Decision: chọn **B**
- Why (loại C vì mobile UI và Should còn Partial)
- Consequences:
  - `docs/03-modules/leave/DOC-16` v0.2: Pass Must hầu hết; **Partial** TC-002/016; **Skip** TC-019.
- Affects: leave DOC-16
- Trace: e2e-api-lev-slice-b…f · DEC-DLV-015
- Confidence: cao

### DEC-DLV-017 — Execute St employee-profile DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: EMP A/B + e2e-full; đủ ô trống 014–016/NFR-001.
- Decision: cập nhật St **Pass** theo evidence (không đổi AC).
- Consequences: `employee-profile/DOC-16` v0.2 Pass catalog Must.
- Trace: e2e-api-emp-slice-a/b · DEC-DLV-016
- Confidence: cao

### DEC-DLV-018 — Execute St lifecycle DOC-16 v0.2 · [2026-09-04]
- Status: accepted *(PGD Dư Hùng — làm tiếp QC)*
- Context: LIF A–D e2e + HostRoleGate; SCR/Should còn mỏng.
- Decision: Pass hầu hết; **Partial** TC-011/012/014/NFR-002; **Pass** HA-001.
- Consequences: `lifecycle/DOC-16` v0.2. **Đủ 7 module Must** DOC-16 execute St v0.2 (cùng IAM/PRB/PAY/TIM/LEV).
- Trace: e2e-api-lif-slice-a…d · DEC-DLV-017
- Confidence: cao
