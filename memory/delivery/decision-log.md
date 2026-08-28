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
