# DOC-14 — WBS & Ước lượng

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (PM/BA soạn) | **Chốt** (DEC-PLN-002) |
| 0.2 | 2026-09-18 | soạn nháp PM (trợ lý) | **Draft — chờ PGD ký** — WBS theo **ADR-013** (một host, không GW/DB-per-service), **ADR-012** (login HRM), **ADR-010** (không DR); ghi nhận **7 module Must đã code, UAT DEV Pass**; phần còn lại là nợ Prod (doc-review pass 3 B2) |

**PMBOK WBS** · Epic / Feature / Story. Tiên quyết: DOC-03 · 7× DOC-06 **Chốt** · DOC-08 v0.4 · DOC-10–13/17 **Chốt**.  
**Cổng:** PGD chốt v0.1 (DEC-PLN-002); v0.2 chờ ký. Nợ: velocity; FTE; EVT/RPT SRS; **ngày go-live 2027 chưa lịch**. **Không** tự DOC-16/17. **Chưa** `02-baseline/`.

> **Thực tế 2026-09-18:** 7 module Must **đã code** trên một `Hrm.Host` (UAT DEV Pass), hàng rào dữ liệu W1 + test kiến trúc W2 **đã xong**. WBS v0.1 (wave theo module) đã qua. v0.2 xếp lại theo **việc còn lại để chạy Prod 2027**.

---

## 1. Tổng quan

| Mục | Giá trị |
|-----|---------|
| **Project / Phase** | HRM mInvoice · planning |
| **Baseline SRS** | 7 module DOC-06 **Chốt** (chưa repo baseline) |
| **Estimation method** | T-shirt (S/M/L/XL) + SP **tương đối** — chưa khóa số SP/sprint |
| **Velocity** | **TBD** (chưa team) |
| **Budget constraint** | CAPEX ~1 tỷ **2026** (CN-004); dùng **2027** (NFR-011) |

## 2. Cấu trúc WBS

```text
1.0 HRM
├── 1.1 Nền tảng (một host · LBS · A/S một DC · schema+role) — W1/W2 XONG; còn Prod config, CI, hosting
├── 1.2 identity
├── 1.3 employee-profile
├── 1.4 leave
├── 1.5 timekeeping
├── 1.6 payroll
├── 1.7 probation
├── 1.8 lifecycle
├── 1.9 NFR / audit / cô lập lương — cô lập lương XONG (W1); audit LEV/IAM còn nợ (B1)
├── 1.10 EVT + RPT (sau SRS) · mobile (chưa code)
└── 1.11 Delivery (DOC-16/17, UAT, cutover)
```

## 3. Phân rã Epic · Feature · Story (hạt Feature)

SP cột = **T-shirt** đến khi có velocity. Trace = dải FR module, không copy từng FR.

| WBS | Epic | Feature | Story ID | Story title | FR trace | Size | Priority |
|-----|------|---------|----------|-------------|----------|------|----------|
| 1.1.1 | EP-PLT | FE-HOST | US-PLT-01 | As NV/HR I use HRM via LBS → `Hrm.Host` with JWT HRM ký | ADR-013 · ADR-012 | ~~XL~~ **XONG** (host) · **M** còn: 8 conn string Prod, xoá `Authority`, guard khởi động, CI/Dockerfile, hosting SPA, OTEL collector | Must |
| 1.1.2 | EP-PLT | FE-HA | US-PLT-02 | As Ops I run Active/Standby **một DC** + backup/restore | ADR-010 | **L** — replication/promote runbook (DOC-17 M9), `Hrm:HostRole`, RK-01 backup ngoài DC (chờ OQ-ARC-012) | Must |
| 1.1.3 | EP-PLT | FE-WALL | US-PLT-03 | As SA I enforce context boundary by schema+role+tests | ADR-011 W1/W2 | **XONG** 2026-09-07/15 | Must |
| 1.2.1 | EP-IAM | FE-RBAC | US-IAM-01 | As IAM I assign roles HRM | IAM DOC-06 | **XONG** | Must |
| 1.2.2 | EP-IAM | FE-LOGIN | US-IAM-02 | As NV I log in with username/password; HR/IT tạo tài khoản, reset, unlock | ADR-012 · CR-002 · DOC-13 S07…S11 | **L** — chặn: IAM DOC-06/07 delta (BA), OQ-DLV-011 | Must |
| 1.3.1 | EP-EMP | FE-HS | US-EMP-01 | As HR/NV I manage hồ sơ + unique + HĐ/KT_TV | EMP-FR | **XONG** (UAT DEV) | Must |
| 1.4.1 | EP-LEV | FE-DON | US-LEV-01 | As NV/LM/HR I submit & C1/C2 trừ quỹ | LEV-FR | **XONG** (UAT DEV) | Must |
| 1.5.1 | EP-TIM | FE-IMP | US-TIM-01 | As HR I import 1 mẫu Excel & chốt tháng | TIM-FR, INT-003 | **XONG** (UAT DEV) | Must |
| 1.6.1 | EP-PAY | FE-TINH | US-PAY-01 | As HR I run kỳ lương; NV xem phiếu mình | PAY-FR, NFR-002 | **XONG** (UAT DEV) | Must |
| 1.7.1 | EP-PRB | FE-TV | US-PRB-01 | As HR I chốt 3 mã; T-15/T-7 job | PRB-FR | **XONG** (UAT DEV) — bộ lập lịch ngoài **TBD** (OQ-ARC-017/019) | Must |
| 1.8.1 | EP-LIF | FE-N3 | US-LIF-01 | As IT I lock Git/CRM sản phẩm N+3 | LIF-FR, INT-004/005 | **XONG tới outbox** — **adapter Git/CRM chưa code** (R-015) **M** | Must |
| 1.8.2 | EP-LIF | FE-ADP | US-INT-01 | As Notif/LIF I drain outbox → SMTP / Git / CRM | INT-002/004/005 · R-015 | **M** — API vendor TBD | Must |
| 1.9.1 | EP-NFR | FE-AUD | US-NFR-01 | As Ops I retain audit chốt công/lương/C2/PRB + login/khoá | NFR-005 | 5/7 context XONG; **LEV + IAM** nợ — **S** (B1, DEC-ARC-033) | Must |
| 1.10.2 | EP-MOB | FE-MOB | US-MOB-01 | As NV I self-service on mobile | NFR-003/008 · BRQ-006 | **chưa code, chưa size** — cùng API | Must |
| 1.10.1 | EP-EVT | FE-CANH | US-EVT-01 | Cảnh báo SN/lễ | *chưa SRS* | — | Must* |
| 1.11.1 | EP-DLV | FE-UAT | US-DLV-01 | UAT 1000 dòng &lt;5s + DOC-16/17 | NFR-001 | L | Must |

\*EVT in BRD Must — **không** estimate chi tiết đến khi DOC-06 EVT.

Chi tiết 1:1 AC → DOC-07 từng module (không lặp Gherkin).

## 4. Chi tiết story (mẫu)

### US-IAM-02 — Đăng nhập HRM tự quản (ADR-012)

| Mục | Nội dung |
|-----|----------|
| **User story** | As a NV, I want đăng nhập bằng username/password do HR/IT cấp, so that vào HRM web/mobile không cần IdP ngoài |
| **AC** | DOC-13 NFR-S07…S11 (blocklist, khoá 5 lần/15′, Argon2id, 429, reset 24 h, một thông báo chung); IAM DOC-07 delta — **chưa có** |
| **Dependencies** | IAM DOC-06/07 delta (BA) · OQ-DLV-011 reset · bỏ JIT-provision `/me` · IAM audit (US-NFR-01) |
| **Size** | L |
| **Notes** | `POST /dev/login` **không** phải cơ chế này — 404 ngoài Development |

## 5. Chấm phức tạp (0–4 × 5 · toàn chương trình)

| Chiều | Điểm | Lý do |
|-------|------|-------|
| Phạm vi | **4** | 7 module Must + mobile chưa code |
| Tích hợp | **2** | SMTP, Excel, Git, CRM sản phẩm — không IdP (ADR-012); API vendor TBD |
| Dữ liệu | **3** | 8 schema / 7 role một DB; migrate as-is; catalog động |
| Bên liên quan | **3** | PGD, chủ đầu tư, HR, IT, NV, LM; Ban HR chưa ký |
| PCN & rủi ro | **3** | 24/7 A/S một DC, không DR (RK-01); PII lương; tự gánh credential (ADR-012) |
| **Tổng** | **15** | **Enterprise** *(v0.1: 17 — giảm vì bỏ MS/GW/DR/IdP)* |

Hàm ý: phần nghiệp vụ đã xong; đường găng 2027 là **Prod readiness + login + adapter + mobile**, phụ thuộc IT Git/CRM API, DevOps LBS/host.

## 6. Ước lượng effort theo vai trò

| Role | Person-days | Ghi chú |
|------|-------------|--------|
| BA | TBD | AC/CR |
| Backend | TBD | login CR-002 · audit LEV/IAM · adapter outbox · guard Prod · CI |
| Frontend web + mobile | TBD | BRQ-006 |
| QA | TBD | DOC-16 chưa |
| DevOps | TBD | LBS, A/S một DC, replication, backup ngoài DC (RK-01), hosting SPA |
| PM | TBD | |
| **Total** | **TBD** | Không quy đổi 1 tỷ → ngày công trên Draft này |

## 7. Giả định & Rủi ro (ước lượng)

| ID | Assumption / Risk | Impact |
|----|-------------------|--------|
| A-01 | Team & velocity chưa có | SP tuyệt đối **không** khóa |
| ~~A-02~~ | ~~Issuer OIDC IT~~ — **hết** (ADR-012) | — |
| A-04 | Chủ đầu tư không cần MS/DR/SSO — **văn bản đang qua bưu điện** (RK-08) | Nếu văn bản khác lời → đảo ADR |
| A-03 | EVT/RPT chưa SRS | Wave riêng |
| R-01 | ~~Microservices + DR hai DC đội ops~~ → **một host, một DC** (ADR-010/013): gánh ops giảm mạnh; còn RK-01 backup cùng DC | Đội ops vẫn chưa có (A-01) |
| R-02 | ~~Saga TIM→PAY~~ — **đóng** (ADR-005 Accepted, guard trong process) | — |
| R-03 | Adapter ra (SMTP/Git/CRM) chưa code; API vendor TBD | NFR-006/009 chưa kiểm đầu-cuối |
| R-04 | Login Prod chưa code; DOC-06/07 IAM delta chưa có | Go-live không thể thiếu |
| R-05 | Bộ lập lịch ngoài + xác thực máy chưa có (OQ-ARC-017/019) | NFR-009 im lặng hỏng |

## 8. Ánh xạ Release / Wave

| Wave | Phạm vi | Target |
|------|---------|--------|
| ~~W0–W3~~ | 7 module Must + hàng rào W1/W2 | **XONG** 2026-09 (UAT DEV Pass) |
| R1 | Audit LEV/IAM (B1) · DOC-10/14/16 ký · doc-review pass 4 · `02-baseline/` | 2026-09 |
| R2 | Login CR-002 (sau IAM DOC-06/07 delta) · bỏ JIT-provision · IAM audit login/khoá | 2026 Q4 |
| R3 | Adapter SMTP/Git/CRM · bộ lập lịch + xác thực máy · guard Prod · CI/Dockerfile · hosting SPA | 2026 Q4 |
| R4 | A/S drill trong DC · backup restore thử · NFR-001 UAT sau LBS · pen test login | 2026 cuối |
| R5 | Mobile · EVT/RPT sau SRS | 2026/27 |
| Go-live | Prod 24/7 | **2027** |

Ngày tháng cụ thể **TBD** DOC-15 khi có FTE.

## 9. Truy vết

| Story | FR / ADR / INT |
|-------|----------------|
| US-PLT-01 | ADR-013 · ADR-012 · DOC-17 |
| US-PLT-02 | ADR-010 · DOC-17 §2.2 |
| US-PLT-03 | ADR-011 W1/W2 · `w1-roles.sql` |
| US-IAM-01 | IAM DOC-06 |
| US-IAM-02 | ADR-012 · CR-002 · DOC-13 S07…S11 |
| US-INT-01 | INT-002/004/005 · R-015 |
| US-MOB-01 | NFR-003/008 |
| US-EMP-01 | EMP-FR |
| US-LEV-01 | LEV-FR |
| US-TIM-01 | TIM-FR · INT-003 |
| US-PAY-01 | PAY-FR · NFR-002 |
| US-PRB-01 | PRB-FR |
| US-LIF-01 | LIF-FR · INT-004/005 |
| US-NFR-01 | NFR-005 |
| US-DLV-01 | NFR-001 · DOC-16/17 |

## 10. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-PLN-002) |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | | ☐ **ký v0.2** · ☐ `02-baseline/` |
| PM | | 2026-08-26 | Soạn → PGD chốt |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
