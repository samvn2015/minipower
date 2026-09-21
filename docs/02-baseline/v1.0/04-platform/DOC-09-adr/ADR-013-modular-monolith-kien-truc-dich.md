# ADR-013 — Không cần microservices; modular monolith có hàng rào là kiến trúc đích

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-09-16 | soạn nháp SA (trợ lý) | **Accepted** — PGD chốt theo trả lời của chủ đầu tư (DEC-ARC-029) |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-09-16 (DEC-ARC-029). Đảo = ADR mới. |
| **Date** | 2026-09-16 |
| **Deciders** | Mr. Dư Hùng, PGD (A) · **chủ đầu tư** (nguồn: *"không cần"*) |
| **Consulted** | SA · Dev (SH-010) · Ops/IT (SH-006) |
| **Informed** | Ban HR · QC · BA · PM |
| **Supersedes** | [ADR-001](ADR-001-stack-style-hosting.md) **§1** (style microservices), **§2** (API Gateway), **§5** vế *database-per-service* · [ADR-002](ADR-002-sso-token.md) **toàn bộ** · [ADR-011](ADR-011-lo-trinh-tach-service.md) **W3 / W4+** |
| **Giữ nguyên** | ADR-001 §3 (LBS), §6 (hosting) · ADR-005 · ADR-009 · ADR-010 · ADR-011 **W1 / W2** · ADR-012 |
| **Đảo ngược** | DEC-ARC-016 vế *"microservices là yêu cầu khách hàng"* — nguồn yêu cầu đã rút |

---

### Bối cảnh

Ngày 2026-08-26, ADR-001 chọn **gói F**: microservices + API Gateway + LBS + SSO + database-per-service. Gói A (modular monolith) bị **loại** vì *"không đáp ứng chỉ đạo"*. Ngày 2026-09-07, [deliberation B1](../../../brainstorm/2026-09-07-arc-b1-monolith-vs-microservices.md) chỉ ra gốc vấn đề: ADR-001 chọn kiến trúc cho một đội ngũ và năng lực vận hành **chưa từng được xác nhận tồn tại** (DOC-14 A-01), trong khi monolith **đã chạy** — 7 module Must UAT DEV Pass. Verdict ♻️ RESHAPE, nhưng dừng ở UN-03: *lý do gốc chỉ đạo microservices là kỹ thuật hay từ khách?* PGD trả lời: **từ khách** (DEC-ARC-016). Từ đó monolith bị coi là *"vĩnh viễn bị loại"*, và ADR-011 chọn đường vòng: **W1** hàng rào dữ liệu → **W2** cưỡng chế trong code → **W3** tách PAY + Gateway → **W4+** tách nốt, kèm OQ-ARC-014 *(khách có chấp nhận lộ trình wave?)* mở.

Đến 2026-09-16, W1 và W2 **đã xong** (7 `DbContext`, 7 schema, 7 role, GRANT theo cột, test kiến trúc — OQ-ARC-011/016 đóng). W3 chưa bắt đầu.

Cùng ngày, PGD báo: **chủ đầu tư trả lời bằng văn bản là không cần ba chức năng — microservices, DR/DC, SSO.** Văn bản đang chuyển qua **đường bưu điện**, chưa có trong `assets/` (RK-08). Với DR/DC và SSO, đây là xác nhận lại ADR-010 và ADR-012. Với **microservices**, đây là điều mới: ràng buộc DEC-ARC-016 **không còn**. Câu hỏi kiến trúc quay về là **câu hỏi kỹ thuật nội bộ** — đúng câu deliberation B1 đã trả lời từ 07-09 nhưng không được phép áp dụng.

### Quyết định

1. **Kiến trúc đích = modular monolith có hàng rào cưỡng chế.** Một process `Hrm.Host`, một instance PostgreSQL, **7 schema + 7 DB role + `shared`** (kết quả W1), ranh giới module chặn bằng **test kiến trúc** (W2). **Trạng thái hiện tại chính là đích** — không còn "trạng thái lai".
2. **Không API Gateway.** Client → LBS → `Hrm.Host`. Việc Gateway từng gánh (TLS, chặn request không token, correlation-id) do **LBS** (TLS) và **middleware Jarvis trong host** (JWT fail-closed, correlation-id, `ApiResponseWrapperMiddleware`) đảm nhiệm — cả hai đã có.
3. **LBS giữ.** Không phải vì scale từng service, mà vì ADR-010 cần LBS để chỉ bơm traffic vào **Active** và failover trong DC. Sản phẩm vẫn TBD DOC-17 (OQ-DLV-002).
4. **Không database-per-service.** Hàng rào NFR-002 là **schema + role + GRANT** trong một instance — đã xác minh thực nghiệm ([OQ-ARC-011](../../../memory/architecture/oq-arc-011-verify-schema-role.md)): role LEV không đọc được PAY, chỉ thấy 4 cột `emp`. .NET 9, Jarvis, PostgreSQL (ADR-009) không đổi.
5. **ADR-011 W3 / W4+ bỏ. W1 / W2 giữ nguyên** — chúng là **tài sản của kiến trúc đích**, không phải bước chuyển tiếp. **Không dỡ** `AppDbContext` về một role; không gộp schema.
6. **Guard TIM↔PAY (ADR-005) là gọi trong process.** Ngữ nghĩa giữ nguyên (đọc đồng bộ, không broker, không saga). Mục *"chốt timeout/retry ở W3"* (RK-05, OQ-ARC-018) **không phát sinh**.
7. **ADR-002 superseded toàn bộ.** Vế SSO đã mất hiệu lực từ ADR-012; vế Gateway mất từ ADR này. Ba điểm còn giá trị được phát biểu lại tại đây, không cần đọc ADR-002: *(a)* **IAM DB là SoT role HRM** — claim ngoài không thay 403 màn HR / cô lập lương; *(b)* mọi kênh dùng **Bearer JWT do HRM ký** (ADR-012 §4); *(c)* **không** nhận token trên query string, **không** refresh token trên URL.
8. **Tách service về sau — nếu có —** là ADR mới với lý do đo được (tải, đội ngũ). W1 đã để ngỏ cửa đó: schema + role tách được thành DB riêng mà không đổi code domain. **Không lên kế hoạch trước.**

### Lý do

Chủ đầu tư **không cần**. Không cần ≠ cấm — nên ADR này không phải "được phép làm monolith" mà là **chọn** monolith vì mọi bằng chứng kỹ thuật đã nghiêng về đó từ deliberation B1: monolith đã chạy (AS-01, confidence cao); đội ops **chưa có** (A-01); vận hành hệ phân tán không có đội ops là điều góc Operations *"nhất định không chấp nhận"*; và ràng buộc cứng duy nhất — **hàng rào NFR-002 dưới tầng ứng dụng** — đã đạt bằng W1 mà **không cần tách service**.

Quan sát ở DEC-ARC-027 (*ba yêu cầu cộng lại = gánh vận hành và bảo mật cao nhất*) nay nhẹ đi một phần lớn: phần **phân tán** biến mất. Còn lại hai gánh có thật — không dự phòng thảm họa (RK-01) và tự gánh credential (ADR-012) — cả hai đều do khách chọn có văn bản.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| **X — Modular monolith có hàng rào (W1 + W2), không GW, giữ LBS** *(chọn)* | Trạng thái hiện tại = đích; NFR-002 đã có hàng rào thật; không thêm hạ tầng; go-live 2027 không phụ thuộc tuyển đội tách service | Scale ngang = nhân bản cả host sau LBS, không scale từng module; blast radius chung một process (RK-11) |
| Y — Tiếp tục ADR-011 W3/W4 dù khách không cần | "Đúng" ADR-001 gói F | **Loại** — chi phí vận hành phân tán không ai yêu cầu; A-01 chưa có đội; RK-03/04/05 tự gánh vô ích |
| Z — Gỡ W1/W2, quay về một `AppDbContext` một role | Ít file hơn | **Loại** — mất hàng rào NFR-002 dưới tầng app; trái ràng buộc cứng #3 của B1; bỏ đi thứ đã xác minh và đã trả giá |
| Y′ — Giữ Gateway trước monolith | Có "lớp middleware" như ADR-001 §2 | **Loại** — thêm một hop và một sản phẩm phải chọn/vận hành mà không làm được gì middleware host chưa làm |

### Hệ quả

**Tích cực:**
- **Hết "trạng thái lai"** — DOC-08 chỉ cần mô tả **một** kiến trúc; nợ *"quy ước phân biệt hiện tại vs đích đến"* (ADR-011 hệ quả tiêu cực) biến mất. Nguyên nhân gốc của Blocker B1 (doc-review 2026-09-07) **đóng hẳn**, không phải đóng bằng lộ trình.
- **RK-03** (khách "chưa thấy 7 service") · **RK-04** (ADR-005 phải trước W3) · **RK-05 / OQ-ARC-018** (timeout guard qua mạng) · **OQ-ARC-013 / 014** — **đóng**, không phải giải.
- NFR-001 đo **sau LBS**, không có hop Gateway — dễ đạt hơn.
- DOC-14: bỏ toàn bộ effort tách service + Gateway; A-01 bớt sức ép.

**Tiêu cực:**
- **DOC-08 phải viết lại phần lớn** — §1.4, §4.0–4.2, §4.4, §6, R-007 — lần sửa thứ 4 trong ba tuần. DOC-11 §1.2 (*"FK xuyên service"*), DOC-12 §1 (*API per service*), DOC-13 NFR-001, DOC-14 (WBS theo wave), DOC-17 §2/§4/§5/§8 (*GW, MS ×7, DB-per-service*) đều lệch.
- Scale ngang thô: nhân bản `Hrm.Host` sau LBS. Chấp nhận — NFR duy nhất có số là 1000 dòng &lt; 5s; không có yêu cầu scale khác.

**Rủi ro:**
- **RK-08** *(vẫn mở)* — ADR này Accepted trên lời PGD; văn bản đang **qua bưu điện**. Khi nhận: đối chiếu đúng chữ *"không cần"* cho **cả ba** mục, và đối chiếu RK-01 như DEC-ARC-028 đã ghi. Nếu văn bản chỉ nói hai mục → ADR này phải đảo bằng ADR mới.
- **RK-11** — Blast radius: một module rò bộ nhớ / treo CPU kéo cả host. Phương án: A/S failover (ADR-010) + health readiness theo từng `DbContext` đã có. Không thêm gì ở MVP.
- **RK-12** — Hàng rào chỉ mạnh bằng GRANT: ai "tiện tay" cấp `SELECT` cả schema hoặc chạy app bằng `hrm_migrator` là hàng rào biến mất **im lặng**. Lớp chặn: `w1-roles.sql` là SoT, review PR **phải soi mọi GRANT**, test kiến trúc chạy trong CI *(CI chưa có — nợ delivery)*.
- **RK-13** — Nếu DOC-08 không viết lại **trước** baseline, doc-review sẽ chặn lần thứ ba vì cùng một lý do.

### Tuân thủ & Tác động NFR

| NFR | Impact |
|-----|--------|
| NFR-001 | Đo sau **LBS**, không GW. Guard TIM↔PAY trong process — không hop mạng |
| **NFR-002** | Hàng rào = schema + role + GRANT theo cột (W1) + test kiến trúc (W2). **Đủ** — đã xác minh OQ-ARC-011. Không cần DB tách |
| NFR-003 / 008 | Web + mobile cùng host, cùng JWT |
| NFR-005 | Audit tại schema `shared` (DEC-ARC-024); correlation-id do middleware host, không do GW |
| NFR-012a–d | Không đổi — ADR-010 giữ; LBS vẫn cần cho A/S |

### DOC bị ảnh hưởng

| DOC | Thay đổi | Owner |
|-----|----------|-------|
| ADR-001 | Status: §1, §2, §5 *(DB-per-service)* superseded — chỉ sửa dòng Status | SA |
| ADR-002 | Superseded toàn bộ — chỉ sửa dòng Status | SA |
| ADR-011 | Status: W3/W4+ superseded; W1/W2 giữ — chỉ sửa dòng Status | SA |
| ADR-005 | Giữ nguyên. Ghi chú register: RK-05 không phát sinh | SA |
| DOC-08 | §1.4 style; §4.0 lớp; §4.1 bỏ GW; §4.2 luồng; §4.4 sơ đồ A/S không GW/MS; §6 bảng ADR; R-007 | SA |
| DOC-11 §1.2 | *"FK xuyên service — chỉ ID"* → *"FK xuyên schema — chỉ ID, cấm join chéo schema trừ closure emp đã cấp"* | SA |
| DOC-12 §1 | Bỏ *"API per service + hợp đồng gateway"*; một OAS một host — `/v1/{ctx}/…` giữ nguyên | SA |
| DOC-13 | NFR-001 đo sau LBS; NFR-P02 nếu có số | SA |
| DOC-14 | Bỏ WBS theo wave tách service; R-01 phát biểu lại | PM |
| DOC-17 | §2 UAT *"sau LBS+GW"*; §4 sơ đồ; §5 bước; §8 rollback *"N DB"* → một DB nhiều schema | SA / DevOps |
| DOC-10 | Không có INT nào cho GW — kiểm lại, dự kiến không đổi | SA |

### Câu hỏi mở

- **OQ-ARC-013** — **đóng**: không còn thứ tự tách service.
- **OQ-ARC-014** — **đóng**: khách không cần microservices; không có lộ trình wave để chấp nhận.
- **OQ-ARC-018** — **đóng**: guard trong process, không có timeout/retry qua mạng.
- **OQ-DLV-002** *(LBS)* — **vẫn mở**, lý do đổi: cần cho A/S, không cần cho scale từng service.

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-09-16 | ☑ **Accepted** — chủ đầu tư không cần microservices (DEC-ARC-029) |
| Chủ đầu tư | | | ☐ **văn bản vào `assets/`** — đang qua bưu điện (RK-08) |
| SA | | | ☐ DOC-08 / 11 / 12 / 13 / 17 |
| PM | | | ☐ DOC-14 |
