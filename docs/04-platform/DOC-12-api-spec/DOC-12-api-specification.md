# DOC-12 — Đặc tả API (khung OpenAPI)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (khung OAS · DEC-ARC-010) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — §4 sinh lại từ Swagger runtime, đóng doc-review **B3** (PGD ký · DEC-ARC-021) |
| 0.3 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — §4.3 đánh dấu hợp đồng liên service theo **ADR-005** (DEC-ARC-026) |
| 0.4 | 2026-09-16 | soạn nháp SA (trợ lý) | **Draft — chờ PGD ký** — theo **ADR-012** (bỏ SSO, thêm auth endpoint — chưa code) và **ADR-013** (một host, không Gateway; §4.3 hết là hợp đồng liên service); §3 phân trang đã hiện thực (S1) |

**OAS 3.0.1** *(Swashbuckle sinh — khớp dòng đầu `openapi.yaml`)* · DOC-08 v0.4 · DOC-11 · **ADR-012** · **ADR-013** · ADR-005.  
**SoT machine:** [`openapi.yaml`](openapi.yaml) — **sinh từ Swagger runtime 2026-09-07**, round-trip đã verify. ⚠️ **Chưa sinh lại sau S1 phân trang (2026-09-15)** — 4 endpoint list đã có `page`/`size` + `X-Total-Count` mà file chưa phản ánh; nợ sinh lại. Nợ khác: Base URL thật; auth endpoint ADR-012 (chưa code); full body FR. *(kiểu PK đã chốt **Guid** trong code — xem §3.)* **Không** tự DOC-17. **Chưa** `02-baseline/`.

---

## 1. Tổng quan

| Mục | Giá trị |
|-----|---------|
| **API title** | HRM API *(Swagger hiện in "HRM Gateway API" — sửa title khi sinh lại OAS)* |
| **Version** | v1 |
| **Base URL** | `{public-host}/v1` — **TBD** DOC-17 (không bịa DNS) |
| **OpenAPI file** | `openapi.yaml` |

Mọi path công khai đi **LBS → `Hrm.Host`** — **một** host, **không** Gateway (ADR-013). Host không expose Internet trực tiếp; TLS tại LBS. Tiền tố `/v1/{iam|emp|lev|tim|pay|prb|lif}/…` là ranh giới bounded context trong cùng process, **không** phải route tới service khác nhau.

## 2. Xác thực & Phân quyền

| Method | Mô tả |
|--------|-------|
| Bearer JWT | `Authorization: Bearer {access_token}` — **JWT do HRM ký** (ADR-012 §4), host validate `ValidateIssuerSigningKey/Issuer/Audience` fail-closed; token không có `sub` → 401 (`RequireIdpSubject`). **Không** IdP ngoài, **không** JWKS. |
| Đăng nhập | `POST /v1/iam/auth/login` (username + password → JWT) · `POST /v1/iam/auth/change-password` · `POST /v1/iam/auth/reset-password` (HR/IT) — **ADR-012 §3, chưa code** (CR-002 gated: DOC-13 policy, IAM DOC-06/07). Chưa có trong `openapi.yaml`. |
| OAuth2 / OIDC | **Không còn** (ADR-012 supersede ADR-007). Mobile dùng cùng `POST /v1/iam/auth/login`. |
| API Key | **Không** cho NV/HR. |

**RBAC:** IAM DB là SoT role (ADR-013 §7a). 403 màn HR / lương — NFR-002/004.

**Cấm trên Production:** `POST /dev/login` và `GET /dev/token`. Hai path này đối chiếu mật khẩu **plaintext trong `appsettings.Development.json`**, chỉ để E2E local, nằm sau guard `IsDevelopment()` → **404 ngoài Development** (ADR-012 §5 · RK-09 · DOC-17 §cutover kiểm `ASPNETCORE_ENVIRONMENT`). Chúng **không** phải và **không được** trở thành luồng đăng nhập thật — luồng thật là `POST /v1/iam/auth/login` với hash trong DB.

> Lịch sử: v0.3 ghi *"Cấm `POST /auth/login` với password trên Production"* theo [CR-001](../../06-changes/CR-001-password-login/CR-001-password-login.md) `closed-rejected` (DEC-DLV-025). Đảo bởi [CR-002](../../06-changes/CR-002-password-auth-production/CR-002-password-auth-production.md) Approved (DEC-DLV-026) theo yêu cầu khách — ADR-012.

## 3. Quy ước chung

| Convention | Value |
|------------|-------|
| Content-Type | `application/json` |
| Date | ISO 8601 |
| PK | **Guid** — chốt trong code, 56 migration đã áp (đóng TBD của v0.1) |
| Pagination | `page` (từ 1), `size` (mặc định **200**, tối đa **500**, clamp) → body là **mảng** (không đổi shape), tổng ở header **`X-Total-Count`**. Đã hiện thực S1 (2026-09-15) trên `GET /v1/emp/employees`, `GET /v1/lif/onboarding`, `GET /v1/prb/reminders`, `GET /v1/prb/evaluations`; `audit-logs` cap 200 sẵn. |
| Correlation | trace-id OpenTelemetry trong host. `X-Request-Id` **chưa hiện thực** — không có GW gắn |

Lỗi:

```json
{
  "error": {
    "code": "FORBIDDEN",
    "message": "Human readable",
    "details": []
  }
}
```

| Code | Usage |
|------|-------|
| 200 / 201 | OK / created |
| 400 | Validation |
| 401 | Không/hết hạn JWT |
| 403 | Sai role / cô lập lương |
| 404 | Not found |
| 409 | Conflict (unique CCCD, chốt kỳ) |
| 500 | Internal |

## 4. Danh mục endpoint

`openapi.yaml` là **nguồn sự thật máy đọc**, sinh trực tiếp từ Swagger runtime của `Hrm.Host` (2026-09-07). Không duy trì danh mục tay song song — bản v0.1 làm vậy và lệch 16/82.

**Quy mô hiện tại: 82 path · 88 operation · 34 schema.**

| Nhóm | Path | Operation | Ghi chú |
|------|------|-----------|---------|
| `/v1/emp` | 11 | 13 | Hồ sơ, catalog, đề nghị đổi LM |
| `/v1/iam` | 6 | 6 | `me`, quản trị account |
| `/v1/lev` | 12 | 12 | Quỹ phép, đơn, C1/C2 approve+reject |
| `/v1/lif` | 15 | 17 | On/offboarding, checklist, khóa N+3 |
| `/v1/pay` | 13 | 13 | Kỳ lương, phiếu, phụ cấp, export |
| `/v1/prb` | 10 | 10 | Case TV, đánh giá, quyết định, master |
| `/v1/tim` | 12 | 14 | Template, import, chốt/mở kỳ, thiết bị |
| `/api` | 1 | 1 | `ping` |
| `/dev` | 2 | 2 | **DEV/UAT only** — 404 ngoài Development |

### 4.1 Sai lệch đã sửa so với v0.1

v0.1 mô tả path **không trùng chữ** với route thật. Ghi lại để người đọc bản cũ không hiểu nhầm:

| v0.1 (sai) | Route thật |
|------------|-----------|
| `GET /lev/balances` | `GET /v1/lev/leave-balances/me` |
| `POST /lev/requests` | `POST /v1/lev/leave-requests` |
| `POST /lev/requests/{id}/c1` | `POST /v1/lev/leave-requests/{id}/c1/approve` **và** `/c1/reject` |
| `POST /lev/requests/{id}/c2` | `POST /v1/lev/leave-requests/{id}/c2/approve` **và** `/c2/reject` |
| `GET /prb/cases/{employeeId}` | `GET /v1/prb/cases` — list, **không** tham số employeeId |
| `POST /prb/cases/{id}/propose\|decide` | `POST /v1/prb/evaluations/{employeeId}/propose\|decide` |

### 4.3 Guard TIM↔PAY (ADR-005) — **không còn là hợp đồng liên service**

v0.3 đánh dấu hai endpoint dưới là *hợp đồng giữa hai service* cho W3. **ADR-013 bỏ W3**: guard đọc chéo chạy **trong process** qua interface Application, không qua HTTP. Hai endpoint **giữ** cho client, là API thường:

| Endpoint | Client dùng | Guard tương ứng (trong process) |
|---|---|---|
| `GET /v1/tim/periods/{ym}` | HR xem trạng thái kỳ công | PAY chặn tính lương khi kỳ công chưa `Closed` (PAY-FR-001) |
| `GET /v1/pay/periods/{ym}` | HR xem trạng thái kỳ lương | TIM chặn mở khoá kỳ công khi lương đã chạy |

**Fail-closed** giữ nguyên: guard lỗi ⇒ từ chối, không đoán. Không có hop mạng ⇒ **không** timeout/retry (OQ-ARC-018 đóng). Đổi shape hai endpoint này là breaking change **với client**, như mọi endpoint khác.

### 4.2 Nợ còn lại

- **`openapi.yaml` chưa sinh lại** sau S1 phân trang — 4 endpoint list thiếu `page`/`size`/`X-Total-Count`. Sinh lại từ Swagger runtime, đổi title.
- **Auth endpoint ADR-012** (`/v1/iam/auth/*`) chưa code, chưa có trong OAS — vào cùng CR-002.
- `/dev/*` xuất hiện trong OAS vì Swagger sinh từ runtime Development. Trên Prod hai path này trả 404.

## 5. Ghi chú endpoint nhạy cảm

- `decide` PRB: **403** nếu LM/NV (không phải HR).
- `payslips`: LM **403** lương cấp dưới.
- `imports` / `close` / `run`: chỉ HR.
- Không path `/crm-sales/**`.

## 6. Khung OpenAPI

→ [`openapi.yaml`](openapi.yaml)

## 7. Giới hạn tốc độ & SLA

| Limit | Value |
|-------|-------|
| Rate | **TBD** (không bịa 100 req/min) |
| Timeout LBS → host | **TBD** DOC-17 |
| Rate limit login | **TBD** DOC-13 (OQ-DLV-010) — bắt buộc trước khi code `POST /v1/iam/auth/login` |
| NFR-001 | 1000 dòng import/tính đo sau **LBS** (không có GW) |

## 8. Truy vết

| Endpoint | FR/NFR/INT |
|----------|------------|
| `GET /v1/iam/me` | ADR-012 · ADR-013 §7a |
| `POST /v1/iam/auth/login` · `change-password` · `reset-password` *(chưa code)* | ADR-012 §3 · CR-002 |
| `GET /v1/lev/leave-balances/me` · `POST /v1/lev/leave-requests` | LEV DOC-06 |
| `POST /v1/lev/leave-requests/{id}/c1/approve`\|`/c1/reject` | LEV C1 |
| `POST /v1/lev/leave-requests/{id}/c2/approve`\|`/c2/reject` | LEV C2 — trừ quỹ |
| `POST /v1/tim/imports` · `/v1/tim/imports/{id}/commit` | INT-003, NFR-001 |
| `GET /v1/pay/payslips/me` · `GET /v1/pay/payslips/{id}` | **NFR-002** |
| `POST /v1/prb/evaluations/{employeeId}/propose`\|`/decide` | PRB-FR-009 |
| `POST /v1/lif/offboarding/{id}/locks` · `/v1/lif/offboarding/jobs/nplus3-locks` | INT-004, INT-005 |
| `POST /dev/login` · `GET /dev/token` | DEC-DLV-011 · ADR-012 §5 *(DEV only — 404 ngoài Development)* |
| *(cấm)* CRM sales | INT-006 |

> Path lấy từ [`openapi.yaml`](openapi.yaml) — kiểm 2026-09-07. Không gõ tay lại; sai lệch §8 là nguyên nhân finding regression Major.

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** khung v0.1 (DEC-ARC-010) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | **2026-09-07** | **Chốt v0.2** (DEC-ARC-021) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | | ☐ **ký v0.4** · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
