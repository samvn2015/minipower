# DOC-14 — WBS & Ước lượng

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (PM/BA soạn) | **Chốt** (DEC-PLN-002) |

**PMBOK WBS** · Epic / Feature / Story. Tiên quyết: DOC-03 · 7× DOC-06 **Chốt** · DOC-08–12 khung **Chốt**.  
**Cổng:** PGD chốt v0.1 (DEC-PLN-002). Nợ: velocity (SP/sprint khi có team); SP tuyệt đối; FTE; EVT/RPT SRS. **Không** tự DOC-16/17. **Chưa** `02-baseline/`.

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
├── 1.1 Nền tảng (GW, LBS, IAM-RP OIDC, A/S+DR)
├── 1.2 identity
├── 1.3 employee-profile
├── 1.4 leave
├── 1.5 timekeeping
├── 1.6 payroll
├── 1.7 probation
├── 1.8 lifecycle
├── 1.9 NFR / audit / cô lập lương
├── 1.10 EVT + RPT (sau SRS)
└── 1.11 Delivery (DOC-16/17, UAT, cutover)
```

## 3. Phân rã Epic · Feature · Story (hạt Feature)

SP cột = **T-shirt** đến khi có velocity. Trace = dải FR module, không copy từng FR.

| WBS | Epic | Feature | Story ID | Story title | FR trace | Size | Priority |
|-----|------|---------|----------|-------------|----------|------|----------|
| 1.1.1 | EP-PLT | FE-GW | US-PLT-01 | As NV/HR I use HRM via LBS+GW+OIDC | ADR-001/002/007 | XL | Must |
| 1.1.2 | EP-PLT | FE-HA | US-PLT-02 | As Ops I run Active/Standby + DR/DC | ADR-003 | XL | Must |
| 1.2.1 | EP-IAM | FE-RBAC | US-IAM-01 | As IAM I map IdP sub → roles HRM | IAM DOC-06 | L | Must |
| 1.3.1 | EP-EMP | FE-HS | US-EMP-01 | As HR/NV I manage hồ sơ + unique + HĐ/KT_TV | EMP-FR | L | Must |
| 1.4.1 | EP-LEV | FE-DON | US-LEV-01 | As NV/LM/HR I submit & C1/C2 trừ quỹ | LEV-FR | L | Must |
| 1.5.1 | EP-TIM | FE-IMP | US-TIM-01 | As HR I import 1 mẫu Excel & chốt tháng | TIM-FR, INT-003 | L | Must |
| 1.6.1 | EP-PAY | FE-TINH | US-PAY-01 | As HR I run kỳ lương; NV xem phiếu mình | PAY-FR, NFR-002 | XL | Must |
| 1.7.1 | EP-PRB | FE-TV | US-PRB-01 | As HR I chốt 3 mã; T-15/T-7 job | PRB-FR | M | Must |
| 1.8.1 | EP-LIF | FE-N3 | US-LIF-01 | As IT I lock Git/CRM sản phẩm N+3 | LIF-FR, INT-004/005 | M | Must |
| 1.9.1 | EP-NFR | FE-AUD | US-NFR-01 | As Ops I retain audit chốt công/lương/C2/PRB | NFR-005 | M | Must |
| 1.10.1 | EP-EVT | FE-CANH | US-EVT-01 | Cảnh báo SN/lễ | *chưa SRS* | — | Must* |
| 1.11.1 | EP-DLV | FE-UAT | US-DLV-01 | UAT 1000 dòng &lt;5s + DOC-16/17 | NFR-001 | L | Must |

\*EVT in BRD Must — **không** estimate chi tiết đến khi DOC-06 EVT.

Chi tiết 1:1 AC → DOC-07 từng module (không lặp Gherkin).

## 4. Chi tiết story (mẫu)

### US-PLT-01 — Gateway + OIDC

| Mục | Nội dung |
|-----|----------|
| **User story** | As a NV/HR, I want mọi API qua LBS+GW với JWT OIDC, so that không login password HRM |
| **AC** | DOC-12 khung; 401 không JWT; ADR-007 |
| **Dependencies** | Issuer IT |
| **Size** | XL |
| **Notes** | Cấm POST password |

## 5. Chấm phức tạp (0–4 × 5 · toàn chương trình)

| Chiều | Điểm | Lý do |
|-------|------|-------|
| Phạm vi | **4** | ≥4 module Must + nền tảng MS |
| Tích hợp | **3** | IdP, SMTP, Excel, Git, CRM sản phẩm — API vendor TBD |
| Dữ liệu | **3** | 7 DB; migrate as-is; catalog động |
| Bên liên quan | **3** | PGD, HR, IT, NV, LM; Ban HR chưa ký |
| PCN & rủi ro | **4** | 24/7 A/S DR; PII lương; MS+GW |
| **Tổng** | **17** | **Enterprise** |

Hàm ý: roadmap đa quý, phụ thuộc IT IdP/Git/CRM, governance.

## 6. Ước lượng effort theo vai trò

| Role | Person-days | Ghi chú |
|------|-------------|--------|
| BA | TBD | AC/CR |
| Backend | TBD | 7 service + GW |
| Frontend web + mobile | TBD | BRQ-006 |
| QA | TBD | DOC-16 chưa |
| DevOps | TBD | LBS, A/S, DR |
| PM | TBD | |
| **Total** | **TBD** | Không quy đổi 1 tỷ → ngày công trên Draft này |

## 7. Giả định & Rủi ro (ước lượng)

| ID | Assumption / Risk | Impact |
|----|-------------------|--------|
| A-01 | Team & velocity chưa có | SP tuyệt đối **không** khóa |
| A-02 | Issuer OIDC IT kịp wave 1 | Trễ → trễ toàn API |
| A-03 | EVT/RPT chưa SRS | Wave riêng |
| R-01 | Microservices + DR hai DC đội ops | Buffer lịch 2026 |
| R-02 | Saga TIM→PAY (ADR-005 Proposed) | +size PAY/TIM |

## 8. Ánh xạ Release / Wave

| Wave | Phạm vi | Target |
|------|---------|--------|
| W0 | Nền tảng GW+OIDC+IAM skeleton | 2026 |
| W1 | EMP + LEV | 2026 |
| W2 | TIM + PAY | 2026 |
| W3 | PRB + LIF N+3 | 2026 |
| W4 | HA/DR drill + NFR-001 UAT | 2026 cuối |
| W5 | EVT/RPT sau SRS | 2026/27 |
| Go-live | Prod 24/7 | **2027** |

Ngày tháng cụ thể **TBD** DOC-15 khi có FTE.

## 9. Truy vết

| Story | FR / ADR / INT |
|-------|----------------|
| US-PLT-01 | ADR-001, 002, 007 · DOC-12 |
| US-PLT-02 | ADR-003 |
| US-IAM-01 | IAM DOC-06 · INT-001 |
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
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | **Chốt** v0.1 (DEC-PLN-002) · ☐ `02-baseline/` |
| PM | | 2026-08-26 | Soạn → PGD chốt |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
