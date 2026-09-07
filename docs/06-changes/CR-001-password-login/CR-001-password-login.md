# CR-001 — Đăng nhập username/password

| Mục | Giá trị |
|-----|---------|
| **Status** | **Closed — Từ chối phần Production** (DEC-DLV-025 · 2026-09-07) |
| **Type** | modify |
| **Module(s)** | `identity` |
| **Ngày** | 2026-09-07 |
| **Người yêu cầu** | Dư Hùng (PGD) |
| **Soạn** | trợ lý |

> Khung CR: [template DOC-18](../../../templates/DOC-18-change-request-register.md)

## Mô tả

PGD yêu cầu HRM có luồng đăng nhập **username + password**, thay vì chỉ SSO.

Đã hiện thực **phần DEV/UAT** (xem [Đã làm](#đã-làm-devuat--không-cần-cr)). CR này chỉ để quyết **phần Production**: có đưa password auth thành cơ chế đăng nhập thật hay không.

## Vì sao cần CR

Ba tài liệu đã **Chốt** cấm luồng này ở Production:

| Nguồn | Nội dung |
|-------|----------|
| ADR-007 (Accepted) | IdP OIDC — Lark SSO |
| ADR-001 §4 (Accepted) | *"mọi truy cập Web/Mobile/HR **qua SSO**"* |
| DOC-12 §2 | *"**Cấm:** `POST /auth/login` với password"* |
| DOC-11 §3.1 | *"**Không** lưu password hash (SSO)"* |

## Đã làm (DEV/UAT — không cần CR)

Không đụng bốn ràng buộc trên vì endpoint **404 ngoài Development** và **không chạm DB**.

| Thay đổi | Path |
|---|---|
| `POST /dev/login` — đối chiếu `DevAuth:Accounts`, cấp JWT local | `hrm/backend/src/Hrm.Host/Controllers/DevAuthController.cs` |
| Tài khoản DEV (`admin@gmail.com` …) | `hrm/backend/src/Hrm.Host/appsettings.Development.json` |
| Form username/password | `hrm/frontend/src/pages/LoginPage.tsx` · `src/api/client.ts` |
| Autotest 3 API + 3 UI | `hrm/autotest/tests/api/dev-login.spec.ts` · `tests/ui/login-flow.spec.ts` |

Password nằm trong file cấu hình DEV, **không** có `PasswordHash` trên `IdentityAccount`, **không** migration.

## Phạm vi Production — **đã từ chối**

> PGD quyết **không** đưa password auth vào Production (DEC-DLV-025). Bảng dưới giữ lại
> làm hồ sơ phương án bị loại; **không** hạng mục nào được thực hiện.

| Hạng mục | Nếu chấp thuận |
|---|---|
| Entity | `IdentityAccount` + `PasswordHash`, `PasswordSalt`, `PasswordChangedAtUtc`, `FailedAttempts`, `LockedUntilUtc` |
| Migration | 1 migration mới trên DB `hrm` |
| Endpoint | `POST /v1/iam/auth/login`, `POST /v1/iam/auth/change-password`, `POST /v1/iam/auth/reset-password` |
| Chính sách | Độ dài / độ phức tạp / hạn dùng mật khẩu; khoá tài khoản sau N lần sai; rate limit |
| Hash | Thuật toán + tham số (khuyến nghị Argon2id hoặc PBKDF2 ≥ 600k vòng) |
| Audit | Bổ sung action đăng nhập / đổi mật khẩu / khoá vào `EmpAuditLog` (NFR-005) |
| Song song SSO | Giữ Lark SSO đồng thời, hay thay hẳn? |

## Affected documents — **không sửa DOC nào**

> Vì Production bị từ chối, **toàn bộ bảng dưới không áp dụng**. ADR-007, ADR-001 §4,
> DOC-11 §3.1 và DOC-12 §2 **giữ nguyên hiệu lực**.

| DOC | Path | Change *(không thực hiện)* |
|-----|------|--------|
| ADR-007 | `04-platform/DOC-09-adr/ADR-007-idp-oidc.md` | Superseded hoặc bổ sung ngoại lệ |
| ADR-001 | `04-platform/DOC-09-adr/ADR-001-stack-style-hosting.md` | §4 SSO — sửa |
| 11 | `04-platform/DOC-11-data-model/DOC-11-data-model.md` | §3.1 bỏ *"Không lưu password hash"*; thêm thuộc tính |
| 12 | `04-platform/DOC-12-api-spec/DOC-12-api-specification.md` | §2 bỏ dòng cấm; §4 thêm endpoint |
| 12 | `04-platform/DOC-12-api-spec/openapi.yaml` | thêm path |
| 13 | `04-platform/DOC-13-nfr.md` | NFR bảo mật: chính sách mật khẩu, khoá, rate limit |
| 06 | `03-modules/identity/DOC-06-srs.md` | +IAM-FR-xxx |
| 07 | `03-modules/identity/DOC-07-acceptance-criteria.md` | +IAM-AC-xxx (gồm negative case) |
| 16 | `03-modules/identity/DOC-16-test-strategy.md` | +TC đăng nhập sai / khoá / đổi mật khẩu |

## Impact

| Dimension | Impact |
|-----------|--------|
| Scope | Module `identity`; lan sang platform (ADR, NFR, API) |
| Bảo mật | **Tăng bề mặt tấn công** — credential stuffing, brute force, lộ hash. SSO vốn đẩy rủi ro này sang IdP |
| Regression | Mọi TC IAM đang giả định SSO; UAT §1 phải chạy lại |
| Timeline | Ước lượng sau khi PGD chốt phạm vi — chưa đưa số |
| Baseline | `02-baseline/` đang **BLOCK** (doc-review 2026-09-07). CR này làm DOC-11/12 lệch thêm |

## Liên quan

- Doc-review gate: `memory/delivery/doc-review-2026-09-07-doc11-12.md` — verdict ⛔ BLOCK
- OQ-DLV-001 (Lark JWKS Prod) · DEC-DLV-011 (bypass JWKS DEV/UAT)
- OQ-DLV-007 — chính sách MFA còn mở; nếu bỏ SSO thì MFA phải tự làm

## Quyết định

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor (A) | Mr. Dư Hùng, PGD | 2026-09-07 | ☑ **Chỉ giữ DEV/UAT** — từ chối Production (DEC-DLV-025) |
| SA | | | — *(không cần: không sửa DOC nào)* |
| Business Owner | Ban HR | | — *(không cần: không đổi hành vi Prod)* |

## Việc còn lại sau quyết định

- [ ] DOC-17 §cutover: thêm mục kiểm `ASPNETCORE_ENVIRONMENT != Development` trên Prod — cấu hình sai sẽ mở `POST /dev/login` và `GET /dev/token`. Owner: SA / DevOps.
