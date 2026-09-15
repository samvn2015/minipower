# ADR-012 — Bỏ SSO; HRM tự quản đăng nhập username/password

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-09-15 | soạn nháp SA (trợ lý) | **Accepted** — PGD chốt theo yêu cầu khách (DEC-ARC-027) |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-09-15 (DEC-ARC-027). Đảo = ADR mới. |
| **Date** | 2026-09-15 |
| **Deciders** | Mr. Dư Hùng, PGD (A) · **khách hàng** (nguồn yêu cầu) |
| **Consulted** | SA · Dev (SH-010) · IT |
| **Informed** | Ban HR · QC · BA |
| **Supersedes** | [ADR-007](ADR-007-idp-oidc.md) **toàn bộ** · [ADR-001](ADR-001-stack-style-hosting.md) **§4** (*"mọi truy cập Web/Mobile/HR qua SSO"*) |
| **Đảo ngược** | [CR-001](../../06-changes/CR-001-password-login/CR-001-password-login.md) `closed-rejected` (DEC-DLV-025, 2026-09-07) → mở lại thành **CR-002** |

---

### Bối cảnh

Từ 2026-08-26 hệ thống đi theo SSO: ADR-007 chọn IdP ngoài (Lark, `@lhqglobal.vn`), ADR-001 §4 bắt mọi truy cập qua SSO, DOC-12 §2 **cấm** `POST /auth/login` với password, DOC-11 §3.1 **không** lưu password hash. Ngày 2026-09-07, CR-001 đề nghị password auth cho Production và **bị từ chối** (DEC-DLV-025) — chỉ giữ `POST /dev/login` cho DEV/UAT.

Ngày 2026-09-15, **khách hàng yêu cầu bỏ SSO**. PGD chốt. Đây là yêu cầu thứ ba từ khách cùng thẩm quyền, sau microservices (OQ-ARC-008) và bỏ DR/DC (DEC-ARC-018).

Điểm đáng ghi: Lark là IdP của **chính khách** (domain `@lhqglobal.vn`). Khách bỏ SSO trên hệ thống của họ là quyền của họ — nhưng nên có văn bản trong `assets/`, hiện **chưa có**.

### Quyết định

1. **Production đăng nhập bằng username/password do HRM tự quản.** Không IdP ngoài, không OIDC, không JWKS.
2. **HRM lưu và xác minh mật khẩu.** `IdentityAccount` thêm `PasswordHash`, `PasswordSalt` (hoặc dùng thuật toán tự mang salt), `PasswordChangedAtUtc`, `FailedAttempts`, `LockedUntilUtc`. Đảo DOC-11 §3.1.
3. **Endpoint:** `POST /v1/iam/auth/login` (cấp JWT), `POST /v1/iam/auth/change-password`, `POST /v1/iam/auth/reset-password` (HR/IT khởi tạo). Đảo DOC-12 §2 *"Cấm"*.
4. **JWT vẫn do HRM ký** — giữ nguyên pipeline `Jarvis.Authentication.Jwt` đã có; chỉ đổi *nguồn cấp* token từ IdP ngoài sang `POST /v1/iam/auth/login`. `IssuerSigningKeys` Prod phải là secret thật, **không** phải chuỗi dev.
5. **`POST /dev/login` KHÔNG trở thành cơ chế Prod.** Nó đối chiếu mật khẩu **plaintext trong appsettings** — đủ cho E2E local, tuyệt đối không cho Prod. Vẫn 404 ngoài Development.
6. **Chính sách mật khẩu và khoá tài khoản** = **TBD DOC-13** — không bịa số trên ADR này. Bắt buộc có trước go-live.
7. **MFA**: OQ-ARC-007 từng hỏi *"MFA sau SSO?"* — nay phát biểu lại: *"MFA sau password?"*. Không có IdP thì MFA phải **tự làm** (TOTP) hoặc **không có**. TBD, phải quyết trước go-live.

### Lý do

Khách yêu cầu. Ràng buộc thương mại, không phải lựa chọn kỹ thuật — cùng loại với DEC-ARC-016/018.

Hệ quả kỹ thuật kèm theo, nói thẳng: **bề mặt tấn công tăng**. SSO đẩy rủi ro credential (brute force, credential stuffing, lộ hash, quên mật khẩu) sang IdP; nay HRM tự gánh. CR-001 §Impact đã ghi điều này khi bị từ chối — lý do từ chối hôm đó vẫn đúng, chỉ là **thẩm quyền quyết định đã đổi**.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **T — Password tự quản, không SSO** *(khách yêu cầu)* | Hết phụ thuộc Lark/IT; OQ-DLV-001 hết chặn Prod | Bề mặt tấn công tăng; phải làm policy, khoá, reset, MFA |
| U — Giữ SSO, đổi IdP khác | Không lưu password | **Loại** — khách muốn bỏ SSO, không phải đổi IdP |
| V — Password chính, SSO tuỳ chọn song song | Linh hoạt | **Loại** — hai luồng xác thực = hai bề mặt, gấp đôi test; không ai yêu cầu |
| W — Giữ ADR-007, đẩy khách | — | **Loại** — trái yêu cầu khách |

### Hệ quả

**Tích cực:** **OQ-DLV-001 (Lark JWKS) đóng** — blocker Prod lớn nhất từ 2026-08-26 biến mất. Không phụ thuộc IT cấp issuer. Go-live không còn chờ bên thứ ba cho xác thực.

**Tiêu cực:** HRM phải xây và vận hành: hash mật khẩu, policy, khoá sau N lần sai, reset qua HR/IT, audit đăng nhập, và có thể MFA. **Toàn bộ TC IAM giả định SSO phải viết lại; UAT §1 chạy lại.**

**Rủi ro:**

- **RK-08** — Chưa có văn bản của khách trong `assets/` (giống DEC-ARC-018). Ba yêu cầu lớn nhất của dự án hiện chỉ truy được qua lời PGD.
- **RK-09** — Nếu ai đó "tiện tay" mở `POST /dev/login` ở Prod thay vì làm luồng thật: mật khẩu plaintext trong config. Guard `IsDevelopment()` là lớp chặn duy nhất — ADR-012 §5 nói rõ, và DOC-17 §cutover đã có mục kiểm `ASPNETCORE_ENVIRONMENT`.
- **RK-10** — Reset mật khẩu: ai xác minh danh tính nhân viên quên mật khẩu? Không có IdP thì đây là quy trình HR, chưa có.

### Tuân thủ & Tác động NFR

| NFR | Impact |
|-----|--------|
| NFR-002 | Không đổi — cô lập lương là RBAC sau khi đã xác thực |
| NFR-003 | *"IAM cùng rule web + mobile"* — giữ; cùng endpoint login cho cả hai kênh |
| NFR-005 | **Thêm** action audit: đăng nhập thành công/thất bại, khoá, đổi/reset mật khẩu |
| **NFR-S0x (mới)** | Chính sách mật khẩu · khoá tài khoản · thuật toán hash · rate limit login — **DOC-13 phải bổ sung**, hiện trống |
| NFR-006 | Không đổi |

### DOC bị ảnh hưởng

| DOC | Thay đổi | Owner |
|-----|----------|-------|
| ADR-007 | Superseded toàn bộ — chỉ sửa dòng Status, không sửa nội dung | SA |
| ADR-001 | §4 superseded bởi ADR này — ghi chú ở register | SA |
| DOC-11 §3.1 | Bỏ *"Không lưu password hash"*; thêm thuộc tính §2 | SA |
| DOC-12 §2, §4, `openapi.yaml` | Bỏ cấm; thêm 3 endpoint auth | SA |
| DOC-13 | +NFR mật khẩu/khoá/hash/rate-limit; OQ-ARC-007 phát biểu lại | SA/BA |
| `03-modules/identity/DOC-06` | +IAM-FR đăng nhập, đổi, reset, khoá | BA |
| `03-modules/identity/DOC-07` | +AC gồm **negative**: sai N lần, hết hạn, khoá | BA |
| `03-modules/identity/DOC-16` | +TC; **bỏ** TC SSO/Lark | QC |
| DOC-17 | Bỏ mục Lark issuer/JWKS; thêm secret signing key Prod | SA |
| DOC-10 INT-001 | Tích hợp IdP không còn — đánh dấu bỏ | SA |
| CR-001 | Ghi chú *"đảo bởi CR-002"*, giữ nguyên hồ sơ từ chối | — |

### Câu hỏi mở

- **OQ-DLV-009** — Quy trình reset mật khẩu khi nhân viên quên: ai xác minh, kênh nào? (RK-10)
- **OQ-ARC-007** *(phát biểu lại)* — MFA sau password: bắt buộc? TOTP tự làm hay không có?
- **OQ-DLV-010** — Chính sách mật khẩu: độ dài, độ phức tạp, hạn dùng, N lần sai thì khoá bao lâu.

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-09-15 | ☑ **Accepted** — theo yêu cầu khách (DEC-ARC-027) |
| Khách hàng | | | ☐ **văn bản vào `assets/`** (RK-08) |
| SA | | | ☐ sửa DOC-11/12/13/17 |
| BA | | | ☐ IAM DOC-06/07 |
