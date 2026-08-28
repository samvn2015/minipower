# DOC-13 — Yêu cầu Phi chức năng (NFR)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (NFR platform · DEC-REQ-038) |

**ISO/IEC 25010** · ISO/IEC/IEEE 29148 (phần NFR).  
**Phạm vi:** cross-cutting HRM (leave, PAY, TIM, EMP, LIF đã có SRS/AC; PRB/EVT/RPT/IAM chưa SRS).  
**Cổng:** PGD chốt v0.1 (DEC-REQ-038). Nợ: SLA/RTO/RPO/thuật toán mã hóa → DOC-08; DOC-16 load/pen; Ban HR ☐; module chưa SRS. **Chưa** `02-baseline/`. **Không** tự SAD/DOC-16.

**Không:** bịa 99,9% uptime; pixel HTML; list field master; notify CRM bán hàng (đã cấm FR).

---

## 1. Giới thiệu

NFR nền tảng từ DOC-03 **Chốt** (CN-001…006, BRQ-006, BRQ-009) + AC-NFR trên DOC-07 năm module đã chốt. Chi tiết kiến trúc / APM → DOC-08. AC chức năng → DOC-07 từng module.

## 2. Ma trận tóm tắt NFR

| NFR ID | Category | Requirement (đo được / kiểm được) | Priority | Verification | Owner |
|--------|----------|-----------------------------------|----------|--------------|-------|
| NFR-001 | Performance | UAT: thao tác HR trên **1000 dòng** hoàn tất **&lt; 5s** (BRQ-009) | Must | Load / UAT | SH-010 |
| NFR-002 | Security | PII + phiếu lương **cô lập**; LM **không** xem lương cấp dưới | Must | Test 403 | SH-001 |
| NFR-003 | Security | IAM **cùng rule** web + mobile (BRQ-006, CN-002) | Must | Test 2 kênh | SH-006 |
| NFR-004 | Security | NV/LM **403** đúng màn HR (TIM import/chốt; PAY kỳ; EMP DS HR; LIF khóa Git) | Must | Test | IAM |
| NFR-005 | Security | Audit **bất biến nghiệp vụ**: chốt công/lương, C1/C2 phép, đổi LM, xác nhận N, khóa Git/CRM | Must | Log review | SH-006 |
| NFR-006 | Security | HR **không** cầm credential Git; khóa Git/CRM = IT/IAM | Must | Test 403 | SH-006 |
| NFR-007 | Privacy | **Không** gửi sự kiện phép/LIF sang CRM **bán hàng** | Must | Test / log | SH-002 |
| NFR-008 | Usability | Self-service NV: hồ sơ / phép / quỹ / phiếu (kỳ chốt) / thông báo trên web **và** mobile MVP | Must | UAT | SH-004 |
| NFR-009 | Reliability | Cảnh báo TV/SN/lễ: **0 sót 0 trễ** so với lịch master (BO-005) — chi tiết FR khi mở EVT/PRB | Should | UAT | SH-002 |
| NFR-010 | Compliance | BH/TNCN tỷ lệ **theo luật/quy chế tại kỳ** — không hardcode URD (CN-001) | Must | Review master | SH-002 |
| NFR-011 | Constraint | Go-live **2027**; 2026 xây; CAPEX ~1 tỷ (CN-004, 005) — không phải metric runtime | Must | PMO | SH-001 |
| NFR-012 | Availability | SLA uptime / RTO / RPO | TBD | DOC-08 | SH-006 |

## 3. Phân loại

### 3.1 Hiệu năng

| NFR ID | Metric | Target | Measurement | Environment |
|--------|--------|--------|-------------|-------------|
| NFR-001 | Thời gian hoàn tất 1000 dòng (tính/preview lương **hoặc** import/preview công — UAT ghi rõ kịch bản) | &lt; 5s | UAT BRQ-009 | UAT |
| NFR-P02 | p95 API NV self-service | **TBD** DOC-08 (không bịa 2s) | k6 | Staging |

### 3.2 Khả dụng & độ tin cậy

| NFR ID | Requirement | Target |
|--------|-------------|--------|
| NFR-012 | Uptime / RTO / RPO | **TBD** architecture — BRD không chốt % |
| NFR-009 | Cảnh báo đúng hạn | 0 sót 0 trễ (BO-005) khi module EVT/PRB có FR |

### 3.3 Bảo mật

| NFR ID | Requirement | Control |
|--------|-------------|---------|
| NFR-002 | Cô lập lương + PII | RBAC IAM; PAY-BR-007 |
| NFR-003 | Mobile = web IAM | Cùng token/role |
| NFR-004 | 403 màn HR | TIM/PAY/EMP/LIF AC-NFR |
| NFR-005 | Audit nghiệp vụ | Log không xóa tay NV |
| NFR-006 | Git credential | LIF-FR-008 |
| NFR-S06 | Mã hóa at-rest / TLS | **TBD** DOC-08 — không đóng AES-256 trên NFR này |

### 3.4 Bảo trì & vận hành

| NFR ID | Requirement |
|--------|-------------|
| NFR-M01 | Log cấu trúc đủ để lần chốt kỳ / C2 / N+3 (chi tiết stack → DOC-08) |
| NFR-M02 | Deploy / blue-green | **TBD** DOC-17 |

### 3.5 Khả năng mở rộng

| NFR ID | Requirement |
|--------|-------------|
| NFR-SC01 | Nội bộ mInvoice; số user **TBD** architecture (không bịa 500) |

### 3.6 Tuân thủ

| NFR ID | Regulation / policy |
|--------|---------------------|
| NFR-010 | BHXH/BHYT/BHTN, TNCN — master kỳ |
| NFR-C02 | PDPA / cư trú dữ liệu VN | **TBD** pháp chế — chưa trên BRD |

### 3.7 Khả năng sử dụng

| NFR ID | Requirement |
|--------|-------------|
| NFR-008 | Web + mobile MVP cùng rule |
| NFR-U02 | WCAG | **Should / TBD** — chưa Must trên BRD |
| NFR-U03 | HTML prototype pixel | **Không** Must |

## 4. Mẫu chi tiết

### NFR-001 — 1000 dòng &lt; 5s

| Mục | Nội dung |
|-----|----------|
| **Statement** | UAT Must: một thao tác HR trên tập **1000 dòng** hoàn tất dưới 5 giây (BRQ-009). |
| **Rationale** | UAT lương/công; tránh hệ chậm lúc chốt kỳ. |
| **Acceptance criteria** | Kịch bản UAT ghi rõ: PAY preview **hoặc** TIM preview 1000 dòng — QC chọn một, đo 3 lần, max &lt; 5s. |
| **Architectural impact** | DOC-08 |
| **Test approach** | UAT + (sau) load DOC-16 |

### NFR-002 — Cô lập lương

| Mục | Nội dung |
|-----|----------|
| **Statement** | LM và NV khác không đọc phiếu / số lương không thuộc mình. |
| **Rationale** | CN-002 · URD III · PAY-BR-007 · EMP-BR-011 |
| **Acceptance criteria** | PAY-AC-NFR-001 · EMP-AC-015 |
| **Test approach** | Role test 403 |

## 5. Truy vết

| NFR ID | Nguồn | AC / checklist module | Test (DOC-16) |
|--------|-------|----------------------|----------------|
| NFR-001 | BRQ-009 | UAT | |
| NFR-002 | CN-002 | PAY-AC-NFR-001 · EMP-AC-015 | |
| NFR-003 | BRQ-006 | LEV-AC-002 · PAY phiếu mobile · EMP-AC-007 | |
| NFR-004 | DOC-07 | TIM/PAY/EMP/LIF AC-NFR-001 | |
| NFR-005 | DOC-07 | *-AC-NFR-002 | |
| NFR-006 | LIF | LIF-AC-008 · LIF-AC-NFR-001 | |
| NFR-007 | DEC-DIS-001 | LEV-AC-009 · LIF-AC-010 | |
| NFR-008 | BO-006 | DOC-07 self-service | |
| NFR-010 | CN-001 | PAY master kỳ | |
| NFR-012 | — | TBD DOC-08 | |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-038) · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Architect | | | ☐ Nợ DOC-08 (SLA/crypto) |
| Business Owner | Ban HR | | ☐ Nợ |
