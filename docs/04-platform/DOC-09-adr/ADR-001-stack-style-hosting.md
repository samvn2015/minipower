# ADR-001 — Stack, style triển khai, hosting

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Accepted** (DEC-ARC-003 · PGD Dư Hùng) |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-08-26 gói F. Đảo = ADR mới. |
| **Date** | 2026-08-26 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · Dev/Tester (SH-010) · IT/IAM (SH-006) |
| **Informed** | Ban HR · BA Trịnh Yên |

---

### Bối cảnh

HRM nội bộ mInvoice: 7 bounded context đã SRS (+ EVT/RPT chưa SRS); web + mobile; job T-15/T-7/N+3; cấm CRM sales.

DEC-ARC-002 đề xuất **gói A** (modular monolith). **PGD yêu cầu:** lớp middleware, kiến trúc **microservices**, **LBS** (cân bằng tải), kiểm soát truy cập qua **SSO**.

Ràng buộc giữ: xây 2026 ~1 tỷ, dùng 2027; private mInvoice; NFR-001; PII/lương. Sản phẩm LBS / IdP SSO / engine DB **chưa** khóa.

### Quyết định *(gói F)*

1. **Style:** microservices — mỗi bounded context Must = **một service** deploy độc lập: IAM, EMP, LEV, TIM, PAY, PRB, LIF. EVT/RPT chỉ khi có SRS. Notification và worker/job có thể service riêng (không gộp vào PAY).
2. **Lớp middleware (bắt buộc):** API Gateway — TLS, định tuyến tới service, **chặn request không có phiên SSO**, gắn correlation-id, không chứa rule nghiệp vụ (trừ quỹ, N_tính, chốt TV).
3. **LBS:** cân bằng tải **trước** gateway (và tùy chọn trước cụm replica từng service). Sản phẩm L4/L7 **TBD DOC-17** (không khóa nginx/cloud LB trên ADR này).
4. **SSO:** mọi truy cập Web/Mobile/HR **qua SSO** (OIDC/SAML — protocol TBD). IAM service **không** thay IdP; IAM map identity → role HRM (403 màn HR, cô lập lương). Cùng IdP cho web và mobile (NFR-003). MFA **chưa** bắt trên ADR này.
5. **Stack:** ASP.NET Core **.NET 9** mỗi service. CSDL: **database-per-service** (một RDBMS engine chung *loại*, schema/instance tách theo service). Engine **TBD**. Jarvis **không** khóa.
6. **Hosting:** private mInvoice; Dev/UAT/Prod. K8s **không** bắt buộc vì đã có LBS (có thể VM + LBS).

### Lý do

PGD chốt hướng vận hành: tách service, cổng giữa client và domain, scale ngang qua LBS, một cửa SSO. Gói A (monolith) **loại** vì không đáp ứng chỉ đạo này.

Giữ .NET 9 + private DC: PII, email Cty, không biến HRM thành SaaS public.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| A — Modular monolith | Đơn giản TIM–PAY | **Loại** — không có lớp MS + LBS + SSO gateway như PGD |
| **F — Microservices + Gateway + LBS + SSO + .NET 9 + private** *(chỉ đạo)* | Đúng yêu cầu PGD; scale từng service | Vận hành N service; saga TIM→PAY; chi phí 2026 |
| B — MS không SSO / không LBS | Nhẹ hơn F | **Loại** — thiếu LBS và SSO |
| C — SaaS mua | Nhanh | Lệch BRD xây nội bộ |
| D — Public K8s bắt buộc | HA bài bản | NFR-012 trống; OPEX chưa tách |

### Hệ quả

**Tích cực:** DOC-12 = API **per service** + hợp đồng gateway; DOC-10 = SSO IdP + Git/CRM/Excel; scale PAY/TIM độc lập sau LBS.

**Tiêu cực:** Giao dịch phân tán TIM chốt → PAY tính (saga/outbox — chi tiết ADR-005). Nhiều DB backup. Team phải vận hành gateway + IdP.

**Rủi ro:** IdP/LBS chưa chọn; NFR-001 đo **qua** LBS+gateway. HA/DR: ADR-003 **Accepted**; RTO/RPO phút TBD.

**Cấm** dùng middleware để publish event sang CRM sales. Đảo stack/style/SSO/LBS = ADR mới, không sửa file này. DOC-10/11/12/17 **không tự** mở — chờ anh chọn slice.

### Tuân thủ & Tác động NFR

| NFR ID | Impact |
|--------|--------|
| NFR-001 | Đo trên nhánh TIM hoặc PAY sau LBS+GW |
| NFR-002…004 | RBAC sau SSO; PAY DB tách; gateway không xem lương |
| NFR-003 / 008 | Cùng SSO + cùng GW cho web/mobile |
| NFR-005 | Audit tại service SoT + correlation-id GW |
| NFR-006 | Git adapter trên LIF service; secret IT |
| NFR-007 | GW/broker **không** route CRM sales |
| NFR-011 | 2027 Prod nội bộ nhiều node hơn gói A |
| NFR-012 | **Không** đóng % |

### Truy vết

| SRS / Integration | Ghi chú |
|-------------------|---------|
| DOC-08 §4.0–4.4 | Layer LBS → GW → MS → DB |
| IAM DOC-06 | Role sau SSO |
| ADR-002 | Token/session chi tiết (JWT từ IdP) |
| ADR-005 | Hàng đợi giữa MS |
| ADR-006 | **Gộp SSO in MVP vào ADR-001**; MFA còn TBD |
| OQ-ARC-001 | **Đóng** — Accepted gói F |
| OQ-ARC-004 | SSO = có; còn IdP |

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Accepted** gói F (DEC-ARC-003) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
