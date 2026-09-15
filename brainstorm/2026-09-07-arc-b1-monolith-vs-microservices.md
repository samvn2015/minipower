# Deliberation — B1: kiến trúc đã xây vs ADR-001 · [2026-09-07]

**Loại:** cross-phase deliberation (Premise Check + nghị luận đa góc nhìn)
**Kích hoạt:** doc-review gate DOC-11/DOC-12 → ⛔ BLOCK, Blocker #1
**Người quyết cuối:** PGD (Dư Hùng) — SA soạn
**Không** sinh DOC. Output → `architecture` (ADR) hoặc `change-control` (CR).

---

## 0. Bằng chứng khách quan

| Nguồn | Nội dung |
|---|---|
| DEC-ARC-003 | PGD chọn gói **F** = MS + API GW + LBS + SSO + DB-per-service; gói **A** (modular monolith) **bị loại** |
| ADR-001 §1 Accepted | *"mỗi bounded context Must = **một service deploy độc lập**: IAM, EMP, LEV, TIM, PAY, PRB, LIF"* |
| ADR-001 §5 Accepted | *"CSDL: **database-per-service**"* |
| DEC-ARC-008 | Chốt DOC-11 khung — *"Affects: **7 DB service** · PAY cô lập"* |
| DEC-ARC-012 | PGD **tái khẳng định** ADR-001, từ chối phương án thay thế |
| **Code thật** | **1** `Hrm.Host` · **1** `AppDbContext` 41 DbSet · **1** DB `hrm` · 13 controller · 82 path · **không** Gateway · **không** LBS |
| **DOC-14 §7 A-01** | *"**Team & velocity chưa có**"* — SP tuyệt đối không khóa |
| **DOC-14 §7 R-01** | *"**Microservices + DR hai DC đội ops**"* — Buffer lịch 2026 |
| DOC-14 §6 | Effort mọi vai trò = **TBD**, kể cả "Backend: 7 service + GW" |
| Thực tế giao hàng | 7 module Must đã landed, **UAT DEV Pass**, 144 unit test, 10 autotest xanh |

---

## 1. Premise Check

| # | Câu hỏi | Trả lời |
|---|---|---|
| 1 | **Vấn đề gốc?** | Hỏi "tại sao" 3 lần: *Vì sao code là monolith?* → một solution, giao theo slice module. *Vì sao chọn cách đó?* → phải landed 7 module Must trong ~5 tuần. *Vì sao ép tiến độ vậy?* → năng lực đội thực tế. **Gốc: ADR-001 chọn kiến trúc cho một quy mô đội ngũ và năng lực vận hành chưa từng được xác nhận tồn tại.** DOC-14 A-01 đã ghi *"Team & velocity chưa có"* ngay lúc lập kế hoạch. |
| 2 | **Ai đau, đo được?** | PGD (chỉ đạo không được thực thi) · SA (3 DOC nền mô tả sai hệ thống) · QC (không viết được test từ DOC-11/12) · IT (không biết triển khai 1 hay 7 service). Đo được: **3 Blocker**, **0 FR baselined**, M5 không có đường đi. |
| 3 | **Đã có giải pháp sẵn?** | Không. Không process/tool nào giải quyết thay; phải là quyết định kiến trúc. |
| 4 | **Do-nothing?** | Hai nhánh, đều xấu: (a) baseline tài liệu sai → đóng băng mô tả một hệ thống không tồn tại, mọi CR sau đó tính delta trên nền sai; (b) treo baseline vô hạn → M5 2027 không có mốc. |
| 5 | **DOC có consumer thật?** | Có. DOC-08/11/12 phục vụ dev, QC, IT triển khai — không phải "cho đủ bộ". |
| 6 | **Tiền đề còn đúng?** | **Đã lung lay.** Bằng chứng mới: monolith **đã chạy** — 7 module Must UAT DEV Pass. Giả thiết ngầm của ADR-001 rằng *phải* microservices mới đạt yêu cầu **chưa từng được kiểm chứng**, và nay có phản chứng thực nghiệm. R-01 (gánh nặng ops) thì vẫn chưa được giải. |

### Verdict: ♻️ **RESHAPE**

Vấn đề là thật, nhưng đang bị phát biểu sai.

- **Phát biểu sai:** *"Code làm sai so với ADR-001 — phải sửa code hoặc sửa tài liệu."*
- **Phát biểu đúng:** *"ADR-001 chốt kiến trúc mục tiêu trước khi biết năng lực đội và vận hành. Nay đã có dữ liệu thật về cả hai. Cần xác định lại kiến trúc mục tiêu theo năng lực thật — rồi tài liệu và code cùng đi theo."*

Khác biệt quan trọng: đây **không** phải phiên xử "ai sai". Đây là quyết định kiến trúc có dữ liệu mới.

---

## 2. Năm góc nhìn — mỗi góc một lượt

| Góc | Mối lo #1 | Tiêu chí thành công | Nhất định không chấp nhận |
|---|---|---|---|
| **Sponsor / PGD** | Chỉ đạo microservices + LBS + SSO đưa ra có lý do (scale, cô lập lương, chuẩn doanh nghiệp). Bỏ nó là bỏ luôn lý do đó? | Go-live 2027 đúng hạn, trong ~1 tỷ, hệ thống không sập khi cả công ty chấm công cuối tháng | Ném bỏ 7 module Must đã UAT Pass để làm lại từ đầu |
| **Operations / Support** | DOC-14 R-01 ghi thẳng: *microservices + DR hai DC = đội ops*. Đội đó **chưa tồn tại**. 7 service + GW + LBS + 7 DB = 7 pipeline, 7 backup, 7 monitor, saga TIM→PAY | Một người trực đêm vẫn khoanh vùng và khôi phục được sự cố | Vận hành hệ phân tán mà không có đội ops và không có runbook |
| **Engineering / Architect** | Monolith hiện tại **chưa có ranh giới module cưỡng chế** — 41 DbSet chung một `AppDbContext`, không gì ngăn PAY join thẳng bảng LEV. Nợ này âm thầm lớn dần | Ranh giới bounded context được cưỡng chế bằng cơ chế (schema riêng, assembly, test kiến trúc), không chỉ bằng quy ước thư mục | Gọi hiện trạng là "modular monolith" khi thực chất chưa có gì cưỡng chế tính module |
| **Security / Compliance** | NFR-002 (cô lập lương) hiện **chỉ được bảo vệ ở tầng ứng dụng**. DOC-11 thiết kế PAY tách DB riêng, LM không có GRANT đọc `Payslip` — hàng rào đó **không tồn tại**. Một bug RBAC là lộ lương toàn công ty | Cô lập dữ liệu lương có hàng rào ở tầng dưới ứng dụng (schema + quyền DB riêng), không phụ thuộc một dòng `if` | Baseline NFR-002 là "Pass" khi hàng rào thiết kế chưa được xây |
| **Finance / Cost** | DOC-14 §6 effort **toàn TBD**, chưa quy đổi 1 tỷ ra ngày công. Tách 7 service là chi phí xây **và** chi phí vận hành vĩnh viễn (7 DB, LBS, GW, DR hai DC) | Biết được chi phí chênh lệch giữa hai hướng trước khi cam kết | Cam kết kiến trúc mà không có một con số ước lượng nào |

---

## 3. Điểm hội tụ — ràng buộc cứng

Cả năm góc đồng ý, bất kể chọn hướng nào:

1. **Không đập đi làm lại.** 7 module Must đã UAT DEV Pass là tài sản; mọi hướng phải kế thừa.
2. **NFR-002 phải có hàng rào thật.** Cô lập lương không được chỉ dựa vào kiểm tra ở tầng ứng dụng — đây là ràng buộc mạnh nhất, cả Security lẫn Sponsor đều không nhượng.
3. **Ranh giới module phải được cưỡng chế bằng cơ chế**, không bằng quy ước. Áp dụng cho cả monolith lẫn microservices.
4. **Tài liệu và code phải hội tụ trước khi baseline.** Không baseline khi còn lệch.
5. **Năng lực vận hành là ràng buộc đầu vào**, không phải biến số điều chỉnh sau.

---

## 4. Căng thẳng còn sống — cần PGD quyết

| # | Trade-off | Hai đầu |
|---|---|---|
| **T1** | **Chỉ đạo ban đầu vs năng lực thật** | Giữ ADR-001 = trung thành với quyết định đã ký, nhưng R-01 (đội ops) vẫn chưa giải. Đổi ADR-001 = thừa nhận chỉ đạo được đưa ra khi thiếu dữ liệu. **Không góc nào giải được thay PGD.** |
| **T2** | **Chi phí cô lập NFR-002** | Đạt hàng rào thật bằng tách DB rẻ hơn nhiều so với tách cả 7 service. Có chấp nhận giải pháp trung gian — một deploy, nhiều schema/DB có quyền riêng — hay coi đó là "nửa vời"? |
| **T3** | **Thời điểm trả nợ kiến trúc** | Trả ngay (chậm M5, đúng ADR) vs trả dần theo wave (kịp M5, mang nợ có ghi nhận) vs không trả (đóng ADR mới). |
| **T4** | **Đơn vị scale thật sự là gì?** | ADR-001 giả định phải scale từng service. Chưa có số đo tải nào chứng minh TIM/PAY cần scale độc lập. NFR-001 (1000 dòng import) chưa được đo trên kiến trúc hiện tại. |
| **T5** | **Ai vận hành?** | Nếu giữ microservices, đội ops đến từ đâu và bao giờ? Nếu không có câu trả lời, T1 tự nó đã được trả lời. |

---

## 5. Vấn đề đã khung lại

> HRM đã giao được 7 module Must đạt UAT DEV trên một kiến trúc **một deploy, một cơ sở dữ liệu**, trong khi ADR-001 (Accepted) và DEC-ARC-003 quy định **bảy service độc lập, bảy cơ sở dữ liệu, có Gateway và LBS**. Khoảng cách này chặn baseline vì DOC-08/11/12 đang mô tả một hệ thống chưa từng được xây.
>
> Câu hỏi cần quyết **không phải** "code hay tài liệu sai", mà là: **kiến trúc mục tiêu của HRM là gì, khi đã biết năng lực đội ngũ và vận hành thật (DOC-14 A-01, R-01) — và lộ trình nào đưa hệ thống hiện có tới đó mà vẫn giữ được hàng rào cô lập dữ liệu lương (NFR-002)?**
>
> Ràng buộc cứng: không làm lại từ đầu · NFR-002 phải có hàng rào dưới tầng ứng dụng · ranh giới module phải được cưỡng chế bằng cơ chế · tài liệu và code hội tụ trước baseline · năng lực vận hành là đầu vào.

**Chưa chốt giải pháp** — đúng kỷ luật deliberation. Các phương án sẽ được dựng và so sánh ở phase `architecture`.

---

## 6. Assumption / Unknowns

| ID | Nội dung | Tin cậy |
|---|---|---|
| AS-01 | 7 module Must chạy được trên monolith — đã có UAT DEV Pass làm chứng | **cao** |
| AS-02 | Đội ops cho 7 service + DR hai DC hiện **chưa tồn tại** (DOC-14 R-01, A-01) | **cao** |
| AS-03 | NFR-002 hiện chỉ có guard tầng ứng dụng, không có hàng rào DB | **cao** |
| AS-04 | Tách schema/DB theo bounded context trong **một** deploy đạt được NFR-002 mà không cần tách service | **vừa** — cần SA xác minh |
| UN-01 | Tải thật: bao nhiêu NV chấm công đồng thời cuối tháng? Chưa đo | **thấp** |
| UN-02 | Chi phí chênh lệch giữa các hướng — DOC-14 §6 toàn TBD | **thấp** |
| UN-03 | Lý do gốc PGD chỉ đạo microservices: kỹ thuật, hay yêu cầu từ mInvoice/khách? | **thấp** — hỏi PGD |
| UN-04 | Đội ops nếu có thì đến từ đâu, khi nào | **thấp** |

---

## 7. Tiếp theo

**→ phase `architecture`** — dựng ADR mới với ≥2 phương án, so trên 5 ràng buộc cứng ở §3, trả lời T1–T5.

Điều kiện đủ để vào phase: PGD trả lời **UN-03** (lý do gốc của chỉ đạo microservices) và **T5** (ai vận hành). Thiếu hai câu này thì mọi phương án đều là đoán.

Ghi decision khi chốt → `memory/architecture/decision-log.md` (`DEC-ARC-*`), kèm phương án bị loại.
