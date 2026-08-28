# Decision Log — Architecture

> Quyết định **có phương án bị loại** (lưu "tại sao"). Schema: minipower pack `docs/decision-log.md`.
> **ID:** `DEC-ARC-NNN` · Nặng → DOC-09 ADR.

### DEC-ARC-001 — Mở DOC-08 SAD Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-ARC-005 chốt SAD)*
- Context: Anh: rollup overview + mở architecture DOC-08 sau khi 7 module AC Chốt (DEC-REQ-059). Skill tiên quyết ghi baseline DOC-06+13; repo **chưa** `02-baseline/`.
- Options: A Hoãn SAD đến baseline + SRS EVT/RPT · B **Mở DOC-08 Draft khung; nợ SLA/stack/SSO; không tự 09–12**
- Decision: chọn B
- Why (loại A vì anh mở SAD ngay; đủ tạm: 7 SRS + DOC-13 Chốt)
- Consequences: `docs/04-platform/DOC-08-sad.md` Draft. ADR-001…006 **pending** không soạn file. Sửa SAD = phiên Draft. **Không** tự DOC-09/10/11/12. Overview rollup 2026-08-26.
- Affects: platform · DOC-08 · overview
- Trace: `docs/04-platform/DOC-08-sad.md` · NFR-012
- Confidence: vừa *(stack/SLA trống)*

### DEC-ARC-002 — Soạn ADR-001 Proposed (gói A) · [2026-08-26]
- Status: accepted *(superseded by DEC-ARC-003 — PGD chỉ đạo gói F)*
- Context: Anh chọn đúng một ADR = **ADR-001**. Chưa có DEC ngôn ngữ trên repo HRM.
- Options: A **Modular monolith + .NET 9 + private mInvoice** (DB engine / Jarvis chưa khóa) · B Microservice / module · C SaaS mua · D K8s public bắt buộc MVP · E Monolith khác runtime (Node/Java)
- Decision: đề xuất **A** — chưa Accepted
- Why (loại B vì coupling TIM–PAY + 2027; loại C vì BRD xây nội bộ; loại D vì NFR-012 trống + OPEX chưa tách)
- Consequences: File ADR-001 Draft. **Không** tự DOC-10/11/12. OQ-ARC-001 còn mở đến khi Accepted.
- Affects: platform · DOC-08 · DOC-09
- Trace: `docs/04-platform/DOC-09-adr/ADR-001-stack-style-hosting.md`
- Confidence: vừa *(.NET = khuyến nghị SA)*

### DEC-ARC-003 — ADR-001 gói F: MS + Gateway + LBS + SSO · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: cần lớp middleware, microservices, LBS, kiểm soát truy cập SSO. Đảo gói A (DEC-ARC-002).
- Options: A Monolith · **F MS + API GW + LBS + SSO + .NET 9 + private + DB-per-service** · B MS thiếu LBS/SSO · C SaaS · D K8s public bắt buộc
- Decision: chọn **F** — **Accepted**
- Why (loại A vì không đáp ứng chỉ đạo PGD; loại B vì thiếu LBS/SSO)
- Consequences: Đảo stack/style = ADR mới. OQ-ARC-001 đóng. IdP/LBS/engine **TBD**. **Không** tự DOC-10/11/12/17.
- Affects: platform · DOC-08 · ADR-001
- Trace: `docs/04-platform/DOC-09-adr/ADR-001-stack-style-hosting.md` · AG-012 · AG-013
- Confidence: cao

### DEC-ARC-004 — ADR-003: 24/7 Active/Standby DR/DC · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: mô hình chạy 24/7, dự phòng Active/Standby, DR/DC.
- Options: **G** 24/7 + A/S trong DC + DC-Prod/DC-DR · H 8×5 · I Active/Active 2 DC · J một DC không DR
- Decision: chọn **G** — **Accepted**
- Why (loại H/J vì trái chỉ đạo; loại I vì double-job T-15/T-7 và split-brain chốt)
- Consequences: Pattern HA/DR khóa. Sửa = ADR mới. RTO/RPO phút **TBD**. **Không** tự DOC-17 (còn nợ ADR-001 Accepted).
- Affects: platform · DOC-08 §4.4 · NFR-012
- Trace: `docs/04-platform/DOC-09-adr/ADR-003-ha-dr-active-standby.md`
- Confidence: cao *(pattern)* · thấp *(số phút)*

### DEC-ARC-005 — Chốt DOC-08 SAD khung · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-08. ADR-001 và ADR-003 đã Accepted.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ IdP/LBS/RTO-RPO/MFA/EVT-RPT/Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng SAD đóng. Sửa kiến trúc đã chốt = CR hoặc ADR mới. **Không** tự DOC-10/11/12/17 / planning DOC-14.
- Affects: platform · DOC-08
- Trace: `docs/04-platform/DOC-08-sad.md`
- Confidence: cao *(khung)* · vừa *(nợ TBD)*

### DEC-ARC-006 — Soạn + chốt DOC-10 INT · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: DOC-10 SSO/IdP + tích hợp, **chốt**.
- Options: A Chỉ Draft · B Kèm OpenAPI DOC-12 · C **Chốt v0.1 INT-001…006; nợ sản phẩm IdP/Git/CRM API**
- Decision: chọn C
- Why (loại A vì anh chốt; loại B vì API sau INT)
- Consequences: `DOC-10-integration-specification.md` Chốt. INT-006 cấm CRM sales. Sửa INT đã chốt = CR. **Không** tự DOC-11/12/17. OQ-ARC-004 còn (sản phẩm IdP).
- Affects: platform · DOC-10 · LIF · IAM · TIM · Notif
- Trace: `docs/04-platform/DOC-10-integration-specification.md`
- Confidence: cao *(hướng INT)* · vừa *(vendor TBD)*

### DEC-ARC-007 — Chốt ADR-002 token SSO · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: DOC-11 + ADR-002, **chốt**.
- Options: K **JWT OIDC tại GW + IAM SoT role; SAML→JWT nội bộ** · L cookie monolith · M MS parse SAML · N API key user
- Decision: chọn **K** — Accepted
- Why (loại L vì trái MS; loại M vì NFR-003; loại N vì không SSO)
- Consequences: File ADR-002 Accepted. `exp` phút TBD. **Không** tự DOC-12. Đảo token = ADR mới.
- Affects: IAM · Gateway · DOC-11 IdentityAccount
- Trace: `docs/04-platform/DOC-09-adr/ADR-002-sso-token.md`
- Confidence: cao

### DEC-ARC-008 — Chốt DOC-11 khung data model · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-11 cùng ADR-002.
- Options: A DDL đầy đủ + engine · B **Chốt khung entity / DB-per-service / master SoT**
- Decision: chọn B
- Why (loại A vì engine TBD; catalog động)
- Consequences: `DOC-11-data-model/DOC-11-data-model.md` Chốt khung. Sửa entity đã chốt = CR. **Không** tự DOC-12.
- Affects: 7 DB service · PAY cô lập
- Trace: `docs/04-platform/DOC-11-data-model/DOC-11-data-model.md`
- Confidence: vừa *(chưa physical)*

### DEC-ARC-009 — Chốt IdP: OIDC Cty, không host HRM · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: DOC-12 + **chốt IdP**. Không có brand trên BRD.
- Options: **P** IdP Cty hiện có + OIDC-only MVP · Q tự host Keycloak · R SAML-only · S để mở OIDC+SAML
- Decision: chọn **P** — Accepted (ADR-007)
- Why (loại Q vì bịa vendor + vận hành IdP; loại R/S vì anh chốt một protocol)
- Consequences: SAML ADR-002 không dùng MVP. IT cung cấp issuer/JWKS. **Không** bịa Keycloak/Entra.
- Affects: INT-001 · DOC-12 · Gateway
- Trace: `docs/04-platform/DOC-09-adr/ADR-007-idp-oidc.md`
- Confidence: cao *(loại IdP)* · vừa *(issuer URL)*

### DEC-ARC-010 — Chốt DOC-12 khung OpenAPI · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh mở DOC-12 cùng chốt IdP.
- Options: A Full schema mọi FR · B **Khung path + openapi.yaml; cấm login password**
- Decision: chọn B
- Why (loại A vì catalog động + PK TBD)
- Consequences: `DOC-12-api-spec/` Chốt khung. Sửa path đã chốt = CR. **Không** tự DOC-17. Rate limit TBD.
- Affects: Gateway API v1
- Trace: `docs/04-platform/DOC-12-api-spec/DOC-12-api-specification.md`
- Confidence: vừa *(chưa body đầy đủ)*

### DEC-ARC-011 — ADR-008 Proposed: React + Go + K8s · [2026-08-26]
- Status: accepted *(superseded by DEC-ARC-012 — PGD giữ ADR-001)*
- Context: Anh muốn FE React, BE Go, triển khai Docker/K8s trên mây. ADR-001 Accepted = .NET 9, K8s không bắt buộc, private mInvoice.
- Options: A Giữ .NET · H Go+React Docker, K8s optional · **I Go+React+K8s, cloud vendor TBD** · J Public cloud + brand EKS/GKE
- Decision: đề xuất **I** — **chưa** Accepted
- Why (loại A vì lệch chỉ đạo; loại J vì bịa vendor + OPEX; loại H nếu anh không bắt K8s MVP)
- Consequences: File ADR-008 Proposed. **Không** sửa ADR-001; **không** rewrite DOC-08/17; **không** xóa `src/iam` đến khi chốt. **Không** viết Helm/kubectl.
- Affects: stack · DOC-08/14/17 (sau chốt) · nháp IAM
- Trace: `docs/04-platform/DOC-09-adr/ADR-008-react-go-k8s.md`
- Confidence: vừa

### DEC-ARC-012 — Giữ stack ADR-001; từ chối React/Go/K8s · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: giữ nguyên công nghệ ban đầu, không thay đổi nữa.
- Options: A Giữ ADR-001 (.NET 9, K8s optional, private) · I ADR-008 React+Go+K8s
- Decision: chọn **A** — ADR-008 **Rejected**
- Why (loại I vì anh đóng đảo stack)
- Consequences: Không CR DOC-08/17 vì stack. Nháp `src/iam` C# giữ. **Không** scaffold Go/React/Helm. Đảo sau = ADR mới + CR.
- Affects: platform · ADR-001 · ADR-008
- Trace: `docs/04-platform/DOC-09-adr/ADR-001-stack-style-hosting.md` · `ADR-008-react-go-k8s.md`
- Confidence: cao

### DEC-ARC-013 — ADR-009 Proposed: PostgreSQL SoT · [2026-08-26]
- Status: accepted *(superseded by DEC-ARC-014 chốt PostgreSQL)*
- Context: Anh hỏi PostgreSQL vs MongoDB cho HRM.
- Options: P PostgreSQL SoT 7 DB · M Mongo SoT · H hybrid
- Decision: đề xuất **P** — chưa Accepted
- Why (loại M vì lệch RDBMS ADR-001 + invariant quỹ/lương; loại H vì hai vận hành MVP)
- Consequences: File ADR-009 Proposed. **Không** sửa DOC-11/17 đến khi chốt. JSONB cho catalog. **Không** bịa RPO.
- Affects: DBA · 7 service
- Trace: `docs/04-platform/DOC-09-adr/ADR-009-postgresql.md`
- Confidence: cao

### DEC-ARC-014 — Chốt PostgreSQL SoT · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt PostgreSQL.
- Options: A Giữ TBD · M Mongo · **P PostgreSQL 7 DB-per-service**
- Decision: chọn P — ADR-009 **Accepted**
- Why (loại A vì anh chốt; loại M vì invariant quỹ/lương)
- Consequences: Mongo không SoT. Version/host/connection TBD. JSONB catalog. **Không** bịa RPO / cài cluster. Sửa loại engine = ADR mới.
- Affects: DBA · DOC-08/11/17 (nợ engine) · 7 service
- Trace: `docs/04-platform/DOC-09-adr/ADR-009-postgresql.md`
- Confidence: cao

### DEC-ARC-015 — Bổ sung ADR-007: IdP Lark · [2026-08-28]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh rollup brand IdP sau DEC-DLV-010: mail `@lhqglobal.vn` trên **Lark**; login Google + Apple + mail công ty.
- Options: A Giữ ADR-007 generic · B **Bổ sung v0.2 ADR-007 + DOC-10/17; không bịa issuer URL**
- Decision: chọn **B**
- Why (loại A vì anh chốt Lark; loại tạo ADR mới vì không đảo protocol/host IdP)
- Consequences: ADR-007 v0.2 · INT-001 Lark · BLK-006 đóng trên overview. OQ-DLV-001 còn (issuer/JWKS). **Không** baseline.
- Affects: identity · Gateway · DOC-10 · DOC-17 · `hrm-backend` JWT config
- Trace: `docs/04-platform/DOC-09-adr/ADR-007-idp-oidc.md` · `memory/delivery/decision-log.md` DEC-DLV-010
- Confidence: cao *(brand)* · vừa *(Lark federation Google/Apple)*
