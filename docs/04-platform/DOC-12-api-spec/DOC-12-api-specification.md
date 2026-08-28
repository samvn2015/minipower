# DOC-12 — Đặc tả API (khung OpenAPI)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Chốt** (khung OAS · DEC-ARC-010) |

**OAS 3.0.3** · DOC-08/10/11 **Chốt** · ADR-001/002/007 **Accepted**.  
**SoT machine:** [`openapi.yaml`](openapi.yaml). Nợ: Base URL thật; issuer OIDC; UUID vs long; full body FR. **Không** tự DOC-17. **Chưa** `02-baseline/`.

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

**Cấm:** `POST /auth/login` với password.

## 3. Quy ước chung

| Convention | Value |
|------------|-------|
| Content-Type | `application/json` |
| Date | ISO 8601 |
| Pagination | `page`, `size` |
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

## 4. Danh mục endpoint (khung)

Prefix Gateway. `{id}` = string (kiểu PK TBD DOC-11).

| Method | Path | Summary | Auth | Trace |
|--------|------|---------|------|-------|
| GET | `/iam/me` | User + roles sau SSO | Bearer | IAM, ADR-002 |
| GET | `/emp/employees/{id}` | Hồ sơ | Bearer | EMP |
| PATCH | `/emp/employees/{id}` | Sửa hồ sơ (rule FR) | Bearer | EMP |
| GET | `/lev/balances` | Quỹ phép | Bearer | LEV |
| POST | `/lev/requests` | Nộp đơn | Bearer | LEV |
| POST | `/lev/requests/{id}/c1` | LM C1 | Bearer | LEV |
| POST | `/lev/requests/{id}/c2` | HR C2 trừ quỹ | Bearer | LEV |
| POST | `/tim/imports` | Upload Excel | Bearer HR | TIM INT-003 |
| POST | `/tim/periods/{ym}/close` | Chốt công | Bearer HR | TIM |
| GET | `/pay/payslips/me` | Phiếu mình | Bearer NV | PAY NFR-002 |
| GET | `/pay/payslips/{id}` | Phiếu (HR / chính chủ) | Bearer | PAY 403 LM |
| POST | `/pay/periods/{ym}/run` | Tính lương | Bearer HR | PAY |
| GET | `/prb/cases/{employeeId}` | Hồ sơ TV | Bearer | PRB |
| POST | `/prb/cases/{employeeId}/propose` | LM đề xuất | Bearer LM | PRB |
| POST | `/prb/cases/{employeeId}/decide` | HR chốt 3 mã | Bearer HR | PRB-AC-009 |
| GET | `/lif/cases/{employeeId}` | On/off | Bearer | LIF |
| POST | `/lif/cases/{employeeId}/locks` | Trigger N+3 (job/IT) | Bearer hệ thống | INT-004/005 |

Chi tiết schema field = FR + catalog — **không** liệt kê hết trên DOC-12 v0.1.

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
| `/iam/me` | ADR-002, INT-001 |
| `/lev/*` | LEV DOC-06 |
| `/tim/imports` | INT-003, NFR-001 |
| `/pay/payslips/*` | NFR-002 |
| `/prb/.../decide` | PRB-FR-009 |
| `/lif/.../locks` | INT-004, INT-005 |
| *(cấm)* CRM sales | INT-006 |

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** khung v0.1 (DEC-ARC-010) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
| BA (R) | Trịnh Yên | 2026-08-26 | Soạn |
