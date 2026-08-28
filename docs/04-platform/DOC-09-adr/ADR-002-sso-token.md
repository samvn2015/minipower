# ADR-002 — Token SSO tại Gateway và IAM

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Accepted** (DEC-ARC-007 · PGD Dư Hùng) |

**Michael Nygard ADR** · Đảo = ADR mới, không sửa file này.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-08-26 |
| **Date** | 2026-08-26 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · IAM/IT (SH-006) |
| **Informed** | Dev · BA Trịnh Yên |

---

### Bối cảnh

ADR-001 Accepted: mọi user qua SSO; IAM **không** thay IdP; web = mobile. DOC-10 INT-001 Chốt: OIDC **hoặc** SAML sản phẩm **TBD**. Cần khóa **cách mang phiên** vào microservices, không chờ chọn vendor IdP.

### Quyết định

1. **Ưu tiên OIDC:** client (web, mobile PKCE) nhận **access token JWT** từ IdP. API Gateway **validate** JWT (chữ ký JWKS, `iss`, `aud`, `exp`). Không nhận token trên query string.
2. **SAML-only (nếu IdP không OIDC):** ACS trên Gateway; GW **đổi** assertion → **JWT nội bộ** (ký GW/IAM) rồi gọi MS như nhánh OIDC. Không để MS parse SAML.
3. **SoT role HRM = IAM DB** (map `sub`/email Cty → user EMP). Claim nhóm IdP **không** thay 403 màn HR / cô lập lương.
4. **MS không tin client:** chỉ tin request đã qua GW (header `Authorization: Bearer` đã validate). Chi tiết mTLS service-to-service **TBD**, không chặn ADR này.
5. **Cấm:** resource-owner password; user/pass local thay SSO; refresh token trên URL; lưu secret Git trong JWT.
6. **Phiên hết hạn:** 401 → client refresh qua IdP (hoặc re-login). Thời `exp` **không** bịa phút trên ADR này.
7. **MFA:** không bắt (OQ-ARC-007).

### Lý do

Tách IdP (xác thực) khỏi IAM (ủy quyền HRM). Một kiểu Bearer vào mọi service. SAML không lan vào PAY/TIM.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **K — JWT OIDC tại GW + IAM SoT role; SAML→JWT nội bộ** *(chọn)* | Khớp INT-001; MS đơn giản | IdP vendor TBD |
| L — Cookie session monolith | Đơn | **Loại** — trái ADR-001 MS |
| M — Mỗi MS tự validate SAML | — | **Loại** — phức tạp, lệch NFR-003 |
| N — API key user | — | **Loại** — không SSO |

### Hệ quả

DOC-12: security scheme Bearer JWT. DOC-11: entity `IdentityAccount` khóa `idp_subject`. Sửa = ADR mới.

**Rủi ro:** `exp`/refresh phút chưa chốt; clock skew IdP.

### Tuân thủ NFR

| NFR | Impact |
|-----|--------|
| NFR-003 | Cùng JWT/IdP web+mobile |
| NFR-002/004 | Role từ IAM sau token hợp lệ |
| NFR-007 | Token không chứa hook CRM sales |

### Truy vết

ADR-001 · DOC-10 INT-001 · IAM DOC-06 · OQ-ARC-004 (vendor)

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Accepted** (DEC-ARC-007) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
