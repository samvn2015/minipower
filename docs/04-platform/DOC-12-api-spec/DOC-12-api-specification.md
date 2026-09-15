# DOC-12 — Đặc tả API (khung OpenAPI)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (khung OAS · DEC-ARC-010) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — §4 sinh lại từ Swagger runtime, đóng doc-review **B3** (PGD ký · DEC-ARC-021) |
| 0.3 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — §4.3 đánh dấu hợp đồng liên service theo **ADR-005** (DEC-ARC-026) |

**OAS 3.0.1** *(Swashbuckle sinh — khớp dòng đầu `openapi.yaml`)* · DOC-08/10/11 · ADR-001/002/007 **Accepted**.  
**SoT machine:** [`openapi.yaml`](openapi.yaml) — **sinh từ Swagger runtime 2026-09-07**, round-trip đã verify. Nợ: Base URL thật; issuer OIDC; full body FR. *(kiểu PK đã chốt **Guid** trong code — xem §3.)* **Không** tự DOC-17. **Chưa** `02-baseline/`.

---

## 1. Tổng quan

| Mục | Giá trị |
|-----|---------|
| **API title** | HRM Gateway API |
| **Version** | v1 |
| **Base URL** | `{public-host}/v1` — **TBD** DOC-17 (không bịa DNS) |
| **OpenAPI file** | `openapi.yaml` |

Mọi path công khai đi **LBS → Gateway**. Service phía sau không expose Internet.

## 2. Xác thực & Phân quyền

| Method | Mô tả |
|--------|-------|
| Bearer JWT | `Authorization: Bearer {access_token}` — OIDC IdP Cty (ADR-007). GW validate JWKS. |
| OAuth2 | Authorization Code + PKCE (mobile). **Không** Resource Owner Password. |
| API Key | **Không** cho NV/HR. |

**RBAC:** IAM DB sau token (ADR-002). 403 màn HR / lương — NFR-002/004.

**Cấm:** `POST /auth/login` với password **trên Production**.

> **Ngoại lệ DEV/UAT — có trace:** `GET /dev/token` và `POST /dev/login` (nhận `username`/`password`) tồn tại để E2E local khi chưa có Lark JWKS (DEC-DLV-011). Cả hai nằm sau guard `IsDevelopment()` → **404 ngoài Development**, không lưu password trong DB (DOC-11 §3.1 giữ nguyên). Đưa password auth vào Production đã bị **từ chối** — [CR-001](../../06-changes/CR-001-password-login/CR-001-password-login.md) `closed-rejected`, DEC-DLV-025.

## 3. Quy ước chung

| Convention | Value |
|------------|-------|
| Content-Type | `application/json` |
| Date | ISO 8601 |
| PK | **Guid** — chốt trong code, 56 migration đã áp (đóng TBD của v0.1) |
| Pagination | `page`, `size` — **quy định, chưa hiện thực** (code-review S1) |
| Correlation | `X-Request-Id` (GW) |

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

### 4.3 Hợp đồng liên service (ADR-005)

Hai endpoint dưới **không chỉ** phục vụ client — từ W3 (ADR-011) chúng là **hợp đồng giữa hai service**, dùng làm guard đọc chéo thay cho saga:

| Endpoint | Ai gọi | Dùng làm gì |
|---|---|---|
| `GET /v1/tim/periods/{ym}` | **PAY** | Chặn tính lương khi kỳ công chưa `Closed` (PAY-FR-001) |
| `GET /v1/pay/periods/{ym}` | **TIM** | Chặn mở khoá kỳ công khi lương đã chạy |

**Fail-closed:** lỗi hoặc timeout ⇒ **từ chối** thao tác, không đoán trạng thái. Đổi shape hai endpoint này là **breaking change liên service**, không phải sửa API nội bộ. Timeout/retry: `OQ-ARC-018`.

### 4.2 Nợ còn lại

- **Phân trang chưa có ở bất kỳ endpoint nào** — §3 quy định `page`/`size`, code chưa hiện thực (code-review S1). Chưa đưa vào `openapi.yaml` vì chưa có trong runtime.
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
| Timeout GW | **TBD** |
| NFR-001 | 1000 dòng import/tính đo sau LBS+GW |

## 8. Truy vết

| Endpoint | FR/NFR/INT |
|----------|------------|
| `GET /v1/iam/me` | ADR-002, INT-001 |
| `GET /v1/lev/leave-balances/me` · `POST /v1/lev/leave-requests` | LEV DOC-06 |
| `POST /v1/lev/leave-requests/{id}/c1/approve`\|`/c1/reject` | LEV C1 |
| `POST /v1/lev/leave-requests/{id}/c2/approve`\|`/c2/reject` | LEV C2 — trừ quỹ |
| `POST /v1/tim/imports` · `/v1/tim/imports/{id}/commit` | INT-003, NFR-001 |
| `GET /v1/pay/payslips/me` · `GET /v1/pay/payslips/{id}` | **NFR-002** |
| `POST /v1/prb/evaluations/{employeeId}/propose`\|`/decide` | PRB-FR-009 |
| `POST /v1/lif/offboarding/{id}/locks` · `/v1/lif/offboarding/jobs/nplus3-locks` | INT-004, INT-005 |
| `POST /dev/login` · `GET /dev/token` | DEC-DLV-011 · CR-001 *(DEV/UAT only)* |
| *(cấm)* CRM sales | INT-006 |

> Path lấy từ [`openapi.yaml`](openapi.yaml) — kiểm 2026-09-07. Không gõ tay lại; sai lệch §8 là nguyên nhân finding regression Major.

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** khung v0.1 (DEC-ARC-010) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | **2026-09-07** | **Chốt v0.2** (DEC-ARC-021) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
