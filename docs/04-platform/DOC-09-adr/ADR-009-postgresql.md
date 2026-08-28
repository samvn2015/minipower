# ADR-009 — Database engine: PostgreSQL (loại RDBMS chung)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (SA soạn) | **Accepted** (DEC-ARC-014 · PGD Dư Hùng) |

**Michael Nygard ADR** · 1 file / 1 quyết định.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-08-26 |
| **Date** | 2026-08-26 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · DBA · Dev |
| **Informed** | Ban HR |

**Khóa:** engine **PostgreSQL** là *loại* RDBMS chung cho 7 DB-per-service.  
**Không khóa:** phiên bản minor, managed vs VM, UUID vs bigint (DOC-11).  
**Không** đảo ADR-001 style MS / .NET 9.

---

### Bối cảnh

ADR-001: một **RDBMS** chung loại, instance/schema tách theo service; engine TBD. DOC-11 nợ engine. Domain Must: unique CCCD/MNV, overlap phép, C2 trừ quỹ atomic, N_tính, một mẫu TIM Active, 403 lương.

PGD hỏi PostgreSQL vs MongoDB.

### Quyết định

1. **SoT nghiệp vụ 7 Must = PostgreSQL** (một engine loại; 7 database/instance theo service).
2. **MongoDB không** làm SoT lương / phép / công / HĐ / IAM role.
3. Catalog động (cột Excel, PC master): **JSONB + bảng master trên PostgreSQL**, không chuyển SoT sang document DB.
4. Replica Prod→DR theo ADR-003 — cơ chế cụ thể sau khi chốt engine (không bịa RPO phút).

### Lý do

Invariants HRM là **quan hệ + giao dịch**: chặn overlap, unique, trừ quỹ cùng commit C2, N_tính = N_thực − N_KHL. PostgreSQL khớp ADR-001 “RDBMS”. MongoDB tối ưu document linh hoạt, kém khớp ràng buộc xuyên bản ghi của PAY/LEV/TIM.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **P — PostgreSQL SoT** *(đề xuất)* | ACID, unique, SQL UAT 0đ, JSONB catalog, khớp Jarvis Npgsql mặc định | Cần migration; DBA SQL |
| M — MongoDB SoT mọi service | Schema linh hoạt | Lệch ADR-001 RDBMS; khó overlap/quỹ atomic/UAT cột; rủi ro lương |
| H — PostgreSQL PAY/LEV/TIM + Mongo EMP | “đúng tool” | Hai operational model; sync hồ sơ–lương |

### Hệ quả

**Nếu Accepted:** OQ-DLV-003 đóng một phần (loại engine). DOC-11/17 ghi PostgreSQL; connection string TBD. **Không** tự cài cluster trên ADR.

**Cấm:** Mongo làm phiếu lương; bịa % replica lag.

### Tuân thủ NFR

| NFR | Impact |
|-----|--------|
| NFR-002 | PAY DB riêng vẫn PostgreSQL |
| NFR-005 | Audit SQL/trigger hoặc bảng log |
| NFR-012 | RPO phút vẫn TBD |

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Accepted** PostgreSQL (DEC-ARC-014) |
| SA | Trịnh Yên | 2026-08-26 | Đề xuất P → PGD chốt |
