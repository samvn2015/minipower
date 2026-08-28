# DOC-16 — Chiến lược Kiểm thử (chương trình HRM)

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-08-26 | Trịnh Yên (QC/BA soạn) | **Chốt** (DEC-DLV-004) |

**ISTQB** levels/types. Phạm vi: 7 module Must đã có DOC-07 **Chốt**.  
**Gói DOC-16 Chốt:** chương trình + 7 file module (DEC-DLV-004). EVT/RPT chưa SRS — không TC.  
**Cổng:** PGD chốt v0.1. Sửa catalog/chiến lược đã chốt = CR. **Không** tự code. **Chưa** `02-baseline/`. EVT/RPT: chưa SRS → chưa TC Must. TC **chưa chạy** (cột Trạng thái trống).

---

## 1. Mục đích

Khóa **cách test** và **quy tắc trace** AC→TC. Mỗi AC Must trên DOC-07 phải có ≥1 TC (happy + negative đã có trên AC) trước UAT sign-off module.

Nguồn: 7× DOC-06/07 Chốt · DOC-12 khung · DOC-10 INT · NFR DOC-13 · ADR-003 HA.

## 2. Phạm vi & ngoài phạm vi

| In | Out |
|----|-----|
| IAM, EMP, LEV, TIM, PAY, PRB, LIF | EVT, RPT (chưa SRS) |
| INT-001…005, **cấm** INT-006 | Pixel HTML MCP |
| NFR-001 (1000 dòng sau LBS+GW), 403 lương, SSO | % uptime bịa; RTO phút chưa chốt |

## 3. Mức kiểm thử (ISTQB)

| Level | Owner | Mục |
|-------|-------|-----|
| Unit | Dev | Domain PAY/LEV/PRB |
| Integration | Dev/QC | API DOC-12 + DB-per-service |
| System / E2E | QC | LBS→GW→OIDC→MS |
| UAT | Ban HR + PGD | AC Must; NFR-001 |
| Security | QC + IAM | 401/403; NFR-002/004/006/007 |
| HA | DevOps | Failover A/S; job **không** chạy Standby/DR |

## 4. Quy tắc catalog (khi mở file module)

| Cột | Giá trị |
|-----|---------|
| TC ID | `{MOD}-TC-nnn` |
| Trace | FR + AC bắt buộc |
| Layer | UT / API / E2E |
| Path | Happy / Unhappy |
| Priority | Must theo AC Must |
| Trạng thái | trống đến khi chạy |

**Smoke go-live (DOC-17):** `/iam/me` OIDC; 1 đơn phép; 1 phiếu **của mình**; PRB 403 LM chốt; probe INT-006 = 0 call.

## 5. Ma trận phủ (khung)

| Module | DOC-07 | File TC module | Coverage |
|--------|--------|----------------|----------|
| IAM | Chốt | [identity/DOC-16](../03-modules/identity/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| EMP | Chốt | [employee-profile/DOC-16](../03-modules/employee-profile/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| LEV | Chốt | [leave/DOC-16](../03-modules/leave/DOC-16-test-strategy.md) **Chốt** | catalog AC Must; chưa chạy |
| TIM | Chốt | [timekeeping/DOC-16](../03-modules/timekeeping/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| PAY | Chốt | [payroll/DOC-16](../03-modules/payroll/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| PRB | Chốt | [probation/DOC-16](../03-modules/probation/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| LIF | Chốt | [lifecycle/DOC-16](../03-modules/lifecycle/DOC-16-test-strategy.md) **Chốt** | catalog ◐; chưa chạy |
| INT-006 | NFR-007 | TC trên LEV/LIF/PRB/IAM | catalog ◐; chưa chạy |

✅ chỉ khi file module có TC map đủ AC Must.

## 6. Môi trường

| Env | Mục đích |
|-----|----------|
| Dev | Unit / API |
| UAT | E2E + UAT business — **sau** LBS+GW |
| Prod | Smoke cutover only |

NFR-001 **không** đo localhost.

## 7. Entry / exit

| Gate | Entry | Exit |
|------|-------|------|
| Test module | DOC-07 Chốt; API path DOC-12 | Mọi AC Must có TC Pass |
| UAT chương trình | 7 module TC Pass; INT-001 IdP UAT | PGD + Ban HR |
| Go-live | DOC-17 dry-run + rollback | M6 2027 |

## 8. Defect

Theo glossary DOC-16 (Blocker/Major/Minor). Blocker 403 lương / CRM sales / job trên DR → **chặn** go-live.

## 9. Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-08-26 | ☑ Chốt v0.1 (DEC-DLV-004) |
| QC | | | Catalog Chốt; chưa execute |
| BA | Trịnh Yên | 2026-08-26 | Soạn |
