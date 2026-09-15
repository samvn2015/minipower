# ADR-010 — Vận hành 24/7, Active/Standby trong **một** DC, bỏ DR/DC

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-09-07 | soạn nháp SA (trợ lý) | **Accepted** (DEC-ARC-017 · PGD Dư Hùng) |
| 0.2 | 2026-09-07 | soạn nháp SA (trợ lý) | **Accepted** — bổ sung nguồn gốc: **khách hàng yêu cầu** bỏ DR/DC (DEC-ARC-018) |
| 0.3 | 2026-09-07 | soạn nháp SA (trợ lý) | **Accepted** — lý do bỏ = **chi phí**; backup đích = **máy Standby cùng DC**; mở rủi ro RK-01 (DEC-ARC-019) |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Accepted** — PGD Dư Hùng 2026-09-07. **Supersede ADR-003 §3–5**; giữ §1–2. Đảo = ADR mới. |
| **Date** | 2026-09-07 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · Ops/IT (SH-006) |
| **Informed** | Ban HR · BA Trịnh Yên · Dev (SH-010) |
| **Supersedes** | [ADR-003](ADR-003-ha-dr-active-standby.md) §3 (hai site), §4 (replicate Prod→DR), §5 (job trên DR) |

---

### Bối cảnh

ADR-003 (Accepted 2026-08-26, DEC-ARC-004) khoá mô hình **24/7 + Active/Standby trong DC + DR/DC hai site**. Từ đó xuất hiện hai dữ kiện mới:

1. **DOC-14 §7** ghi A-01 *"Team & velocity chưa có"* và R-01 *"Microservices + DR hai DC đội ops"* — gánh vận hành hai site chưa từng có người đảm nhiệm, effort §6 toàn TBD.
2. [Deliberation B1](../../../brainstorm/2026-09-07-arc-b1-monolith-vs-microservices.md) (2026-09-07): góc Operations nêu đây là mối lo #1, và *"vận hành hệ phân tán mà không có đội ops"* là điều họ nhất định không chấp nhận.

PGD trả lời (DEC-ARC-016): **bỏ DR/DC**; giữ **vận hành 24/7** và **cặp Active/Standby trong cùng một DC**.

**Nguồn gốc yêu cầu (v0.2 · DEC-ARC-018):** bỏ DR/DC là **yêu cầu từ khách hàng**, **lý do: chi phí hai site quá cao** (làm rõ ở v0.3 · DEC-ARC-019). Điều này đặt nó cùng thẩm quyền với ràng buộc microservices (OQ-ARC-008) — cả hai đều do khách đưa ra. Hệ quả: **không có rủi ro vi phạm cam kết**; ngược lại, giữ hai site sẽ là làm sai yêu cầu khách.

**Quan sát gửi kèm (không phải quyết định):** khách yêu cầu **microservices** (làm tăng độ phức tạp vận hành) đồng thời yêu cầu **bỏ DR/DC** (làm giảm khả năng chịu thảm họa). Hai yêu cầu kéo ngược chiều nhau về mặt vận hành. Không có gì sai — nhưng nên xác nhận với khách rằng họ hiểu hệ quả ở §6 dưới đây.

Lưu ý: phương án này **không có trong bảng của ADR-003**. ADR-003 chỉ xét J = *"Một DC, chỉ RAID/backup"* và đã loại vì không 24/7. Phương án hiện tại là một DC **có** Active/Standby — mạnh hơn J, yếu hơn G.

### Quyết định

1. **Mục tiêu vận hành:** giữ nguyên ADR-003 §1 — HRM phục vụ **24/7**, gồm ngoài giờ hành chính (phép đột xuất, job T-15/T-7/N+3). Không đóng % uptime trên ADR này.
2. **Trong DC Prod:** giữ nguyên ADR-003 §2 — cặp **Active/Standby** cho LBS, Gateway, từng service và primary DB. LBS chỉ bơm traffic vào Active; Standby nóng, không xử lý request nghiệp vụ đến khi failover.
3. **Bỏ site thứ hai.** **Không** DC-DR. Không replicate xuyên site. Không runbook chuyển site.
4. **Backup:** đích lưu = **máy Standby trong cùng DC** (DEC-ARC-019). Chu kỳ và thời gian restore mục tiêu = **TBD** Ops/IT.
   - Bảo vệ được: hỏng ổ đĩa Active · hỏng dữ liệu logic trên Active.
   - **Không** bảo vệ được: mất cả DC — xem **RK-01** dưới đây.
5. **Job:** chỉ scheduler **Active** chạy T-15/T-7/N+3, như ADR-003 §5 vế Active. Vế "disable trên DR" không còn đối tượng.
6. **Rủi ro chấp nhận:** mất toàn bộ DC (cháy, mất điện kéo dài, mất mạng nhà cung cấp) → HRM **ngừng phục vụ**. Khách hàng đã **xác nhận bằng văn bản** hiểu hệ quả này (DEC-ARC-019); lý do bỏ DR/DC là **chi phí quá cao**.

### RK-01 — Backup cùng DC không sống sót qua mất DC *(mở)*

Quyết định §4 đặt backup trên máy Standby, mà Standby ở **cùng DC** với Active (§2). Hệ quả logic:

| Sự cố | Backup cứu được? |
|-------|------------------|
| Hỏng ổ đĩa máy Active | ✅ |
| Hỏng dữ liệu logic trên Active | ✅ |
| **Mất cả DC** | ❌ **mất luôn backup** |

Nghĩa là **NFR-012c (RTO-restore khi mất DC) hiện không thể đạt** — không còn nguồn để restore. Mất DC = **mất dữ liệu vĩnh viễn**, không phải "ngừng phục vụ tạm thời" như §6 mô tả.

Đây **không** phải phản đối quyết định bỏ DR/DC. Site nóng thứ hai đắt; một **bản sao lạnh ngoài DC** thì không — đó là hai mức chi phí rất khác nhau. Cần PGD quyết riêng:

- **(a)** Chấp nhận RK-01 — mất DC là mất dữ liệu. Khi đó §6 và DOC-13 NFR-012c phải phát biểu lại cho đúng, và khách cần xác nhận **mức này**, không phải mức "ngừng phục vụ tạm thời" đã ký.
- **(b)** Thêm một bản sao **ngoài DC** (cold storage / băng từ / object storage nhà cung cấp khác) — rẻ hơn DR site nhiều bậc, giữ được ý nghĩa của §2.2.

Trạng thái: **mở**, chờ PGD. Ghi tại `OQ-ARC-012`.

### Lý do

Gánh vận hành hai site không có người đảm nhiệm (DOC-14 R-01, A-01). Bỏ site thứ hai loại bỏ phần lớn chi phí vận hành thường trực — replicate N database, drill failover, đồng bộ cấu hình hai nơi — trong khi vẫn giữ được mục tiêu 24/7 nhờ cặp Active/Standby.

### Các phương án đã xem xét

| Option | Pros | Cons |
|--------|------|------|
| G — 24/7 + A/S trong DC + DC-Prod/DC-DR *(ADR-003)* | Chịu được mất một nhà | **Loại** — đội ops không tồn tại (R-01); CAPEX/OPEX 2 site |
| **K — 24/7 + A/S trong một DC, backup trên Standby** *(chọn)* | Giữ 24/7; gánh ops vừa với năng lực thật; chi phí thấp | Mất cả DC = **mất dữ liệu** (RK-01), không chỉ ngừng phục vụ |
| J — Một DC, chỉ RAID/backup | Rẻ nhất | **Loại** — không đạt 24/7 (đã loại ở ADR-003) |
| I — Active/Active hai DC | RTO thấp nhất | **Loại** — double-job, conflict chốt công/lương (đã loại ở ADR-003) |

### Hệ quả

**Tích cực:** bỏ CAPEX/OPEX site thứ hai; không còn replicate xuyên site cho N database; DOC-17 bỏ chương chuyển site; R-01 (DOC-14) khép lại.

**Tiêu cực:** không còn khả năng chịu mất cả DC. RTO khi mất DC = thời gian restore, **lớn hơn nhiều** so với failover site.

**Rủi ro:** ~~thay đổi cam kết với khách~~ — đóng (v0.2). ~~Kỳ vọng chưa xác nhận~~ — **đóng (v0.3)**: khách đã xác nhận bằng văn bản, lý do bỏ là **chi phí quá cao**. **Rủi ro còn mở: RK-01** — văn bản khách ký mô tả hệ quả là *ngừng phục vụ đến khi restore*, nhưng với backup cùng DC thì hệ quả thật là **mất dữ liệu**. Nếu chọn phương án (a) thì phải xác nhận lại với khách ở đúng mức đó.

### Tuân thủ & Tác động NFR

| NFR ID | Impact |
|--------|--------|
| NFR-012 | Pattern đổi thành **24/7 + A/S một DC**. **RPO replicate xuyên site không còn đối tượng**; RTO tính theo failover trong DC (nhanh) và theo restore khi mất DC (chậm) — hai con số khác nhau, đều TBD. `OQ-ARC-002` / BLK-002 phải phát biểu lại, không chỉ điền số |
| NFR-001 | Đo trên Active sau failover giả lập — giữ nguyên |
| NFR-005 | Audit không split-brain — giữ nguyên (chỉ một Active) |
| NFR-009 | Job chỉ Active — giữ nguyên |
| NFR-011 | 2027 Prod sẵn sàng; **bỏ** yêu cầu DR sẵn sàng |

### DOC bị ảnh hưởng

| DOC | Thay đổi |
|-----|----------|
| [ADR-003](ADR-003-ha-dr-active-standby.md) | §3–5 superseded bởi ADR này |
| DOC-13 NFR-012 | Phát biểu lại pattern + tách hai loại RTO |
| DOC-17 | Bỏ topology 2 site và runbook chuyển site; thêm quy trình backup/restore |
| DOC-14 R-01 | Đóng — không còn "DR hai DC" |
| DOC-08 SAD | Sơ đồ triển khai bỏ DC-DR |

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | 2026-09-07 | ☑ Accepted (DEC-ARC-017) |
| SA | | | ☐ soạn lại DOC-13 / DOC-17 theo ADR này |
| Ops / IT | | | ☐ chốt chu kỳ backup + thời gian restore mục tiêu |
| Sponsor **(A)** | Mr. Dư Hùng, PGD | | ☐ **RK-01**: chọn (a) chấp nhận mất dữ liệu · (b) thêm bản sao ngoài DC |
