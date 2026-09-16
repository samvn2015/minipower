# DOC-13 — Yêu cầu Phi chức năng (NFR)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-25 | Trịnh Yên (BA) | **Chốt** (NFR platform · DEC-REQ-038) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Chốt** — NFR-012 phát biểu lại theo **ADR-010** (DEC-ARC-017/018): bỏ DR/DC |
| 0.3 | 2026-09-16 | soạn nháp SA (trợ lý) | **Draft — chờ PGD ký** — **+NFR-S07…S10** mật khẩu/khoá/hash/rate-limit theo **ADR-012** (số là **đề xuất SA**, PGD chốt = đóng OQ-DLV-010); NFR-001/003/SC01 theo **ADR-013** |

**ISO/IEC 25010** · ISO/IEC/IEEE 29148 (phần NFR).  
**Phạm vi:** cross-cutting HRM (leave, PAY, TIM, EMP, LIF đã có SRS/AC; PRB/EVT/RPT/IAM chưa SRS).  
**Cổng:** PGD chốt v0.1 (DEC-REQ-038). Nợ: uptime %, **RTO-failover / RTO-restore**, chu kỳ backup, thuật toán mã hóa → DOC-08 · ADR-010; **chốt số NFR-S07…S10** (OQ-DLV-010) — **chặn code login** (CR-002); quy trình reset (OQ-DLV-009); MFA (OQ-ARC-007); DOC-16 load/pen; Ban HR ☐; module chưa SRS. **Chưa** `02-baseline/`. **Không** tự SAD/DOC-16.

**Không:** bịa 99,9% uptime; pixel HTML; list field master; notify CRM bán hàng (đã cấm FR).

---

## 1. Giới thiệu

NFR nền tảng từ DOC-03 **Chốt** (CN-001…006, BRQ-006, BRQ-009) + AC-NFR trên DOC-07 năm module đã chốt. Chi tiết kiến trúc / APM → DOC-08. AC chức năng → DOC-07 từng module.

## 2. Ma trận tóm tắt NFR

| NFR ID | Category | Requirement (đo được / kiểm được) | Priority | Verification | Owner |
|--------|----------|-----------------------------------|----------|--------------|-------|
| NFR-001 | Performance | UAT: thao tác HR trên **1000 dòng** hoàn tất **&lt; 5s** (BRQ-009), đo **sau LBS** — không có Gateway (ADR-013) | Must | Load / UAT | SH-010 |
| NFR-002 | Security | PII + phiếu lương **cô lập**; LM **không** xem lương cấp dưới. Hàng rào **dưới tầng ứng dụng**: schema `pay` chỉ role `hrm_app_pay` đọc được (ADR-011 W1 · ADR-013) | Must | Test 403 + `psql` role test | SH-001 |
| NFR-003 | Security | IAM **cùng rule** web + mobile (BRQ-006, CN-002) — cùng `POST /v1/iam/auth/login`, cùng JWT (ADR-012) | Must | Test 2 kênh | SH-006 |
| NFR-004 | Security | NV/LM **403** đúng màn HR (TIM import/chốt; PAY kỳ; EMP DS HR; LIF khóa Git) | Must | Test | IAM |
| NFR-005 | Security | Audit **bất biến nghiệp vụ**: chốt công/lương, C1/C2 phép, đổi LM, xác nhận N, khóa Git/CRM; **+ đăng nhập thành công/thất bại, khoá, đổi/reset mật khẩu** (ADR-012) | Must | Log review | SH-006 |
| NFR-006 | Security | HR **không** cầm credential Git; khóa Git/CRM = IT/IAM | Must | Test 403 | SH-006 |
| NFR-007 | Privacy | **Không** gửi sự kiện phép/LIF sang CRM **bán hàng** | Must | Test / log | SH-002 |
| NFR-008 | Usability | Self-service NV: hồ sơ / phép / quỹ / phiếu (kỳ chốt) / thông báo trên web **và** mobile MVP | Must | UAT | SH-004 |
| NFR-009 | Reliability | Cảnh báo TV/SN/lễ: **0 sót 0 trễ** so với lịch master (BO-005) — chi tiết FR khi mở EVT/PRB | Should | UAT | SH-002 |
| NFR-010 | Compliance | BH/TNCN tỷ lệ **theo luật/quy chế tại kỳ** — không hardcode URD (CN-001) | Must | Review master | SH-002 |
| NFR-011 | Constraint | Go-live **2027**; 2026 xây; CAPEX ~1 tỷ (CN-004, 005) — không phải metric runtime | Must | PMO | SH-001 |
| NFR-012 | Availability | Uptime · **RTO-failover** (trong DC) · **RTO-restore** (mất DC) — **không** RPO xuyên site (ADR-010) | TBD | DOC-08 · ADR-010 | SH-006 |
| **NFR-S07** | Security | **Chính sách mật khẩu** — độ dài tối thiểu, blocklist, hạn dùng (ADR-012 §6) | Must | Test negative IAM DOC-07 | SH-006 |
| **NFR-S08** | Security | **Khoá tài khoản** sau N lần sai; mở khoá sau T hoặc bởi HR/IT | Must | Test negative | SH-006 |
| **NFR-S09** | Security | **Hash mật khẩu** — thuật toán + tham số; không log, không export, không API trả về | Must | Code review + pen | SH-010 |
| **NFR-S10** | Security | **Rate limit** `POST /v1/iam/auth/login` theo IP + username | Must | Test | SH-010 |

## 3. Phân loại

### 3.1 Hiệu năng

| NFR ID | Metric | Target | Measurement | Environment |
|--------|--------|--------|-------------|-------------|
| NFR-001 | Thời gian hoàn tất 1000 dòng (tính/preview lương **hoặc** import/preview công — UAT ghi rõ kịch bản) | &lt; 5s | UAT BRQ-009 | UAT |
| NFR-P02 | p95 API NV self-service | **TBD** DOC-08 (không bịa 2s) | k6 | Staging |

### 3.2 Khả dụng & độ tin cậy

| NFR ID | Requirement | Target |
|--------|-------------|--------|
| NFR-012a | Uptime | **TBD** — BRD không chốt %; pattern 24/7 + Active/Standby **một DC** (ADR-010) |
| NFR-012b | **RTO-failover** — Active hỏng, Standby cùng DC tiếp quản | **TBD phút** |
| NFR-012c | **RTO-restore** — mất cả DC, khôi phục từ backup | ⚠️ **hiện không thể đạt** — backup đặt cùng DC (RK-01 · OQ-ARC-012). Chờ PGD quyết |
| NFR-012d | Chu kỳ backup | **TBD** — Ops/IT. Nơi lưu = **máy Standby cùng DC** (DEC-ARC-019), **không** off-site |
| NFR-009 | Cảnh báo đúng hạn | 0 sót 0 trễ (BO-005) khi module EVT/PRB có FR |

### 3.3 Bảo mật

| NFR ID | Requirement | Control |
|--------|-------------|---------|
| NFR-002 | Cô lập lương + PII | RBAC IAM; PAY-BR-007; **schema + role + GRANT theo cột** (`w1-roles.sql`) |
| NFR-003 | Mobile = web IAM | Cùng endpoint login, cùng JWT HRM ký, cùng role |
| NFR-004 | 403 màn HR | TIM/PAY/EMP/LIF AC-NFR |
| NFR-005 | Audit nghiệp vụ | Log không xóa tay NV |
| NFR-006 | Git credential | LIF-FR-008 |
| NFR-S06 | Mã hóa at-rest / TLS | TLS tại LBS; at-rest **TBD** ADR-004 — không đóng AES-256 trên NFR này |
| NFR-S07 | Chính sách mật khẩu | **Đề xuất SA (chưa chốt):** tối thiểu **12** ký tự, tối đa 128; **không** ép độ phức tạp theo lớp ký tự; **blocklist** mật khẩu phổ biến + chứa username; **không** ép đổi định kỳ, chỉ ép đổi khi nghi lộ hoặc lần đầu (HR cấp). *Theo NIST SP 800-63B.* PGD chốt = OQ-DLV-010 |
| NFR-S08 | Khoá tài khoản | **Đề xuất SA:** khoá **15 phút** sau **5** lần sai liên tiếp; đếm reset khi đăng nhập đúng; HR/IT mở khoá tay được; ghi audit mỗi lần khoá |
| NFR-S09 | Hash mật khẩu | **Đề xuất SA:** **Argon2id** (m=64 MiB, t=3, p=1) *hoặc* PBKDF2-HMAC-SHA256 ≥ **600 000** vòng nếu Argon2 không sẵn trong stack — tham số lưu cùng hash để nâng cấp về sau; salt ngẫu nhiên ≥ 16 byte. **Cấm** MD5/SHA-1/SHA-256 trần |
| NFR-S10 | Rate limit login | **Đề xuất SA:** **10 req/phút** theo IP **và** 5 req/phút theo username; trả 429; không tiết lộ tài khoản tồn tại hay không (cùng thông báo lỗi) |
| NFR-S11 | Reset mật khẩu | Quy trình **HR/IT xác minh danh tính** → cấp mật khẩu tạm dùng một lần, hết hạn **24 h** (đề xuất), ép đổi khi đăng nhập. Ai xác minh, kênh nào → **OQ-DLV-009** |
| NFR-S12 | MFA | **Chưa bắt** — *"MFA sau password?"* TOTP tự làm hay không có → OQ-ARC-007. Quyết trước go-live |

### 3.4 Bảo trì & vận hành

| NFR ID | Requirement |
|--------|-------------|
| NFR-M01 | Log cấu trúc đủ để lần chốt kỳ / C2 / N+3 (chi tiết stack → DOC-08) |
| NFR-M02 | Deploy / blue-green | **TBD** DOC-17 |
| NFR-M03 | Ranh giới bounded context **cưỡng chế bằng cơ chế**: test kiến trúc (`Hrm.Architecture.Tests`) + `w1-roles.sql` là SoT GRANT; mọi thay đổi GRANT qua review; test phải chạy trong CI (**CI chưa có** — RK-12) |

### 3.5 Khả năng mở rộng

| NFR ID | Requirement |
|--------|-------------|
| NFR-SC01 | Nội bộ mInvoice; số user **TBD** (không bịa 500). Scale ngang = **nhân bản `Hrm.Host`** sau LBS, JWT stateless (ADR-013) — không scale từng module |

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

### NFR-S07…S10 — Mật khẩu (ADR-012)

| Mục | Nội dung |
|-----|----------|
| **Statement** | HRM tự lưu và xác minh mật khẩu; policy, khoá, hash, rate limit như §3.3. |
| **Rationale** | ADR-012: không IdP → HRM gánh rủi ro credential (brute force, credential stuffing, lộ hash). CR-001 §Impact đã nêu. |
| **Acceptance criteria** | IAM DOC-07 **chưa có** — phải gồm negative: sai N lần → khoá; mật khẩu trong blocklist → từ chối; 429 khi vượt rate; reset hết hạn → từ chối; log audit từng sự kiện. |
| **Architectural impact** | `IdentityAccount` +5 cột (DOC-11 §3.1); 3 endpoint (DOC-12 §2); Jarvis có sẵn JWT, **chưa có** hash/policy — chọn thư viện khi code. |
| **Test approach** | Unit (policy, hash) · API negative · pen test trước go-live (DOC-16) |
| **Trạng thái** | Số là **đề xuất SA** — PGD chốt (OQ-DLV-010) **trước** khi mở code CR-002. |

### NFR-002 — Cô lập lương

| Mục | Nội dung |
|-----|----------|
| **Statement** | LM và NV khác không đọc phiếu / số lương không thuộc mình. |
| **Rationale** | CN-002 · URD III · PAY-BR-007 · EMP-BR-011 |
| **Acceptance criteria** | PAY-AC-NFR-001 · EMP-AC-015 · `psql` bằng `hrm_app_lev` đọc `pay.*` → *permission denied* (kiểm 8/8, OQ-ARC-011) |
| **Test approach** | Role test 403 + role test DB |

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
| NFR-012a–d | — | ADR-010 · TBD DOC-08 | Bỏ RPO xuyên site (DEC-ARC-017) |
| NFR-S07…S12 | ADR-012 · CR-002 | IAM DOC-06/07 **chưa có** (BA) | negative login TC — DOC-16 chưa có |
| NFR-M03 | ADR-013 · ADR-011 W2 | `Hrm.Architecture.Tests` 9 test | CI chưa có |

## 6. Phê duyệt

| Vai trò | Họ tên | Ngày | Baseline |
|---------|--------|------|----------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-25 | **Chốt** v0.1 (DEC-REQ-038) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | | ☐ **ký v0.3** — chốt số S07…S10 · ☐ `02-baseline/` |
| BA (R) | Trịnh Yên | 2026-08-25 | Soạn → PGD chốt |
| Architect | | | ☐ Nợ DOC-08 (SLA/crypto) |
| Business Owner | Ban HR | | ☐ Nợ |
