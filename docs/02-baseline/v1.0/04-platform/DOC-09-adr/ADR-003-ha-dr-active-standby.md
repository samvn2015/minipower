# ADR-003 — Vận hành 24/7, Active/Standby, DR/DC

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (soạn nháp SA) | **Accepted** (DEC-ARC-004 · PGD Dư Hùng) |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-08-26. Đảo = ADR mới. |
| **Date** | 2026-08-26 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · Ops/IT (SH-006) · Dev (SH-010) |
| **Informed** | Ban HR · BA Trịnh Yên |

---

### Bối cảnh

ADR-001 gói F: microservices + API Gateway + LBS + SSO, private mInvoice. NFR-012 (uptime / RTO / RPO) để trống — **không** bịa %.

PGD: mô hình chạy **đảm bảo 24/7**, dự phòng **Active/Standby**, có **DR/DC** (trung tâm chính + trung tâm dự phòng thảm họa).

### Quyết định

1. **Mục tiêu vận hành:** HRM phục vụ **24/7** (kể cả ngoài giờ hành chính — phép đột xuất, job T-15/T-7/N+3). Không đóng % uptime trên ADR này.
2. **Trong một DC (Prod):** cặp **Active / Standby** cho LBS, Gateway, từng microservice, IdP (nếu tự host), và **primary DB từng service**. LBS chỉ bơm traffic vào **Active**; Standby nóng (process lên, nhận replicate), không xử lý request nghiệp vụ đến khi failover.
3. **Giữa hai site — DR/DC:**
   - **DC-Prod (Active):** nhận 100% user.
   - **DC-DR (Standby):** bản sao GW + services + DB replicate; **không** Active/Active hai DC (tránh double-job T-15/T-7, double-chốt).
4. **Dữ liệu:** replicate DB-per-service Prod → DR. Sync/async và **RPO phút** = TBD (OQ-ARC-002). Không dual-write hai DC.
5. **Job:** chỉ scheduler **Active** được phép chạy T-15/T-7/N+3. Standby/DR **disable** job đến khi site được promote.
6. **SSO:** IdP phải surviving cùng mô hình A/S hoặc phụ thuộc IdP Cty (DOC-10). Failover HRM không có IdP = không login.
7. **Chuyển site:** runbook DOC-17. Tự động vs tay **TBD**. **RTO phút** TBD — không bịa.
8. **UAT/Dev:** không bắt buộc đủ cặp DR; Prod (+ DR) mới đủ mô hình này.

### Lý do

24/7 + PII/lương nội bộ → không single-node. Active/Standby đơn giản hơn Active/Active cho SoT chốt công/lương/TV. DR/DC tách thảm họa mất một nhà.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **G — 24/7 + A/S trong DC + DC-Prod/DC-DR** *(chỉ đạo)* | Đúng PGD; job một nơi | RTO/RPO chưa số; chi phí 2 site |
| H — Chỉ 8×5, backup đêm | Rẻ | **Loại** — không 24/7 |
| I — Active/Active hai DC | RTO thấp | Double-job; conflict chốt; phức tạp saga TIM–PAY |
| J — Một DC, chỉ RAID/backup | Rẻ | **Loại** — không DR/DC |

### Hệ quả

**Tích cực:** DOC-17 có topology 2 site; capacity Standby ≈ Active (không scale 10%).

**Tiêu cực:** CAPEX/OPEX 2 DC; replicate N database (gói F). Drill failover định kỳ (tần suất TBD).

**Rủi ro:** Chưa RTO/RPO → nhà thầu không cam kết SLA pháp lý. IdP ngoài HRM sập → HRM 24/7 vẫn không login.

**Không** tự bịa 99,9%. **Không** mở DOC-17 đầy đủ đến khi **ADR-001** Accepted (ADR-003 đã Accepted). Đảo HA/DR = ADR mới, không sửa file này.

### Tuân thủ & Tác động NFR

| NFR ID | Impact |
|--------|--------|
| NFR-012 | Pattern **24/7 + A/S + DR/DC** khóa; **số** uptime/RTO/RPO vẫn TBD |
| NFR-001 | Đo trên Active sau failover giả lập UAT |
| NFR-005 | Audit không split-brain hai Active |
| NFR-009 | Job chỉ Active — 0 sót nhờ failover kịp T-15 |
| NFR-011 | 2027 Prod **và** DR sẵn sàng |

### Truy vết

| SRS / Integration | Ghi chú |
|-------------------|---------|
| DOC-08 §4.4 | Sơ đồ DC / DR |
| ADR-001 | LBS + GW + MS đặt lên A/S |
| ADR-005 | Broker replicate hoặc failover cùng site Active |
| OQ-ARC-002 | Còn số phút RTO/RPO |
| NFR-M02 | Blue-green hoãn DOC-17; không mâu A/S |

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Accepted** (DEC-ARC-004) · ☐ `02-baseline/` |
| SA | | 2026-08-26 | Soạn → PGD chốt |
