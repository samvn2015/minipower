# CR-002 — Password auth vào Production (mở lại phạm vi CR-001)

| Mục | Giá trị |
|-----|---------|
| **Status** | **Approved** — PGD 2026-09-15, theo yêu cầu khách (DEC-ARC-027 · DEC-DLV-026) |
| **Type** | modify |
| **Module(s)** | `identity` |
| **Ngày** | 2026-09-15 |
| **Người yêu cầu** | Khách hàng → PGD Dư Hùng |
| **Đảo ngược** | [CR-001](../CR-001-password-login/CR-001-password-login.md) `closed-rejected` (DEC-DLV-025) |
| **ADR nền** | [ADR-012](../../04-platform/DOC-09-adr/ADR-012-bo-sso-password-tu-quan.md) |

> Khung CR: [template DOC-18](../../../templates/DOC-18-change-request-register.md)

## Mô tả

Khách hàng yêu cầu **bỏ SSO**. Production đăng nhập bằng **username/password do HRM tự quản** — đúng phạm vi mà CR-001 đã liệt kê và **bị từ chối** ngày 2026-09-07.

CR-001 **không bị xoá**. Nó vẫn là hồ sơ của lần từ chối và lý do từ chối (bề mặt tấn công); CR-002 ghi nhận rằng thẩm quyền quyết định đã đổi, không phải lý do hôm đó sai.

## Phạm vi thực hiện

Lấy nguyên bảng *"Phạm vi Production — đã từ chối"* của CR-001, nay **được thực hiện**:

| Hạng mục | Nội dung | Trạng thái |
|---|---|---|
| Entity | `IdentityAccount` + `PasswordHash`, `PasswordSalt`, `PasswordChangedAtUtc`, `FailedAttempts`, `LockedUntilUtc` | ☐ |
| Migration | 1 migration mới, chạy bằng `hrm_migrator` (③ii) | ☐ |
| Endpoint | `POST /v1/iam/auth/login` · `change-password` · `reset-password` | ☐ |
| Hash | Thuật toán + tham số — **TBD DOC-13** (khuyến nghị Argon2id hoặc PBKDF2 ≥ 600k vòng) | ☐ |
| Chính sách | Độ dài/phức tạp/hạn dùng; khoá sau N lần sai; rate limit — **TBD DOC-13** (OQ-DLV-010) | ☐ |
| Reset | Quy trình HR/IT xác minh danh tính — **TBD** (OQ-DLV-011) | ☐ |
| MFA | TOTP tự làm hay không có — **TBD** (OQ-ARC-007 phát biểu lại) | ☐ |
| Audit | +action: LoginSucceeded, LoginFailed, AccountLocked, PasswordChanged, PasswordReset (NFR-005) | ☐ |
| **Không** dùng `POST /dev/login` cho Prod | Nó đối chiếu plaintext trong config — giữ 404 ngoài Development | ☑ đã đúng |

## Tiền đề trước khi viết code (readiness gate)

Theo `CLAUDE.md`, code chỉ mở khi tài liệu đủ. Hiện **thiếu 3 tiền đề chặn**:

| Tiền đề | Trạng thái | Owner |
|---|---|---|
| IAM DOC-06: FR đăng nhập/đổi/reset/khoá | ❌ chưa có | BA |
| IAM DOC-07: AC kèm **negative case** (sai N lần, khoá, hết hạn) | ❌ chưa có | BA |
| DOC-13: chính sách mật khẩu, hash, khoá, rate limit | ❌ chưa có | SA |
| DOC-11 §3.1 sửa | ☐ | SA |
| DOC-12 §2/§4 sửa | ☐ | SA |

**Không mở slice code cho tới khi ba dòng ❌ có nội dung.** Viết code xác thực trước khi có policy là viết hai lần.

## Affected documents

| DOC | Path | Change |
|-----|------|--------|
| ADR-007 | `04-platform/DOC-09-adr/ADR-007-idp-oidc.md` | Superseded bởi ADR-012 |
| ADR-001 | `04-platform/DOC-09-adr/ADR-001-stack-style-hosting.md` | §4 superseded |
| 11 | `04-platform/DOC-11-data-model/DOC-11-data-model.md` | §3.1 + §2 IAM |
| 12 | `04-platform/DOC-12-api-spec/*` | §2 bỏ cấm; §4 +3 endpoint; `openapi.yaml` |
| 13 | `04-platform/DOC-13-nfr.md` | +NFR bảo mật mật khẩu |
| 10 | `04-platform/DOC-10-integration-specification.md` | INT-001 (IdP) bỏ |
| 17 | `04-platform/DOC-17-deployment-guide.md` | Bỏ Lark; +signing key Prod |
| 06 | `03-modules/identity/DOC-06-srs.md` | +IAM-FR |
| 07 | `03-modules/identity/DOC-07-acceptance-criteria.md` | +IAM-AC |
| 16 | `03-modules/identity/DOC-16-test-strategy.md` | +TC, bỏ TC Lark |

## Impact

| Dimension | Impact |
|-----------|--------|
| Scope | `identity` + platform (ADR, NFR, API, deploy) |
| Bảo mật | **Tăng bề mặt tấn công** — lý do CR-001 bị từ chối, vẫn đúng |
| Regression | Toàn bộ TC IAM giả định SSO; **UAT §1 chạy lại** |
| Prod blocker | **OQ-DLV-001 (Lark JWKS) đóng** — hết chặn Prod |
| Timeline | TBD — sau khi có DOC-06/07/13 |
| Baseline | DOC-11/12 vừa ký v0.2/v0.3 (DEC-ARC-021) **lệch lại** — phải v0.3/v0.4 trước khi baseline |

## Liên quan

- DEC-ARC-027 · DEC-DLV-026 · DEC-DLV-025 *(bị đảo)*
- OQ-DLV-001 *(đóng)* · OQ-DLV-011, OQ-DLV-010 *(mới)* · OQ-ARC-007 *(phát biểu lại)*
- RK-08: văn bản khách chưa có trong `assets/`
