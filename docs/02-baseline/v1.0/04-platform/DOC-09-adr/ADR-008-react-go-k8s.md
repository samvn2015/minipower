# ADR-008 — Đảo runtime: React + Go + container/K8s

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (SA soạn) | **Rejected** (DEC-ARC-012 · PGD giữ ADR-001) |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa ADR-001 Accepted — đảo bằng ADR này.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Rejected** — PGD Dư Hùng 2026-08-26: giữ công nghệ ban đầu (ADR-001) |
| **Date** | 2026-08-26 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · DevOps · IT |
| **Informed** | Ban HR · BA |

**Không supersede ADR-001.** PGD từ chối đảo stack.  
**Giữ nguyên:** microservices 7 BC · API Gateway · LBS · SSO OIDC (ADR-001/002/007) · DB-per-service · Active/Standby (ADR-003) · cấm CRM sales · không login password.

---

### Bối cảnh

PGD yêu cầu đổi FE **React**, BE **Go**, triển khai **Docker / Kubernetes** trên nền tảng đám mây.

Đã **Accepted:** ADR-001 gói F — BE **.NET 9**; K8s **không** bắt buộc (VM + LBS); hosting **private mInvoice**. Có nháp `src/iam` C# (DEC-DLV-008).

Chưa khóa: IdP issuer, DB engine, cloud vendor, RTO phút.

### Quyết định *(Rejected)*

PGD **không** chấp nhận đảo. Runtime MVP = ADR-001: **.NET 9**, K8s **không** bắt buộc, hosting **private mInvoice**. React + Go + K8s bắt buộc **không** vào baseline kiến trúc.

Các mục 1–6 dưới đây là **phương án đã loại** (giữ để audit), không thi hành.

1. **Frontend:** SPA **React** (web). Mobile: **cùng API GW + OIDC** (NFR-003); native/RN **TBD** — không giả định React Native trên ADR này.
2. **Backend:** mỗi service Must = **Go** (một binary / service). Gateway có thể Go hoặc sản phẩm GW — **không** nhét rule quỹ/N_tính/chốt TV vào GW.
3. **Packaging:** mỗi service + FE build **container image** (Docker). Không khóa registry brand.
4. **Orchestration:** **Kubernetes** cho UAT/Prod *(nếu Accepted)* — thay “K8s không bắt buộc” của ADR-001. Lệnh/`kubectl` cụ thể **không** ghi trên ADR; DOC-17 sửa **sau khi chốt** (CR).
5. **Mây:** “đám mây” = **K8s có thể chạy trên DC mInvoice hoặc cloud managed**. **Vendor cloud TBD** — không khóa EKS/GKE/AKS trên ADR này. Public SaaS HRM **vẫn cấm** (BRD nội bộ).
6. **Tạm dừng** nháp .NET (`src/iam`) khi ADR này Accepted — thay slice Go+React, không dual-stack MVP.

### Lý do

Chỉ đạo PGD về skillset và vận hành container. Giữ lớp LBS→GW→MS→DB đã chốt để không phá NFR SSO/lương.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **Giữ ADR-001** (.NET 9, K8s optional) | Đã Accepted; nháp IAM C# | Lệch chỉ đạo mới |
| **H — React + Go + Docker; K8s optional** | Ít đảo DOC-17 | PGD nêu K8s |
| **I — React + Go + K8s; cloud vendor TBD** *(đề xuất)* | Đúng hướng FE/BE/K8s; chưa bịa AWS | Đảo stack; OPEX K8s; team Go; bỏ nháp .NET |
| J — Public cloud bắt buộc + brand EKS | Rõ vendor | OPEX/NFR-012 trống; lệch private mInvoice |

### Hệ quả

**Nếu Accepted:** DOC-08 § stack, DOC-14/15 (skill), DOC-17 (deploy) = **CR/delta**. Jarvis .NET **không** dùng MVP. NFR-001 vẫn đo sau LBS+GW.

**Rủi ro:** chưa có Go/React trên repo; chưa cluster; RTO phút vẫn TBD; IAM C# thành dead code.

**Cấm:** bịa % SLA; Keycloak/Entra; kubectl cookbook trên ADR.

### Tuân thủ NFR

| NFR | Impact |
|-----|--------|
| NFR-001 | Không đổi điểm đo (sau LBS+GW) |
| NFR-002…004 | RBAC vẫn IAM DB; React không được nhúng secret lương |
| NFR-003/008 | React + mobile cùng OIDC/GW |
| NFR-011 | K8s tăng độ phức tạp vận hành 2027 |
| NFR-012 | **Không** đóng % |

### Truy vết

| Nguồn | Ghi chú |
|-------|---------|
| ADR-001 | Đảo §5–§6 nếu Accepted |
| DOC-17 | Runbook A/S; chưa K8s — cập nhật sau chốt |
| DEC-DLV-008 | `src/iam` .NET nháp |

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Rejected** — giữ ADR-001 |
| SA | Trịnh Yên | 2026-08-26 | Soạn Proposed → PGD từ chối |
