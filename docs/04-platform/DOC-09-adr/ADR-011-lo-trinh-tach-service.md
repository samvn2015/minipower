# ADR-011 — Lộ trình từ monolith hiện tại tới microservices khách yêu cầu

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.1 | 2026-09-07 | soạn nháp SA (trợ lý) | **Proposed** — chờ PGD chốt |

**Michael Nygard ADR** · 1 file / 1 quyết định. **Không** sửa khi Accepted — đảo = ADR mới.

| Mục | Giá trị |
|-----|---------|
| **Status** | **Proposed** — giải Blocker B1 của [doc-review 2026-09-07](../../../memory/delivery/doc-review-2026-09-07-doc11-12.md) |
| **Date** | 2026-09-07 |
| **Deciders** | Mr. Dư Hùng, PGD (A) |
| **Consulted** | SA · Ops/IT (SH-006) · Dev (SH-010) · khách hàng *(ràng buộc microservices)* |
| **Informed** | Ban HR · QC |
| **Liên quan** | ADR-001 (giữ) · [ADR-010](ADR-010-ha-single-dc-active-standby.md) (một DC) · [deliberation B1](../../../brainstorm/2026-09-07-arc-b1-monolith-vs-microservices.md) |

---

### Bối cảnh

Doc-review chặn baseline vì DOC-08/11/12 mô tả **7 service + 7 DB + Gateway + LBS**, còn code là **một `Hrm.Host`, một `AppDbContext` 41 DbSet, một DB `hrm`, 13 controller, không Gateway, không LBS**.

Deliberation B1 khung lại vấn đề và dừng ở hai câu chặn. Cả hai nay đã có đáp án:

| Dữ kiện | Nguồn | Hệ quả |
|---|---|---|
| Microservices là **yêu cầu khách hàng** | DEC-ARC-016 | **Không** thể đảo ADR-001 bằng CR nội bộ. Monolith vĩnh viễn bị loại |
| **Bỏ DR/DC**, một DC, A/S | ADR-010 · DEC-ARC-017/018 | Gánh vận hành giảm mạnh — microservices khả thi hơn giả định ban đầu |
| Đội ngũ / velocity **chưa có** | DOC-14 A-01 | Không thể tách 7 service cùng lúc |
| 7 module Must **UAT DEV Pass** trên monolith | memory/delivery | Tài sản phải kế thừa, không đập đi |

Năm ràng buộc cứng từ deliberation §3 giữ nguyên hiệu lực, mạnh nhất là: **NFR-002 phải có hàng rào dưới tầng ứng dụng**.

### Vấn đề cần quyết

Đích đến đã bị khoá bởi khách (microservices). Câu hỏi còn lại là **lộ trình**: đi tới đó bằng đường nào, thứ tự nào, và làm sao có hàng rào NFR-002 **trước** khi tách xong.

### Các phương án đã xem xét

| Option | Mô tả | Pros | Cons |
|--------|-------|------|------|
| **L — Big bang** | Dừng tính năng, tách thẳng 7 service + GW + LBS | Đến đích nhanh nhất trên giấy | Đội chưa có (A-01); 7 module UAT Pass phải verify lại toàn bộ; saga TIM→PAY phải làm ngay; **rủi ro trượt 2027 cao nhất** |
| **M — Hàng rào dữ liệu trước, tách deploy sau** *(đề xuất)* | Wave 1: tách **schema + DB role** theo bounded context trong cùng instance PostgreSQL, mỗi context một `DbContext`. Wave 2+: tách deploy từng service theo thứ tự rủi ro | **NFR-002 có hàng rào thật ngay wave 1** với chi phí thấp nhất; ranh giới bị cưỡng chế bằng GRANT chứ không bằng quy ước; mỗi wave giao được, không có giai đoạn "hệ thống không chạy" | Đích cuối tới chậm hơn L; phải chấp nhận trạng thái lai trong nhiều wave |
| N — Tách deploy trước, DB sau | Tách process 7 service, vẫn chung một DB | Có "7 service" sớm để trình khách | **NFR-002 vẫn không có hàng rào**; 7 service chung DB là distributed monolith — tệ hơn hiện tại về vận hành lẫn thiết kế |
| O — Giữ monolith | Không làm gì | Rẻ nhất | **Loại** — trái yêu cầu khách (DEC-ARC-016) |

### Quyết định *(đề xuất — chờ PGD)*

Chọn **M**. Lộ trình theo wave, mỗi wave có tiêu chí ra rõ ràng:

| Wave | Nội dung | Tiêu chí ra |
|------|----------|-------------|
| **W1 — Hàng rào dữ liệu** | Tách `AppDbContext` thành N `DbContext` theo bounded context; mỗi context một **schema** PostgreSQL và một **DB role** riêng; thu hồi quyền chéo. PAY làm trước. | `psql` với role của LEV **không** đọc được bảng PAY. Test kiến trúc chặn reference chéo giữa module. 144 unit test + 10 autotest vẫn xanh |
| **W2 — Cưỡng chế ranh giới trong code** | Mỗi bounded context một assembly; giao tiếp qua interface trong Application, không qua entity của nhau | Test kiến trúc fail khi có reference chéo. Không còn join chéo context trong repository |
| **W3 — Tách service rủi ro cao nhất** | Tách **PAY** thành service + DB riêng (cô lập lương là NFR mạnh nhất). Gateway xuất hiện ở wave này | PAY chạy độc lập; `/v1/pay/*` qua GW; TIM→PAY dùng outbox (ADR-005) |
| **W4+** | Tách các context còn lại theo thứ tự khách/ nghiệp vụ ưu tiên | Từng service một, không big bang |

**Nguyên tắc xuyên suốt:** không wave nào để hệ thống ở trạng thái không chạy được; sau mỗi wave, autotest + unit test phải xanh trước khi mở wave sau.

### Lý do

Hàng rào NFR-002 là ràng buộc cứng mà cả năm góc nhìn đều không nhượng, và nó **không đòi hỏi phải tách service** — chỉ đòi hỏi tách quyền truy cập dữ liệu. Làm nó ở wave 1 nghĩa là rủi ro lớn nhất được đóng sớm nhất, với chi phí nhỏ nhất, độc lập với tiến độ tách service vốn phụ thuộc nhân sự chưa có.

Ngược lại, N tạo ra "7 service chung một DB" — trông giống đích đến nhưng **không** có hàng rào, đồng thời gánh trọn chi phí vận hành phân tán. Đó là kết cục xấu nhất trong bốn phương án.

### Hệ quả

**Tích cực:** NFR-002 có hàng rào thật từ wave 1 · mỗi wave giao được và verify được · đích đến vẫn là microservices như khách yêu cầu · tách được tiến độ kiến trúc khỏi tiến độ tuyển người.

**Tiêu cực:** hệ thống ở trạng thái lai trong nhiều wave — tài liệu phải mô tả **trạng thái hiện tại** và **đích đến** tách bạch, nếu không doc-review sẽ chặn lại lần nữa. Cần một quy ước đánh dấu trong DOC-08.

**Rủi ro:**

- **RK-02** — W1 đụng cả 56 migration và 28 repository. Cần một slice riêng, có rollback, không gộp với tính năng.
- **RK-03** — Khách nhìn "chưa thấy 7 service" ở W1/W2 có thể hiểu là chưa làm. Cần trình bày lộ trình cho khách, kèm mốc.
- **RK-04** — ADR-005 (saga TIM→PAY) vẫn **Proposed**, mà W3 cần nó. Phải chốt trước W3.

### Tuân thủ & Tác động NFR

| NFR | Impact |
|-----|--------|
| **NFR-002** | Đạt hàng rào dưới tầng ứng dụng **từ W1** — sớm hơn nhiều so với L hoặc N |
| NFR-001 | Đo lại sau mỗi wave; W3 thêm hop Gateway → cần đo lại 1000 dòng &lt; 5s |
| NFR-005 | Audit hiện dùng `EmpAuditLog` chung; W1 phải quyết audit theo context hay tập trung |
| NFR-012a–d | Không đổi — ADR-010 giữ nguyên qua mọi wave |

### DOC bị ảnh hưởng

| DOC | Thay đổi |
|-----|----------|
| DOC-08 | Thêm quy ước phân biệt **kiến trúc hiện tại** vs **đích đến**; sơ đồ theo wave |
| DOC-11 | Viết lại §3 theo schema thật (Blocker B2 của doc-review) + đánh dấu context nào đã tách |
| DOC-12 | Sinh lại `openapi.yaml` từ Swagger runtime (Blocker B3) |
| DOC-14 | WBS theo wave; effort vẫn TBD đến khi có đội |
| ADR-005 | Phải chuyển từ Proposed sang Accepted trước W3 |

### Câu hỏi mở

- **OQ-ARC-013** — Thứ tự tách service sau PAY do ai quyết: khách, hay nội bộ theo rủi ro kỹ thuật?
- **OQ-ARC-014** — Khách có chấp nhận lộ trình theo wave, hay yêu cầu đủ 7 service tại thời điểm go-live 2027?
- **OQ-ARC-011** *(đang mở)* — xác minh kỹ thuật: schema + role riêng trong một instance PostgreSQL có đủ cho NFR-002 không?

### Phê duyệt

| Vai trò | Họ tên | Ngày | Kết quả |
|---------|--------|------|---------|
| Sponsor **(A)** | Mr. Dư Hùng, PGD | | ☐ Accepted · ☐ Rejected · ☐ Đổi phương án |
| SA | | | ☐ |
| Ops / IT | | | ☐ |
