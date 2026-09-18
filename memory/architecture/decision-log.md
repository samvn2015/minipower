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

### DEC-ARC-016 — Microservices là yêu cầu khách hàng; bỏ DR/DC · [2026-09-07]
- Status: accepted *(PGD trả lời OQ-ARC-008 + OQ-ARC-009 sau [deliberation B1](../../brainstorm/2026-09-07-arc-b1-monolith-vs-microservices.md))*
- Context: Deliberation B1 dừng ở hai câu chặn — lý do gốc chỉ đạo microservices, và đội ops cho 7 service + DR hai DC.
- Options: *(không phải lựa chọn phương án — đây là PGD cung cấp dữ kiện còn thiếu)*
- Decision:
  - **OQ-ARC-008 → microservices là _ý kiến khách hàng_**, không phải lựa chọn kỹ thuật nội bộ.
  - **OQ-ARC-009 → _không làm DR/DC_ nữa.**
- Why: khách quyết kiến trúc mục tiêu; DR/DC bị loại khỏi phạm vi vận hành.
- Consequences:
  - **T1 gần như đã có đáp án:** không thể tự đảo ADR-001 sang monolith — đó là cam kết với khách. Muốn đổi phải qua kênh khách hàng, không phải CR nội bộ.
  - **ADR-003 bị đảo một phần** — §3 (hai site DC-Prod/DC-DR), §4 (replicate Prod→DR), §5 (job disable trên DR) không còn áp dụng. ADR-003 ghi rõ *"Không sửa khi Accepted — đảo = ADR mới"* → **phải soạn ADR mới supersede**.
  - **R-01 (DOC-14) nhẹ đi đáng kể** — bỏ site thứ hai là bỏ phần lớn gánh vận hành mà góc Operations lo nhất. Microservices trở nên khả thi hơn so với giả định trong deliberation.
  - **OQ-ARC-002 / BLK-002 (RTO/RPO) đổi bản chất** — không còn DR site thì RPO replicate hết nghĩa; RTO tính theo khôi phục trong một DC.
  - DOC-17 §DR và DOC-13 NFR-012 phải sửa theo ADR mới.
  - **Chưa rõ:** bỏ DR/DC có kèm bỏ luôn cặp Active/Standby trong **một** DC (ADR-003 §2) không — cần PGD xác nhận trước khi soạn ADR.
- Affects: ADR-001 · **ADR-003** · DOC-13 NFR-012 · DOC-17 · DOC-14 R-01 · memory/delivery BLK-002
- Trace: OQ-ARC-008 · OQ-ARC-009 · DEC-ARC-003 · DEC-ARC-004 · deliberation B1
- Confidence: cao *(dữ kiện từ PGD)* · vừa *(phạm vi đảo ADR-003 — còn câu hỏi Active/Standby)*

### DEC-ARC-017 — ADR-010: 24/7 + A/S một DC, bỏ DR/DC · [2026-09-07]
- Status: accepted *(PGD «vận hành 24/7, cặp active/standby trong cùng 1 DC»)*
- Context: DEC-ARC-016 chốt bỏ DR/DC nhưng chưa rõ có bỏ luôn Active/Standby trong DC không. PGD xác nhận **giữ** 24/7 và A/S.
- Options: G 24/7 + A/S + DR/DC hai site *(ADR-003)* · **K 24/7 + A/S một DC, backup/restore cho thảm họa** · J một DC chỉ RAID/backup · I Active/Active hai DC
- Decision: chọn **K** → soạn **ADR-010**, supersede **ADR-003 §3–5**, giữ §1–2
- Why (loại G vì đội ops hai site không tồn tại — DOC-14 R-01/A-01; loại J vì không đạt 24/7; loại I vì double-job, conflict chốt công/lương — đã loại từ ADR-003)
- Consequences:
  - Bỏ site thứ hai, bỏ replicate xuyên site, bỏ runbook chuyển site.
  - Mất cả DC → **ngừng phục vụ đến khi restore**. Rủi ro PGD chấp nhận có ý thức.
  - **NFR-012 tách hai loại RTO**: failover trong DC (nhanh) vs restore khi mất DC (chậm). RPO replicate xuyên site **không còn đối tượng** → OQ-ARC-002 / BLK-002 phải phát biểu lại chứ không chỉ điền số.
  - Nợ mới: chu kỳ backup, nơi lưu off-site, thời gian restore mục tiêu — Ops/IT.
  - Phương án K **không có** trong bảng options của ADR-003 (ADR-003 chỉ xét J); đây là điểm mới, không phải chọn lại phương án cũ.
  - Nếu khách từng được cam kết DR/DC → là **thay đổi cam kết**, phải qua khách (cùng kênh với OQ-ARC-008).
- Affects: **ADR-003 §3–5** · ADR-010 (mới) · DOC-13 NFR-012 · DOC-17 · DOC-08 SAD · DOC-14 R-01 · BLK-002
- Trace: DEC-ARC-016 · DEC-ARC-004 · OQ-ARC-002 · OQ-ARC-009 · deliberation B1
- Confidence: cao

### DEC-ARC-018 — Bỏ DR/DC là yêu cầu khách hàng · ADR-010 v0.2 · [2026-09-07]
- Status: accepted *(PGD «khách hàng yêu cầu bỏ DR/DC»)*
- Context: ADR-010 v0.1 ghi rủi ro *"nếu khách từng được cam kết DR/DC thì đây là thay đổi cam kết"*. PGD làm rõ nguồn gốc.
- Options: *(không chọn phương án — bổ sung dữ kiện nguồn gốc yêu cầu)*
- Decision: bỏ DR/DC **do khách hàng yêu cầu**, không phải quyết định nội bộ cắt chi phí → ADR-010 lên **v0.2**
- Why: đặt yêu cầu này cùng thẩm quyền với ràng buộc microservices (OQ-ARC-008) — cả hai từ khách
- Consequences:
  - **Rủi ro "vi phạm cam kết" đóng.** Ngược lại, **giữ** hai site mới là làm sai yêu cầu khách.
  - Rủi ro còn lại chuyển thành **kỳ vọng**: cần khách xác nhận bằng văn bản đã hiểu hệ quả — mất DC là ngừng phục vụ đến khi restore.
  - **Quan sát cần đưa lại khách:** khách yêu cầu microservices (tăng phức tạp vận hành) *đồng thời* bỏ DR/DC (giảm chịu thảm họa) — hai yêu cầu kéo ngược chiều. Không sai, nhưng nên xác nhận khách hiểu.
  - DOC-02 luồng phê duyệt: cần dấu vết yêu cầu của khách trong `assets/` để trace, hiện **chưa có file gốc**.
- Affects: ADR-010 v0.2 · DOC-13 NFR-012 · DOC-17 · quan hệ khách hàng
- Trace: DEC-ARC-017 · DEC-ARC-016 · OQ-ARC-008 · OQ-ARC-009
- Confidence: cao *(dữ kiện từ PGD)* · thấp *(chưa có văn bản gốc từ khách trong assets/)*

### DEC-ARC-019 — Backup sang máy Standby; khách đã ký; lý do bỏ DR/DC là chi phí · [2026-09-07]
- Status: accepted *(PGD: «backup sang máy standby» · «khách có văn bản xác nhận đã hiểu, bỏ do chi phí quá cao»)*
- Context: ADR-010 v0.2 để mở hai điểm — đích lưu backup, và văn bản xác nhận của khách.
- Options: *(bổ sung dữ kiện, không chọn phương án)*
- Decision:
  - **Đích backup = máy Standby** (cùng DC). ADR-010 §4 · DOC-17 §2.2 · NFR-012d.
  - **Khách đã ký văn bản** hiểu hệ quả → rủi ro kỳ vọng đóng.
  - **Lý do bỏ DR/DC = chi phí hai site quá cao** (không phải gánh ops như v0.1 suy đoán).
- Why: khách chịu chi phí, chọn mức bảo vệ thấp hơn.
- Consequences:
  - **RK-01 mở (OQ-ARC-012 · chặn go-live):** Standby ở cùng DC ⇒ mất DC là **mất luôn backup**. **NFR-012c hiện không thể đạt**; mất DC = **mất dữ liệu**, không phải ngừng phục vụ tạm thời.
  - **Lệch phạm vi văn bản khách đã ký:** khách xác nhận ở mức *"ngừng phục vụ đến khi restore"*, không phải *"mất dữ liệu"*. Chọn (a) thì phải xác nhận lại với khách ở đúng mức.
  - Hai lựa chọn cho PGD: **(a)** chấp nhận mất dữ liệu, phát biểu lại ADR-010 §6 + DOC-13 NFR-012c + xác nhận lại với khách · **(b)** thêm bản sao lạnh ngoài DC — rẻ hơn DR site nhiều bậc, giữ nguyên ý nghĩa §2.2.
  - Runbook "mất cả DC" trong DOC-17 §8 **treo** đến khi giải OQ-ARC-012.
  - Văn bản khách nên đưa vào `assets/public/` để trace (hiện chưa có file).
- Affects: ADR-010 v0.3 · DOC-13 NFR-012c/d · DOC-17 §2.2 + §8 · OQ-ARC-012 · quan hệ khách hàng
- Trace: DEC-ARC-017 · DEC-ARC-018 · OQ-ARC-002 · RK-01
- Confidence: cao *(dữ kiện từ PGD)* · **RK-01 là suy luận kỹ thuật, tin cậy cao**

### DEC-ARC-020 — Mở ADR-011: lộ trình tách service theo wave · [2026-09-07]
- Status: **proposed** *(chưa chốt — chờ PGD)*
- Context: Deliberation B1 đã khung lại vấn đề; DEC-ARC-016 + ADR-010 gỡ hai câu chặn. Đích đến bị khoá bởi khách (microservices), câu còn lại là **lộ trình**.
- Options: L Big bang tách 7 service · **M Hàng rào dữ liệu trước, tách deploy sau** · N Tách deploy trước, DB sau · O Giữ monolith
- Decision: **đề xuất M** — chưa Accepted
- Why (loại O vì trái yêu cầu khách; loại L vì đội chưa có, rủi ro trượt 2027 cao nhất; **loại N vì 7 service chung một DB = distributed monolith — vẫn không có hàng rào NFR-002 mà đã gánh trọn chi phí vận hành phân tán**)
- Consequences:
  - W1 tách schema + DB role theo bounded context → **NFR-002 có hàng rào thật sớm nhất, chi phí thấp nhất**, không phụ thuộc tiến độ tuyển người.
  - W2 cưỡng chế ranh giới bằng assembly + test kiến trúc. W3 tách PAY + Gateway. W4+ phần còn lại.
  - Hệ thống ở trạng thái **lai** nhiều wave → DOC-08 cần quy ước phân biệt *hiện tại* vs *đích đến*, nếu không doc-review chặn lần nữa.
  - RK-02 W1 đụng 56 migration + 28 repository → slice riêng, có rollback.
  - RK-03 khách có thể hiểu W1/W2 là "chưa làm" → cần trình lộ trình.
  - **RK-04 ADR-005 (saga TIM→PAY) còn Proposed nhưng W3 cần** → phải chốt trước W3.
- Affects: ADR-011 (mới) · DOC-08 · DOC-11 · DOC-12 · DOC-14 · ADR-005
- Trace: deliberation B1 · DEC-ARC-016 · ADR-010 · doc-review 2026-09-07 Blocker B1/B2/B3
- Confidence: vừa *(lộ trình hợp lý nhưng W1 chưa được xác minh kỹ thuật — OQ-ARC-011)*

### DEC-ARC-021 — Ký DOC-11 v0.2 + DOC-12 v0.2 · [2026-09-07]
- Status: accepted *(PGD ký sau regression doc-review)*
- Context: Regression review đóng B2/B3 về nội dung nhưng mở Blocker F5 — hai DOC mang nhãn Chốt v0.2 mà chưa ai ký, không có DEC. Trợ lý đã hạ về Draft.
- Options: A Giữ Draft đến khi có đội review · **B PGD ký v0.2 ngay** · C Rollback về v0.1
- Decision: chọn **B** — ký v0.2, nhãn trở lại **Chốt**
- Why (loại C vì v0.1 mô tả hệ thống không tồn tại; loại A vì không có đội review độc lập, giữ Draft chỉ trì hoãn)
- Consequences:
  - DOC-11 §9 và DOC-12 §9 thêm dòng ký **2026-09-07**; dòng v0.1 giữ nguyên làm lịch sử.
  - F5 **đóng**. `02-baseline/` nay có phiên bản hợp lệ để lấy — nhưng vẫn chờ B1.
  - 3 Minor của regression chưa sửa: DOC-11 §2 trỏ ADR đang Proposed *(nay đã Accepted — DEC-ARC-022)*, §3 sinh từ snapshot không phải DB thật, §4 tên khái niệm cũ.
- Affects: DOC-11 v0.2 · DOC-12 v0.2 · BLK-004
- Trace: doc-review regression 2026-09-07 F5 · DEC-ARC-008 · DEC-ARC-010
- Confidence: cao

### DEC-ARC-022 — ADR-011 Accepted: lộ trình wave (phương án M) · [2026-09-07]
- Status: accepted *(PGD chốt)*
- Context: B1 là Blocker cuối chặn baseline. ADR-011 dựng 4 phương án sau khi DEC-ARC-016 khoá đích đến và ADR-010 gỡ gánh DR/DC.
- Options: L Big bang · **M Hàng rào dữ liệu trước, tách deploy sau** · N Tách deploy trước DB sau · O Giữ monolith
- Decision: chọn **M** → ADR-011 **Proposed → Accepted**
- Why (loại O trái yêu cầu khách; loại L vì đội chưa có, rủi ro trượt 2027 cao nhất; loại N vì 7 service chung một DB = distributed monolith, vẫn không có hàng rào NFR-002 mà đã gánh trọn chi phí vận hành)
- Consequences:
  - **B1 đóng về mặt quyết định nội bộ.** W1 (schema + DB role theo bounded context) là slice tiếp theo được phép mở.
  - **OQ-ARC-014 VẪN MỞ** — ADR này là quyết định **nội bộ về cách đi**. Nếu khách đòi đủ 7 service tại go-live 2027 thì M sụp, phải quay lại L. **Cần trình khách trước khi mở W1.**
  - RK-04: ADR-005 (saga TIM→PAY) còn Proposed, phải chốt trước W3.
  - DOC-08 cần quy ước phân biệt *kiến trúc hiện tại* vs *đích đến*, nếu không doc-review chặn lại ở lần sau.
  - RK-02: W1 đụng 56 migration + 28 repository → slice riêng, có rollback.
- Affects: ADR-011 · DOC-08 · DOC-11 · DOC-12 · DOC-14 · ADR-005 · BLK-004
- Trace: deliberation B1 · DEC-ARC-016 · ADR-010 · doc-review 2026-09-07 B1
- Confidence: vừa *(lộ trình chốt, nhưng W1 chưa xác minh kỹ thuật — OQ-ARC-011 — và khách chưa xác nhận — OQ-ARC-014)*

### DEC-ARC-023 — Xác minh OQ-ARC-011: schema + role đủ cho NFR-002 · [2026-09-07]
- Status: accepted *(kết quả đo, không phải lựa chọn phương án)*
- Context: ADR-011 W1 dựa trên AS-04 — tin cậy **vừa**, chưa ai kiểm. Sai giả định này thì cả W1 phải thiết kế lại, mà W1 đụng 56 migration + 28 repository (RK-02).
- Options: *(đo, không chọn)*
- Decision: **AS-04 đúng** — nâng tin cậy từ *vừa* lên **cao**. W1 giữ nguyên thiết kế.
- Why: đo thật trên PostgreSQL 16.8, database riêng `hrm_oq011`, 8/8 case đúng kỳ vọng. Quan trọng nhất: **JOIN chéo bị chặn ở tầng DB** và role **không tự nâng quyền** được.
- Consequences:
  - **Hàng rào NFR-002 khả thi mà không cần tách service** — nền tảng của phương án M được xác nhận bằng thực nghiệm.
  - **Phát hiện 1:** app role chỉ có `USAGE` thì **không migrate được** kể cả schema của mình. Cần chốt ở W1: (i) app role có `CREATE`, hay (ii) **migrator role riêng** — nghiêng (ii) vì DOC-17 §2.1 đã ghi *"Prod: pipeline migrate riêng"*. Đã kiểm: cấp `CREATE` **không** làm thủng hàng rào.
  - **Phát hiện 2:** `pg_catalog` lộ **tên bảng** schema khác (không lộ cột, không lộ dữ liệu). Chuẩn PostgreSQL, không tắt được bằng GRANT. **Không** vi phạm NFR-002 — ghi để audit không bất ngờ.
  - **Phát hiện 3:** role `admin` có `rolcreaterole=false` → W1 **cần DBA/superuser** provision schema + role. Gộp yêu cầu vào OQ-DLV-003 khi gửi IT.
  - Database thử nghiệm đã **dọn sạch**; `hrm` thật không bị đụng.
- Affects: ADR-011 W1 · OQ-ARC-011 *(đóng)* · OQ-DLV-003 · DOC-17 §2.1
- Trace: AS-04 deliberation B1 · ADR-011 · DEC-ARC-022
- Confidence: cao *(thực nghiệm)*

### DEC-ARC-024 — W1 hoàn tất; audit theo phương án A · [2026-09-07]
- Status: accepted *(PGD chọn A sau khi readiness-gate nêu 3 hướng)*
- Context: W1a/W1b dựng hàng rào nhưng app vẫn nối bằng **một** role `admin` → grant nằm im. W1c tách context thì 4 module ghi audit (EMP/TIM/PRB/LIF, cộng PAY ở query) gặp vấn đề: `AppendAsync` gọi `SaveChanges` của chính DbContext, tách ra sẽ thành **hai transaction**; thêm nữa `IEmpAuditLogRepository` chỉ bind được **một** implementation.
- Options: **A Map `EmpAuditLog` vào mọi context + tách interface theo module** · B Giữ audit trên `AppDbContext`, chấp nhận hai transaction · C Outbox trong context rồi job đẩy sang `shared`
- Decision: chọn **A**
- Why (loại B vì nghiệp vụ commit mà audit fail = sót audit, trái NFR-005 *"0 sót"*; loại C vì thêm job + độ trễ cho thứ chưa cần)
- Consequences:
  - **Bảng audit vẫn là MỘT** ở schema `shared` — ①a và DEC-DLV-022/024 nguyên vẹn. Chỉ tách *đường vào*, không tách dữ liệu. `GET /v1/emp/audit-logs?action=` vẫn đọc đủ (đã kiểm).
  - `EmpAuditLogRepositoryBase(DbContext)` dùng `Set<EmpAuditLog>()`; mỗi context một lớp con + interface đánh dấu (`ITimAuditLogRepository`…). Lệnh nghiệp vụ + audit **cùng một `SaveChanges`**.
  - Sửa consumer: 2 file TIM · 1 PAY · 1 PRB · 1 LIF · 10 test fake. EMP giữ interface gốc.
  - **W1 đóng:** 7/7 context có role riêng; 28/28 repository rời `AppDbContext`; `AppDbContext` chỉ còn là migration owner.
  - **Bài học ghi lại:** map entity từ context khác kéo theo **cả closure navigation**, EF chỉ báo lúc runtime từng cái một (`Employee` → `EducationLevel` → `OrgUnit`). `LevDbContext` phải nạp trọn nhóm `emp` chỉ vì một JOIN lọc line manager → **lập luận mạnh nhất cho W3 thay JOIN bằng API**.
  - **Đính chính khảo sát:** bản quét "0/28 repository chạm >1 context" **sai** (regex `\bEmployee\b` không khớp `db.Employees`). Đúng là **1/28** — `LeaveRequestRepository`.
- Affects: 7 DbContext · 28 repository · 5 consumer Application · 10 test · ADR-011 W1 · OQ-ARC-016 *(đóng)*
- Trace: ADR-011 W1 · DEC-ARC-022 · DEC-ARC-023 · readiness-gate 2026-09-07 (①a ②a ③ii)
- Confidence: cao *(đo bằng pg_stat_activity + test + endpoint)*

### DEC-ARC-025 — Mở ADR-005: coupling TIM↔PAY không phải saga · [2026-09-07]
- Status: **proposed** *(chưa chốt — chờ PGD)*
- Context: RK-04 chặn ADR-011 W3. DOC-08 §4.5 ghi *"saga TIM→PAY"*, §5 ghi *"hàng đợi giữa service TBD"*.
- Options: **P Đọc guard qua API, không broker** · Q Broker + saga · R TIM publish event, PAY giữ read model · S Đọc thẳng DB chéo
- Decision: **đề xuất P** — chưa Accepted
- Why: soi code cho thấy **không có luồng ghi nào trải trên cả hai context**. PAY→TIM và TIM→PAY đều là **đọc guard đồng bộ** (`FindPeriodByYmAsync`, `IPayPeriodGate`). Saga giải một bài toán **không tồn tại**. Loại S vì phá hàng rào W1. Loại Q/R vì thêm hệ thống trạng thái phải vận hành trong khi đội ops chưa có (DOC-14 A-01/R-01, ADR-010), và cửa sổ dữ liệu cũ trái ngữ nghĩa chốt kỳ.
- Consequences:
  - **DOC-08 §4.5 R-007 sai với hệ thống thật** — phải bỏ chữ "saga".
  - Không broker ở MVP ⇒ không thêm thành phần cần A/S + backup (khớp ADR-010).
  - `GET /v1/tim/periods/{ym}` và `GET /v1/pay/periods/{ym}` thành **hợp đồng liên service** từ W3.
  - Guard **fail-closed**: gọi lỗi/timeout ⇒ từ chối thao tác, không đoán.
  - **Phát hiện thêm:** không có `BackgroundService`/`IHostedService` nào. Job T-15/T-7/N+3 chạy bằng endpoint do **bộ lập lịch ngoài** gọi, chưa tài liệu hoá (RK-06 · OQ-ARC-017).
  - RK-05 timeout/retry guard chéo (OQ-ARC-018) · RK-07 idempotency `POST /v1/prb/jobs/reminders/run` chưa kiểm, trùng nhắc là rủi ro NFR-009.
- Affects: ADR-005 (mới) · ADR-011 W3 *(gỡ RK-04)* · DOC-08 §4.5/§5 · DOC-12 · DOC-17
- Trace: ADR-011 RK-04 · DOC-08 R-007 · DEC-ARC-022
- Confidence: cao *(đọc code trực tiếp)* · vừa *(chưa đo hiệu năng hop mạng sau W3)*

### DEC-ARC-026 — ADR-005 Accepted: bỏ saga, không broker · [2026-09-07]
- Status: accepted *(PGD chốt phương án P)*
- Context: RK-04 chặn ADR-011 W3. DOC-08 mang giả định "saga TIM→PAY" từ 2026-08-26, chưa ai đối chiếu code.
- Options: **P Đọc guard qua API, không broker** · Q Broker + saga · R Publish event + read model · S Đọc thẳng DB chéo
- Decision: chọn **P** → ADR-005 **Proposed → Accepted**
- Why (loại Q/R vì thêm hệ thống trạng thái phải backup/giám sát/khôi phục trong khi đội ops chưa có, và đưa vào cửa sổ dữ liệu cũ đúng chỗ nghiệp vụ chốt kỳ không chấp nhận; loại S vì phá hàng rào W1)
- Consequences:
  - **RK-04 đóng** — W3 hết chặn về mặt kiến trúc; còn OQ-ARC-014 (khách).
  - **DOC-08 v0.3**: §4.2 dòng Job, §4.5 luồng PAY-sau-công-chốt, §5 hàng đợi, bảng ADR, và **R-007 đóng** — giả định saga đã được gỡ khỏi tài liệu.
  - **DOC-12 v0.3 §4.3**: `GET /v1/tim/periods/{ym}` và `GET /v1/pay/periods/{ym}` thành **hợp đồng liên service** — đổi shape là breaking change, không phải sửa API nội bộ.
  - **DOC-17 v0.3 §2.3**: tài liệu hoá bộ lập lịch job — trước đây **không có ở đâu cả** dù NFR-009 phụ thuộc vào nó.
  - Guard **fail-closed**; job chỉ chạy trên Active qua LBS (ADR-010 §5); job phải idempotent theo ngày.
  - Nợ mới: RK-05/OQ-ARC-018 timeout+retry guard (chốt ở W3) · RK-06/OQ-ARC-017 sản phẩm lập lịch · **RK-07 idempotency `POST /v1/prb/jobs/reminders/run` chưa kiểm** — gọi hai lần có thể nhắc trùng, NFR-009 không nói gì về trùng.
- Affects: ADR-005 · ADR-011 W3 · DOC-08 v0.3 · DOC-12 v0.3 · DOC-17 v0.3 · NFR-009
- Trace: ADR-011 RK-04 · DOC-08 R-007 · DEC-ARC-025
- Confidence: cao *(đọc code trực tiếp)* · vừa *(chưa đo hop mạng sau W3)*

### DEC-ARC-027 — ADR-012: bỏ SSO, HRM tự quản password · [2026-09-15]
- Status: accepted *(PGD «đã chốt bỏ không sử dụng Login web SSO» · nguồn: **khách hàng yêu cầu**)*
- Context: SSO là nền từ 26-08 (ADR-007 Lark, ADR-001 §4, DOC-12 §2 cấm password, DOC-11 §3.1 không lưu hash). CR-001 xin password auth Prod ngày 07-09 và **bị từ chối** (DEC-DLV-025). Nay khách yêu cầu bỏ SSO.
- Options: **T Password tự quản, không SSO** · U Giữ SSO đổi IdP · V Password + SSO song song · W Giữ ADR-007
- Decision: chọn **T** → **ADR-012** supersede **ADR-007 toàn bộ** + **ADR-001 §4**; CR-001 mở lại thành **CR-002 Approved**
- Why (khách quyết — ràng buộc thương mại, cùng thẩm quyền với DEC-ARC-016/018; loại U vì khách bỏ SSO chứ không đổi IdP; loại V vì hai luồng xác thực = hai bề mặt tấn công, không ai yêu cầu)
- Consequences:
  - **OQ-DLV-001 (Lark JWKS) đóng** — blocker Prod lớn nhất từ 26-08 biến mất; go-live không còn chờ IT cấp issuer.
  - **Lý do từ chối CR-001 vẫn đúng** (bề mặt tấn công tăng) — chỉ thẩm quyền đổi. Ghi rõ để không ai đọc lại tưởng hôm 07-09 sai.
  - `POST /dev/login` **KHÔNG** thành cơ chế Prod — plaintext trong config, giữ 404 ngoài Development (ADR-012 §5, RK-09).
  - DOC-11/12 vừa ký v0.2/v0.3 (DEC-ARC-021) **lệch lại** — phải sửa trước baseline.
  - **Code chưa được mở**: thiếu IAM DOC-06/07 và DOC-13 policy (CR-002 §Tiền đề). Viết xác thực trước khi có policy = viết hai lần.
  - Nợ mới: OQ-DLV-009 reset mật khẩu (RK-10) · OQ-DLV-010 policy · OQ-ARC-007 phát biểu lại (MFA sau password).
  - **RK-08**: văn bản khách chưa có trong `assets/` — ba yêu cầu lớn nhất dự án (microservices, bỏ DR/DC, bỏ SSO) hiện chỉ truy được qua lời PGD.
  - **Quan sát đưa lại khách** (không phải phản đối): ba yêu cầu này cộng lại = kiến trúc phân tán + không dự phòng thảm họa + tự gánh credential. Mỗi cái hợp lý riêng; cộng lại là gánh vận hành và bảo mật cao nhất trong các tổ hợp có thể — với đội ops chưa có (DOC-14 A-01).
- Affects: ADR-012 · ADR-007 · ADR-001 §4 · CR-001 · CR-002 · DOC-10/11/12/13/17 · identity DOC-06/07/16 · OQ-DLV-001/009/010 · OQ-ARC-007
- Trace: DEC-DLV-025 *(bị đảo)* · DEC-ARC-016 · DEC-ARC-018 · CR-001
- Confidence: cao *(dữ kiện PGD)* · **thấp** *(chưa có văn bản khách — RK-08)*

### DEC-ARC-028 — Văn bản khách chốt cả ba yêu cầu (RK-08) · [2026-09-16]
- Status: accepted *(PGD: «khách chốt rồi, đã gửi vb sang» · «văn bản này chốt cả ba yêu cầu»)*
- Context: DEC-ARC-016 (microservices), DEC-ARC-018 (bỏ DR/DC), DEC-ARC-027 (bỏ SSO) đều ghi confidence **thấp** ở vế "chưa có văn bản khách". RK-08 mở từ 07-09.
- Options: *(ghi nhận dữ kiện)*
- Decision: **một** văn bản của khách chốt **cả ba** yêu cầu. Đăng ký chỗ tại `assets/public/README.md`, tên theo quy ước `2026-09-15_xac-nhan_khach-chot-3-yeu-cau.<ext>`.
- Consequences:
  - Ba DEC trên nâng vế confidence "văn bản khách" từ **thấp → vừa**: tồn tại theo PGD, **chưa** nằm trong repo.
  - **RK-08 chưa đóng** — đóng khi file thật vào `assets/public/`. Đã tìm assets/, Downloads, Desktop, Documents gốc (2026-09-16): không có file mới từ 14-09.
  - Khi có file, việc **phải làm**: đọc và đối chiếu **RK-01**. Văn bản DR/DC trước ký ở mức *"ngừng phục vụ đến khi restore"*; backup cùng DC nghĩa là **mất dữ liệu**. Nếu văn bản mới cũng dừng ở mức cũ thì OQ-ARC-012 vẫn lệch phạm vi.
- Affects: RK-08 · DEC-ARC-016/018/027 · OQ-ARC-012 · assets/public
- Trace: DEC-ARC-027
- Confidence: vừa *(chờ file)*

### DEC-ARC-029 — ADR-013: chủ đầu tư **không cần** ba chức năng — microservices cũng bỏ · [2026-09-16]
- Status: accepted *(PGD: «chủ đầu tư đã trả lời bằng vb là không cần ba chức năng, vb đang gửi qua đường bưu điện» · hỏi lại: ba chức năng = **microservices + DR/DC + SSO**)*
- Context: DEC-ARC-028 ghi văn bản khách *"chốt cả ba yêu cầu"* — hiểu là **xác nhận** microservices là yêu cầu. Nay PGD nói rõ: khách trả lời **không cần** cả ba. Với DR/DC và SSO, đây là xác nhận lại ADR-010/012. Với **microservices** là đảo chiều: ràng buộc DEC-ARC-016 *(«microservices là ý kiến khách hàng»)* **rút**. Đây là điều deliberation B1 (UN-03) hỏi từ 07-09 và nay có đáp án: **khách không đòi**.
- Options: **X modular monolith có hàng rào W1+W2, không GW, giữ LBS** · Y tiếp tục ADR-011 W3/W4 dù không ai cần · Z gỡ W1/W2 về một `AppDbContext` một role · Y′ giữ Gateway trước monolith
- Decision: chọn **X** → **ADR-013** supersede **ADR-001 §1/§2/§5**, **ADR-002 toàn bộ**, **ADR-011 W3/W4+**. W1/W2 **giữ** làm kiến trúc đích. LBS giữ (cho A/S, ADR-010). ADR-005 giữ; guard TIM↔PAY nay trong process.
- Why (khách không cần ≠ cấm → chọn monolith vì bằng chứng kỹ thuật đã nghiêng về đó từ B1: monolith đã chạy 7 Must, đội ops chưa có (A-01), hàng rào NFR-002 đã đạt bằng W1 mà không cần tách service; loại Y vì gánh phân tán không ai yêu cầu; loại Z vì bỏ thứ đã xác minh và đã trả giá; loại Y′ vì GW không làm được gì middleware host chưa làm)
- Consequences:
  - **Sửa cách đọc DEC-ARC-028**: không phải *"chốt cả ba yêu cầu"* mà là *"không cần cả ba chức năng"*. DEC-ARC-028 giữ nguyên, đọc kèm mục này. Nội dung dòng đăng ký `assets/public/README.md` sửa theo.
  - **Trạng thái hiện tại = đích.** Hết "trạng thái lai"; nguyên nhân gốc Blocker B1 (doc-review 07-09) **đóng hẳn**.
  - **Đóng không cần giải:** RK-03 · RK-04 · RK-05 · OQ-ARC-013 · OQ-ARC-014 · OQ-ARC-018.
  - **Nợ tài liệu lớn:** DOC-08 viết lại §1.4/§4.0–4.2/§4.4/§6/R-007 (lần 4) · DOC-11 §1.2 · DOC-12 §1 · DOC-13 NFR-001 · DOC-14 WBS wave · DOC-17 §2/§4/§5/§8. **Chưa sửa** — slice riêng, SA. RK-13: không sửa trước baseline thì doc-review chặn lần ba.
  - **Code: không làm gì.** Không gỡ W1/W2. Không W3. Nợ CI (test kiến trúc phải chạy tự động) — RK-12.
  - Nợ mới: **RK-11** blast radius một process · **RK-12** GRANT "tiện tay" phá hàng rào im lặng · **RK-13** DOC-08 chưa viết lại trước baseline.
  - **RK-08 vẫn mở** — văn bản qua **bưu điện**, chưa có ngày tới. ADR-013 Accepted trên lời PGD. Khi nhận: đối chiếu đúng chữ *"không cần"* cho **cả ba** mục + RK-01. Nếu văn bản không nói tới microservices → đảo ADR-013 bằng ADR mới.
  - Quan sát DEC-ARC-027 (*ba yêu cầu = gánh cao nhất*) nhẹ đi: phần phân tán biến mất. Còn hai gánh khách chọn có văn bản: không DR (RK-01), tự gánh credential (ADR-012).
- Affects: ADR-013 · ADR-001 · ADR-002 · ADR-011 · ADR-005 *(ghi chú)* · DOC-08/11/12/13/14/17 · OQ-ARC-013/014/018 · OQ-DLV-002 *(lý do đổi)* · RK-03/04/05/08/11/12/13 · assets/public
- Trace: DEC-ARC-016 *(đảo vế microservices)* · DEC-ARC-022 *(W3/W4 bị thay)* · DEC-ARC-028 *(sửa cách đọc)* · deliberation B1 UN-03
- Confidence: cao *(dữ kiện PGD, hỏi lại một lần)* · **thấp** *(văn bản chưa tới — RK-08)*

### DEC-ARC-030 — DOC-08 v0.4 Chốt: SAD mô tả một kiến trúc — modular monolith đang chạy · [2026-09-16]
- Status: accepted *(PGD «ký v0.4»)*
- Context: DOC-08 v0.3 vẫn vẽ 7 microservice + Gateway + DB-per-service + SSO IdP. ADR-012 (bỏ SSO) và ADR-013 (không cần microservices) làm §1.4, §2, §4.0–4.5, §5, §6, §7 lệch cùng lúc. Viết lại hai lần là phí → một bản v0.4 gộp cả hai.
- Options: *(không có phương án — cập nhật DOC theo ADR đã Accepted)*
- Decision: **DOC-08 v0.4 Chốt.** SAD từ nay mô tả **một** kiến trúc, soi từ `hrm/` 2026-09-16: một `Hrm.Host`, một instance PostgreSQL 8 schema / 7 role / migrator, LBS trước host cho A/S, không Gateway, đăng nhập HRM tự quản, TIM↔PAY guard trong process. Bỏ cặp "hiện tại / đích đến" của ADR-011.
- Why (kiến trúc đích = kiến trúc đang chạy; DOC không được mô tả thứ chưa xây — đúng lỗi Blocker B1 hôm 07-09)
- Consequences:
  - **+AG-015** (ranh giới bằng cơ chế) · **+AG-016** (một deploy unit, không GW/DB-per-service/broker).
  - §4.3 dùng **path thật** thay "nháp"; `w1-roles.sql` được nêu là SoT của GRANT.
  - §4.5 thêm 3 kịch bản **đã kiểm** (token giả 401 · role LEV không đọc `pay.*` · mất DB → readiness 503).
  - **Nói thẳng hai điều SAD cũ không có:** mobile **chưa có code** (R-013); Prod **chưa chạy được** (R-014). Login Prod chưa code (R-012).
  - Rủi ro mới vào SAD: R-008 (RK-01 backup cùng DC) · R-009 (RK-08 văn bản khách) · R-010 (RK-11 blast radius) · R-011 (RK-12 GRANT im lặng).
  - **Nợ kéo theo, chưa sửa:** DOC-11 §1.2/§3.1 · DOC-12 §1/§2/§4.3 *(hai endpoint `periods/{ym}` không còn là hợp đồng liên service)* · DOC-13 NFR-001 + mật khẩu · DOC-14 · DOC-17 §2/§4/§5/§8. RK-13: sửa trước baseline.
- Affects: DOC-08 v0.4 · DOC-11 · DOC-12 · DOC-13 · DOC-14 · DOC-17
- Trace: DEC-ARC-029 · DEC-ARC-027 · DEC-ARC-026 · ADR-011 *(hệ quả tiêu cực "trạng thái lai" — hết)*
- Confidence: cao *(soi code trực tiếp)* · **thấp** *(vế văn bản khách — RK-08)*

### DEC-ARC-031 — Ký DOC-11 v0.3 · DOC-12 v0.4 · DOC-13 v0.3 · DOC-17 v0.4 theo ADR-012/013 · [2026-09-16]
- Status: accepted *(PGD «ký cả 4, merge»)*
- Context: Sau DOC-08 v0.4 (DEC-ARC-030), bốn DOC platform còn mô tả DB-per-service, Gateway, OIDC/Lark, hợp đồng liên service. RK-13: không sửa trước baseline thì doc-review chặn lần ba.
- Options: *(cập nhật DOC theo ADR đã Accepted; riêng DOC-13 §3.3 là chọn số)*
- Decision:
  - **DOC-11 v0.3** — schema/role/DbContext ghi trên từng §3.x; FK xuyên schema chỉ ID, ngoại lệ LEV→`emp` 4 cột; 5 cột password **chưa migration**; §6 bỏ replicate DR (sót từ v0.1).
  - **DOC-12 v0.4** — bỏ GW/OIDC/JWKS; §2 ba endpoint auth ADR-012 (chưa code); §4.3 guard TIM↔PAY trong process, hai endpoint `periods/{ym}` là API thường; §3 phân trang đã hiện thực S1.
  - **DOC-13 v0.3** — **+NFR-S07…S12**. **PGD chốt số theo đề xuất SA:** ≥12 ký tự, blocklist, không ép đổi định kỳ (NIST 800-63B); khoá 15′ sau 5 lần sai; Argon2id m=64MiB t=3 p=1 hoặc PBKDF2-SHA256 ≥600k; rate 10/phút/IP + 5/phút/username, 429, không lộ tài khoản tồn tại; reset một lần 24h ép đổi. **OQ-DLV-010 đóng.** +NFR-M03 ranh giới cưỡng chế, NFR-SC01 scale = nhân bản host.
  - **DOC-17 v0.4** — 8 connection string + bẫy fallback `hrm_migrator`; `w1-roles.sql` sau khi đổi `CHANGE_ME_*`, `pg_hba` scram; secret ký JWT thay Lark; §7 kiểm **`/dev/login` → 404** (mục này trước chỉ là TODO ở CR-001 — ADR-012 RK-09 ghi "đã có" là chưa đúng, nay có thật).
- Why (bốn DOC phải mô tả đúng hệ thống đang chạy và đích đã chốt; số mật khẩu lấy chuẩn công khai thay vì bịa)
- Consequences:
  - **Code login (CR-002) còn chặn bởi:** IAM DOC-06 FR + DOC-07 negative AC (BA) · OQ-DLV-009 quy trình reset · OQ-ARC-007 MFA. Policy **không** còn chặn.
  - **Nợ mới nhìn thấy:** `openapi.yaml` chưa sinh lại sau S1 (4 endpoint thiếu `page`/`size`/`X-Total-Count`), Swagger title còn "HRM Gateway API" · guard khởi động Prod từ chối thiếu connection string **chưa code** · CI/Dockerfile/hosting SPA/OTEL collector chưa có.
  - Mọi DOC platform giờ cùng một kiến trúc → có thể **chạy doc-review** lần ba cho `04-platform` trước khi mở `02-baseline/`.
- Affects: DOC-11 · DOC-12 · DOC-13 · DOC-17 · OQ-DLV-010 · CR-002 §Tiền đề
- Trace: DEC-ARC-029 · DEC-ARC-030 · DEC-ARC-027 · ADR-012 §6 · RK-13
- Confidence: cao *(soi code)* · số mật khẩu: **vừa** *(chuẩn công khai, chưa pen test)*

### DEC-ARC-032 — Doc-review pass 3 `04-platform`: ⛔ BLOCK; sửa Major thuộc SA, hai Blocker chờ PGD · [2026-09-18]
- Status: accepted *(PGD «chạy doc-review lần ba» → «thực hiện tiếp»)*
- Context: Pass 3 chạy bằng subagent context sạch trên DOC-08/11/12/13/17 vừa ký (DEC-ARC-030/031): **2 Blocker · 15 Major · 20 Minor** — [`memory/delivery/doc-review-2026-09-16-platform.md`](../delivery/doc-review-2026-09-16-platform.md). Agent chính đã kiểm lại 4 claim nặng nhất trên code trước khi ghi — đúng cả 4.
- Options: *(sửa lỗi theo owner; không đổi quyết định nào)*
- Decision: sửa ngay các finding **SA là owner và không cần PGD quyết** — v0.x.1 của DOC-08/11/12/13/17, nhãn *"sửa lỗi, không đổi quyết định"*:
  - **M1** DOC-17: key JWT đúng tên Jarvis (`Authentication:Jwt:Bearer:IssuerSigningKeys/ValidIssuers/ValidAudiences`); **xoá `Authority`** — Jarvis thấy `Authority` là bỏ `IssuerSigningKeys`; template Prod còn `TBD-LARK-OIDC-ISSUER` → **Dev sửa template, nợ**.
  - **M3** DOC-12 §3: envelope theo `BaseResponse` thật (`code` top-level `Hrm.Host:*`, `data`, `error{message,systemMessage,details}`); **422** cho mọi `BusinessException`; 429 chưa có đường trả. Envelope `{"error":{"code"…}}` v0.1–0.4 **chưa từng đúng**.
  - **M2** DOC-08 §1.3/§5/R-004/R-012 trỏ bản đã ký. **M10** `Hrm:HostRole` vào DOC-08/17 (mặc định Active khi rỗng — fail-open, ghi rõ). **M11** bỏ `idempotencyKey` không tồn tại; RK-07 **đóng** (unique + `ExistsAsync` + test). **M13** adapter ra ghi *chưa code* — R-015. **M12** → **OQ-ARC-019** (scheduler xác thực bằng gì) — R-016. **M7** DOC-13: IAM DOC-06/07 **có**, thiếu delta password. **M15** → NFR-S13 (vòng đời JWT) TBD. **M6** ghi nợ trong DOC-13 §3.3 với đề xuất, **chờ PGD chốt (1)–(3)**. Minor: 29 migration, 163 = 154 + 9, S11/S12 vào §2, TIM template, unique tổ hợp, `/v1`, path `hrm/…`, **OQ-DLV-009 trùng ID → reset mật khẩu đổi thành OQ-DLV-011** (ADR-012 Accepted giữ nguyên chữ 009 — đọc kèm mục này).
  - **B1** (NFR-005 đứt ở LEV/IAM): DOC-08/11 sửa thành **5/7 context**, R-017. **Cách đóng chờ PGD**: (a) chấp nhận nợ có deadline, hay (b) code LEV+IAM audit trước baseline.
  - **B2** (DOC-10/14/16 còn GW/Lark/DR): **chưa sửa** — owner SA/PM/QC; **hoặc** PGD DEC baseline theo file. Chờ PGD.
  - **Chưa sửa, có owner:** M4 `openapi.yaml` info (Dev sinh lại) · M5 vòng đời tài khoản (BA delta IAM DOC-06/07; DOC-11/12 sửa sau) · M8 Prod lần đầu (DBA tách `w1-roles.sql`) · M9 replication/promote (DevOps) · M14 LBS→host forwarded headers (DevOps+Dev) · Minor 9/10/11/13/16/17/18/19/20.
- Why (Major M1/M3 là loại "người làm theo DOC sẽ làm sai ngay" — sửa trước; hai Blocker là lựa chọn có chi phí, thuộc PGD)
- Consequences:
  - Gate **vẫn BLOCK** cho tới khi PGD quyết B1 (a/b) và B2 (sửa 3 DOC / baseline theo file).
  - Lớp lỗi lặp 3 pass: **tên cột viết tay không tồn tại** và **path thiếu `/v1`** — quy tắc: mọi cột/path trong DOC copy từ snapshot/OAS, không gõ.
  - `appsettings.Production.json` **sai theo ADR-012** (còn `Authority` Lark) — code, nợ Dev; guard khởi động chưa có.
- Affects: DOC-08 v0.4.1 · DOC-11 v0.3.1 · DOC-12 v0.4.1 · DOC-13 v0.3.1 · DOC-17 v0.4.1 · CR-002 · OQ-DLV-011 · OQ-ARC-019 · R-015/016/017
- Trace: DEC-ARC-030 · DEC-ARC-031 · doc-review-2026-09-16-platform · ADR-012 · ADR-013
- Confidence: cao *(mọi sửa đối chiếu code)*

### DEC-ARC-033 — Đóng gate pass 3: B1 code audit LEV+IAM · B2 sửa DOC-10/14/16 · M6 chốt số · [2026-09-18]
- Status: accepted *(PGD trả lời 3 câu, chọn cả 3 phương án khuyến nghị)*
- Context: Doc-review pass 3 BLOCK (DEC-ARC-032). Ba việc còn chờ PGD.
- Options: B1 **(a)** code audit trước baseline / (b) nợ có deadline · B2 **(a)** sửa 3 DOC / (b) baseline theo file · M6 **(a)** chốt theo đề xuất SA / (b) để sau
- Decision: **B1 (a)** — slice code: `EmpAuditLog` vào `LevDbContext` + `IamDbContext`, repo `Lev/IamAuditLogRepository`, allowlist test kiến trúc, ghi audit C1/C2/huỷ phép + gán/thu role/disable. **B2 (a)** — SA sửa DOC-10 (bỏ INT-001 Lark, bỏ GW), DOC-14 (bỏ WBS wave tách service, R-01 phát biểu lại), DOC-16 (một host, bỏ TC SSO), mỗi DOC một commit, PGD ký. **M6 (a)** — DOC-13 v0.3.2: blocklist top-100k HIBP/NIST + chứa username; **rate limit xét trước khoá** (429 không tăng `FailedAttempts`); **một thông báo chung**, cùng độ trễ, khoá báo riêng qua email/HR.
- Why (NFR-005 là Must và IAM audit là nền cho login/khoá ADR-012 — trả nợ bây giờ rẻ hơn sau CR-002; baseline nguyên khối thư mục tránh 02-baseline/ thiếu integration/WBS/test; số M6 lấy chuẩn công khai)
- Consequences:
  - Gate mở lại khi: B1 slice xanh (unit + kiến trúc + autotest) · DOC-10/14/16 ký · rồi **doc-review pass 4 hẹp** (chỉ 3 DOC + B1) trước `02-baseline/`.
  - M6 còn (4) forwarded headers và (5) 429 chưa có đường trả — **code**, mở cùng CR-002.
  - B1 kéo theo: role `hrm_app_lev`/`hrm_app_iam` cần INSERT trên `shared.emp_audit_log` — kiểm `w1-roles.sql` ①a đã cấp cho 7 role chưa.
- Affects: DOC-13 v0.3.2 · DOC-10 · DOC-14 · DOC-16 · `hrm/` LEV/IAM audit · w1-roles.sql · Hrm.Architecture.Tests
- Trace: DEC-ARC-032 · doc-review-2026-09-16-platform B1/B2/M6
- Confidence: cao
