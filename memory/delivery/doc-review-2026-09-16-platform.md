# Doc-review pass 3 — `04-platform` sau ADR-012/013 · [2026-09-16]

**Slice:** DOC-08 v0.4 · DOC-11 v0.3 · DOC-12 v0.4 + `openapi.yaml` · DOC-13 v0.3 · DOC-17 v0.4 (DEC-ARC-030/031) — đối chiếu ADR-005/010/011/012/013 và code `hrm/` cùng ngày.
**Reviewer:** subagent **context sạch** (skill doc-review §Cơ chế) — người viết 5 DOC không tự soi. Agent chính đã **kiểm lại 4 claim nặng nhất** trên code trước khi ghi (B1, DOC-17 JWT/Prod template, envelope, DOC-10/14/16).
**Verdict: ⛔ BLOCK baseline `04-platform`** — **2 Blocker · 15 Major · 20 Minor**.

> Cái đã đúng, không sửa lại: DOC-11 §3 khớp 41/41 bảng-schema, tên cột 100 %; DOC-12 §4/§8 khớp `openapi.yaml` (82 path / 88 op / 34 schema); §3 phân trang khớp code 4 endpoint; pipeline §4.0 đúng thứ tự `HostLayerExtension.cs:94-109`; 8 connection string; 13 controller; 9 test kiến trúc; 163 test; 14 link tương đối tồn tại; 5 file .md không còn dấu vết GW/Lark/DR/saga chưa phủ định; version ↔ DEC khớp decision-log.

---

## 🔴 Blocker

**[B1] Traceability + Mâu thuẫn chéo — NFR-005 · DOC-08 §2 AG-005, §4.1, §4.5 · DOC-11 §2 · DOC-13 NFR-005**
Bằng chứng: DOC-08 *"Audit | `shared.emp_audit_log` — mọi context INSERT"*; DOC-11 §2 *"audit dùng chung 7 module"*; DOC-08 §4.5 *"LEV C2 trừ quỹ … Validates NFR-005"*. Code: `LevDbContext.cs:11` *"LEV **không** ghi EmpAuditLog"*; `IamDbContext.cs:11` *"IAM **không** ghi"*; `BoundedContextBoundaryTests.cs:39-44` allowlist audit chỉ Tim/Pay/Prb/Lif; `InfrastructureLayerExtension.cs:40-44` không có Lev/Iam repo. LEV lưu C1/C2 in-row trên `LeaveRequest`, role LEV có UPDATE/DELETE → không "bất biến". *(Đã kiểm lại — đúng.)*
Vì sao lỗi: NFR-005 Must; chuỗi NFR-005 → C1/C2 → audit **đứt ở LEV**, và sẽ đứt ở IAM (đăng nhập/khoá/reset theo ADR-012). Baseline đóng băng một khẳng định sai.
Đề xuất: **(a)** SA sửa DOC-08/11 thành *"5/7 context ghi; LEV, IAM chưa — nợ R-0xx có deadline"* + PGD DEC chấp nhận nợ; **hoặc (b)** Dev làm slice LEV+IAM audit trước baseline. Owner: SA + Dev + PGD.

**[B2] Mâu thuẫn chéo / Nhất quán — DOC-10 v0.1, DOC-14 v0.1, DOC-16 v0.1 (cùng thư mục sẽ baseline)**
Bằng chứng: DOC-10 *"INT-001 = OIDC Lark … API Gateway"*; DOC-14 *"1.1 Nền tảng (GW, LBS, IAM-RP OIDC, A/S+DR)"*, *"7 service + GW"*; DOC-16 *"1000 dòng sau LBS+GW … SSO"*, *"DB-per-service"*, *"LBS→GW→OIDC→MS"*. Cả ba **Chốt**. ADR-012 giao DOC-10 (SA), ADR-013 giao DOC-14 (PM) — chưa làm; ADR-013 ghi DOC-10 *"dự kiến không đổi"* — **sai**. *(Đã kiểm: 25/16/7 hit.)*
Vì sao lỗi: mở `02-baseline/` cho thư mục = baseline luôn ba DOC mô tả kiến trúc đã bỏ.
Đề xuất: SA sửa DOC-10, PM sửa DOC-14, QC sửa DOC-16 — **hoặc** PGD DEC baseline **theo file**, loại ba file này. Owner: SA / PM / QC / PGD.

## 🟡 Major

| # | Chỗ | Bằng chứng (rút gọn) | Owner |
|---|---|---|---|
| M1 | **DOC-17 §2.1/§3/§3.1 — cấu hình JWT Prod trỏ key không tồn tại; template Prod còn Lark** | `appsettings.Production.json` có `Authority: TBD-LARK-OIDC-ISSUER`, `Audience`; Jarvis `AuthenticationBuilderExtension.cs:141-163`: có `Authority` ⇒ **bỏ qua `IssuerSigningKeys`**, validate qua metadata OIDC. DOC-17 ghi key `Authentication:Jwt:IssuerSigningKeys` (thật: `…:Jwt:Bearer:IssuerSigningKeys`), `Issuer`/`Audience` (thật: `ValidIssuers`/`ValidAudiences`). DOC-17 nói *"3 placeholder đã thay"* — phải **xoá `Authority`**, không phải thay. *(Đã kiểm lại — đúng.)* | SA + Dev |
| M2 | DOC-08 §1.3, l.12, §5, R-004, R-012 · DOC-12 §2/§7 vs DOC-13 v0.3 cùng ngày | DOC-08 vẫn *"DOC-13 Chốt v0.2, NFR mật khẩu trống, chặn code login"*; DOC-12 §7 *"Rate limit TBD DOC-13"* — trong khi DOC-13 v0.3 đã chốt số, OQ-DLV-010 đóng | SA |
| M3 | **DOC-12 §3 envelope lỗi + "body là mảng"** | Jarvis `BaseResponse`: `code` **top-level**, `error:{message,systemMessage,details}`, `data`; 200 bọc `{code, data:[…]}` (`client.ts` unwrap `.data`); bảng mã thiếu **422** (mọi `BusinessException`) và 429. DOC-12 tự nhận `openapi.yaml` là SoT nhưng §3 trái SoT. *(Đã kiểm lại — đúng.)* | SA |
| M4 | `openapi.yaml` `info` | title *"HRM Gateway API"*, description *"issuer OIDC TBD … JWT từ IdP (ADR-007)"* — viết tay, không sinh từ runtime (Swagger title thật = assembly `Hrm.Host`) | SA + Dev |
| M5 | **Vòng đời tài khoản sau ADR-012** — DOC-11 §3.1, DOC-12 §2/§8, DOC-13 S08/S11 | Chỉ có login/change/reset; thiếu **tạo tài khoản + cấp mật khẩu ban đầu + unlock**. Code còn **JIT-provision** tại `GET /v1/iam/me` (`IdentityAccountProvisioner`, IAM-FR-017 thời IdP) — không DOC nào ghi phải bỏ. DOC-11 thiếu cột mật khẩu tạm/ép đổi. IAM không map audit; `ActorIdpSubject` required → login thất bại với username lạ không ghi được | SA + BA |
| M6 | DOC-13 §3.3 S07/S08/S10/S11 testable | blocklist không nói danh sách nào; S08 khoá sau 5 vs S10 5 req/phút/username — lần thứ 6 nhận 429 hay khoá?; *"không lộ tài khoản tồn tại"* vs thông báo khoá; *"theo IP"* — sau LBS host chỉ thấy IP LBS, **không có ForwardedHeaders** trong code/Jarvis; Jarvis `RateLimitedException` đang comment → 429 chưa có đường trả; S11 còn chữ *"(đề xuất)"* trong bảng đã Chốt | SA + BA + PGD |
| M7 | DOC-13 l.10/§4/§5 *"IAM DOC-06/07 chưa có"* | Thật: `identity/DOC-06` v0.1 Chốt (DEC-REQ-047), DOC-07 Chốt (DEC-REQ-049). Cái thiếu là **FR/AC password trong DOC đã Chốt** → delta qua CR-002, không phải "viết mới" | SA/BA |
| M8 | DOC-17 §2.1 Migrate / §5 — **Prod lần đầu không chạy được theo thứ tự đã viết** | `w1-roles.sql` GRANT trên schema (cần schema có trước), header *"tiền đề: migration W1 đã áp"*; nhưng `hrm_migrator` do chính file này tạo, không có `CREATE ON DATABASE`; migrate bằng superuser ⇒ bảng thuộc superuser ⇒ `ALTER` sau bằng migrator fail | DevOps/DBA + SA |
| M9 | DOC-08 §4.4 *"PostgreSQL-S replica, Standby nóng"* vs DOC-17 | DOC-17 không có replication/promote/switchover; §5 chỉ đổi node app; *"migrate trên Standby trước failover"* — replica read-only | DevOps + SA |
| M10 | **`Hrm:HostRole`** không DOC nào nhắc | `HostRoleGate.cs`: rỗng ⇒ **Active** (fail-open); job từ chối 400 khi Standby. Failover phải lật key; node quên cấu hình ⇒ job chạy song song | SA + DevOps + Dev |
| M11 | DOC-17 §2.3 *"`LifAccessLockOutbox` có `idempotencyKey`"* | Cột **không tồn tại** (11 cột, không có); idempotency thật = bỏ qua case đã `GitLockedAtUtc && CrmSpLockedAtUtc`. Cùng lớp lỗi `Mst`/`TaxId`. ADR-005 §3 mang cùng lỗi | SA |
| M12 | Bộ lập lịch ngoài **xác thực bằng gì** sau ADR-012 | Job handler `RequireItOrPgdForLocks(actor)` → cần JWT của người IT/PGD; JWT chỉ cấp qua login mật khẩu ⇒ scheduler phải giữ mật khẩu người — trái S07/NFR-006; không NFR cho service account | SA + PGD |
| M13 | DOC-08 §4.0/§4.1/§4.5 **Adapter ra vẽ như đã có** | grep smtp/MailKit/HttpClient trong backend: 0; không package mail/HTTP; outbox ghi nhưng **không có consumer**. Login/mobile được ghi "chưa code", adapter thì không | SA |
| M14 | LBS → host: giao thức + forwarded headers | `UseHttpsRedirection` ngoài Development + không `ForwardedHeaders` ⇒ TLS terminate ở LBS + HTTP tới host = **vòng lặp 301**; IP thật không có cho S10/audit | DevOps + SA + Dev |
| M15 | DOC-13 thiếu NFR vòng đời JWT | không TTL/idle/logout/xoay khoá; dev token 8h; *"xoay khoá = mọi token hết hạn"* nằm trong runbook chứ không phải NFR. Disable tài khoản hiệu lực tức thì (`IamAccessGuard`) — tốt, chưa ghi | SA/BA + PGD |

## ⚪ Minor (20)

1. DOC-11 §1.2, DOC-12 §3 *"56 migration"* — thật **29** (57 file kể `.Designer.cs`).
2. DOC-08 §4.3 *"Domain + Application = 163 test"* — 154; 163 gồm 9 kiến trúc.
3. DOC-11 §3.1 đoạn văn chèn giữa bảng → dòng `Role` rơi khỏi bảng.
4. DOC-12 §4 thứ tự 4.1 → 4.3 → 4.2.
5. DOC-13 §2 thiếu S11/S12; §5 nói S07…S12; §6 nói S07…S10.
6. DOC-13 l.19 *"năm module đã chốt"* — 7.
7. DOC-17 §5 bước 5 `/iam/me` thiếu `/v1` — **lớp lỗi regression 07-09 lặp lại**.
8. DOC-17 `../hrm/…` không resolve từ `docs/04-platform/`; README hrm không có mục User Secrets.
9. DOC-08 §6 ADR-004/006 "Proposed" — không có file, không có trong register; l.12 bỏ ADR-003 §1-2.
10. `DOC-09-adr/README.md` bảng bị cắt làm 3 bởi dòng trống; ADR-010 §2 còn *"Gateway, từng service"* chưa chú thích ở register.
11. `04-platform/README.md` còn *"DOC-17 Draft"*, DOC-11/12 v0.1; `05-traceability/doc-registry.md` path sai, Status Draft.
12. `memory/delivery/open-questions.md` **hai `{OQ-DLV-009}`** (Ban HR ký BO — đóng; reset mật khẩu — mở).
13. Business Owner ☐ Nợ ở DOC-08/11/13 vs DEC-DLV-008 — **Minor pass 1 còn nguyên**.
14. DOC-11 §4 *"Mẫu Excel CC | TIM template"* — **Minor regression 07-09 còn nguyên**.
15. **RK-07 đã đóng trong code** (`ExistsAsync` + unique `(EmployeeId, Kind, ProbationEndDate)` + unit test) mà DOC-08 §4.2, DOC-17 §2.3 nói "chưa kiểm"; DOC-11 §3 cột Unique ghi "—" cho **10 bảng có unique tổ hợp**.
16. DOC-17 §2.2 *"at-rest bắt buộc"* vs DOC-13 S06 *"TBD ADR-004"*.
17. `Swagger.Enable: true` — template Prod không tắt; DOC-17 không nói Swagger UI trên Prod.
18. `openapi.yaml` `version: 0.2.0`; `servers: hrm.example.internal` trong khi §1 *"không bịa DNS"*.
19. Comment code còn Lark/W3: `HostLayerExtension.cs:26`, `MeController.cs:13`, `DevAuthController.cs:10,34`, `LoginPage.tsx:59`, `login-flow.spec.ts:8`, `w1-roles.sql:25`, `BoundedContextBoundaryTests.cs:51` — SAD tuyên bố soi từ code.
20. `LevDbContext` map 6 entity `emp` khi GRANT chỉ 4 cột một bảng; test comment *"nợ W3 trả bằng API"* — ADR-013 bỏ W3, nợ **mồ côi**.

## Trace gaps (đứt)

| Chuỗi | Đứt ở |
|---|---|
| NFR-005 → C1/C2 phép → `shared.emp_audit_log` | LEV không ghi (B1) |
| NFR-005 → đăng nhập/khoá/reset | IAM không map audit; `ActorIdpSubject` required (M5) |
| NFR-006 → LIF N+3 → adapter Git/CRM | chỉ outbox, không consumer (M13) |
| NFR-012b → runbook failover DB | DOC-17 không có replication/promote (M9) |
| NFR-S11 → cột mật khẩu tạm | DOC-11 §3.1 thiếu (M5) |
| DOC-12 §3 envelope / 422 | trái `BaseResponse` + `openapi.yaml` (M3) |
| DOC-17 §3 key JWT Prod | key/option không tồn tại; template còn Lark (M1) |
| ADR-012 → DOC-10 INT-001 · ADR-013 → DOC-14/16 | chưa sửa (B2) |

## Minor nợ cũ

Đóng: `AuthAudit`, `Contract/Education`, Notification "nếu tách", ADR-011 Proposed, `Mst`→`TaxId`, §8 `/v1`, OAS 3.0.1, §2 `/dev/login`. **Chưa đóng:** Business Owner ☐ (pass 1) · DOC-11 §4 TIM template (regression) · snapshot-vs-DB drift chưa ghi cảnh báo (nửa).

## Điều kiện mở lại gate

1. Đóng **B1** (a hoặc b — PGD chọn) và **B2** (sửa 3 DOC, hoặc DEC baseline theo file).
2. Đóng **M1** và **M3** — hai Major mà người làm theo DOC sẽ làm sai ngay.
3. Còn lại Major → backlog SA/BA/DevOps có owner; Minor gộp cuối phase.

**Không tự sửa DOC trong pass này** — chuyển owner. Hai lớp lỗi lặp ba pass liên tiếp: **tên cột viết tay không tồn tại** và **path thiếu `/v1`** — mọi DOC nhắc cột/path phải copy từ snapshot/OAS, không gõ.
