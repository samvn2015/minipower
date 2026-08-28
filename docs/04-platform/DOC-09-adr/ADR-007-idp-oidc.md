# ADR-007 — IdP SSO (OIDC, không host trong HRM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Accepted** (DEC-ARC-009 · PGD Dư Hùng) |
| 0.2 | 2026-08-28 | Trịnh Yên (bổ sung SA) | **Accepted** — bổ sung brand Lark (DEC-DLV-010 · DEC-ARC-015) |

**Michael Nygard ADR** · Đảo = ADR mới. **Không** sửa ADR-002.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-08-26 |
| **Date** | 2026-08-26 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | IT/IAM SH-006 · SA |
| **Informed** | Dev · BA |

---

### Bối cảnh

OQ-ARC-004: sản phẩm IdP + OIDC vs SAML. Anh: **chốt IdP**. DOC-10 INT-001 để TBD. ADR-002 Accepted mô tả cả OIDC và nhánh SAML→JWT.

Không có tên vendor trên BRD. **Không** bịa Keycloak / Entra / Google.

### Quyết định

1. **HRM không triển khai IdP riêng.** IdP = **Lark** (Feishu) — hệ SSO doanh nghiệp cho domain **@lhqglobal.vn** (DEC-DLV-010 · PGD 2026-08-28). HRM là *relying party*.
2. **MVP chỉ OIDC** (Authorization Code; mobile **PKCE**). Discovery: issuer + JWKS do IT cung cấp khi implement.
3. **SAML không vào MVP.** Nhánh SAML trong ADR-002 **không dùng** đến khi CR + ADR mới.
4. **Brand IdP:** **Lark** (bổ sung v0.2). Issuer URL / tenant / region do IT (OQ-DLV-001). **Không** bịa discovery URL trên tài liệu này.
5. **Phương thức đăng nhập MVP:** Google · Apple · mail **@lhqglobal.vn** — cấu hình trên **Lark / GW** (IT); HRM validate **một** JWT issuer Lark (khuyến nghị).
6. Client **không** gọi `/login` password trên API HRM (cấm trong DOC-12).

### Lý do

Chốt cổng xác thực đủ để viết OpenAPI. Tách brand để khỏi bịa vendor. Một protocol → GW đơn giản, khớp NFR-003.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **P — IdP Cty hiện có + OIDC-only MVP** *(chọn)* | Không dựng IdP; khớp ADR-002 JWT | IT phải chỉ định issuer |
| Q — Tự host Keycloak trong HRM | Chủ động | **Loại** — bịa sản phẩm + vận hành IdP |
| R — SAML-only MVP | Khớp AD FS cũ | **Loại** — phức tạp MS; anh chốt IdP theo hướng OIDC GW |
| S — Để mở OIDC+SAML | Linh hoạt | **Loại** — anh chốt; dual-stack chậm 2027 |

### Hệ quả

OQ-ARC-004: protocol **đóng**; brand **Lark** (DEC-DLV-010). Còn *issuer URL + JWKS* do IT (OQ-DLV-001). DOC-12: security OAuth2/OIDC, không POST password. Map `sub` OIDC → IAM `IdpSubject`; roles SoT PostgreSQL (ADR-002).

Đảo OIDC-only hoặc tự host IdP = ADR mới.

### NFR

NFR-003 cùng IdP web/mobile · INT-001 · ADR-001/002.

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Accepted** (DEC-ARC-009) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-28 | **Accepted** bổ sung v0.2 Lark (DEC-DLV-010) |
| SA | | 2026-08-26 | Soạn → PGD chốt |
